
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace SQL

    Public Class SQLSetTransactionIsolationLevel
        Inherits SQL.SQLStatement

        Private peIsolationLevel As Data.IsolationLevel

        Public Sub New(ByVal eIsolationLevel As Data.IsolationLevel)

            peIsolationLevel = eIsolationLevel

        End Sub

        Public Overrides ReadOnly Property SQL() As String
            Get

                Select Case MyBase.ConnectionType
                    Case Database.ConnectionType.SQLServer, _
                         Database.ConnectionType.MySQL, _
                         Database.ConnectionType.Pervasive, _
                         Database.ConnectionType.SQLServerCompactEdition, _
                         Database.ConnectionType.HyperSQL
                        Return "SET TRANSACTION ISOLATION LEVEL " & GetIsolationLevelString(peIsolationLevel)
                    Case Else
                        Throw New NotImplementedException(Me.ConnectionType.ToString & "; Transaction isolation levels")
                End Select

            End Get
        End Property

        Private Function GetIsolationLevelString(ByVal eIsolationLevel As Data.IsolationLevel) As String

            Select Case eIsolationLevel
                Case IsolationLevel.ReadCommitted
                    Return "READ COMMITTED"
                Case IsolationLevel.ReadUncommitted
                    Return "READ UNCOMMITTED"
                Case IsolationLevel.RepeatableRead
                    Return "REPEATABLE READ"
                Case IsolationLevel.Serializable
                    Return "SERIALIZABLE"
                Case IsolationLevel.Snapshot
                    'Only SQL Server 2005+ supports snapshot isolation levels
                    If MyBase.ConnectionType = Database.ConnectionType.SQLServer Then
                        Return "SNAPSHOT"
                    Else
                        Throw New NotSupportedException("Snapshots isolation level is not supported for " & MyBase.ConnectionType)
                    End If
                Case Else
                    Throw New NotImplementedException("Isolation level")
            End Select

        End Function

    End Class

End Namespace
