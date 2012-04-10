
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On
Option Infer On

Imports System.Linq
Imports System.Reflection

''' --------------------------------------------------------------------------------
''' <summary>
''' Used by all of the DatabaseObjects*UsingAttributes classes.
''' All DatabaseObjects*UsingAttributes classes can use attributes to specify the 
''' IDatabaseObjects.* functions that are normally overridden.
''' But using attributes provides a nice and cleaner way of doing this - although
''' sometimes it is a bit limiting and the functions sometimes need to be overriden
''' and attributes not used.
''' </summary>
''' --------------------------------------------------------------------------------
Friend Class DatabaseObjectsUsingAttributesHelper

    Private pobjDatabaseObjects As DatabaseObjects
    Private pobjDistinctField As DistinctFieldAttribute = Nothing
    Private pobjTable As TableAttribute = Nothing
    Private pobjKeyField As KeyFieldAttribute = Nothing
    Private pobjOrderByAttributes As New System.Collections.Generic.List(Of OrderByFieldAttribute)
    Private pobjSubset As SubsetAttribute = Nothing
    Private pobjTableJoins As New System.Collections.Generic.List(Of TableJoinAttribute)
    Private pobjItemInstance As ItemInstanceAttribute = Nothing

    Public Sub New(ByVal objDatabaseObjects As DatabaseObjects)

        If objDatabaseObjects Is Nothing Then
            Throw New ArgumentNullException
        End If

        pobjDatabaseObjects = objDatabaseObjects

        Dim objAttributes As Object() = objDatabaseObjects.GetType.GetCustomAttributes(True)

        If Not objAttributes Is Nothing Then
            For Each objAttribute As Attribute In objAttributes
                If TypeOf objAttribute Is DistinctFieldAttribute Then
                    pobjDistinctField = DirectCast(objAttribute, DistinctFieldAttribute)
                ElseIf TypeOf objAttribute Is KeyFieldAttribute Then
                    pobjKeyField = DirectCast(objAttribute, KeyFieldAttribute)
                ElseIf TypeOf objAttribute Is OrderByFieldAttribute Then
                    pobjOrderByAttributes.Add(DirectCast(objAttribute, OrderByFieldAttribute))
                ElseIf TypeOf objAttribute Is SubsetAttribute Then
                    pobjSubset = DirectCast(objAttribute, SubsetAttribute)
                ElseIf TypeOf objAttribute Is TableAttribute Then
                    pobjTable = DirectCast(objAttribute, TableAttribute)
                ElseIf TypeOf objAttribute Is TableJoinAttribute Then
                    pobjTableJoins.Add(DirectCast(objAttribute, TableJoinAttribute))
                ElseIf TypeOf objAttribute Is ItemInstanceAttribute Then
                    pobjItemInstance = DirectCast(objAttribute, ItemInstanceAttribute)
                End If
            Next
        End If

    End Sub

    Public Function ItemInstance() As IDatabaseObject

        Dim itemInstanceType = GetItemInstanceType()

        Return DatabaseObjectsItemInstance.CreateItemInstance(itemInstanceType, pobjDatabaseObjects)

    End Function

    ''' <summary>
    ''' Returns the item instance type to instantiate.
    ''' It may be defined as a T argument on a generic collection or explicitly
    ''' via the ItemInstance attribute.
    ''' </summary>
    Private Function GetItemInstanceType() As Type

        If pobjItemInstance Is Nothing Then
            Try
                Return DatabaseObjectsItemInstance.GetGenericCollectionTArgument(pobjDatabaseObjects.GetType)
            Catch ex As Exceptions.DatabaseObjectsException
                Throw New Exceptions.DatabaseObjectsException("ItemInstanceAttribute has not been specified and " + ex.Message)
            End Try
        Else
            Return pobjItemInstance.Type
        End If

    End Function

    Public ReadOnly Property DistinctFieldName() As String
        Get

            If pobjDistinctField Is Nothing Then
                Throw New Exceptions.DatabaseObjectsException("DistinctFieldAttribute has not been specified on " & pobjDatabaseObjects.GetType.FullName)
            End If

            Return pobjDistinctField.Name

        End Get
    End Property

#If UseAutoAssignment Then

    Public ReadOnly Property DistinctFieldAutoAssignment() As SQL.FieldValueAutoAssignmentType
        Get

            If pobjDistinctField Is Nothing Then
                Throw New Exceptions.DatabaseObjectsException("DistinctFieldAttribute has not been specified on " & pobjDatabaseObjects.GetType.FullName)
            End If

            Return pobjDistinctField.AutomaticAssignment

        End Get
    End Property

#Else

    Public ReadOnly Property DistinctFieldAutoIncrements() As Boolean
        Get

            If pobjDistinctField Is Nothing Then
                Throw New Exceptions.DatabaseObjectsException("DistinctFieldAttribute has not been specified on " & pobjDatabaseObjects.GetType.FullName)
            End If

            Return pobjDistinctField.AutomaticAssignment = SQL.FieldValueAutoAssignmentType.AutoIncrement

        End Get
    End Property

#End If

    Public ReadOnly Property TableName() As String
        Get

            If pobjTable Is Nothing Then
                Throw New Exceptions.DatabaseObjectsException("TableAttribute has not been specified on " & pobjDatabaseObjects.GetType.FullName)
            End If

            Return pobjTable.Name

        End Get
    End Property

    Public ReadOnly Property KeyFieldName() As String
        Get

            'If attribute was not specified 
            If pobjKeyField Is Nothing Then
                Return String.Empty
            Else
                Return pobjKeyField.Name
            End If

        End Get
    End Property

    Public ReadOnly Property OrderBy() As SQL.SQLSelectOrderByFields
        Get

            'If no attributes were specified 
            If pobjOrderByAttributes.Count = 0 Then
                Return Nothing
            Else
                Dim objOrderBy As New SQL.SQLSelectOrderByFields

                For Each objOrderByAttribute As OrderByFieldAttribute In pobjOrderByAttributes.OrderBy(Function(item) item.Precendence)
                    objOrderBy.Add(objOrderByAttribute.Name, objOrderByAttribute.Ordering)
                Next

                Return objOrderBy
            End If

        End Get
    End Property

    Public ReadOnly Property Subset() As SQL.SQLConditions
        Get

            'If attribute was not specified 
            If pobjSubset Is Nothing Then
                Return Nothing
            Else

                If pobjDatabaseObjects.Parent Is Nothing Then
                    Throw New Exceptions.DatabaseObjectsException("Subset attribute requires the collection to have valid parent in order to obtain the value to subset the collection. Calls DatabaseObjects.Parent.DistinctValue")
                End If

                Dim objConditions As New SQL.SQLConditions

                objConditions.Add(pobjSubset.FieldName, SQL.ComparisonOperator.EqualTo, _
                    DirectCast(pobjDatabaseObjects.Parent, IDatabaseObject).DistinctValue)

                Return objConditions
            End If

        End Get
    End Property

    Public Function TableJoins(ByVal objPrimaryTable As SQL.SQLSelectTable, ByVal objTables As SQL.SQLSelectTables) As SQL.SQLSelectTableJoins

        Dim tableJoinsCollection As SQL.SQLSelectTableJoins

        'If attribute was not specified 
        If pobjTableJoins.Count > 0 Then
            tableJoinsCollection = TableJoinsFromAttributes(pobjTableJoins.ToArray, objPrimaryTable, objTables)
        Else
            'If the ObjectReferenceEarlyBindingAttribute is specified on the item instance then create the table joins that will be required
            Dim earlyBindingTableJoins = ObjectReferenceEarlyBinding.GetTableJoins(pobjDatabaseObjects, objPrimaryTable, Me.GetItemInstanceType)

            If earlyBindingTableJoins.Count > 0 Then
                tableJoinsCollection = earlyBindingTableJoins
            Else
                tableJoinsCollection = Nothing
            End If
        End If

        Return tableJoinsCollection

    End Function

    Private Shared Function TableJoinsFromAttributes(tableJoinAttributes As TableJoinAttribute(), ByVal primaryTable As SQL.SQLSelectTable, ByVal tables As SQL.SQLSelectTables) As SQL.SQLSelectTableJoins

        Dim tableJoinsCollection As New SQL.SQLSelectTableJoins

        Dim leftTable As SQL.SQLSelectTableBase = primaryTable
        Dim leftTableName As String = primaryTable.Name

        For Each tableJoinAttribute In tableJoinAttributes
            Dim rightTable As SQL.SQLSelectTableBase = tables.Add(tableJoinAttribute.ToTableName)
            Dim tableJoin = tableJoinsCollection.Add(leftTable, SQL.SQLSelectTableJoin.Type.Inner, rightTable)
            tableJoin.Where.Add(New SQL.SQLFieldExpression(New SQL.SQLSelectTable(leftTableName), tableJoinAttribute.FieldName), SQL.ComparisonOperator.EqualTo, New SQL.SQLFieldExpression(New SQL.SQLSelectTable(tableJoinAttribute.ToTableName), tableJoinAttribute.ToFieldName))
            leftTable = tableJoin
            leftTableName = tableJoinAttribute.ToTableName
        Next

        Return tableJoinsCollection

    End Function


End Class
