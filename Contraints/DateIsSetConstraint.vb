' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    Public Class DateIsSetConstraint
        Implements IConstraint(Of Date)

        Private unsetDateValue As Date

        ''' <summary>
        ''' The unset / default value for the date is Date.MinValue.
        ''' So if the date is not Date.MinValue then the constraint passes.
        ''' </summary>
        Public Sub New()

            Me.unsetDateValue = Date.MinValue

        End Sub

        Public Sub New(ByVal unsetDateValue As Date)

            Me.unsetDateValue = unsetDateValue

        End Sub

        Public Function ValueSatisfiesConstraint(ByVal value As Date) As Boolean Implements IConstraint(Of Date).ValueSatisfiesConstraint

            Return value <> unsetDateValue

        End Function

        Public Overrides Function ToString() As String

            Return "date must not be " & unsetDateValue.ToString

        End Function

    End Class

End Namespace
