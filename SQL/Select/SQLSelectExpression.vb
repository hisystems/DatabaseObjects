
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectExpression
        Inherits SQLStatement

        Private pobjExpression As SQL.SQLExpression

        Public Sub New(ByVal objExpression As SQL.SQLExpression)

            If objExpression Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjExpression = objExpression

        End Sub

        Public Overrides ReadOnly Property SQL() As String
            Get

                Return "SELECT " & pobjExpression.SQL(MyBase.ConnectionType)

            End Get
        End Property

    End Class

End Namespace
