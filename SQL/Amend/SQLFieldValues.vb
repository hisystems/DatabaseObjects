
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLFieldValues
        Implements IEnumerable

        Protected pobjFields As ArrayList = New ArrayList

        Public Sub New()

            MyBase.New()

        End Sub

        Public Overridable Function Add() As SQLFieldValue

            Return Add(String.Empty, Nothing)

        End Function

        Public Overridable Function Add(ByVal objFieldAndValue As SQLFieldValue) As SQLFieldValue

            Return Add(objFieldAndValue.Name, objFieldAndValue.Value)

        End Function

        Public Overridable Function Add( _
            ByVal strDestinationFieldName As String, _
            ByVal objEqualToValue As Object) As SQLFieldValue

            Dim objSQLFieldValue As New SQLFieldValue(strDestinationFieldName, objEqualToValue)

            pobjFields.Add(objSQLFieldValue)

            Return objSQLFieldValue

        End Function

        Public Overridable Sub Add(ByVal objFieldValues As SQLFieldValues)

            For Each objFieldValue As SQLFieldValue In objFieldValues
                Me.Add(objFieldValue)
            Next

        End Sub

        Default Public ReadOnly Property Item(ByVal strFieldName As String) As SQLFieldValue
            Get

                If Not Me.Exists(strFieldName) Then
                    Throw New ArgumentException("Field '" & strFieldName & "' does not exist.")
                End If

                Return DirectCast(pobjFields.Item(FieldNameIndex(strFieldName)), SQLFieldValue)

            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal intIndex As Integer) As SQLFieldValue
            Get

                Return DirectCast(pobjFields.Item(intIndex), SQLFieldValue)

            End Get
        End Property

        Public ReadOnly Property Count() As Integer
            Get

                Return pobjFields.Count()

            End Get
        End Property

        Public Function Exists(ByVal strFieldName As String) As Boolean

            Return FieldNameIndex(strFieldName) >= 0

        End Function

        Public Sub Delete(ByRef objFieldValue As SQLFieldValue)

            If Not pobjFields.Contains(objFieldValue) Then
                Throw New IndexOutOfRangeException
            End If

            pobjFields.Remove(objFieldValue)
            objFieldValue = Nothing

        End Sub

        Friend Function FieldNameIndex(ByVal strFieldName As String) As Integer

            Dim objSQLFieldValue As SQLFieldValue

            strFieldName = strFieldName.Trim()

            For intIndex As Integer = 0 To Me.Count - 1
                objSQLFieldValue = DirectCast(pobjFields.Item(intIndex), SQLFieldValue)
                If String.Compare(strFieldName, objSQLFieldValue.Name, ignoreCase:=True) = 0 Then
                    Return intIndex
                End If
            Next

            Return -1

        End Function

        Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjFields.GetEnumerator

        End Function

    End Class

End Namespace
