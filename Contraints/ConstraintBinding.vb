' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    Public Class ConstraintBinding(Of T)

        Private getValue As Func(Of T)
        Private pconstraint As IConstraint(Of T)
        Private perrorMessage As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="getValue"></param>
        ''' <param name="constraint"></param>
        ''' <param name="errorMessage">
        ''' An error message that is raised as part of an exception, and/or from the user interface. 
        ''' Parameter {0} represents the value from the callback.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal getValue As Func(Of T), ByVal constraint As IConstraint(Of T), ByVal errorMessage As String)

            Me.pconstraint = constraint
            Me.getValue = getValue
            Me.perrorMessage = errorMessage

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="getValue"></param>
        ''' <param name="constraint"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal getValue As Func(Of T), ByVal constraint As IConstraint(Of T))

            Me.New(getValue, constraint, "Value '{0}' did not satisfy constraint; " & constraint.ToString())

        End Sub

        ''' <summary>
        ''' Uses the current value from the object to determine whether the constraint passes.
        ''' True indicates that the constraint passes.
        ''' </summary>
        ''' <remarks></remarks>
        Public Function ConstraintSatisfied() As Boolean

            Return pconstraint.ValueSatisfiesConstraint(Me.getValue())

        End Function

        ''' <summary>
        ''' Uses the current value from the callback to determine whether the constraint passes.
        ''' If not, then an ArgumentException is thrown.
        ''' </summary>
        Public Sub EnsureConstraintSatisfied()

            Dim value As T = getValue()

            If Not pconstraint.ValueSatisfiesConstraint(value) Then
                Throw New ArgumentException(ErrorMessage(value))
            End If

        End Sub

        ''' <summary>
        ''' Returns the error message using the current value associated with this constraint.
        ''' </summary>
        Public Function ErrorMessage() As String

            Return ErrorMessage(Me.getValue())

        End Function

        Private Function ErrorMessage(ByVal value As T) As String

            Return String.Format(perrorMessage, value)

        End Function

        Public ReadOnly Property Constraint As IConstraint(Of T)
            Get

                Return pconstraint

            End Get
        End Property

        ''' <summary>
        ''' Copies the current constraint binding, but binds to the variable specified.
        ''' </summary>
        ''' <example>
        ''' Public Property Name As String
        '''     Set(ByVal value As String)
        '''
        '''        nameIsSetBinding.Clone(value).EnsureConstraintSatisfied()
        '''        Me._name = value
        '''
        '''     End Set
        ''' End Property
        ''' </example>
        Public Function Clone(ByVal valueToBind As T) As ConstraintBinding(Of T)

            Return New ConstraintBinding(Of T)(Function() valueToBind, Me.pconstraint, Me.perrorMessage)

        End Function

        ''' <summary>
        ''' Copies the current constraint binding, but binds to a new 'getValue' callback.
        ''' </summary>
        Public Function Clone(ByVal getValue As Func(Of T)) As ConstraintBinding(Of T)

            Return New ConstraintBinding(Of T)(getValue, Me.pconstraint, Me.perrorMessage)

        End Function

    End Class

End Namespace
