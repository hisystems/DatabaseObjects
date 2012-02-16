
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace SQL

    Public Class SQLRollbackTransaction
        Inherits SQL.SQLStatement

        Public Overrides ReadOnly Property SQL() As String
            Get

                Select Case MyBase.ConnectionType
                    Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                        Return "ROLLBACK TRANSACTION"
                    Case Database.ConnectionType.MicrosoftAccess
                        Return "ROLLBACK TRANSACTION"
                    Case Database.ConnectionType.MySQL
                        Return "ROLLBACK"
                    Case Database.ConnectionType.Pervasive
                        Return "ROLLBACK"
                    Case Database.ConnectionType.HyperSQL
                        Return "ROLLBACK"
                    Case Else
                        Throw New NotImplementedException(MyBase.ConnectionType.ToString)
                End Select

            End Get
        End Property

    End Class

End Namespace
