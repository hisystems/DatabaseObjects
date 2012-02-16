' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    Public Class StringMaxLengthConstraint
        Implements IConstraint(Of String)

        Private pintMaxLength As Integer

        Public Sub New(ByVal intMaxLength As Integer)

            If intMaxLength < 1 Then
                Throw New ArgumentException("Maxlength cannot be less than 1 " & intMaxLength)
            End If

            pintMaxLength = intMaxLength

        End Sub

        Public ReadOnly Property MaxLength As Integer
            Get

                Return pintMaxLength

            End Get
        End Property

        Public Function ValueSatisfiesConstraint(ByVal value As String) As Boolean Implements IConstraint(Of String).ValueSatisfiesConstraint

            Return Not String.IsNullOrEmpty(value) AndAlso value.Length <= pintMaxLength

        End Function

        Public Overrides Function ToString() As String

            Return "string length is less than or equal to " & pintMaxLength & " characters"

        End Function

    End Class

End Namespace
