' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    Public Class NumberIsMaximumOrLesserConstraint
        Implements IConstraint(Of Decimal)

        Private maximumValue As Decimal

        Public Sub New(ByVal maximumValue As Decimal)

            Me.maximumValue = maximumValue

        End Sub

        Public Function ValueSatisfiesConstraint(ByVal value As Decimal) As Boolean Implements IConstraint(Of Decimal).ValueSatisfiesConstraint

            Return value <= maximumValue

        End Function

        Public Overrides Function ToString() As String

            Return "number must be " & maximumValue & " or lower"

        End Function

    End Class

End Namespace
