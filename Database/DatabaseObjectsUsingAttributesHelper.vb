
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

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
    Private pobjTableJoin As TableJoinAttribute = Nothing
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
                    pobjTableJoin = DirectCast(objAttribute, TableJoinAttribute)
                ElseIf TypeOf objAttribute Is ItemInstanceAttribute Then
                    pobjItemInstance = DirectCast(objAttribute, ItemInstanceAttribute)
                End If
            Next
        End If

        If pobjDistinctField Is Nothing Then
            Throw New Exceptions.DatabaseObjectsException("DistinctField attribute has not been specified.")
        ElseIf pobjTable Is Nothing Then
            Throw New Exceptions.DatabaseObjectsException("Table attribute has not been specified.")
        End If

    End Sub

    Public Function ItemInstance() As IDatabaseObject

        Dim itemInstanceType As Type

        If pobjItemInstance Is Nothing Then
            Try
                itemInstanceType = DatabaseObjectsItemInstance.GetGenericCollectionTArgument(pobjDatabaseObjects.GetType)
            Catch ex As Exceptions.DatabaseObjectsException
                Throw New Exceptions.DatabaseObjectsException("ItemInstanceAttribute has not been specified and " + ex.Message)
            End Try
        Else
            itemInstanceType = pobjItemInstance.Type
        End If

        Return DatabaseObjectsItemInstance.CreateItemInstance(itemInstanceType, pobjDatabaseObjects)

    End Function

    Public ReadOnly Property DistinctFieldName() As String
        Get

            Return pobjDistinctField.Name

        End Get
    End Property

#If UseAutoAssignment Then

    Public ReadOnly Property DistinctFieldAutoAssignment() As SQL.FieldValueAutoAssignmentType
        Get

            Return pobjDistinctField.AutomaticAssignment

        End Get
    End Property

#Else

    Public ReadOnly Property DistinctFieldAutoIncrements() As Boolean
        Get

            Return pobjDistinctField.AutomaticAssignment = SQL.FieldValueAutoAssignmentType.AutoIncrement

        End Get
    End Property

#End If

    Public ReadOnly Property TableName() As String
        Get

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

        'If attribute was not specified 
        If pobjTableJoin Is Nothing Then
            Return Nothing
        Else
            Dim objTableJoins As SQL.SQLSelectTableJoins = New SQL.SQLSelectTableJoins

            With objTableJoins.Add(objPrimaryTable, SQL.SQLSelectTableJoin.Type.Inner, objTables.Add(pobjTableJoin.ToTableName))
                .Where.Add(pobjTableJoin.FieldName, SQL.ComparisonOperator.EqualTo, pobjTableJoin.ToFieldName)
            End With

            Return objTableJoins
        End If

    End Function


End Class
