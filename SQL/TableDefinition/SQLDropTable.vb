
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLDropTable
        Inherits SQLStatement

        Private pstrName As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal strTableName As String)

            Me.Name = strTableName

        End Sub

        Public Property Name() As String
            Get

                Return pstrName

            End Get

            Set(ByVal Value As String)

                pstrName = Value.Trim

            End Set
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                If Me.Name = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("TableName has not been set.")
                End If

                Return "DROP TABLE " & SQLConvertIdentifierName(Me.Name, Me.ConnectionType)

            End Get
        End Property

    End Class

End Namespace
