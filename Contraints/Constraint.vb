
' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    ''' <summary>
    ''' </summary>
    Public Class Constraint(Of T)
        Implements IConstraint(Of T)

        Private pobjPredicate As Func(Of T, Boolean)

        Public Sub New(ByVal objPredicate As Func(Of T, Boolean))

            If objPredicate Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjPredicate = objPredicate

        End Sub

        Private Function ValueSatisfiesConstraint(value As T) As Boolean Implements IConstraint(Of T).ValueSatisfiesConstraint

            Return pobjPredicate.Invoke(value)

        End Function

    End Class

End Namespace
