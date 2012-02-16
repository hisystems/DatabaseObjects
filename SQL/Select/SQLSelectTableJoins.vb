
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectTableJoins
        Implements Collections.Generic.IEnumerable(Of SQLSelectTableJoin)

        Private pobjJoins As New Collections.Generic.List(Of SQLSelectTableJoin)

        Public Sub New()

            MyBase.New()

        End Sub

        Public Function Add() As SQLSelectTableJoin

            Dim objJoin As SQLSelectTableJoin = New SQLSelectTableJoin(Me)

            pobjJoins.Add(objJoin)

            Return objJoin

        End Function

        Public Function Add( _
            ByVal objLeftTable As SQLSelectTable, _
            ByVal eJoin As SQLSelectTableJoin.Type, _
            ByVal objRightTable As SQLSelectTable) As SQLSelectTableJoin

            Dim objJoin As SQLSelectTableJoin = New SQLSelectTableJoin(Me)

            objJoin.LeftTable = objLeftTable
            objJoin.TheType = eJoin
            objJoin.RightTable = objRightTable

            pobjJoins.Add(objJoin)

            Return objJoin

        End Function

        Default Public ReadOnly Property Item(ByVal intIndex As Integer) As SQLSelectTableJoin
            Get

                Return pobjJoins.Item(intIndex)

            End Get
        End Property

        Public ReadOnly Property Count() As Integer
            Get

                Return pobjJoins.Count()

            End Get
        End Property

        Public Function Exists(ByVal objTable As SQLSelectTableBase) As Boolean

            For intIndex As Integer = 0 To pobjJoins.Count() - 1
                With Me.Item(intIndex)
                    If .LeftTable Is objTable Or .RightTable Is objTable Then
                        Return True
                    End If
                End With
            Next

        End Function

        Public Sub Delete(ByVal objJoin As SQLSelectTableJoin)

            If Not pobjJoins.Contains(objJoin) Then
                Throw New IndexOutOfRangeException
            End If

            pobjJoins.Remove(objJoin)
            objJoin = Nothing

        End Sub

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                If Me.Count > 0 Then
                    'recurse through the joins from right to left
                    Return Me.Item(Me.Count - 1).SQL(eConnectionType)
                Else
                    Return String.Empty
                End If

            End Get
        End Property

        Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjJoins.GetEnumerator

        End Function

        Private Function GetEnumerator1() As System.Collections.Generic.IEnumerator(Of SQLSelectTableJoin) Implements System.Collections.Generic.IEnumerable(Of SQLSelectTableJoin).GetEnumerator

            Return pobjJoins.GetEnumerator

        End Function

    End Class

End Namespace
