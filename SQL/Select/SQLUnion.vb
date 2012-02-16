
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLUnion
        Inherits SQLStatement

        Public Enum Type
            Distinct        'default
            All
        End Enum

        Private Const pceDefaultUnionType As SQLUnion.Type = Type.Distinct

        Private pobjSelectStatements As New Collections.Generic.List(Of SQL.SQLSelect)
        Private pobjUnionType As New ArrayList
        Private pobjOrderByFields As SQLSelectOrderByFields = New SQLSelectOrderByFields

        Public Sub New()

        End Sub

        Public Sub New(ByVal objSelect1 As SQLSelect, ByVal objSelect2 As SQLSelect)

            Me.New(objSelect1, pceDefaultUnionType, objSelect2)

        End Sub

        Public Sub New(ByVal objSelect1 As SQLSelect, ByVal eType As SQLUnion.Type, ByVal objSelect2 As SQLSelect)

            Add(objSelect1)
            AddUnionType(eType)
            Add(objSelect1)

        End Sub

        Public Sub Add(ByVal objSelect As SQLSelect)

            EnsurePreviousTypeExists()
            pobjSelectStatements.Add(objSelect)

        End Sub

        Public Sub AddUnionType(ByVal eType As SQLUnion.Type)

            If pobjUnionType.Count() + 1 > pobjSelectStatements.Count() Then
                Throw New Exceptions.DatabaseObjectsException("First call the Add function - this function has been called without a prior call to Add")
            End If

            pobjUnionType.Add(eType)

        End Sub

        Public Property OrderBy() As SQLSelectOrderByFields
            Get

                Return pobjOrderByFields

            End Get

            Set(ByVal Value As SQLSelectOrderByFields)

                pobjOrderByFields = Value

            End Set
        End Property

        Friend ReadOnly Property SelectStatements() As SQL.SQLSelect()
            Get

                Return pobjSelectStatements.ToArray

            End Get
        End Property

        Private Sub EnsurePreviousTypeExists()

            'Add the AND operator if an operator hasn't been called after the previous Add call
            If pobjUnionType.Count() < pobjSelectStatements.Count() Then
                Me.AddUnionType(pceDefaultUnionType)
            End If

        End Sub

        Public Overrides ReadOnly Property SQL() As String
            Get

                Dim strSQL As String = String.Empty
                Dim objSelect As SQLSelect
                Dim strOrderBy As String

                If pobjSelectStatements.Count = 0 Then
                    Throw New Exceptions.DatabaseObjectsException("The table has not been set.")
                End If

                For intIndex As Integer = 0 To pobjSelectStatements.Count() - 1
                    If intIndex > 0 Then
                        strSQL &= " UNION "
                        If DirectCast(pobjUnionType(intIndex - 1), SQLUnion.Type) = Type.All Then
                            strSQL &= "ALL "
                        End If
                    End If

                    objSelect = DirectCast(pobjSelectStatements(intIndex), SQLSelect)
                    objSelect.ConnectionType = Me.ConnectionType
                    strSQL &= "(" & objSelect.SQL & ")"
                Next

                If Not pobjOrderByFields Is Nothing Then
                    strOrderBy = pobjOrderByFields.SQL(Me.ConnectionType)
                    If strOrderBy <> String.Empty Then
                        strSQL &= " ORDER BY " & strOrderBy
                    End If
                End If

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
