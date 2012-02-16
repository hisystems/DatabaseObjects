
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLAutoIncrementValue
        Inherits SQLStatement

        Private pstrReturnFieldName As String = "AutoIncrementValue"

        Public Sub New()

            MyBase.New()

        End Sub

        Public Property ReturnFieldName() As String
            Get
                Return pstrReturnFieldName
            End Get

            Set(ByVal Value As String)

                If Value.Trim = String.Empty Then
                    Throw New ArgumentNullException
                End If

                pstrReturnFieldName = Value

            End Set
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                Select Case Me.ConnectionType
                    Case Database.ConnectionType.SQLServer
                        Return "SELECT SCOPE_IDENTITY() AS " & SQLConvertIdentifierName(Me.ReturnFieldName, Me.ConnectionType)
                    Case Database.ConnectionType.MicrosoftAccess, Database.ConnectionType.Pervasive, Database.ConnectionType.SQLServerCompactEdition
                        Return "SELECT @@IDENTITY AS " & SQLConvertIdentifierName(Me.ReturnFieldName, Me.ConnectionType)
                    Case Database.ConnectionType.MySQL
                        'The @@IDENTITY function is supported by MySQL from version 3.23.25
                        'but I've put the original function here just in case
                        Return "SELECT LAST_INSERT_ID() AS " & SQLConvertIdentifierName(Me.ReturnFieldName, Me.ConnectionType)
                    Case Database.ConnectionType.HyperSQL
                        Return "SELECT IDENTITY() AS " & SQLConvertIdentifierName(Me.ReturnFieldName, Me.ConnectionType)
                    Case Else
                        Throw New NotImplementedException(Me.ConnectionType.ToString)
                End Select

            End Get
        End Property

    End Class

End Namespace
