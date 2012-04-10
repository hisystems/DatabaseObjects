
' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLAggregateExpression
        Inherits SQLFunctionExpression

        Public Sub New(ByVal aggregate As AggregateFunction, ByVal fieldName As String)

            Me.New(aggregate, New SQLFieldExpression(fieldName))

        End Sub

        Public Sub New(ByVal aggregate As AggregateFunction, ByVal expression As SQLExpression)

            MyBase.New(Misc.SQLConvertAggregate(aggregate), expression)

        End Sub

    End Class

End Namespace


