
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLConditionExpression
        Inherits SQLExpression

        Private pobjLeftExpression As SQLExpression
        Private peCompare As ComparisonOperator
        Private pobjRightExpression As SQLExpression

        Friend Sub New()

        End Sub

        Public Sub New( _
            ByVal objLeftExpression As SQLExpression, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objRightExpression As SQLExpression)

            Me.LeftExpression = objLeftExpression
            Me.Compare = eCompare
            Me.RightExpression = objRightExpression

        End Sub

        Public Property LeftExpression() As SQLExpression
            Get

                Return pobjLeftExpression

            End Get

            Set(ByVal Value As SQLExpression)

                If Value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjLeftExpression = Value

            End Set
        End Property

        Public Property Compare() As ComparisonOperator
            Get

                Return peCompare

            End Get

            Set(ByVal Value As ComparisonOperator)

                peCompare = Value

            End Set
        End Property

        Public Property RightExpression() As SQLExpression
            Get

                Return pobjRightExpression

            End Get

            Set(ByVal Value As SQLExpression)

                If Value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjRightExpression = Value

            End Set
        End Property

        Friend Overrides Function SQL(eConnectionType As Database.ConnectionType) As String

            If pobjLeftExpression Is Nothing Then
                Throw New InvalidOperationException("Left expression is not set")
            ElseIf pobjRightExpression Is Nothing Then
                Throw New InvalidOperationException("Right expression is not set")
            End If

            Return Condition(pobjLeftExpression, peCompare, pobjRightExpression, eConnectionType)

        End Function

        Private Function Condition( _
            ByVal objLeftExpression As SQLExpression, _
            ByVal eCompare As ComparisonOperator, _
            ByVal objRightExpression As SQLExpression, _
            ByVal eConnectionType As Database.ConnectionType) As String

            Dim strSQL As String = pobjLeftExpression.SQL(eConnectionType) & " "

            If TypeOf objRightExpression Is SQLValueExpression Then
                strSQL &= _
                    SQLConvertCondition(eCompare, DirectCast(objRightExpression, SQLValueExpression).Value, eConnectionType)
            Else
                strSQL &= _
                    SQLConvertCompare(eCompare) & " " & _
                    objRightExpression.SQL(eConnectionType)
            End If

            Return strSQL

        End Function

    End Class

End Namespace
