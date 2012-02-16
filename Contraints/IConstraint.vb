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
    ''' A constraint / piece of business logic / rule that is shared by the user interface and business logic layers.
    ''' </summary>
    Public Interface IConstraint(Of T)

        ''' <summary>
        ''' Indicates that the constraint passes, and no user error message should be displayed.
        ''' </summary>
        Function ValueSatisfiesConstraint(ByVal value As T) As Boolean

    End Interface

End Namespace
