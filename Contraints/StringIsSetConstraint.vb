' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    Public Class StringIsSetConstraint
        Implements IConstraint(Of String)

        Public Function ValueSatisfiesConstraint(ByVal value As String) As Boolean Implements IConstraint(Of String).ValueSatisfiesConstraint

            Return Not String.IsNullOrEmpty(value)

        End Function

        Public Overrides Function ToString() As String

            Return "string must not be empty"

        End Function

    End Class

End Namespace
