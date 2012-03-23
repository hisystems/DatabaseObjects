
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public MustInherit Class SQLSelectTableBase

        Protected MustOverride ReadOnly Property Source(ByVal eConnectionType As Database.ConnectionType) As String

        Private pstrAlias As String = String.Empty

        Public Sub New()

        End Sub

        Public Overridable Property [Alias]() As String
            Get

                Return pstrAlias

            End Get

            Set(ByVal value As String)

                pstrAlias = value

            End Set
        End Property

        Friend ReadOnly Property SQL(ByVal eConnectionType As Database.ConnectionType) As String
            Get

                Dim strSQL As String = Me.Source(eConnectionType)

                If Me.Alias <> String.Empty Then
                    strSQL &= " " & SQLConvertIdentifierName(Me.Alias, eConnectionType)
                End If

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
