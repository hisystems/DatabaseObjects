
' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On


Namespace SQL

    Public Enum BitwiseOperator
        [And]
        [Or]
    End Enum

    Public Class SQLBitwiseExpression
        Inherits SQLExpression

        Private pobjLeft As SQLExpression
        Private peOperator As BitwiseOperator
        Private pobjRight As SQLExpression

        Public Sub New()

        End Sub

        Public Sub New(ByVal strLeftFieldName As String, ByVal eOperator As BitwiseOperator, ByVal objRightValue As Object)

            Me.New(New SQLFieldExpression(strLeftFieldName), eOperator, New SQLValueExpression(objRightValue))

        End Sub

        Public Sub New(ByVal strLeftFieldName As String, ByVal eOperator As BitwiseOperator, ByVal objRightExpression As SQLExpression)

            Me.New(New SQLFieldExpression(strLeftFieldName), eOperator, objRightExpression)

        End Sub

        Public Sub New(ByVal objLeftExpression As SQLExpression, ByVal eOperator As BitwiseOperator, ByVal objRightExpression As SQLExpression)

            pobjLeft = objLeftExpression
            peOperator = eOperator
            pobjRight = objRightExpression

        End Sub

        Public Property LeftExpression() As SQLExpression
            Get

                Return pobjLeft

            End Get

            Set(ByVal value As SQLExpression)

                If value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjLeft = value

            End Set
        End Property

        Public Property RightExpression() As SQLExpression
            Get

                Return pobjRight

            End Get

            Set(ByVal value As SQLExpression)

                If value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjRight = value

            End Set
        End Property

        Public Property [Operator]() As BitwiseOperator
            Get

                Return peOperator

            End Get

            Set(ByVal value As BitwiseOperator)

                peOperator = value

            End Set
        End Property

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            If pobjLeft Is Nothing Then
                Throw New ArgumentNullException(Me.GetType.Name & ".LeftExpression")
            ElseIf pobjRight Is Nothing Then
                Throw New ArgumentNullException(Me.GetType.Name & ".RightExpression")
            End If

            Return "(" & pobjLeft.SQL(eConnectionType) & " " & OperatorString(peOperator) & " " & pobjRight.SQL(eConnectionType) & ")"

        End Function

        Private Function OperatorString(ByVal eOperator As SQL.BitwiseOperator) As String

            Select Case eOperator
                Case BitwiseOperator.And
                    Return "&"
                Case BitwiseOperator.Or
                    Return "|"
                Case Else
                    Throw New NotSupportedException(GetType(SQL.BitwiseOperator).Name)
            End Select

        End Function

    End Class

End Namespace
