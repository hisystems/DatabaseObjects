
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public MustInherit Class SQLExpression

        Friend MustOverride Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

        Public Shared Operator +(left As SQLExpression, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Add, right)

        End Operator

        Public Shared Operator +(left As SQLExpression, right As Decimal) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Add, New SQLValueExpression(right))

        End Operator

        Public Shared Operator +(left As Decimal, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(New SQLValueExpression(left), ArithmeticOperator.Add, right)

        End Operator

        Public Shared Operator +(left As SQLExpression, right As DateTime) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Add, New SQLValueExpression(right))

        End Operator

        Public Shared Operator +(left As DateTime, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(New SQLValueExpression(left), ArithmeticOperator.Add, right)

        End Operator

        Public Shared Operator +(left As String, right As SQLExpression) As SQLStringConcatExpression

            Return New SQLStringConcatExpression(New SQLValueExpression(left), right)

        End Operator

        Public Shared Operator +(left As SQLExpression, right As String) As SQLStringConcatExpression

            Return New SQLStringConcatExpression(left, New SQLValueExpression(right))

        End Operator

        Public Shared Operator +(left As SQLStringConcatExpression, right As SQLExpression) As SQLStringConcatExpression

            Return New SQLStringConcatExpression(left, right)

        End Operator

        Public Shared Operator +(left As SQLExpression, right As SQLStringConcatExpression) As SQLStringConcatExpression

            Return New SQLStringConcatExpression(left, right)

        End Operator

        Public Shared Operator &(left As String, right As SQLExpression) As SQLStringConcatExpression

            Return New SQLStringConcatExpression(New SQLValueExpression(left), right)

        End Operator

        Public Shared Operator &(left As SQLExpression, right As String) As SQLStringConcatExpression

            Return New SQLStringConcatExpression(left, New SQLValueExpression(right))

        End Operator

        Public Shared Operator &(left As SQLStringConcatExpression, right As SQLExpression) As SQLStringConcatExpression

            Return New SQLStringConcatExpression(left, right)

        End Operator

        Public Shared Operator &(left As SQLExpression, right As SQLStringConcatExpression) As SQLStringConcatExpression

            Return New SQLStringConcatExpression(left, right)

        End Operator

        Public Shared Operator -(left As SQLExpression, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Subtract, right)

        End Operator

        Public Shared Operator -(left As SQLExpression, right As Decimal) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Subtract, New SQLValueExpression(right))

        End Operator

        Public Shared Operator -(left As Decimal, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(New SQLValueExpression(left), ArithmeticOperator.Subtract, right)

        End Operator

        Public Shared Operator -(left As SQLExpression, right As DateTime) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Subtract, New SQLValueExpression(right))

        End Operator

        Public Shared Operator -(left As DateTime, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(New SQLValueExpression(left), ArithmeticOperator.Subtract, right)

        End Operator

        Public Shared Operator *(left As SQLExpression, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Multiply, right)

        End Operator

        Public Shared Operator *(left As SQLExpression, right As Decimal) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Multiply, New SQLValueExpression(right))

        End Operator

        Public Shared Operator *(left As Decimal, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(New SQLValueExpression(left), ArithmeticOperator.Multiply, right)

        End Operator

        Public Shared Operator /(left As SQLExpression, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Divide, right)

        End Operator

        Public Shared Operator /(left As SQLExpression, right As Decimal) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Divide, New SQLValueExpression(right))

        End Operator

        Public Shared Operator /(left As Decimal, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(New SQLValueExpression(left), ArithmeticOperator.Divide, right)

        End Operator

        Public Shared Operator Mod(left As SQLExpression, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Modulus, right)

        End Operator

        Public Shared Operator Mod(left As SQLExpression, right As Decimal) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(left, ArithmeticOperator.Modulus, New SQLValueExpression(right))

        End Operator

        Public Shared Operator Mod(left As Decimal, right As SQLExpression) As SQLArithmeticExpression

            Return New SQLArithmeticExpression(New SQLValueExpression(left), ArithmeticOperator.Modulus, right)

        End Operator

        Public Shared Operator Or(left As SQLExpression, right As SQLExpression) As SQLBitwiseExpression

            Return New SQLBitwiseExpression(left, BitwiseOperator.Or, right)

        End Operator

        Public Shared Operator Or(left As SQLExpression, right As Decimal) As SQLBitwiseExpression

            Return New SQLBitwiseExpression(left, BitwiseOperator.Or, New SQLValueExpression(right))

        End Operator

        Public Shared Operator Or(left As Decimal, right As SQLExpression) As SQLBitwiseExpression

            Return New SQLBitwiseExpression(New SQLValueExpression(left), BitwiseOperator.Or, right)

        End Operator

        Public Shared Operator And(left As SQLExpression, right As SQLExpression) As SQLBitwiseExpression

            Return New SQLBitwiseExpression(left, BitwiseOperator.And, right)

        End Operator

        Public Shared Operator And(left As SQLExpression, right As Decimal) As SQLBitwiseExpression

            Return New SQLBitwiseExpression(left, BitwiseOperator.And, New SQLValueExpression(right))

        End Operator

        Public Shared Operator And(left As Decimal, right As SQLExpression) As SQLBitwiseExpression

            Return New SQLBitwiseExpression(New SQLValueExpression(left), BitwiseOperator.And, right)

        End Operator

    End Class

End Namespace
