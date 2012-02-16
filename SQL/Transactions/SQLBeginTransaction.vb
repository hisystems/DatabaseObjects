
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Namespace SQL

    Public Class SQLBeginTransaction
        Inherits SQL.SQLStatement

        Public Overrides ReadOnly Property SQL() As String
            Get

                Select Case MyBase.ConnectionType
                    Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                        Return "BEGIN TRANSACTION"
                    Case Database.ConnectionType.MicrosoftAccess
                        Return "BEGIN TRANSACTION"
                    Case Database.ConnectionType.MySQL
                        Return "START TRANSACTION"
                    Case Database.ConnectionType.Pervasive
                        Return "START TRANSACTION"
                    Case Database.ConnectionType.HyperSQL
                        Return "START TRANSACTION"
                    Case Else
                        Throw New NotImplementedException(Me.ConnectionType.ToString)
                End Select

            End Get
        End Property

    End Class

End Namespace
