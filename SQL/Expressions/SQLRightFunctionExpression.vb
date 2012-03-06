﻿' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au<http://www.hisystems.com.au> - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLRightFunctionExpression
        Inherits SQLFunctionExpression

        Public Sub New(ByVal strFieldName As String, ByVal intLength As Integer)

            Me.New(New SQLFieldExpression(strFieldName), intLength)

        End Sub

        Public Sub New(ByVal objExpression As SQLExpression, ByVal intLength As Integer)

            MyBase.New("RIGHT", objExpression, New SQLValueExpression(intLength))

            If intLength < 0 Then
                Throw New ArgumentException("Length: " & intLength)
            End If

        End Sub

    End Class

End Namespace