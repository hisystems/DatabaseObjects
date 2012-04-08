
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

''' --------------------------------------------------------------------------------
''' <summary>
''' This class can be used to simplify the process of creating and returning late-bound 
''' IDatabaseObject or DatabaseObject objects. This is is particularly useful in decreasing 
''' load times of referenced objects by post-poning loading any objects until the property or 
''' method is called directly. 
''' The referenced object is only loaded on the first call to the Object property. 
''' Subsequent calls return the already loaded object.
''' A variable of this type can be marked with the FieldMapping attribute so that the distinct 
''' values are automatically set and read by the library (this particular facility can only be 
''' used by an object that inherits from DatabaseObjectsUsingAttributes).
''' </summary>
''' --------------------------------------------------------------------------------
Public Class ObjectReference

    Private pobjDatabase As Database = Nothing
    Private pobjCollection As IDatabaseObjects = Nothing
    Private pobjDistinctValue As Object = Nothing
    Private pobjObject As IDatabaseObject = Nothing

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes the class with the collection that the object is contained within.
    ''' </summary>
    ''' <param name="objCollection">
    ''' The collection that contains the referenced object. The collection's
    ''' MyBase.Object function is called to load the object.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Public Sub New( _
        ByVal objCollection As DatabaseObjects)

        Me.New(objCollection.ParentDatabase, objCollection)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes the database and collection class that the object is contained within.
    ''' </summary>
    ''' <param name="objDatabase">
    ''' The database that the collection is associated with.
    ''' </param>
    ''' <param name="objCollection">
    ''' The collection that contains the referenced object. The class' MyBase.Object
    ''' function is called to load the object.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Public Sub New( _
        ByVal objDatabase As Database, _
        ByVal objCollection As IDatabaseObjects)

        If objDatabase Is Nothing Then
            Throw New ArgumentNullException("Database has not been set")
        ElseIf objCollection Is Nothing Then
            Throw New ArgumentNullException("Collection has not been set")
        End If

        pobjDatabase = objDatabase
        pobjCollection = objCollection

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Indicates that the first the object in collection should be loaded.
    ''' After this call the Object property will return this first object.
    ''' Specifically, this function calls Database.ObjectByOrdinalFirst().
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Sub ObjectSetToOrdinalFirst()

        pobjObject = pobjDatabase.ObjectByOrdinalFirst(pobjCollection)
        pobjDistinctValue = pobjObject.DistinctValue

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Sets or returns the referenced object. 
    ''' When reading the value, if the object has not already been loaded then the object
    ''' is loaded. On subsequent calls the already loaded object is returned.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Overridable Property [Object]() As IDatabaseObject
        Get

            If pobjObject Is Nothing AndAlso Not pobjDistinctValue Is Nothing Then
                pobjObject = pobjDatabase.Object(pobjCollection, pobjDistinctValue)
            End If

            Return pobjObject

        End Get

        Set(ByVal Value As IDatabaseObject)

            pobjObject = Value

            If Value Is Nothing Then
                pobjDistinctValue = Nothing
            Else
                pobjDistinctValue = Value.DistinctValue
            End If

        End Set
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Sets or returns the distinct value of the object. 
    ''' If Object returns Nothing then Nothing is returned.
    ''' Simulates using Me.Object.DistinctValue except that loading of 
    ''' the associated object is avoided if the Me.Object get property had not been called 
    ''' and the object loaded from the database.
    ''' If the ObjectReference class is used with the FieldMapping attribute then this
    ''' property is set and read automatically when respectively loading and saving the object.
    ''' See Example 1 below for details.
    ''' </summary>
    ''' <example> 
    ''' <code>
    ''' Example 1:
    ''' 
    ''' &lt;DatabaseObjects.FieldMapping("ProductGroupID")&gt; _
    ''' Private pobjGroup As New Generic.ObjectReference(Of ProductGroup, Integer)(Database.ProductGroups)
    ''' 
    ''' Example 2:
    ''' 
    ''' Private pobjGroup As New Generic.ObjectReference(Of ProductGroup, Integer)(Database.ProductGroups)
    ''' 
    ''' &lt;DatabaseObjects.FieldMapping("ProductGroupID")&gt; _
    ''' Private Property GroupID() As Integer
    '''     Get
    '''         
    '''         Return pobjGroup.DistinctValue
    ''' 
    '''     End Get
    ''' 
    '''     Set(ByVal Value As Integer)
    ''' 
    '''         pobjGroup.DistinctValue = Value
    ''' 
    '''     End Set
    ''' End Property
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    Public Property DistinctValue() As Object
        Get

            'If the object has not been loaded return the 
            'distinct value variable, otherwise extract it directly from the object.
            If pobjObject Is Nothing Then
                Return pobjDistinctValue
            Else
                Return pobjObject.DistinctValue
            End If

        End Get

        Set(ByVal value As Object)

            'If the distinct value has already been set / do not cause a reloading of the object
            'unless the distinct value is different.
            If pobjDistinctValue Is Nothing OrElse Not pobjDistinctValue.Equals(value) Then
                pobjDistinctValue = value
                'Set it to nothing so that the new object will be loaded if/when Object is called
                pobjObject = Nothing
            End If

        End Set
    End Property

    Friend ReadOnly Property ParentCollection As IDatabaseObjects
        Get

            Return pobjCollection

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the Object property would return Nothing.
    ''' This property can be used rather than calling Me.Object Is Nothing as it avoids
    ''' unnecessarily loading the associated object if the object had not been loaded
    ''' from the database.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public ReadOnly Property ObjectIsNothing() As Boolean
        Get

            Return pobjDistinctValue Is Nothing

        End Get
    End Property

End Class