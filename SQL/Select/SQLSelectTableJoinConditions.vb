
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
        Implements Collections.Generic.IEnumerable(Of SQLSelectTableJoinCondition)

        Private pobjLogicalOperators As New Collections.Generic.List(Of LogicalOperator)
        Private pobjConditions As New Collections.Generic.List(Of SQLSelectTableJoinCondition)
        Private pobjParent As SQLSelectTableJoin

        Friend Sub New(ByVal objParent As SQLSelectTableJoin)

            MyBase.New()
            pobjParent = objParent

        End Sub

        Friend ReadOnly Property Parent() As SQLSelectTableJoin
            Get

                Return pobjParent

            End Get
        End Property

        Public Function Add() As SQLSelectTableJoinCondition

            Return Add("", ComparisonOperator.EqualTo, "")

        End Function

        Public Function Add( _
            ByVal strLeftTableFieldName As String, _
            ByVal eCompare As ComparisonOperator, _
            ByVal strRightTableFieldName As String) As SQLSelectTableJoinCondition

            'Add the AND operator if an operator hasn't been called after the previous Add call
            If pobjLogicalOperators.Count() < pobjConditions.Count() Then
                Me.AddLogicalOperator(LogicalOperator.And)
            End If

            If eCompare = ComparisonOperator.Like Or eCompare = ComparisonOperator.NotLike Then
                Throw New Exceptions.DatabaseObjectsException("LIKE operator is not supported for table joins.")
            End If

            Add = New SQLSelectTableJoinCondition(Me)

            Add.LeftTableFieldName = strLeftTableFieldName
            Add.Compare = eCompare
            Add.RightTableFieldName = strRightTableFieldName

            pobjConditions.Add(Add)

        End Function

        Public Sub AddLogicalOperator( _
            ByVal eLogicalOperator As LogicalOperator)

            If pobjLogicalOperators.Count() + 1 > pobjConditions.Count() Then
                Throw New Exceptions.DatabaseObjectsException("First call the Add function - this function has been called without a prior call to Add")
            End If

            pobjLogicalOperators.Add(eLogicalOperator)

        End Sub

        Default Public ReadOnly Property Item(ByVal intIndex As Integer) As SQLSelectTableJoinCondition
            Get

                Return pobjConditions.Item(intIndex)

            End Get
        End Property

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
                Dim objCondition As SQLSelectTableJoinCondition

                With pobjConditions
                    For intIndex As Integer = 0 To .Count() - 1
                        If intIndex > 0 Then
                            strSQL &= " " & SQLConvertLogicalOperator(DirectCast(pobjLogicalOperators.Item(intIndex - 1), LogicalOperator)) & " "
                        End If

                        objCondition = DirectCast(.Item(intIndex), SQLSelectTableJoinCondition)
                        strSQL &= objCondition.SQL(eConnectionType)
                    Next
                End With

                Return strSQL

            End Get
        End Property

        Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjConditions.GetEnumerator

        End Function

        Private Function GetEnumerator1() As System.Collections.Generic.IEnumerator(Of SQLSelectTableJoinCondition) Implements System.Collections.Generic.IEnumerable(Of SQLSelectTableJoinCondition).GetEnumerator

            Return pobjConditions.GetEnumerator

        End Function

    End Class

End Namespace
