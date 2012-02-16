' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    Public Class ObjectIsSetConstraint
        Implements IConstraint(Of Object)

        Public Function ValueSatisfiesConstraint(ByVal value As Object) As Boolean Implements IConstraint(Of Object).ValueSatisfiesConstraint

            Return Not value Is Nothing

        End Function

        Public Overrides Function ToString() As String

            Return "object must not be null"

        End Function

    End Class

End Namespace
