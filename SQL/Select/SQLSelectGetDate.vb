
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelectGetDate
        Inherits SQLSelectExpression

        Public Sub New()

            MyBase.New(New SQLGetDateFunctionExpression)

        End Sub
         
    End Class

End Namespace
