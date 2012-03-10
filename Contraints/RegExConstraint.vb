' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Imports System.Text.RegularExpressions

Namespace Constraints

    ''' <summary>
    ''' Ensures that the associated binding value is a match for the regular expression.
    ''' Utilises the System.Text.RegularExpressions.RegEx.IsMatch() function to determine whether the value matches.
    ''' </summary>
    Public Class RegExConstraint
        Implements IConstraint(Of String)

        Private pobjRegularExpression As Regex

        Public Sub New(ByVal objRegularExpression As Regex)

            If objRegularExpression Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjRegularExpression = objRegularExpression

        End Sub

        Public Sub New(ByVal strRegularExpression As String)

            Me.New(New Regex(strRegularExpression))

        End Sub

        Private Function ValueSatisfiesConstraint(value As String) As Boolean Implements IConstraint(Of String).ValueSatisfiesConstraint

            Return pobjRegularExpression.IsMatch(value)

        End Function

        Public Overrides Function ToString() As String

            Return pobjRegularExpression.ToString

        End Function

    End Class

End Namespace
