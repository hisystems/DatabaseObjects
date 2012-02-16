
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public Class SQLDropIndex
        Inherits SQLStatement

        Private pstrName As String
        Private pstrTableName As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal strIndexName As String, ByVal strTableName As String)

            Me.Name = strIndexName
            Me.TableName = strTableName

        End Sub

        Public Property Name() As String
            Get

                Return pstrName

            End Get

            Set(ByVal Value As String)

                pstrName = Value.Trim

            End Set
        End Property

        Public Property TableName() As String
            Get

                Return pstrTableName

            End Get

            Set(ByVal Value As String)

                pstrTableName = Value.Trim

            End Set
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get
                If Me.Name = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("IndexName has not been set.")
                End If

                If Me.TableName = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("TableName has not been set.")
                End If

                Select Case Me.ConnectionType
                    Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                        Return "DROP INDEX " & SQLConvertIdentifierName(Me.TableName, Me.ConnectionType) & "." & SQLConvertIdentifierName(Me.Name, Me.ConnectionType)
                    Case Database.ConnectionType.MicrosoftAccess, Database.ConnectionType.MySQL, Database.ConnectionType.Pervasive
                        Return "DROP INDEX " & SQLConvertIdentifierName(Me.Name, Me.ConnectionType) & " ON " & SQLConvertIdentifierName(Me.TableName, Me.ConnectionType)
                    Case Else
                        Throw New NotSupportedException
                End Select

            End Get
        End Property

    End Class

End Namespace
