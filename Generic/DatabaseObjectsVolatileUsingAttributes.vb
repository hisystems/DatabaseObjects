
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace Generic

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This class is a generic version of DatabaseObjectsVolatile. 
    ''' This class stores all objects associated with this DatabaseObjects collection in 
    ''' memory. Any objects added via VolatileObjectAdd or VolatileObjectDelete only 
    ''' affect the memory list until VolatileObjectsSave() is called. 
    ''' VolatileObjectsSave() will delete any objects flagged for deletion via VolatileObjectDelete()
    ''' and then save any pre-loaded or newly added objects via VolatileObjectsAdd() 
    ''' to the database.
    ''' Item objects can implement IDatabaseObjectVolatile to override the default saving
    ''' behaviour of VolatileObjectsSave().
    ''' Furthermore, rather than overriding the DatabaseObjects' functions, 
    ''' attributes can be used to specify the database specific information using
    ''' class attributes DistinctField, KeyField, OrderBy, Subset, Table and TableJoin.
    ''' </summary>
    ''' <example>
    ''' <code>
    '''   &lt;DistinctField("InvoiceLineID", bAutoIncrements:=True)&gt; _
    '''   &lt;Table("InvoiceLines")&gt; _
    '''   Public Class InvoiceLines
    '''      Inherits Generic.DatabaseObjectsVolatileUsingAttributes(Of InvoiceLine)
    '''      ...
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    Public MustInherit Class DatabaseObjectsVolatileUsingAttributes(Of T As IDatabaseObject)
        Inherits Generic.DatabaseObjectsVolatile(Of T)

        Private pobjAttributeHelper As DatabaseObjectsUsingAttributesHelper

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance with the parent that it is associated with.
        ''' </summary>
        ''' 
        ''' <param name="objParent">
        ''' The parent collection that this collection is associated with. This is often
        ''' useful so that the SubSet property can use the Parent to filter 
        ''' by a particular value pertinent to the parent object. 
        ''' </param>
        ''' 
        ''' --------------------------------------------------------------------------------
        Protected Sub New(ByVal objParent As DatabaseObject)

            MyBase.New(objParent)

        End Sub

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance with it's associated database.
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
        ''' Initializes a new instance with it's associated database and data that 
        ''' can be used for specifying a subset.
        ''' </summary>
        ''' <remarks>
        ''' The Data propety is set before the items list is loaded.
        ''' </remarks>
        ''' 
        ''' <param name="objDatabase">
        ''' The database that this collection is associated with.
        ''' </param>
        ''' 
        ''' <param name="objData">
        ''' An additional object that is usually required so that it can be used 
        ''' as a filter in the SubSet function.
        ''' </param>
        ''' --------------------------------------------------------------------------------
        Protected Sub New(ByVal objDatabase As Database, ByVal objData As Object)

            MyBase.New(objDatabase, objData)

        End Sub

        ''' <summary>
        ''' Must override VolatileItemsLoad so that the attributes can be read before
        ''' any of the objects are loaded.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overrides Sub VolatileItemsLoad()

            pobjAttributeHelper = New DatabaseObjectsUsingAttributesHelper(Me)
            MyBase.VolatileItemsLoad()

        End Sub

#If UseAutoAssignment Then

        Protected Overrides Function DistinctFieldAutoAssignment() As SQL.FieldValueAutoAssignmentType

            Return pobjAttributeHelper.DistinctFieldAutoAssignment

        End Function

#Else

        <Obsolete()> _
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
