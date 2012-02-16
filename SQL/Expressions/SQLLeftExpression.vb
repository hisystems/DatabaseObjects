' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au<http://www.hisystems.com.au> - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLLeftExpression
        Inherits SQLExpression

        Private pobjExpression As SQLExpression
        Private pintLength As Integer

        Public Sub New(ByVal strFieldName As String, ByVal intLength As Integer)

            Me.New(New SQLFieldExpression(strFieldName), intLength)

        End Sub

        Public Sub New(ByVal objExpression As SQLExpression, ByVal intLength As Integer)

            If objExpression Is Nothing Then
                Throw New ArgumentNullException
            ElseIf intLength < 0 Then
                Throw New ArgumentException("Length: " & intLength)
            End If

            pobjExpression = objExpression
            pintLength = intLength

        End Sub

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Return "LEFT(" & pobjExpression.SQL(eConnectionType) & ", " & pintLength & ")"

        End Function

    End Class

End Namespace