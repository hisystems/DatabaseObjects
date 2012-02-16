' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectGroupByFields
        Implements Collections.Generic.IEnumerable(Of SQLSelectGroupByField)

        Private pobjGroupByFields As New Collections.Generic.List(Of SQLSelectGroupByField)

        Public Sub New()

            MyBase.New()

        End Sub

        Public Function Add() As SQLSelectGroupByField

            Return Add("", Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String) As SQLSelectGroupByField

            Return Add(strFieldName, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal objTable As SQLSelectTable) As SQLSelectGroupByField

            Dim objFieldOrder As New SQLSelectGroupByField(New SQLFieldExpression(objTable, strFieldName))

            pobjGroupByFields.Add(objFieldOrder)

            Return objFieldOrder

        End Function

        Public Function Add( _
            ByVal objExpression As SQLExpression) As SQLSelectGroupByField

            Dim objFieldOrder As New SQLSelectGroupByField(objExpression)

            pobjGroupByFields.Add(objFieldOrder)

            Return objFieldOrder

        End Function

        Default Public ReadOnly Property Item(ByVal strFieldName As String) As SQLSelectGroupByField
            Get

                Return Me(FieldNameIndex(strFieldName))

            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal intIndex As Integer) As SQLSelectGroupByField
            Get

                Return pobjGroupByFields.Item(intIndex)

            End Get
        End Property

        Public ReadOnly Property Count() As Integer
            Get

                Return pobjGroupByFields.Count()

            End Get
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = String.Empty

                For intIndex As Integer = 0 To Me.Count - 1
                    strSQL &= Me.Item(intIndex).SQL(eConnectionType)
                    If intIndex <> Me.Count - 1 Then
                        strSQL &= ", "
                    End If
                Next

                Return strSQL

            End Get
        End Property

        Public Function Exists(ByVal strFieldName As String) As Boolean

            Return FieldNameIndex(strFieldName) >= 0

        End Function

        Public Sub Delete(ByRef objGroupByField As SQLSelectGroupByField)

            If Not pobjGroupByFields.Contains(objGroupByField) Then
                Throw New IndexOutOfRangeException
            End If

            pobjGroupByFields.Remove(objGroupByField)
            objGroupByField = Nothing

        End Sub

        Private Function FieldNameIndex(ByVal strFieldName As String) As Integer

            Dim objGroupByField As SQLSelectGroupByField

            strFieldName = strFieldName.Trim()

            For intIndex As Integer = 0 To Me.Count - 1
                objGroupByField = DirectCast(pobjGroupByFields.Item(intIndex), SQLSelectGroupByField)
                If TypeOf objGroupByField.Expression Is SQLFieldExpression Then
                    If String.Compare(strFieldName, DirectCast(objGroupByField.Expression, SQLFieldExpression).Name, ignoreCase:=True) = 0 Then
                        Return intIndex
                    End If
                End If
            Next

            Return -1

        End Function

        Public ReadOnly Property IsEmpty As Boolean
            Get

                Return pobjGroupByFields.Count = 0

            End Get
        End Property

        Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjGroupByFields.GetEnumerator

        End Function

        Private Function GetEnumerator1() As System.Collections.Generic.IEnumerator(Of SQLSelectGroupByField) Implements System.Collections.Generic.IEnumerable(Of SQLSelectGroupByField).GetEnumerator

            Return pobjGroupByFields.GetEnumerator

        End Function

    End Class

End Namespace