
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Imports System.Linq
Imports System.Collections.Generic

Namespace SQL

    ''' <summary>
    ''' Allows the use of CASE statements. Specifically:
    ''' 
    ''' Simple CASE expression: 
    ''' CASE input_expression 
    ''' WHEN when_expression THEN result_expression [ ...n ] 
    ''' [ ELSE else_result_expression ] 
    ''' END 
    ''' 
    ''' Searched CASE expression:
    ''' CASE
    ''' WHEN Boolean_expression THEN result_expression [ ...n ] 
    ''' [ ELSE else_result_expression ] 
    ''' END
    ''' </summary>
    Public Class SQLCaseExpression
        Inherits SQLExpression

        Public Class CasesCollection

            Private pobjCases As New List(Of [Case])

            Public Sub Add(ByVal objWhenCondition As SQLExpression, ByVal objResult As SQLExpression)

                pobjCases.Add(New [Case](objWhenCondition, objResult))

            End Sub

            Public Sub Add(ByVal objCase As [Case])

                pobjCases.Add(objCase)

            End Sub

            Friend Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

                If pobjCases.Count = 0 Then
                    Throw New InvalidOperationException("There are no cases for the CASE statement")
                End If

                Return String.Join(" ", pobjCases.Select(Function(objCase) objCase.SQL(eConnectionType)).ToArray)

            End Function

        End Class

        Public Class [Case]

            Private pobjWhenCondition As SQLExpression
            Private pobjResult As SQLExpression

            ''' <summary>
            ''' 
            ''' </summary>
            Public Sub New(ByVal objWhenCondition As SQLExpression, ByVal objResult As SQLExpression)

                If objWhenCondition Is Nothing Then
                    Throw New ArgumentNullException("When condition")
                ElseIf objResult Is Nothing Then
                    Throw New ArgumentNullException("Result")
                End If

                pobjWhenCondition = objWhenCondition
                pobjResult = objResult

            End Sub

            Friend Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

                Return "WHEN " & pobjWhenCondition.SQL(eConnectionType) & " THEN " & pobjResult.SQL(eConnectionType)

            End Function

        End Class

        Private pobjInputExpression As SQLExpression
        Private pobjCases As New CasesCollection
        Private pobjElseResult As SQLExpression

        Public Sub New()

        End Sub

        Public Sub New(ByVal objInputExpression As SQLExpression)

            If objInputExpression Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjInputExpression = objInputExpression

        End Sub

        Public ReadOnly Property Cases As CasesCollection
            Get

                Return pobjCases

            End Get
        End Property

        Public Property ElseResult As SQLExpression
            Get

                Return pobjElseResult

            End Get

            Set(value As SQLExpression)

                pobjElseResult = value

            End Set
        End Property

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Dim strSQL As String = "CASE"

            If pobjInputExpression IsNot Nothing Then
                strSQL &= " " & pobjInputExpression.SQL(eConnectionType)
            End If

            strSQL &= " " & pobjCases.SQL(eConnectionType)

            If pobjElseResult IsNot Nothing Then
                strSQL &= " ELSE " & pobjElseResult.SQL(eConnectionType)
            End If

            strSQL &= " END"

            Return strSQL

        End Function

    End Class

End Namespace
