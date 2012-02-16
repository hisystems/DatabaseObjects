
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On


Namespace SQL

    Public Enum ArithmeticOperator
        Add
        Subtract
        Multiply
        Divide
        Modulus
    End Enum

    Public Class SQLArithmeticExpression
        Inherits SQLExpression

        Private pobjLeft As SQLExpression
        Private peOperator As ArithmeticOperator
        Private pobjRight As SQLExpression

        Public Sub New()


        End Sub

        Public Sub New(ByVal strLeftFieldName As String, ByVal eOperator As ArithmeticOperator, ByVal objRightValue As Object)

            Me.New(New SQLFieldExpression(strLeftFieldName), eOperator, New SQLValueExpression(objRightValue))

        End Sub

        Public Sub New(ByVal strLeftFieldName As String, ByVal eOperator As ArithmeticOperator, ByVal objRightExpression As SQLExpression)

            Me.New(New SQLFieldExpression(strLeftFieldName), eOperator, objRightExpression)

        End Sub

        Public Sub New(ByVal objLeftExpression As SQLExpression, ByVal eOperator As ArithmeticOperator, ByVal objRightExpression As SQLExpression)

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

        Public Property [Operator]() As ArithmeticOperator
            Get

                Return peOperator

            End Get

            Set(ByVal value As ArithmeticOperator)

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

        Private Function OperatorString(ByVal eOperator As SQL.ArithmeticOperator) As String

            Select Case eOperator
                Case ArithmeticOperator.Add
                    Return "+"
                Case ArithmeticOperator.Subtract
                    Return "-"
                Case ArithmeticOperator.Divide
                    Return "/"
                Case ArithmeticOperator.Multiply
                    Return "*"
                Case ArithmeticOperator.Modulus
                    Return "%"
                Case Else
                    Throw New NotSupportedException(GetType(SQL.ArithmeticOperator).Name)
            End Select

        End Function

    End Class

End Namespace
