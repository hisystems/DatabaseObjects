' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLAlterTable
        Inherits SQLStatement

        Private pstrName As String
        Private pobjFields As SQLTableFields = New SQLTableFields

        Public Sub New()

        End Sub

        Public Property Name() As String
            Get

                Return pstrName

            End Get

            Set(ByVal Value As String)

                pstrName = Value.Trim

            End Set
        End Property

        Public ReadOnly Property Fields() As SQLTableFields
            Get

                Return pobjFields

            End Get
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                If Me.Name = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("TableName has not been set.")
                End If

                Return _
                    "ALTER TABLE " & SQLConvertIdentifierName(Me.Name, Me.ConnectionType) & " " & _
                    pobjFields.SQL(Me.ConnectionType, bIncludeColumnModifier:=True)

            End Get
        End Property

    End Class

End Namespace
