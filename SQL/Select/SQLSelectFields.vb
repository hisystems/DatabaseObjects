
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectFields
        Implements Collections.Generic.IEnumerable(Of SQLSelectField)

        Private pobjFieldNames As New Collections.Generic.List(Of SQLSelectField)

        Public Sub New()

        End Sub

        Public Function Add() As SQLSelectField

            Return Add(String.Empty, String.Empty, 0, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String) As SQLSelectField

            Return Add(strFieldName, String.Empty, 0, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eAggregateFunction As SQL.AggregateFunction) As SQLSelectField

            Return Add(strFieldName, String.Empty, eAggregateFunction, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal strAlias As String, _
            ByVal eAggregateFunction As SQL.AggregateFunction) As SQLSelectField

            Return Add(strFieldName, strAlias, eAggregateFunction, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal strAlias As String, _
            ByVal objTable As SQLSelectTable) As SQLSelectField

            Return Add(strFieldName, strAlias, 0, objTable)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal objTable As SQLSelectTable) As SQLSelectField

            Return Add(strFieldName, String.Empty, 0, objTable)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal strAlias As String, _
            ByVal eAggregateFunction As SQL.AggregateFunction, _
            ByVal objTable As SQLSelectTable) As SQLSelectField

            If eAggregateFunction = AggregateFunction.None Then
                Return Add(New SQLFieldExpression(objTable, strFieldName), strAlias)
            Else
                Return Add(New SQLFieldAggregateExpression(objTable, strFieldName, eAggregateFunction), strAlias)
            End If

        End Function

        Public Function Add(ByVal objExpression As SQLExpression) As SQLSelectField

            Return Add(objExpression, String.Empty)

        End Function

        Public Function Add(ByVal objExpression As SQLExpression, ByVal strAlias As String) As SQLSelectField

            Dim objSQLField As New SQLSelectField(objExpression)
            objSQLField.Alias = strAlias
            pobjFieldNames.Add(objSQLField)

            Return objSQLField

        End Function

        Public Sub Add(ByVal objFieldNames() As String)

            For intIndex As Integer = 0 To objFieldNames.Length - 1
                Me.Add(objFieldNames(intIndex))
            Next

        End Sub

        Default Public ReadOnly Property Item(ByVal strFieldName As String) As SQLSelectField
            Get

                Return Me(FieldNameIndex(strFieldName))

            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal intIndex As Integer) As SQLSelectField
            Get

                Return pobjFieldNames.Item(intIndex)

            End Get
        End Property

        Public ReadOnly Property Count() As Integer
            Get

                Return pobjFieldNames.Count()

            End Get
        End Property

        Public Sub Clear()

            pobjFieldNames.Clear()

        End Sub

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = String.Empty

                If Me.Count = 0 Then
                    strSQL = "*"
                Else
                    For intIndex As Integer = 0 To Me.Count - 1
                        strSQL &= Me.Item(intIndex).SQL(eConnectionType)
                        If intIndex <> Me.Count - 1 Then
                            strSQL &= ", "
                        End If
                    Next
                End If

                Return strSQL

            End Get
        End Property

        Public Function Exists(ByVal strFieldName As String) As Boolean

            Return FieldNameIndex(strFieldName) >= 0

        End Function

        Public Sub Delete(ByRef objSelectField As SQLSelectField)

            If Not pobjFieldNames.Contains(objSelectField) Then
                Throw New IndexOutOfRangeException
            End If

            pobjFieldNames.Remove(objSelectField)
            objSelectField = Nothing

        End Sub

        Private Function FieldNameIndex(ByVal strFieldName As String) As Integer

            strFieldName = strFieldName.Trim()

            For intIndex As Integer = 0 To Me.Count - 1
                If TypeOf Me.Item(intIndex).Expression Is SQLFieldExpression Then
                    Dim objFieldExpression As SQLFieldExpression = DirectCast(Me.Item(intIndex).Expression, SQLFieldExpression)
                    If String.Compare(strFieldName, objFieldExpression.Name, ignoreCase:=True) = 0 Then
                        Return intIndex
                    End If
                End If
            Next

            Return -1

        End Function

        Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjFieldNames.GetEnumerator

        End Function

        Private Function GetEnumerator1() As System.Collections.Generic.IEnumerator(Of SQLSelectField) Implements System.Collections.Generic.IEnumerable(Of SQLSelectField).GetEnumerator

            Return pobjFieldNames.GetEnumerator

        End Function

    End Class

End Namespace
