
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Imports System.Linq

Namespace SQL

    ''' <summary>
    ''' Allows an SQL function to utilised in a WHERE clause or as a calculated column.
    ''' </summary>
    ''' <remarks>
    ''' Typically used to access a non database-agnostic function.
    ''' </remarks>
    Public Class SQLFunctionExpression
        Inherits SQLExpression

        Private pstrFunctionName As String
        Private pobjFunctionArguments As SQLExpression()

        Public Sub New(strFunctionName As String)

            If String.IsNullOrEmpty(strFunctionName) Then
                Throw New ArgumentNullException
            End If

            pstrFunctionName = strFunctionName

        End Sub

        Public Sub New(strFunctionName As String, ParamArray arguments() As SQLExpression)

            Me.New(strFunctionName)
            InitializeArguments(arguments)

        End Sub

        ''' <summary>
        ''' Used when the arguments are known but the function name is dependant on the connection type.
        ''' Typically, in this scenario the FunctionName() will be overridden.
        ''' </summary>
        ''' <param name="arguments"></param>
        ''' <remarks></remarks>
        Protected Sub New(ParamArray arguments() As SQLExpression)

            InitializeArguments(arguments)

        End Sub

        Private Sub InitializeArguments(ParamArray arguments() As SQLExpression)

            If arguments.Any(Function(item) item Is Nothing) Then
                Throw New ArgumentNullException
            End If

            pobjFunctionArguments = arguments

        End Sub

        Protected Overridable Function FunctionName(ByVal eConnectionType As Database.ConnectionType) As String

            Return pstrFunctionName

        End Function

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Dim strArguments As String = String.Empty

            If pobjFunctionArguments IsNot Nothing Then
                strArguments = String.Join(", ", pobjFunctionArguments.Select(Function(argument) argument.SQL(eConnectionType)).ToArray)
            End If

            Return Me.FunctionName(eConnectionType) & "(" & strArguments & ")"

        End Function

    End Class

End Namespace
