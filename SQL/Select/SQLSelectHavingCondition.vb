
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectHavingCondition
        Inherits SQLExpression

        Private pobjLeftHandExpression As SQLExpression
        Private peCompare As ComparisonOperator
        Private pobjRightHandExpression As SQLExpression

        Public Sub New(ByVal objLeftHandExpression As SQLExpression, ByVal eCompare As ComparisonOperator, ByVal objRightHandExpression As SQLExpression)

            If objLeftHandExpression Is Nothing Then
                Throw New ArgumentNullException
            ElseIf objRightHandExpression Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjLeftHandExpression = objLeftHandExpression
            peCompare = eCompare
            pobjRightHandExpression = objRightHandExpression

        End Sub

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Return pobjLeftHandExpression.SQL(eConnectionType) & " " & SQLConvertCompare(peCompare) & " " & pobjRightHandExpression.SQL(eConnectionType)

        End Function

    End Class

End Namespace
