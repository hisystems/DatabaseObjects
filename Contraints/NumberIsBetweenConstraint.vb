' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    ''' <summary>
    ''' Indicates that the number must fall between (exclusive)
    ''' the numbers specified.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class NumberIsBetweenConstraint
        Implements IConstraint(Of Decimal)

        Private startingNumber As Decimal
        Private endingNumber As Decimal

        Public Sub New(ByVal startingNumber As Decimal, ByVal endingNumber As Decimal)

            Me.startingNumber = startingNumber
            Me.endingNumber = endingNumber

        End Sub

        Public Function ValueSatisfiesConstraint(ByVal value As Decimal) As Boolean Implements IConstraint(Of Decimal).ValueSatisfiesConstraint

            Return value > startingNumber AndAlso value < endingNumber

        End Function

        Public Overrides Function ToString() As String

            Return "number must be between " & startingNumber & " to " & endingNumber & " (exclusive)"

        End Function

    End Class

End Namespace
