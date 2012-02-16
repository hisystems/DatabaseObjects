
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace Generic

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This class extends DatabaseObjects and wraps all calls with the type T associated 
    ''' this collection. For more information see DatabaseObjects.DatabaseObjects.
    ''' Furthermore, rather than overriding the DatabaseObjects' functions, 
    ''' attributes can be used to specify the database specific information using
    ''' class attributes DistinctField, KeyField, OrderBy, Subset, Table and TableJoin.
    ''' </summary>
    ''' <example>
    ''' <code>
    '''   &lt;DistinctField("CustomerID", bAutoIncrements:=True)&gt; _
    '''   &lt;Table("Customers")&gt; _
    '''   Public Class Customers
    '''      Inherits Generic.DatabaseObjectsUsingAttributes(Of Customer)
    '''      ...
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Public MustInherit Class DatabaseObjectsUsingAttributes(Of T As IDatabaseObject)
        Inherits Generic.DatabaseObjects(Of T)

        Private pobjAttributeHelper As DatabaseObjectsUsingAttributesHelper

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
            pobjAttributeHelper = New DatabaseObjectsUsingAttributesHelper(Me)

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
            pobjAttributeHelper = New DatabaseObjectsUsingAttributesHelper(Me)

        End Sub

#If UseAutoAssignment Then

        Protected Overrides Function DistinctFieldAutoAssignment() As SQL.FieldValueAutoAssignmentType

            Return pobjAttributeHelper.DistinctFieldAutoAssignment

        End Function

#Else

        Protected Overrides Function DistinctFieldAutoIncrements() As Boolean

            Return pobjAttributeHelper.DistinctFieldAutoIncrements

        End Function

#End If

        Protected Overrides Function DistinctFieldName() As String

            Return pobjAttributeHelper.DistinctFieldName

        End Function

        Protected Overrides Function KeyFieldName() As String

            Return pobjAttributeHelper.KeyFieldName

        End Function

        Protected Overrides Function OrderBy() As SQL.SQLSelectOrderByFields

            Return pobjAttributeHelper.OrderBy

        End Function

        Protected Overrides Function Subset() As SQL.SQLConditions

            Return pobjAttributeHelper.Subset

        End Function

        Protected Overrides Function TableJoins(ByVal objPrimaryTable As SQL.SQLSelectTable, ByVal objTables As SQL.SQLSelectTables) As SQL.SQLSelectTableJoins

            Return pobjAttributeHelper.TableJoins(objPrimaryTable, objTables)

        End Function

        Protected Overrides Function TableName() As String

            Return pobjAttributeHelper.TableName

        End Function

        Protected Overrides Function ItemInstance_() As T

            Return DirectCast(pobjAttributeHelper.ItemInstance, T)

        End Function

    End Class

End Namespace
