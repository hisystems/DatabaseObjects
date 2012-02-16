
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectTables
        Implements Collections.Generic.IEnumerable(Of SQLSelectTableBase)

        Private pobjTables As New Collections.Generic.List(Of SQLSelectTableBase)
        Private pobjJoins As SQLSelectTableJoins = New SQLSelectTableJoins

        Public Sub New()

            MyBase.New()

        End Sub

        Public Function Add() As SQLSelectTable

            Return Add(String.Empty)

        End Function

        Public Function Add(ByVal strTableName As String) As SQLSelectTable

            Dim objTable As SQLSelectTable = New SQLSelectTable(strTableName)

            pobjTables.Add(objTable)

            Return objTable

        End Function

        Public Function Add(ByVal strTableName As String, ByVal strAlias As String) As SQLSelectTable

            Dim objTable As SQLSelectTable = New SQLSelectTable(strTableName, strAlias)

            pobjTables.Add(objTable)

            Return objTable

        End Function

        Public Sub Add(ByVal objTable As SQL.SQLSelectTable)

            pobjTables.Add(objTable)

        End Sub

        Public Sub Add(ByVal objTable As SQL.SQLSelectTableFromSelect)

            pobjTables.Add(objTable)

        End Sub

        Default Public ReadOnly Property Item(ByVal strTableName As String) As SQLSelectTableBase
            Get

                Return Me(TableNameIndex(strTableName))

            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal intIndex As Integer) As SQLSelectTableBase
            Get

                Return pobjTables.Item(intIndex)

            End Get
        End Property

        Public ReadOnly Property Count() As Integer
            Get

                Return pobjTables.Count()

            End Get
        End Property

        Public Property Joins() As SQLSelectTableJoins
            Get

                Return pobjJoins

            End Get

            Set(ByVal Value As SQLSelectTableJoins)

                pobjJoins = Value

            End Set
        End Property

        Public Function Exists(ByVal strTableName As String) As Boolean

            Return TableNameIndex(strTableName) >= 0

        End Function

        Public Sub Delete(ByRef objTable As SQLSelectTable)

            If pobjTables.Contains(objTable) Then
                Throw New IndexOutOfRangeException
            End If

            pobjTables.Remove(objTable)
            objTable = Nothing

        End Sub

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = String.Empty
                Dim bAddTable As Boolean

                For intIndex As Integer = 0 To Me.Count - 1
                    'Include the table if it's not being used in a join
                    If pobjJoins Is Nothing Then
                        bAddTable = True
                    ElseIf Not pobjJoins.Exists(Me.Item(intIndex)) Then
                        bAddTable = True
                    Else
                        bAddTable = False
                    End If

                    If bAddTable Then
                        strSQL &= Me.Item(intIndex).SQL(eConnectionType)
                        If intIndex <> Me.Count - 1 Then
                            strSQL &= ", "
                        End If
                    End If
                Next

                If Not pobjJoins Is Nothing Then
                    If pobjJoins.SQL(eConnectionType) <> String.Empty And strSQL <> String.Empty Then
                        strSQL &= " "
                    End If
                    strSQL &= pobjJoins.SQL(eConnectionType)
                End If

                Return strSQL

            End Get
        End Property

        Private Function TableNameIndex(ByVal strTableName As String) As Integer

            Dim objTable As SQLSelectTable

            strTableName = strTableName.Trim()

            For intIndex As Integer = 0 To Me.Count - 1
                objTable = DirectCast(pobjTables.Item(intIndex), SQLSelectTable)
                If String.Compare(strTableName, objTable.Name, ignoreCase:=True) = 0 Then
                    Return intIndex
                End If
            Next

            Return -1

        End Function

        Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjTables.GetEnumerator

        End Function

        Private Function GetEnumerator1() As System.Collections.Generic.IEnumerator(Of SQLSelectTableBase) Implements System.Collections.Generic.IEnumerable(Of SQLSelectTableBase).GetEnumerator

            Return pobjTables.GetEnumerator

        End Function

    End Class

End Namespace
