
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectTableJoinConditions

        Private pobjLogicalOperators As New Collections.Generic.List(Of LogicalOperator)
        Private pobjConditions As New ArrayList
        Private pobjParent As SQLSelectTableJoin

        Public Sub New()

        End Sub

        Friend Sub New(ByVal objParent As SQLSelectTableJoin)

            MyBase.New()
            pobjParent = objParent

        End Sub

        Public Function Add() As SQLSelectTableJoinCondition

            Return Add("", ComparisonOperator.EqualTo, "")

        End Function

        Public Sub Add(ByVal conditions As SQLSelectTableJoinConditions)

            AddLogicalOperatorIfRequired()
            pobjConditions.Add(conditions)

        End Sub

        Public Function Add( _
            ByVal strLeftTableFieldName As String, _
            ByVal eCompare As ComparisonOperator, _
            ByVal strRightTableFieldName As String) As SQLSelectTableJoinCondition

            If pobjParent Is Nothing Then
                Throw New InvalidOperationException("Use overloaded Add(SQLExpression, ComparisonOperator, SQLExpression) instead")
            End If

            'The Add function is here basically for backward compatibility when the conditions could only accept field names and the left and right tables from the parent join were used as table aliases. 
            'Now that that SQLSelectTableBase is being used (which can represent a table or joined tables) we need to check that parent left and right tables are only SQLSelectTable objects.
            If Not TypeOf pobjParent.LeftTable Is SQLSelectTable Then
                Throw New ArgumentException("The left table is expected to be an SQLSelectTable so that the left table alias can be utilised for the strLeftTableFieldName argument. Use the Add(SQLExpression, ComparisonOperator, SQLExpression) overload instead.")
            ElseIf Not TypeOf pobjParent.RightTable Is SQLSelectTable Then
                Throw New ArgumentException("The right table is expected to be an SQLSelectTable so that the right table alias can be utilised for the strRightTableFieldName argument. Use the Add(SQLExpression, ComparisonOperator, SQLExpression) overload instead.")
            End If

            EnsureComparisonOperatorValid(eCompare)
            AddLogicalOperatorIfRequired()

            Add = New SQLSelectTableJoinCondition(Me)
            Add.LeftExpression = New SQLFieldExpression(DirectCast(pobjParent.LeftTable, SQLSelectTable), strLeftTableFieldName)
            Add.Compare = eCompare
            Add.RightExpression = New SQLFieldExpression(DirectCast(pobjParent.RightTable, SQLSelectTable), strRightTableFieldName)

            pobjConditions.Add(Add)

        End Function

        Public Function Add( _
            ByVal objLeftExpression As SQLExpression, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objRightExpression As SQLExpression) As SQLSelectTableJoinCondition

            EnsureComparisonOperatorValid(eCompare)
            AddLogicalOperatorIfRequired()

            Add = New SQLSelectTableJoinCondition(Me)

            Add.LeftExpression = objLeftExpression
            Add.Compare = eCompare
            Add.RightExpression = objRightExpression

            pobjConditions.Add(Add)

        End Function

        Private Sub AddLogicalOperatorIfRequired()

            'Add the AND operator if an operator hasn't been called after the previous Add call
            If pobjLogicalOperators.Count() < pobjConditions.Count() Then
                Me.AddLogicalOperator(LogicalOperator.And)
            End If

        End Sub

        Private Sub EnsureComparisonOperatorValid(eCompare As ComparisonOperator)

            If eCompare = ComparisonOperator.Like Or eCompare = ComparisonOperator.NotLike Then
                Throw New Exceptions.DatabaseObjectsException("LIKE operator is not supported for table joins.")
            End If

        End Sub

        Public Sub AddLogicalOperator( _
            ByVal eLogicalOperator As LogicalOperator)

            If pobjLogicalOperators.Count() + 1 > pobjConditions.Count() Then
                Throw New Exceptions.DatabaseObjectsException("First call the Add function - this function has been called without a prior call to Add")
            End If

            pobjLogicalOperators.Add(eLogicalOperator)

        End Sub

        Public ReadOnly Property IsEmpty As Boolean
            Get

                Return pobjConditions.Count = 0

            End Get
        End Property

        Public ReadOnly Property Count() As Integer
            Get

                Return pobjConditions.Count()

            End Get
        End Property

        Public Sub Delete(ByRef objConditions As SQLSelectTableJoinConditions)

            If Not pobjConditions.Contains(objConditions) Then
                Throw New IndexOutOfRangeException
            End If

            pobjConditions.Remove(objConditions)
            objConditions = Nothing

        End Sub

        Public Sub Delete(ByRef objOrderByField As SQLSelectTableJoinCondition)

            If Not pobjConditions.Contains(objOrderByField) Then
                Throw New IndexOutOfRangeException
            End If

            pobjConditions.Remove(objOrderByField)
            objOrderByField = Nothing

        End Sub

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = String.Empty
                Dim intIndex As Integer

                For Each objCondition As Object In pobjConditions
                    If intIndex > 0 Then
                        strSQL &= " " & SQLConvertLogicalOperator(CType(pobjLogicalOperators.Item(intIndex - 1), LogicalOperator)) & " "
                    End If

                    If TypeOf objCondition Is SQLSelectTableJoinConditions Then
                        strSQL &= "(" & DirectCast(objCondition, SQLSelectTableJoinConditions).SQL(eConnectionType) & ")"
                    ElseIf TypeOf objCondition Is SQLSelectTableJoinCondition Then
                        strSQL &= DirectCast(objCondition, SQLSelectTableJoinCondition).SQL(eConnectionType)
                    Else
                        Throw New NotImplementedException(objCondition.GetType.FullName)
                    End If

                    intIndex += 1
                Next

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
