' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    Public Class NumberIsMinimumOrGreaterConstraint
        Implements IConstraint(Of Decimal)

        Private minimumValue As Decimal

        Public Sub New(ByVal minimumValue As Decimal)

            Me.minimumValue = minimumValue

        End Sub

        Public Function ValueSatisfiesConstraint(ByVal value As Decimal) As Boolean Implements IConstraint(Of Decimal).ValueSatisfiesConstraint

            Return value >= minimumValue

        End Function

        Public Overrides Function ToString() As String

            Return "number must be " & minimumValue & " or greater"

        End Function

    End Class

End Namespace
