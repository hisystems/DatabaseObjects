
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace SQL

    Public Class SQLCommitTransaction
        Inherits SQL.SQLStatement

        Public Overrides ReadOnly Property SQL() As String
            Get

                Select Case MyBase.ConnectionType
                    Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                        Return "COMMIT TRANSACTION"
                    Case Database.ConnectionType.MicrosoftAccess
                        Return "COMMIT TRANSACTION"
                    Case Database.ConnectionType.MySQL
                        Return "COMMIT"
                    Case Database.ConnectionType.Pervasive
                        Return "COMMIT"
                    Case Database.ConnectionType.HyperSQL
                        Return "COMMIT"
                    Case Else
                        Throw New NotImplementedException(Me.ConnectionType.ToString)
                End Select

            End Get
        End Property

    End Class

End Namespace
