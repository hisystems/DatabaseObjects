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
    ''' This constraint will ensure that the date is either today or a future
    ''' when the associated object is not saved. 
    ''' If the object is not new, then the date constraint does not apply.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DateIsTodayOrFutureForNewObjectConstraint
        Implements IConstraint(Of Date)

        Private _associatedObject As IDatabaseObject

        Public Sub New(ByVal associatedObject As IDatabaseObject)

            If associatedObject Is Nothing Then
                Throw New ArgumentNullException
            End If

            Me._associatedObject = associatedObject

        End Sub

        Public Function ValueSatisfiesConstraint(ByVal value As Date) As Boolean Implements IConstraint(Of Date).ValueSatisfiesConstraint

            Return _associatedObject.IsSaved OrElse value >= Date.Today

        End Function

        Public Overrides Function ToString() As String

            Return "date must be today or a future date for a new object"

        End Function

    End Class

End Namespace
