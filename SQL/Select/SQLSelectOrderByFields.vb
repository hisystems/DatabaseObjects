
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectOrderByFields
        Implements Collections.Generic.IEnumerable(Of SQLSelectOrderByField)

        Private pobjOrderByFields As New Collections.Generic.List(Of SQLSelectOrderByField)

        Public Sub New()

            MyBase.New()

        End Sub

        Public Function Add() As SQLSelectOrderByField

            Return Add(String.Empty, 0, OrderBy.Ascending, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String) As SQLSelectOrderByField

            Return Add(strFieldName, 0, OrderBy.Ascending, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eOrder As OrderBy) As SQLSelectOrderByField

            Return Add(strFieldName, 0, eOrder, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eOrder As OrderBy, _
            ByVal eAggregate As SQL.AggregateFunction) As SQLSelectOrderByField

            Return Add(strFieldName, eAggregate, eOrder, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eOrder As OrderBy, _
            ByVal objTable As SQLSelectTable) As SQLSelectOrderByField

            Return Add(strFieldName, 0, eOrder, objTable)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eAggregate As SQL.AggregateFunction) As SQLSelectOrderByField

            Return Add(strFieldName, eAggregate, OrderBy.Ascending, Nothing)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eAggregate As SQL.AggregateFunction, _
            ByVal eOrder As OrderBy, _
            ByVal objTable As SQLSelectTable) As SQLSelectOrderByField

            Dim objFieldOrder As SQLSelectOrderByField
            objFieldOrder = New SQLSelectOrderByField

            With objFieldOrder
                .Table = objTable
                .Name = strFieldName
                .Order = eOrder
                .AggregateFunction = eAggregate
            End With

            pobjOrderByFields.Add(objFieldOrder)

            Return objFieldOrder

        End Function

        Default Public ReadOnly Property Item(ByVal intIndex As Integer) As SQLSelectOrderByField
            Get

                Return pobjOrderByFields.Item(intIndex)

            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal strFieldName As String) As SQLSelectOrderByField
            Get

                Return Me(FieldNameIndex(strFieldName))

            End Get
        End Property

        Public ReadOnly Property Count() As Integer
            Get

                Return pobjOrderByFields.Count()

            End Get
        End Property

        Public ReadOnly Property IsEmpty As Boolean
            Get

                Return pobjOrderByFields.Count = 0

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

        Public Sub Delete(ByRef objOrderByField As SQLSelectOrderByField)

            If Not pobjOrderByFields.Contains(objOrderByField) Then
                Throw New IndexOutOfRangeException
            End If

            pobjOrderByFields.Remove(objOrderByField)
            objOrderByField = Nothing

        End Sub

        Public Sub OrderingReverseAll()

            For Each objOrderBy As SQLSelectOrderByField In Me
                objOrderBy.OrderingReverse()
            Next

        End Sub

        Private Function FieldNameIndex(ByVal strFieldName As String) As Integer

            Dim objOrderByField As SQLSelectOrderByField

            strFieldName = strFieldName.Trim()

            For intIndex As Integer = 0 To Me.Count - 1
                objOrderByField = DirectCast(pobjOrderByFields.Item(intIndex), SQLSelectOrderByField)
                If String.Compare(strFieldName, objOrderByField.Name, ignoreCase:=True) = 0 Then
                    Return intIndex
                End If
            Next

            Return -1

        End Function

        Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjOrderByFields.GetEnumerator

        End Function

        Private Function GetEnumerator1() As System.Collections.Generic.IEnumerator(Of SQLSelectOrderByField) Implements System.Collections.Generic.IEnumerable(Of SQLSelectOrderByField).GetEnumerator

            Return pobjOrderByFields.GetEnumerator

        End Function

    End Class

End Namespace
