
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

            pobjFunctionArguments = arguments

        End Sub

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Dim strArguments As String = String.Empty

            If pobjFunctionArguments IsNot Nothing Then
                strArguments = String.Join(", ", pobjFunctionArguments.Select(Function(argument) argument.SQL(eConnectionType)).ToArray)
            End If

            Return pstrFunctionName & "(" & strArguments & ")"

        End Function

    End Class

End Namespace
