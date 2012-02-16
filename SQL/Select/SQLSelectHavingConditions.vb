
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLSelectHavingConditions

        Private pobjConditions As New ArrayList
        Private pobjLogicalOperators As New Collections.Generic.List(Of LogicalOperator)

        Public Sub New()

        End Sub

        Public Function Add( _
            ByVal eAggregate As AggregateFunction, _
            ByVal strFieldName As String, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objValue As Object) As SQLSelectHavingCondition

            Return Add(Nothing, eAggregate, strFieldName, eCompare, objValue)

        End Function

        Public Function Add( _
            ByVal objTable As SQLSelectTable, _
            ByVal eAggregate As AggregateFunction, _
            ByVal strFieldName As String, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objValue As Object) As SQLSelectHavingCondition

            If strFieldName = String.Empty Then
                Throw New ArgumentNullException("Fieldname is null")
            End If

            Dim objLeftHandExpression As SQLFieldExpression

            If eAggregate = AggregateFunction.None Then
                objLeftHandExpression = New SQLFieldExpression(objTable, strFieldName)
            Else
                objLeftHandExpression = New SQLFieldAggregateExpression(objTable, strFieldName, eAggregate)
            End If

            Return Add(objLeftHandExpression, eCompare, New SQLValueExpression(objValue))

        End Function

        Public Sub Add( _
            ByVal objConditions As SQLSelectHavingConditions)

            If objConditions.IsEmpty Then
                Throw New ArgumentException("SQLConditions does not contain any conditions.")
            End If

            EnsurePreviousLogicalOperatorExists()
            pobjConditions.Add(objConditions)

        End Sub

        Public Function Add( _
            ByVal objLeftExpression As SQLExpression, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objRightExpression As SQLExpression) As SQLSelectHavingCondition

            Add = New SQLSelectHavingCondition(objLeftExpression, eCompare, objRightExpression)
            EnsurePreviousLogicalOperatorExists()
            pobjConditions.Add(Add)

        End Function

        Public ReadOnly Property IsEmpty() As Boolean
            Get

                Return pobjConditions.Count = 0

            End Get
        End Property

        Private Sub EnsurePreviousLogicalOperatorExists()

            'Add the AND operator if an operator hasn't been called after the previous Add call
            If pobjLogicalOperators.Count() < pobjConditions.Count() Then
                Me.AddLogicalOperator(LogicalOperator.And)
            End If

        End Sub

        Public Sub AddLogicalOperator( _
            ByVal eLogicalOperator As LogicalOperator)

            If pobjLogicalOperators.Count() + 1 > pobjConditions.Count() Then
                Throw New Exceptions.DatabaseObjectsException("First call the Add function - this function has been called without a prior call to Add")
            End If

            pobjLogicalOperators.Add(eLogicalOperator)

        End Sub

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = String.Empty

                For intIndex As Integer = 0 To pobjConditions.Count() - 1
                    If intIndex > 0 Then
                        strSQL &= " " & SQLConvertLogicalOperator(pobjLogicalOperators.Item(intIndex - 1)) & " "
                    End If

                    If TypeOf pobjConditions(intIndex) Is SQLSelectHavingCondition Then
                        strSQL &= DirectCast(pobjConditions(intIndex), SQLSelectHavingCondition).SQL(eConnectionType)
                    ElseIf TypeOf pobjConditions(intIndex) Is SQLSelectHavingConditions Then
                        strSQL &= "(" & DirectCast(pobjConditions(intIndex), SQLSelectHavingConditions).SQL(eConnectionType) & ")"
                    Else
                        Throw New NotImplementedException(pobjConditions(intIndex).GetType.FullName)
                    End If
                Next

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
