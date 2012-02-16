
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
''' An IDatabaseObjects object represents either a database table or a sub set of a 
''' database table. The Database class provides the facility for classes implementing 
''' IDatabaseObjects to specify the database table the class is bound to, how to
''' identify each unique record within the table (or subset), how the table should
''' be sorted and table joins (in order to improve performance). Rather than
''' directly implementing IDatabaseObjects inherit from DatabaseObjects 
''' as the DatabaseObjects class provides the basic "plumbing" code required by 
''' IDatabaseObjects.
''' </summary>
''' --------------------------------------------------------------------------------
''' 
Public Interface IDatabaseObjects

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return an instance of the class that is associated with this 
    ''' collection of objects. The associated class must implement the IDatabaseObject interface.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function ItemInstance() As IDatabaseObject Implements IDatabaseObjects.ItemInstance
    ''' 
    '''     Return New Product
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    Function ItemInstance() As IDatabaseObject

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return the conditions that define the collection's subset. 
    ''' If the collection should include the entire table then this function should return Nothing.
    ''' Implementing this function is optional.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function Subset() As SQL.SQLConditions Implements IDatabaseObjects.Subset
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
    Function Subset() As SQL.SQLConditions

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return the name of the table associated with this collection. 
    ''' This function should almost always be implemented.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function TableName() As String Implements IDatabaseObjects.TableName
    ''' 
    '''     Return "Products"
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>  
    ''' --------------------------------------------------------------------------------
    Function TableName() As String

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This property should return the field name that uniquely identifies each object 
    ''' within the collection. As opposed to the ordinal/index position, the key field 
    ''' provides another method of accessing a particular object within the collection. 
    ''' The key field must be unique within the collection. If the IDatabaseObjects.Subset 
    ''' function has been implemented then the key field only needs to be unique within 
    ''' the specified subset, not the entire table. Implementing this function is optional.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function KeyFieldName() As String Implements IDatabaseObjects.KeyFieldName
    ''' 
    '''     Return "ProductCode"
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>   
    ''' --------------------------------------------------------------------------------
    Function KeyFieldName() As String

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return the field name that uniquely identifies each object 
    ''' within the collection. Typically, this is the field name of an identity or auto 
    ''' increment field. If the IDatabaseObjects.SubSet function has been implemented 
    ''' then the DistinctFieldName need only be unique within the subset not the 
    ''' entire table. The DistinctFieldName and KeyFieldName can be identical. This 
    ''' function should almost always be implemented.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function DistinctFieldName() As String Implements IDatabaseObjects.DistinctFieldName
    ''' 
    '''     Return "ProductID"
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>  
    ''' --------------------------------------------------------------------------------
    Function DistinctFieldName() As String

#If UseAutoAssignment Then
    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return whether the Distinct field as specified in the 
    ''' associated collection's IDatabaseObject.DistinctField is an identity
    ''' (Autonumber in Microsoft Access) or is a unique identifier field. 
    ''' If set to either value then the IDatabaseObject.DistinctValue value is 
    ''' automatically set when a new object is saved.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function DistinctFieldAutoAssignment() As SQL.FieldValueAutoAssignmentType Implements IDatabaseObjects.DistinctFieldAutoAssignment
    ''' 
    '''     Return SQL.FieldValueAutoAssignmentType.AutoIncrement
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>  
    ''' --------------------------------------------------------------------------------
    Function DistinctFieldAutoAssignment() As SQL.FieldValueAutoAssignmentType
#Else
    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return whether the Distinct field as specified in the 
    ''' associated collection's IDatabaseObject.DistinctField is an identity (SQL Server) 
    ''' or autonumber (Microsoft Access) field. If set to true, then the 
    ''' IDatabaseObject.DistinctValue value is set when a new object is saved.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function DistinctFieldAutoIncrements() As Boolean Implements IDatabaseObjects.DistinctFieldAutoIncrements
    ''' 
    '''     Return True
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>  
    ''' --------------------------------------------------------------------------------
    <Obsolete(DatabaseObjects.DistinctFieldAutoIncrementsObsoleteWarningMessage)> _
    Function DistinctFieldAutoIncrements() As Boolean
#End If


    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return an SQLSelectOrderByFields object containing the list 
    ''' of fields the collection will be sorted by. Just as with an SQL statement, the 
    ''' order of the fields added to the collection indicates the group sorting. If 
    ''' IDatabaseObjects.TableJoins has been implemented then fields from the adjoining 
    ''' table or tables can be utilized. The sort order is used by the ObjectByOrdinal, 
    ''' ObjectByOrdinalFirst and ObjectsSearch functions. 
    ''' Should return Nothing if no ordering is required.
    ''' Implementing this function is optional.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function OrderBy() As SQL.SQLSelectOrderByFields Implements IDatabaseObjects.OrderBy
    ''' 
    '''     OrderBy = New SQL.SQLSelectOrderByFields
    '''     OrderBy.Add("ProductCode", SQL.OrderBy.Ascending)
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>   
    ''' --------------------------------------------------------------------------------
    Function OrderBy() As SQL.SQLSelectOrderByFields

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
    ''' Protected Function TableJoins(ByVal objPrimaryTable As SQL.SQLSelectTable, ByVal objTables As SQL.SQLSelectTables) As SQL.SQLSelectTableJoins Implements IDatabaseObjects.TableJoins
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
    Function TableJoins(ByVal objPrimaryTable As SQL.SQLSelectTable, ByVal objTables As SQL.SQLSelectTables) As SQL.SQLSelectTableJoins

End Interface
