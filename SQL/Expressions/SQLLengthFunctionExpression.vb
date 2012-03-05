
' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    ''' <summary>
    ''' SQL function that returns the length of a string.
    ''' </summary>
    Public Class SQLLengthFunctionExpression
        Inherits SQLFunctionExpression

        Public Sub New(objExpression As SQLExpression)

            MyBase.New(objExpression)

        End Sub

        Protected Overrides Function FunctionName(ByVal eConnectionType As Database.ConnectionType) As String

            Select Case eConnectionType
                Case Database.ConnectionType.MySQL
                    Return "LENGTH"
                Case Database.ConnectionType.SQLServer, Database.ConnectionType.MicrosoftAccess
                    Return "LEN"
                Case Else
                    Throw New NotImplementedException(eConnectionType.ToString)
            End Select

            Return MyBase.FunctionName(eConnectionType)

        End Function

    End Class

End Namespace
