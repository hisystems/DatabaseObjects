
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace Generic

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This class extends Generic.DatabaseObjectsList by providing a public
    ''' Add(inherited) Item by key, Item by integer/ordinal (inherited), 
    ''' Count (inherited) and Exists properties and function. 
    ''' For more information please see DatabaseObjects.DatabaseObjects.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    '''
    Public MustInherit Class DatabaseObjectsListKeyed(Of T As IDatabaseObject, TKey)
        Inherits Generic.DatabaseObjectsList(Of T)

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new DatabaseObjects with it's associated database.
        ''' </summary>
        ''' 
        ''' <param name="objDatabase">
        ''' The database that this collection is associated with.
        ''' </param>
        ''' --------------------------------------------------------------------------------
        Protected Sub New(ByVal objDatabase As Database)

            MyBase.New(objDatabase)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes with it the associated root container and database.
        ''' </summary>
        ''' 
        ''' <param name="rootContainer">
        ''' The root object that this collection is associated with.
        ''' </param>
        ''' --------------------------------------------------------------------------------
        Protected Sub New(ByVal rootContainer As RootContainer)

            MyBase.New(rootContainer)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new DatabaseObjects with it's associated parent object.
        ''' The Parent property can be used to access the parent variable passed into this constructor.
        ''' </summary>
        ''' 
        ''' <param name="objParent">
        ''' The parent object that this collection is associated with.
        ''' </param>
        ''' --------------------------------------------------------------------------------
        Protected Sub New(ByVal objParent As DatabaseObject)

            MyBase.New(objParent)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns an object for a key value. 
        ''' </summary>
        ''' <remarks>
        ''' This function onforwards a call to DatabaseObjects.ObjectByKey().
        ''' </remarks> 
        ''' --------------------------------------------------------------------------------
        ''' 
        Default Public Overridable Overloads ReadOnly Property Item(ByVal Key As TKey) As T
            Get

                Return MyBase.ObjectByKey(Key)

            End Get
        End Property

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the key exists within the collection. 
        ''' </summary>
        ''' <remarks>
        ''' This function onforwards a call to DatabaseObjects.ObjectExists().
        ''' </remarks>
        ''' --------------------------------------------------------------------------------
        '''
        Public Overridable Function Exists(ByVal Key As TKey) As Boolean

            Return MyBase.ObjectExists(Key)

        End Function

    End Class

End Namespace
