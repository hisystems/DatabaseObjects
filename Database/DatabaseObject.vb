
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

''' --------------------------------------------------------------------------------
''' <summary>
''' This class can be used in conjunction with the DatabaseObjects class to simplify
''' the process of using the DatabaseObjects library. This class implements the 
''' IDatabaseObject interface and provides the basic "plumbing" code required by the 
''' interface. For this reason, inheriting from this class is preferable to 
''' implementing the IDatabaseObject interface directly.
''' </summary>
''' --------------------------------------------------------------------------------
''' 
Public MustInherit Class DatabaseObject
    Implements IDatabaseObject

    Private pbIsSaved As Boolean
    Private pobjDistinctValue As Object
    Private pobjParentCollection As DatabaseObjects

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new DatabaseObject with the parent collection that this object is 
    ''' associated with.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' 'Code from a class that has inherited from DatabaseObjects
    ''' 'So that this object has a reference to the parent
    ''' Protected Overrides Function ItemInstance() As DatabaseObjects.IDatabaseObject
    ''' 
    '''     Return New Product(Me)
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    Protected Sub New(ByVal objParentCollection As DatabaseObjects)

        If objParentCollection Is Nothing Then
            Throw New ArgumentNullException
        End If

        pobjParentCollection = objParentCollection

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Loads this object with the object's fields and properties with the fields from the database.
    ''' Automatically sets the IDatabaseObject.DistinctValue and IDatabaseObject.IsSaved properties so that the
    ''' object is correctly initialized.
    ''' Onforwards a call to Database.ObjectLoad()
    ''' </summary>
    ''' <remarks>
    ''' Typically used from within the overridden LoadFields function when completely loading and initializing an 
    ''' IDatabaseObject object from the database fields. This is different to LoadFieldsForObject in
    ''' that the IDatabaseObject.DistinctValue and IDatabaseObject.IsSaved properties are not initialized.
    ''' Furthermore the object typically does not implement IDatabaseObject 
    ''' and therefore these properties cannot be set.
    ''' Onforwards a call to Database.ObjectLoad()
    ''' </remarks>
    ''' --------------------------------------------------------------------------------
    Protected Sub Load(ByVal objFields As SQL.SQLFieldValues)

        Database.ObjectLoad(Me, objFields)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Loads this object with the object's fields and properties with the fields from the database.
    ''' Automatically sets the IDatabaseObject.DistinctValue and IDatabaseObject.IsSaved properties so that the
    ''' object is correctly initialized.
    ''' Onforwards a call to Database.ObjectLoad()
    ''' </summary>
    ''' <remarks>
    ''' Typically used from within the overridden LoadFields function when completely loading and initializing an 
    ''' IDatabaseObject object from the database fields. This is different to LoadFieldsForObject in
    ''' that the IDatabaseObject.DistinctValue and IDatabaseObject.IsSaved properties are not initialized.
    ''' Furthermore the object typically does not implement IDatabaseObject 
    ''' and therefore these properties cannot be set.
    ''' Onforwards a call to Database.ObjectLoad()
    ''' </remarks>
    ''' --------------------------------------------------------------------------------
    Protected Sub Load(ByVal objData As IDataReader)

        Database.ObjectLoad(Me, objData)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Sets/returns the parent collection (DatabaseObjects instance) that this object is 
    ''' associated with.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Friend Property ParentCollection() As DatabaseObjects
        Get

            Return pobjParentCollection

        End Get

        Set(ByVal value As DatabaseObjects)

            If value Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjParentCollection = value

        End Set
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the database associated with this object.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Friend ReadOnly Property ParentDatabase() As Database
        Get

            Return pobjParentCollection.ParentDatabase

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the grand parent of this object. This is usually the object that
    ''' contains the collection that this object is contained with in.
    ''' For example, a GrandParent object of an InvoiceDetail would be an Invoice.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' <exception cref="InvalidCastException">
    ''' Throws an exception if the grand parent object is not of type DatabaseObject and has
    ''' only implemented IDatabaseObject.
    ''' </exception>
    Protected Friend ReadOnly Property GrandParent() As DatabaseObject
        Get

            Return Me.ParentCollection.Parent

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the distinct value from the grand parent of this object.
    ''' This is usually the object that contains the collection that this object is contained within.
    ''' For example, a GrandParent object of an InvoiceDetail would be an Invoice.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' <exception cref="InvalidCastException">
    ''' Throws an exception if the grand parent object is not of type DatabaseObject and has
    ''' only implemented IDatabaseObject.
    ''' </exception>
    Protected Friend ReadOnly Property GrandParentDistinctValue() As Object
        Get

            Return Me.ParentCollection.Parent.DistinctValue

        End Get
    End Property

    ''' <summary>
    ''' Returns the root container object that this object is a child of.
    ''' </summary>
    ''' <remarks>
    ''' Traverses up the object heirarchy to find the root container class.
    ''' </remarks>
    Protected Friend Function RootContainer(Of TRootContainer As RootContainer)() As TRootContainer

        Return pobjParentCollection.RootContainer(Of TRootContainer)()

    End Function

    ''' <summary>
    ''' Deletes the record from the database associated with this record.
    ''' After which this object becomes invalid.
    ''' The IsSaved property is automtically set to false.
    ''' Performs the same function as IDatabaseObjects.ObjectDelete().
    ''' </summary>
    Protected Sub Delete()

        Dim objMeReference As IDatabaseObject = Me
        Me.ParentDatabase.ObjectDelete(Me.ParentCollection, objMeReference)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Extracts the fields to save to the database from the objItem.SaveFields function.
    ''' The fields are then written to the database using either an SQL INSERT or UPDATE 
    ''' depending on whether the object has already been saved. If the collection has 
    ''' implemented IDatabaseObjects.KeyFieldName then objItem's key is also validated to
    ''' ensure it is not null and unique within the collection.
    ''' If the parent collection has implemented Subset then this object should exist 
    ''' within the parent collection. If not, a duplicate key error may occur if the key 
    ''' is being used in another subset in the same table. If a record is being amended 
    ''' (MyBase.IsSaved is True) then the function will "AND" the parent collection's 
    ''' Subset conditions and the DistinctValue value to create the WHERE clause in the 
    ''' UPDATE statement. Therefore, the combination of the IDatabaseObjects.Subset and 
    ''' IDatabaseObject.DistinctValue conditions MUST identify only one record in the 
    ''' table. Otherwise multiple records will be updated with the same data. If data is
    ''' only inserted and not amended (usually a rare occurance) then this requirement 
    ''' is unnecessary.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' 'Make the inherited "Protected Sub Save" public
    ''' Public Overrides Sub Save()
    ''' 
    '''     MyBase.Save()
    ''' 
    ''' End Sub
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    Protected Overridable Sub Save()

        Me.ParentDatabase.ObjectSave(Me.ParentCollection, Me)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the distinct value that uniquely identifies this object in the 
    ''' database. If a new object is saved or an existing object is loaded then this 
    ''' property is automatically set by the library. 
    ''' Typically, this is the value of an identity or auto increment database field.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Overridable Property DistinctValue() As Object Implements IDatabaseObject.DistinctValue
        Get

            Return pobjDistinctValue

        End Get

        Set(ByVal Value As Object)

            pobjDistinctValue = Value

        End Set
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether this object has been saved to the database. If a new object is 
    ''' saved (which uses an auto increment field) or an existing object is loaded then 
    ''' this property is automatically set to true by the library.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Overridable Property IsSaved() As Boolean Implements IDatabaseObject.IsSaved
        Get

            Return pbIsSaved

        End Get

        Set(ByVal Value As Boolean)

            pbIsSaved = Value

        End Set
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Locks the database record associated with this object by selecting and locking
    ''' the row in the database. Supported in Microsoft SQLServer, Pervasive and MySQL.
    ''' The record lock is released when the transaction is committed or rolled back.
    ''' Throws an exception if not in transaction mode.
    ''' Returns the field values from the record that has been locked.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Function LockRecord() As SQL.SQLFieldValues

        Return Me.ParentDatabase.ObjectLockRecord(Me.ParentCollection, Me)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets and returns the field value from the database record associated with this object.
    ''' Returns DBNull.Value if the field is NULL.
    ''' </summary>
    ''' <param name="strFieldName">
    ''' The name of the database field that is to be read.
    ''' Must be a field in the table associated with this object's record.
    ''' </param>
    ''' <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved.</exception>
    ''' --------------------------------------------------------------------------------
    Protected Function GetFieldValue( _
        ByVal strFieldName As String) As Object

        Return Me.ParentDatabase.ObjectGetFieldValue(Me, strFieldName)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Sets the field value for the database record associated with this object.
    ''' </summary>
    ''' <param name="strFieldName">
    ''' The name of the database field that is to be set.
    ''' Must be a field in the table associated with this object's record.
    ''' </param>
    ''' <param name="objNewValue">
    ''' The new value that the database field it to be set to.
    ''' If Nothing/null then the field is set to NULL.
    ''' </param>
    ''' <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved.</exception>
    ''' --------------------------------------------------------------------------------
    Protected Sub SetFieldValue( _
        ByVal strFieldName As String, _
        ByVal objNewValue As Object)

        Me.ParentDatabase.ObjectSetFieldValue(Me, strFieldName, objNewValue)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Performs a shallow memberwise copy of fields in this and all base classes,
    ''' but does not copy any of the DatabaseObjects fields ensuring that the
    ''' objects are not considered equal.
    ''' </summary>
    ''' <remarks>
    ''' This type and the objCopyTo must be of the same type.
    ''' </remarks>
    ''' --------------------------------------------------------------------------------
    Protected Overridable Sub MemberwiseCopy(ByVal objCopyTo As DatabaseObject)

        Me.MemberwiseCopy(objCopyTo, bCopyReferenceTypes:=True)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Performs a shallow memberwise copy of fields in this and all base classes,
    ''' but does not copy any of the DatabaseObjects fields ensuring that the
    ''' objects are not considered equal.
    ''' </summary>
    ''' <remarks>
    ''' This type and the objCopyTo must be of the same type.
    ''' </remarks>
    ''' <param name="bCopyReferenceTypes">
    ''' Indicates whether reference/object types are also copied.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    Protected Overridable Sub MemberwiseCopy(ByVal objCopyTo As DatabaseObject, ByVal bCopyReferenceTypes As Boolean)

        If Not objCopyTo.GetType.Equals(Me.GetType) Then
            Throw New ArgumentException("Type '" & Me.GetType.Name & "' does not equal type '" & objCopyTo.GetType.Name & "'")
        End If

        For Each objField As Reflection.FieldInfo In Me.GetType.GetFields(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic)
            'Check that the field is not defined in the DatabaseObjects library
            If Not objField.DeclaringType.Assembly.Equals(Reflection.Assembly.GetExecutingAssembly) Then
                If bCopyReferenceTypes OrElse (Not bCopyReferenceTypes AndAlso Not objField.FieldType.IsValueType) Then
                    objField.SetValue(objCopyTo, objField.GetValue(Me))
                End If
            End If
        Next

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should copy the database fields from objFields to this object's variables. 
    ''' objFields is populated with all of the fields from the associated record.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Sub LoadFields(ByVal objFields As DatabaseObjects.SQL.SQLFieldValues)
    ''' 
    '''     pstrCode = objFields("ProductCode").Value
    '''     pstrDescription = objFields("ProductDescription").Value
    ''' 
    ''' End Sub
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected MustOverride Sub LoadFields(ByVal objFields As SQL.SQLFieldValues) Implements IDatabaseObject.LoadFields


    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return an SQLFieldValues object populated with the  
    ''' fields to be written to the database. The first argument of the 
    ''' SQLFieldValues.Add function is the database field name, the second is the 
    ''' field's value.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function SaveFields() As DatabaseObjects.SQL.SQLFieldValues
    ''' 
    '''     SaveFields = New SQL.SQLFieldValues
    '''     SaveFields.Add("ProductCode", pstrCode)
    '''     SaveFields.Add("ProductDescription", pstrDescription)
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected MustOverride Function SaveFields() As SQL.SQLFieldValues Implements IDatabaseObject.SaveFields

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Compares whether two objects are equal using the distinct values of each object.
    ''' Specifically, true is returned if the distinct values match, both objects are
    ''' not Nothing and both object types are the same.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Overrides Function Equals(ByVal obj As Object) As Boolean

        If TypeOf obj Is IDatabaseObject Then
            Return AreEqual(Me, DirectCast(obj, IDatabaseObject))
        Else
            Return False
        End If

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Compares whether two objects are equal using the distinct values of each object.
    ''' Specifically, true is returned if the distinct values match, both objects are
    ''' not Nothing and both object types are the same.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Shared Operator =(ByVal objItem1 As DatabaseObject, ByVal objItem2 As DatabaseObject) As Boolean

        Return AreEqual(objItem1, objItem2)

    End Operator

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Compares whether two objects are equal using the distinct values of each object.
    ''' Specifically, true is returned if the distinct values match, both objects are
    ''' not Nothing and both object types are the same.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Shared Operator =(ByVal objItem1 As DatabaseObject, ByVal objItem2 As IDatabaseObject) As Boolean

        Return AreEqual(objItem1, objItem2)

    End Operator

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Compares whether two objects are equal using the distinct values of each object.
    ''' Specifically, true is returned if the distinct values match, both objects are
    ''' not Nothing and both object types are the same.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Shared Operator =(ByVal objItem1 As IDatabaseObject, ByVal objItem2 As DatabaseObject) As Boolean

        Return AreEqual(objItem1, objItem2)

    End Operator

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Compares whether two objects are not equal using the distinct values of each object.
    ''' Specifically, true is returned if the distinct values do not match, an object is
    ''' Nothing or both object types are not the same.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Shared Operator <>(ByVal objItem1 As DatabaseObject, ByVal objItem2 As DatabaseObject) As Boolean

        Return Not AreEqual(objItem1, objItem2)

    End Operator

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Compares whether two objects are not equal using the distinct values of each object.
    ''' Specifically, true is returned if the distinct values do not match, an object is
    ''' Nothing or both object types are not the same.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Shared Operator <>(ByVal objItem1 As DatabaseObject, ByVal objItem2 As IDatabaseObject) As Boolean

        Return Not AreEqual(objItem1, objItem2)

    End Operator

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Compares whether two objects are not equal using the distinct values of each object.
    ''' Specifically, true is returned if the distinct values do not match, an object is
    ''' Nothing or both object types are not the same.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Shared Operator <>(ByVal objItem1 As IDatabaseObject, ByVal objItem2 As DatabaseObject) As Boolean

        Return Not AreEqual(objItem1, objItem2)

    End Operator
 
    Private Shared Function AreEqual(ByVal objItem1 As IDatabaseObject, ByVal objItem2 As IDatabaseObject) As Boolean

        If objItem1 Is Nothing AndAlso objItem2 Is Nothing Then
            'If both of the objects are nothing then they are considered equal
            Return True
        ElseIf objItem1 Is Nothing Or objItem2 Is Nothing Then
            'If one of the objects is nothing then they cannot be equal
            Return False
        ElseIf objItem1 Is objItem2 Then
            'If both objects are the same. Test this before the type and distinct value check
            'because the object may not have been saved, but true should still be returned.
            Return True
        ElseIf Not (objItem1.IsSaved And objItem2.IsSaved) Then
            'If one of the objects have not been saved then the objects must be different.
            'Because the object equality would have returned true in the "objItem1 Is objItem2" test above.
            Return False
        ElseIf CType(objItem1, Object).GetType.Equals(CType(objItem2, Object).GetType) Then
            If IsNumeric(objItem1.DistinctValue) Then
                'If the objects are the same type then check whether their distinct numeric values are equal
                'casting to decimal because INT IDENTITY fields in SQL SERVER returns DECIMAL
                'whereas the actual value when read from a SELECT is INT
                Return CDec(objItem1.DistinctValue) = (CDec(objItem2.DistinctValue))
            Else
                'If the objects are the same type then check whether their distinct values are equal
                Return objItem1.DistinctValue.Equals(objItem2.DistinctValue)
            End If
        Else
            'Return false because the object types are not equal
            Return False
        End If

    End Function

    Private Shared Function IsNumeric(ByVal objValue As Object) As Boolean

        If TypeOf objValue Is IConvertible Then
            Dim eTypeCode As System.TypeCode = DirectCast(objValue, IConvertible).GetTypeCode()
            Return TypeCode.Char <= eTypeCode And eTypeCode <= TypeCode.Decimal
        Else
            Return False
        End If

    End Function

End Class
