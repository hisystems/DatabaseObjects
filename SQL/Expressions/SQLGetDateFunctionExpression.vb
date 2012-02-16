
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLGetDateFunctionExpression
        Inherits SQLExpression

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Select Case eConnectionType
                Case Database.ConnectionType.MicrosoftAccess
                    Return "Now()"
                Case Database.ConnectionType.SQLServer
                    Return "GetDate()"
                Case Database.ConnectionType.SQLServerCompactEdition
                    Return "GetDate()"
                Case Database.ConnectionType.MySQL
                    Throw New NotImplementedException()
                Case Database.ConnectionType.Pervasive
                    Throw New NotImplementedException()
                Case Else
                    Throw New NotSupportedException()
            End Select

        End Function

    End Class

End Namespace
