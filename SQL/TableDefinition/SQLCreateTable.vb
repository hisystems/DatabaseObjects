' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLCreateTable
        Inherits SQLStatement

        Private pstrName As String
        Private pobjFields As SQLTableFields

        Public Sub New()

            pobjFields = New SQLTableFields
            pobjFields.AlterMode = SQLTableFields.AlterModeType.Add     'set that fields can only be added

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
                Dim strSQL As String

                If Me.Name = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("TableName has not been set.")
                End If

                strSQL = _
                    "CREATE TABLE " & _
                    SQLConvertIdentifierName(Me.Name, Me.ConnectionType) & " (" & pobjFields.SQL(Me.ConnectionType, bIncludeMode:=False) & ")"

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
