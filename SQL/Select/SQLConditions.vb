
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLConditions
        Implements IEnumerable

        Private pobjSQLConditions As New ArrayList
        Private pobjLogicalOperators As New Collections.Generic.List(Of LogicalOperator)

        Public Sub New()

        End Sub

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objValue As Object) As SQLCondition

            Return Add(strFieldName, eCompare, objValue, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objValue As Object, _
            ByVal objTable As SQLSelectTable) As SQLCondition

            If strFieldName = String.Empty Then
                Throw New ArgumentNullException("Fieldname is null")
            End If

            EnsurePreviousLogicalOperatorExists()

            Add = New SQLCondition

            Add.Table = objTable
            Add.FieldName = strFieldName
            Add.Compare = eCompare
            Add.Value = objValue

            pobjSQLConditions.Add(Add)

        End Function

        Public Sub Add( _
            ByVal objCondition As SQLCondition)

            Me.Add(objCondition.FieldName, objCondition.Compare, objCondition.Value, objCondition.Table)

        End Sub

        Public Sub Add( _
            ByVal objConditions As SQLConditions)

            If objConditions.IsEmpty Then
                Throw New ArgumentException("SQLConditions does not contain any conditions.")
            End If

            EnsurePreviousLogicalOperatorExists()
            pobjSQLConditions.Add(objConditions)

        End Sub

        'Public Function Add( _
        '    ByVal strFieldName As String, _
        '    ByVal eCompare As ComparisonOperator, _
        '    ByVal objRightExpression As SQLExpression) As SQLConditionExpression

        '    Return Me.Add(New SQLFieldExpression(strFieldName), eCompare, objRightExpression)

        'End Function

        Public Function Add( _
            ByVal objLeftExpression As SQLExpression, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objRightExpression As SQLExpression) As SQLConditionExpression

            Dim objCondition As New SQLConditionExpression(objLeftExpression, eCompare, objRightExpression)

            Me.Add(objCondition)

            Return objCondition

        End Function

        Public Sub Add(ByVal objCondition As SQLConditionExpression)

            If objCondition Is Nothing Then
                Throw New ArgumentNullException
            End If

            EnsurePreviousLogicalOperatorExists()
            pobjSQLConditions.Add(objCondition)

        End Sub

        Public Function Add() As SQLCondition

            Return Add("", ComparisonOperator.EqualTo, Nothing, Nothing)

        End Function

        Public Function AddInSelect() As SQLConditionInSelect

            Return AddInSelect(String.Empty, Nothing, Nothing)

        End Function

        Public Function AddInSelect( _
            ByVal strFieldName As String, _
            ByVal objSelect As SQLSelect) As SQLConditionInSelect

            Return AddInSelect(strFieldName, objSelect, Nothing)

        End Function

        Public Function AddInSelect( _
            ByVal strFieldName As String, _
            ByVal objSelect As SQLSelect, _
            ByVal objTable As SQLSelectTable) As SQLConditionInSelect

            EnsurePreviousLogicalOperatorExists()

            AddInSelect = New SQLConditionInSelect
            AddInSelect.Table = objTable
            AddInSelect.FieldName = strFieldName
            AddInSelect.Select = objSelect

            pobjSQLConditions.Add(AddInSelect)

        End Function

        Public Function AddNotInSelect() As SQLConditionInSelect

            Return AddNotInSelect(String.Empty, Nothing, Nothing)

        End Function

        Public Function AddNotInSelect( _
            ByVal strFieldName As String, _
            ByVal objSelect As SQLSelect) As SQLConditionInSelect

            Return AddNotInSelect(strFieldName, objSelect, Nothing)

        End Function

        Public Function AddNotInSelect( _
            ByVal strFieldName As String, _
            ByVal objSelect As SQLSelect, _
            ByVal objTable As SQLSelectTable) As SQLConditionInSelect

            AddNotInSelect = AddInSelect(strFieldName, objSelect, objTable)
            AddNotInSelect.NotInSelect = True

        End Function

        Public Function AddSelect() As SQLConditionSelect

            Return AddSelect(Nothing, ComparisonOperator.EqualTo, Nothing)

        End Function

        Public Function AddSelect( _
            ByVal objSelect As SQLSelect, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objValue As Object) As SQLConditionSelect

            EnsurePreviousLogicalOperatorExists()

            AddSelect = New SQLConditionSelect
            AddSelect.Select = objSelect
            AddSelect.Compare = eCompare
            AddSelect.Value = objValue

            pobjSQLConditions.Add(AddSelect)

        End Function

        Public Function AddFieldCompare() As SQLConditionFieldCompare

            Return AddFieldCompare(Nothing, "", ComparisonOperator.EqualTo, Nothing, "")

        End Function

        Public Function AddFieldCompare( _
            ByVal strFieldName1 As String, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objTable2 As SQLSelectTable, _
            ByVal strFieldName2 As String) As SQLConditionFieldCompare

            Return AddFieldCompare(Nothing, strFieldName1, eCompare, objTable2, strFieldName2)

        End Function

        Public Function AddFieldCompare( _
            ByVal objTable1 As SQLSelectTable, _
            ByVal strFieldName1 As String, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objTable2 As SQLSelectTable, _
            ByVal strFieldName2 As String) As SQLConditionFieldCompare

            EnsurePreviousLogicalOperatorExists()

            AddFieldCompare = New SQLConditionFieldCompare

            With AddFieldCompare
                .Table1 = objTable1
                .FieldName1 = strFieldName1
                .Compare = eCompare
                .Table2 = objTable2
                .FieldName2 = strFieldName2
            End With

            pobjSQLConditions.Add(AddFieldCompare)

        End Function

        Public ReadOnly Property IsEmpty() As Boolean
            Get

                Return pobjSQLConditions.Count = 0

            End Get
        End Property

        Private Sub EnsurePreviousLogicalOperatorExists()

            'Add the AND operator if an operator hasn't been called after the previous Add call
            If pobjLogicalOperators.Count() < pobjSQLConditions.Count() Then
                Me.AddLogicalOperator(LogicalOperator.And)
            End If

        End Sub

        Public Sub AddLogicalOperator( _
            ByVal eLogicalOperator As LogicalOperator)

            If pobjLogicalOperators.Count() + 1 > pobjSQLConditions.Count() Then
                Throw New Exceptions.DatabaseObjectsException("First call the Add function - this function has been called without a prior call to Add")
            End If

            pobjLogicalOperators.Add(eLogicalOperator)

        End Sub

        Public Sub Delete(ByRef objCondition As SQLCondition)

            Dim intConditionIndex As Integer = pobjSQLConditions.IndexOf(objCondition)

            If Not pobjSQLConditions.Contains(objCondition) Then
                Throw New IndexOutOfRangeException
            End If

            If intConditionIndex > 0 Then
                pobjLogicalOperators.Remove(pobjLogicalOperators(intConditionIndex - 1))
            ElseIf intConditionIndex = 0 Then
                pobjLogicalOperators.Remove(pobjLogicalOperators(0))
            End If

            pobjSQLConditions.Remove(objCondition)
            objCondition = Nothing

        End Sub

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = String.Empty

                For intIndex As Integer = 0 To pobjSQLConditions.Count() - 1
                    If intIndex > 0 Then
                        strSQL &= " " & SQLConvertLogicalOperator(pobjLogicalOperators.Item(intIndex - 1)) & " "
                    End If

                    If TypeOf pobjSQLConditions(intIndex) Is SQLCondition Then
                        strSQL &= DirectCast(pobjSQLConditions(intIndex), SQLCondition).SQL(eConnectionType)
                    ElseIf TypeOf pobjSQLConditions(intIndex) Is SQLConditions Then
                        strSQL &= "(" & DirectCast(pobjSQLConditions(intIndex), SQLConditions).SQL(eConnectionType) & ")"
                    ElseIf TypeOf pobjSQLConditions(intIndex) Is SQLConditionInSelect Then
                        strSQL &= DirectCast(pobjSQLConditions(intIndex), SQLConditionInSelect).SQL(eConnectionType)
                    ElseIf TypeOf pobjSQLConditions(intIndex) Is SQLConditionSelect Then
                        strSQL &= DirectCast(pobjSQLConditions(intIndex), SQLConditionSelect).SQL(eConnectionType)
                    ElseIf TypeOf pobjSQLConditions(intIndex) Is SQLConditionFieldCompare Then
                        strSQL &= DirectCast(pobjSQLConditions(intIndex), SQLConditionFieldCompare).SQL(eConnectionType)
                    ElseIf TypeOf pobjSQLConditions(intIndex) Is SQLConditionExpression Then
                        strSQL &= DirectCast(pobjSQLConditions(intIndex), SQLConditionExpression).SQL(eConnectionType)
                    Else
                        Throw New NotImplementedException(pobjSQLConditions(intIndex).GetType.FullName)
                    End If
                Next

                Return strSQL

            End Get
        End Property

        Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjSQLConditions.GetEnumerator

        End Function

    End Class

End Namespace
