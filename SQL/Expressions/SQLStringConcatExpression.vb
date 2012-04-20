' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On
Option Infer On

Imports System.Linq

Namespace SQL

    Public Class SQLStringConcatExpression
        Inherits SQLExpression

        Private _left As SQLExpression
        Private _right As SQLExpression

        Public Sub New()


        End Sub

        Public Sub New(ByVal leftString As String, ByVal rightString As String)

            Me.New(New SQLValueExpression(leftString), New SQLValueExpression(rightString))

        End Sub

        Public Sub New(ByVal leftExpression As SQLExpression, ByVal rightExpression As SQLExpression)

            Me.LeftExpression = leftExpression
            Me.RightExpression = rightExpression

        End Sub

        Public Property LeftExpression() As SQLExpression
            Get

                Return _left

            End Get

            Set(ByVal value As SQLExpression)

                If value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                _left = value

            End Set
        End Property

        Public Property RightExpression() As SQLExpression
            Get

                Return _right

            End Get

            Set(ByVal value As SQLExpression)

                If value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                _right = value

            End Set
        End Property

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            If _left Is Nothing Then
                Throw New ArgumentNullException(Me.GetType.Name & ".LeftExpression")
            ElseIf _right Is Nothing Then
                Throw New ArgumentNullException(Me.GetType.Name & ".RightExpression")
            End If

            Return _left.SQL(eConnectionType) & " + " & _right.SQL(eConnectionType)

        End Function

        Public Shared Function ConcatenateAll(ParamArray sqlExpressions() As SQLExpression) As SQLExpression

            If sqlExpressions.Length < 2 Then
                Throw New ArgumentException("Two or more expressions are required for string concatenation")
            End If

            Dim currentExpression As SQLStringConcatExpression = Nothing

            currentExpression = New SQLStringConcatExpression()
            currentExpression.LeftExpression = sqlExpressions(0)
            currentExpression.RightExpression = sqlExpressions(1)

            For Each sqlExpression In sqlExpressions.Skip(2)

                Dim newExpression As New SQLStringConcatExpression()
                newExpression.LeftExpression = currentExpression
                newExpression.RightExpression = sqlExpression

                currentExpression = newExpression
            Next

            Return currentExpression

        End Function

    End Class

End Namespace

