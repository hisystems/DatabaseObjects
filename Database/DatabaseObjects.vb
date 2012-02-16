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
''' This class can be used in conjunction with the DatabaseObject class to simplify
''' the process of using the DatabaseObjects library. This class implements the 
''' IDatabaseObjects interface and provides the basic "plumbing" code required by 
''' the interface. For this reason, inheriting from this class is preferable to 
''' implementing the IDatabaseObjects interface directly.
''' </summary>
''' --------------------------------------------------------------------------------
''' 
Public MustInherit Class DatabaseObjects
    Implements IDatabaseObjects

    Friend Const DistinctFieldAutoIncrementsObsoleteWarningMessage As String = _
        "Deprecated and replaced by DistinctFieldAutoAssignment to support both auto increment and automatically assigned globally unique identifiers. The DistinctFieldAutoIncrements function will be removed in future. " & _
        "To migrate enable compilation constant UseAutoAssignment in DatabaseObjects project and modify all code by using Visual Studio's search and replace with the following regular expressions:" & Microsoft.VisualBasic.ControlChars.CrLf & _
        "Replace: " & _
            """DistinctFieldAutoIncrements\(\):bAs:bBoolean[\n:b]*Return:bTrue"" with " & _
            """DistinctFieldAutoAssignment() As DatabaseObjects.SQL.FieldValueAutoAssignmentType\n\nReturn DatabaseObjects.SQL.FieldValueAutoAssignmentType.AutoIncrement""." & Microsoft.VisualBasic.ControlChars.CrLf & _
        "And replace: " & _
            """DistinctFieldAutoIncrements\(\):bAs:bBoolean[\n:b]*Return:bFalse"" with " & _
            """DistinctFieldAutoAssignment() As DatabaseObjects.SQL.FieldValueAutoAssignmentType\n\nReturn DatabaseObjects.SQL.FieldValueAutoAssignmentType.None"""

    Private pobjDatabase As Database
    Private pobjParent As DatabaseObject

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

        If objDatabase Is Nothing Then
            Throw New ArgumentNullException
        End If

        pobjDatabase = objDatabase

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

        If objParent Is Nothing Then
            Throw New ArgumentNullException
        End If

        pobjDatabase = objParent.ParentDatabase
        pobjParent = objParent

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the parent object that this collection is associated with.
    ''' This property will return Nothing if the 'New(DatabaseObjects.Database)'
    ''' constructor is used.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Friend ReadOnly Property Parent() As DatabaseObject
        Get

            Return pobjParent

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the distinct value of the parent i.e. Parent.DistinctValue
    ''' Throws a NullReferenceException if there is no parent.
    ''' </summary>
    ''' <exception cref="NullReferenceException">Parent is nothing</exception>
    ''' --------------------------------------------------------------------------------
    Protected Friend ReadOnly Property ParentDistinctValue() As Object
        Get

            Return DirectCast(Me.Parent, IDatabaseObject).DistinctValue

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the database associated with this collection/database table.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Friend ReadOnly Property ParentDatabase() As Database
        Get

            Return pobjDatabase

        End Get
    End Property

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an instance of an object from this collection using a distinct value as 
    ''' specified by DistinctFieldName. If Subset has been implemented then the objDistinctValue 
    ''' need only be unique within the subset specified, not the entire database table.
    ''' </summary>
    ''' 
    ''' <param name="objDistinctValue">
    ''' The value that uniquely identifies the object within this collection. This is the value
    ''' of the field defined by this collection's DistinctFieldName function.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example> Loads a product using a product ID of 123
    ''' <code>
    ''' Dim objProduct As Product = MyBase.Object(123)
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Function [Object](ByVal objDistinctValue As Object) As IDatabaseObject

        Return Me.ParentDatabase.Object(Me, objDistinctValue)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an instance of an object from this collection using a distinct value as 
    ''' specified by DistinctFieldName. If Subset has been implemented then the objDistinctValue 
    ''' need only be unique within the subset specified, not the entire database table.
    ''' Returns Nothing/null if the distinct value does not exist in the database.
    ''' This feature is what differentiates DatabaseObjects.Object() from DatabaseObjects.ObjectIfExists().
    ''' </summary>
    ''' 
    ''' <param name="objDistinctValue">
    ''' The value that uniquely identifies the object within this collection. This is the value
    ''' of the field defined by this collection's DistinctFieldName function.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example> Loads a product using a product ID of 123
    ''' <code>
    ''' Dim objProduct As Product = MyBase.Object(123)
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Function ObjectIfExists(ByVal objDistinctValue As Object) As IDatabaseObject

        Return Me.ParentDatabase.ObjectIfExists(Me, objDistinctValue)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the database fields for an object from the collection using a distinct value 
    ''' (see IDatabaseObjects.DistinctFieldName). If the collection has implemented the 
    ''' IDatabaseObjects.Subset function then the objDistinctValue need only be unique 
    ''' within the collection's subset, not the entire database table.
    ''' This is typically used to interogate the database fields before loading the 
    ''' object with a call to ObjectFromFieldValues.
    ''' This function is rarely used and generally the Object function suffices.
    ''' </summary>
    ''' 
    ''' <param name="objDistinctValue">
    ''' The value that uniquely identifies the object within the collection. This is the value
    ''' of the field defined by the collection's IDatabaseObjects.DistinctFieldName function.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' --------------------------------------------------------------------------------
    Protected Function ObjectFieldValues(ByVal objDistinctValue As Object) As SQL.SQLFieldValues

        Return Me.ParentDatabase.ObjectFieldValues(Me, objDistinctValue)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an object exists for the specified distinct value in the collection.
    ''' </summary>
    ''' 
    ''' <param name="objDistinctValue">
    ''' The value to search for in the collection. This is the value of the field defined 
    ''' by the collection's IDatabaseObjects.DistinctFieldName function.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Function ObjectExistsByDistinctValue(ByVal objDistinctValue As Object) As Boolean

        Return Me.ParentDatabase.ObjectExistsByDistinctValue(Me, objDistinctValue)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Extracts the fields to save to the database from the objItem.SaveFields function.
    ''' The fields are then written to the database using either an SQL INSERT or UPDATE 
    ''' depending on whether the object has already been saved.
    ''' </summary>
    ''' 
    ''' <param name="objItem">
    ''' The object to save to the database. The values saved to the database are extracted from the 
    ''' SQLFieldValues object returned from IDatabaseObject.SaveFields.
    ''' </param>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Sub ObjectSave(ByVal objItem As IDatabaseObject)

        Me.ParentDatabase.ObjectSave(Me, objItem)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an object using a unique key value. 
    ''' The key must be unique within this collection. If the collection's DatabaseObjects.Subset
    ''' has been implemented then the key need only be unique within the subset specified, not the 
    ''' entire database table. 
    ''' </summary>
    ''' 
    ''' <param name="objKey">
    ''' The key that identifies the object with this collection. The key is the value of the field
    ''' defined by this collection's KeyFieldName.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example>
    ''' <code>
    ''' Default Public ReadOnly Property Item(ByVal strProductCode As String) As Product
    '''     Get
    ''' 
    '''         Return MyBase.ObjectByKey(strProductCode)
    ''' 
    '''     End Get
    ''' End Property
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Function ObjectByKey(ByVal objKey As Object) As IDatabaseObject

        Return Me.ParentDatabase.ObjectByKey(Me, objKey)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an object using a unique key value. 
    ''' The key must be unique within this collection. If the collection's DatabaseObjects.Subset
    ''' has been implemented then the key need only be unique within the subset specified, not the 
    ''' entire database table. 
    ''' Returns Nothing/null if the object does exist with the specified key.
    ''' This feature is what differentiates DatabaseObjects.ObjectByKey() from DatabaseObjects.ObjectByKeyExists().
    ''' </summary>
    ''' 
    ''' <param name="objKey">
    ''' The key that identifies the object with this collection. The key is the value of the field
    ''' defined by this collection's KeyFieldName.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example>
    ''' <code>
    ''' Default Public ReadOnly Property Item(ByVal strProductCode As String) As Product
    '''     Get
    ''' 
    '''         Return MyBase.ObjectByKey(strProductCode)
    ''' 
    '''     End Get
    ''' End Property
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Function ObjectByKeyIfExists(ByVal objKey As Object) As IDatabaseObject

        Return Me.ParentDatabase.ObjectByKeyIfExists(Me, objKey)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' ObjectByOrdinalFirst returns the first object in the collection respectively 
    ''' filtered and sorted by the collection's Subset and OrderBy values. It differs 
    ''' from ObjectByOrdinal in that it only loads the first record from the database 
    ''' table not the entire table.
    ''' </summary>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' 
    ''' <example>
    ''' <code>
    ''' 'Assuming this class is the Suppliers class
    ''' 
    ''' 'Ideal for loading default objects
    ''' Dim objDefaultSupplier As Supplier = MyBase.ObjectByOrdinalFirst
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Function ObjectByOrdinalFirst() As IDatabaseObject

        Return Me.ParentDatabase.ObjectByOrdinalFirst(Me)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the last object in the collection respectively
    ''' filtered and sorted by the collection's Subset and OrderBy values. It differs
    ''' from ObjectByOrdinal in that it only loads the first record from the database
    ''' table not the entire table.
    ''' </summary>
    '''
    ''' <returns><see cref="IDatabaseObject" /> (DatabaseObjects.IDatabaseObject)</returns>
    '''
    ''' <example>
    ''' <code>
    ''' 'Assuming this class is the Suppliers class
    '''
    ''' 'Ideal for loading default objects
    ''' Dim objDefaultSupplier As Supplier = MyBase.ObjectByOrdinalFirst
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    '''
    Protected Function ObjectByOrdinalLast() As IDatabaseObject

        Return Me.ParentDatabase.ObjectByOrdinalLast(Me)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Deletes an object's database record. If this collection's Subset has been 
    ''' implemented then the object must exist within the subset, otherwise the object 
    ''' will not be deleted. If the object has not been saved to the database the function 
    ''' will exit without executing an SQL DELETE command. After deleting the database 
    ''' record the object is set to Nothing. The calling function should receive the 
    ''' object ByRef for this to have any affect. Setting the object to Nothing 
    ''' minimises the possibility of the deleted object being used in code after 
    ''' ObjectDelete has been called.
    ''' </summary>
    ''' 
    ''' <param name="objItem">
    ''' The object to delete. The calling function should receive this object ByRef 
    ''' as the object is set to Nothing after deletion. 
    ''' </param>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Public Sub Delete(ByRef objProduct As Product)
    ''' 
    '''     MyBase.ObjectDelete(objProduct)
    ''' 
    ''' End Sub
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Sub ObjectDelete(ByRef objItem As IDatabaseObject)

        Me.ParentDatabase.ObjectDelete(Me, objItem)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the key exists within the collection. If this collection's Subset 
    ''' has been set then only the subset is searched not the entire table.
    ''' </summary>
    ''' 
    ''' <param name="objKey">
    ''' The key value to search by.
    ''' </param>
    ''' 
    ''' <returns><see cref="Boolean" />	(System.Boolean)</returns>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Public Function Exists(ByVal strProductCode As String) As Boolean
    ''' 
    '''     Return MyBase.ObjectExists(strProductCode)
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    '''
    Protected Function ObjectExists(ByVal objKey As Object) As Boolean

        Return Me.ParentDatabase.ObjectExists(Me, objKey)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Loads an object from the current record of an IDataReader object.
    ''' </summary>
    ''' 
    ''' <param name="objReader">
    ''' The data to be copied into a new DatabaseObject object.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' --------------------------------------------------------------------------------
    '''  
    Protected Function ObjectFromDataReader(ByVal objReader As IDataReader) As IDatabaseObject

        Return Database.ObjectFromDataReader(Me, objReader)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Loads an object from the values contained in an SQLFieldValues object. 
    ''' This function is generally used from within an DatabaseObject.Load function when 
    ''' the TableJoins function has been implemented.
    ''' </summary>
    ''' 
    ''' <param name="objFieldValues">
    ''' The data container from which to load a new object.
    ''' </param>
    ''' 
    ''' <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
    ''' --------------------------------------------------------------------------------
    '''  
    Protected Function ObjectFromFieldValues(ByVal objFieldValues As SQL.SQLFieldValues) As IDatabaseObject

        Return Database.ObjectFromFieldValues(Me, objFieldValues)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an IList object containing all of this collection's objects. This 
    ''' function is useful when loading a set of objects for a subset or for use with 
    ''' the IEnumerable interface. 
    ''' </summary>
    ''' 
    ''' <returns><see cref="Collections.IList" />	(System.Collections.IList)</returns>
    ''' 
    ''' <example> 
    ''' <code>
    ''' 'Alternatively, the DatabaseObjectsEnumerable class can be used which 
    ''' 'automatically incorporates an enumerator
    ''' Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
    ''' 
    '''     Return MyBase.ObjectsList.GetEnumerator
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Function ObjectsList() As IList

        Return Me.ParentDatabase.ObjectsList(Me)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an array of IDatabaseObject objects contained within this collection.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Protected Function ObjectsArray() As IDatabaseObject()

        Return Me.ParentDatabase.ObjectsArray(Me)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an IDictionary object. Each key/value pair contains a key and
    ''' the object associated with the key.
    ''' </summary>
    ''' 
    ''' <returns><see cref="Collections.IDictionary" />	(System.Collections.IDictionary)</returns>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Function ObjectsDictionary() As IDictionary

        Return Me.ParentDatabase.ObjectsDictionary(Me)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an IDictionary object. Each key/value pair contains a distinct 
    ''' value and the object associated with the distinct value.
    ''' </summary>
    ''' 
    ''' <returns><see cref="Collections.IDictionary" />	(System.Collections.IDictionary)</returns>
    ''' --------------------------------------------------------------------------------
    '''
    Protected Function ObjectsDictionaryByDistinctValue() As IDictionary

        Return Me.ParentDatabase.ObjectsDictionaryByDistinctValue(Me)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of items in this collection. If this collection's Subset 
    ''' has been implemented then this function returns the number of records within the 
    ''' subset, not the entire table.
    ''' Also utilises the table joins so that any filters specified on the subset
    ''' can be used.
    ''' </summary>
    ''' 
    ''' <returns><see cref="Int32" />	(System.Int32)</returns>
    ''' 
    ''' <example> 
    ''' <code>
    ''' 'Return the number of items in this collection.
    ''' Public ReadOnly Property Count() As Integer
    '''     Get
    ''' 
    '''         Return MyBase.ObjectsCount
    ''' 
    '''     End Get
    ''' End Property
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    '''
    Protected Function ObjectsCount() As Integer

        Return Me.ParentDatabase.ObjectsCount(Me)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Deletes all of the objects in this collection. If Subset has been implemented 
    ''' then only the objects within the subset are deleted, not the table's entire 
    ''' contents.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected Sub ObjectsDeleteAll()

        Me.ParentDatabase.ObjectsDeleteAll(Me)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Returns a collection of objects that match the specified search criteria. 
    ''' This function utilises any subsets, ordering or table joins specified in this 
    ''' collection. To add a set of conditions to the objSearchCriteria object with 
    ''' higher precendance use the "Add(SQLConditions)" overloaded as this will wrap 
    ''' the conditions within parentheses.
    ''' </summary>
    ''' 
    ''' <param name="objSearchCriteria">
    ''' The criteria to search for within this collection. To add set a of conditions with 
    ''' with higher precendance use the "Add(SQLConditions)" overloaded function as this 
    ''' will wrap the conditions within parentheses.
    ''' </param>
    ''' 
    ''' <returns><see cref="Collections.IList" />	(System.Collections.IList)</returns>
    ''' 
    ''' <remarks>
    ''' The following wildcard characters are used when using the LIKE operator (extract
    ''' from Microsoft Transact-SQL Reference):
    ''' 
    ''' <table width="659" border="1" cellspacing="1" cellpadding="4">
    '''   <tr>
    '''     <th width="16%" height="20">Wildcard character</th>
    '''     <th width="22%">Description</th>
    '''     <th width="62%">Example</th>
    '''   </tr>
    '''   <tr>
    '''     <td>%</td>
    '''     <td>Any string of zero or more characters.</td>
    '''     <td>WHERE title LIKE '%computer%' finds all book titles with the word 
    '''         'computer' anywhere in the book title.</td>
    '''   </tr>
    '''   <tr>
    '''     <td>_ (underscore)</td>
    '''     <td>Any single character.</td>
    '''     <td>WHERE au_fname LIKE '_ean' finds all four-letter first names that end
    '''       with ean (Dean, Sean, and so on).</td>
    '''   </tr>
    ''' </table>
    ''' </remarks>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Public Function Search(ByVal objSearchCriteria As Object, ByVal eType As SearchType) As IList
    ''' 
    '''     Dim objConditions As SQL.SQLConditions = New SQL.SQLConditions
    ''' 
    '''     Select Case eType
    '''         Case SearchType.DescriptionPrefix
    '''             objConditions.Add("ProductName", SQL.ComparisonOperator.Like, objSearchCriteria &amp; "%")
    '''         Case SearchType.Description
    '''             objConditions.Add("ProductName", SQL.ComparisonOperator.Like, "%" &amp; objSearchCriteria &amp; "%")
    '''     End Select
    ''' 
    '''     Return MyBase.ObjectsSearch(objConditions)
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    '''  
    Protected Function ObjectsSearch(ByVal objSearchCriteria As SQL.SQLConditions) As IList

        Return Me.ParentDatabase.ObjectsSearch(Me, objSearchCriteria)

    End Function

#If UseAutoAssignment Then
    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return whether the Distinct field as specified in the 
    ''' associated collection's DatabaseObject.DistinctField is an identity field
    ''' (Autonumber in Microsoft Access) or is a unique identifier field. 
    ''' If set to either value then the IDatabaseObject.DistinctValue value is 
    ''' automatically set when a new object is saved.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function DistinctFieldAutoAssignment() As SQL.FieldValueAutoAssignmentType
    ''' 
    '''     Return SQL.FieldValueAutoAssignmentType.AutoIncrement
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>  
    ''' --------------------------------------------------------------------------------
    Protected MustOverride Function DistinctFieldAutoAssignment() As SQL.FieldValueAutoAssignmentType Implements IDatabaseObjects.DistinctFieldAutoAssignment
#Else
    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return whether the Distinct field as specified in the 
    ''' associated collection's DatabaseObject.DistinctField is an identity field
    ''' (Autonumber in Microsoft Access). If set to true, then the 
    ''' DatabaseObject.DistinctValue value is set when a new object is saved.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function DistinctFieldAutoIncrements() As Boolean
    ''' 
    '''     Return True
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    <Obsolete(DatabaseObjects.DistinctFieldAutoIncrementsObsoleteWarningMessage)> _
    Protected MustOverride Function DistinctFieldAutoIncrements() As Boolean Implements IDatabaseObjects.DistinctFieldAutoIncrements
#End If

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return the field name that uniquely identifies each object 
    ''' within the collection. Typically, this is the field name of an identity or auto 
    ''' increment field. If the DatabaseObjects.SubSet function has been implemented 
    ''' then the DistinctFieldName need only be unique within the subset not the 
    ''' entire table. The DistinctFieldName and KeyFieldName can be identical. This 
    ''' function should almost always be implemented.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function DistinctFieldName() As String
    ''' 
    '''     Return "ProductID"
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected MustOverride Function DistinctFieldName() As String Implements IDatabaseObjects.DistinctFieldName

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return an instance of the class that is associated with this 
    ''' collection of objects. The associated class must implement the IDatabaseObject 
    ''' interface. Typically, a DatabaseObject (implements IDatabaseObject) instance is 
    ''' returned from this function.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function ItemInstance() As IDatabaseObject
    ''' 
    '''     Return New Product
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected MustOverride Function ItemInstance() As IDatabaseObject Implements IDatabaseObjects.ItemInstance

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This property should return the field name that uniquely identifies each object 
    ''' within the collection. As opposed to the ordinal/index position, the key field 
    ''' provides another method of accessing a particular object within the collection. 
    ''' The key field must be unique within the collection. If the DatabaseObjects.Subset 
    ''' function has been implemented then the key field only needs to be unique within 
    ''' the specified subset, not the entire table. Implementing this function is optional.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function KeyFieldName() As String
    ''' 
    '''     Return "ProductCode"
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected MustOverride Function KeyFieldName() As String Implements IDatabaseObjects.KeyFieldName

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return an SQLSelectOrderByFields object containing the list 
    ''' of fields the collection will be sorted by. Just as with an SQL statement, the 
    ''' order of the fields added to the collection indicates the group sorting. If 
    ''' DatabaseObjects.TableJoins has been implemented then fields from the adjoining 
    ''' table or tables can be utilized. The sort order is used by the ObjectByOrdinal, 
    ''' ObjectByOrdinalFirst and ObjectsSearch functions. 
    ''' Should return Nothing if no ordering is required.
    ''' Implementing this function is optional.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function OrderBy() As SQL.SQLSelectOrderByFields
    ''' 
    '''     OrderBy = New SQL.SQLSelectOrderByFields
    '''     OrderBy.Add("ProductCode", SQL.OrderBy.Ascending)
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected MustOverride Function OrderBy() As SQL.SQLSelectOrderByFields Implements IDatabaseObjects.OrderBy

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return the conditions that define the collection's subset. 
    ''' If the collection should include the entire table then this function should return Nothing. 
    ''' Implementing this function is optional.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function Subset() As SQL.SQLConditions
    ''' 
    '''     Dim objConditions As New SQL.SQLConditions
    ''' 
    '''     'Only include products that are in group ID 1234
    '''     objConditions.Add("GroupID", SQL.ComparisonOperator.EqualTo, 1234)
    ''' 
    '''     Return objConditions
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected MustOverride Function Subset() As SQL.SQLConditions Implements IDatabaseObjects.Subset

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return an SQLSelectTableJoins object containing the table 
    ''' or tables to be joined to the primary table. This function is useful in 
    ''' optimising database loading speeds by allowing multiple tables to be joined into 
    ''' one data set. The resultant data set can then be used to load 
    ''' objects from the associated tables avoiding subsequent SQL calls. For a complete 
    ''' example, see the demonstration program. 
    ''' Should return Nothing if no table joins are required.
    ''' Implementing this function is optional.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function TableJoins(ByVal objPrimaryTable As SQL.SQLSelectTable, ByVal objTables As SQL.SQLSelectTables) As SQL.SQLSelectTableJoins
    ''' 
    '''     'Implementing this function is optional, but is useful when attempting to optimise loading speeds.
    '''     'This function is used by the ObjectsList, Object, ObjectByKey, ObjectOrdinal and ObjectSearch functions.
    '''     'If this function has been implemented Search can also search fields in the joined table(s).
    '''     'In this example, the Products table will always be joined with the Supplier table. We could also join the Products
    '''     'table to the Category table, however the Product.Category property is not used often enough to warrant
    '''     'always joining the category table whenever loading a product. Of course, you can always join different
    '''     'tables in different situations, for example you might want join to other tables when searching and to
    '''     'not join other tables in normal circumstances.
    ''' 
    '''     Dim objTableJoins As SQL.SQLSelectTableJoins = New SQL.SQLSelectTableJoins
    ''' 
    '''     With objTableJoins.Add(objPrimaryTable, SQL.SQLSelectTableJoin.Type.Inner, objTables.Add("Suppliers"))
    '''         .Where.Add("SupplierID", SQL.ComparisonOperator.EqualTo, "SupplierID")
    '''     End With
    ''' 
    '''     With objTableJoins.Add(objPrimaryTable, SQL.SQLSelectTableJoin.Type.Inner, objTables.Add("Categories"))
    '''         .Where.Add("CategoryID", SQL.ComparisonOperator.EqualTo, "CategoryID")
    '''     End With
    ''' 
    '''     Return objTableJoins
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected MustOverride Function TableJoins(ByVal objPrimaryTable As SQL.SQLSelectTable, ByVal objTables As SQL.SQLSelectTables) As SQL.SQLSelectTableJoins Implements IDatabaseObjects.TableJoins

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return the name of the table associated with this collection. 
    ''' This function should almost always be implemented.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Overrides Function TableName() As String
    ''' 
    '''     Return "Products"
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    ''' 
    Protected MustOverride Function TableName() As String Implements IDatabaseObjects.TableName

End Class
