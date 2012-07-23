// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
    public abstract class SQLExpression
    {
        internal abstract string SQL(Database.ConnectionType eConnectionType);

        public static SQLArithmeticExpression operator +(SQLExpression left, SQLExpression right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Add, right);
        }

        public static SQLArithmeticExpression operator +(SQLExpression left, decimal right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Add, new SQLValueExpression(right));
        }

        public static SQLArithmeticExpression operator +(decimal left, SQLExpression right)
        {
            return new SQLArithmeticExpression(new SQLValueExpression(left), ArithmeticOperator.Add, right);
        }

        public static SQLArithmeticExpression operator +(SQLExpression left, DateTime right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Add, new SQLValueExpression(right));
        }

        public static SQLArithmeticExpression operator +(DateTime left, SQLExpression right)
        {
            return new SQLArithmeticExpression(new SQLValueExpression(left), ArithmeticOperator.Add, right);
        }

        public static SQLStringConcatExpression operator +(string left, SQLExpression right)
        {
            return new SQLStringConcatExpression(new SQLValueExpression(left), right);
        }

        public static SQLStringConcatExpression operator +(SQLExpression left, string right)
        {
            return new SQLStringConcatExpression(left, new SQLValueExpression(right));
        }

        public static SQLStringConcatExpression operator +(SQLStringConcatExpression left, SQLExpression right)
        {
            return new SQLStringConcatExpression(left, right);
        }

        public static SQLStringConcatExpression operator +(SQLExpression left, SQLStringConcatExpression right)
        {
            return new SQLStringConcatExpression(left, right);
        }
        
        public static SQLArithmeticExpression operator -(SQLExpression left, SQLExpression right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Subtract, right);
        }

        public static SQLArithmeticExpression operator -(SQLExpression left, decimal right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Subtract, new SQLValueExpression(right));
        }

        public static SQLArithmeticExpression operator -(decimal left, SQLExpression right)
        {
            return new SQLArithmeticExpression(new SQLValueExpression(left), ArithmeticOperator.Subtract, right);
        }

        public static SQLArithmeticExpression operator -(SQLExpression left, DateTime right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Subtract, new SQLValueExpression(right));
        }

        public static SQLArithmeticExpression operator -(DateTime left, SQLExpression right)
        {
            return new SQLArithmeticExpression(new SQLValueExpression(left), ArithmeticOperator.Subtract, right);
        }

        public static SQLArithmeticExpression operator *(SQLExpression left, SQLExpression right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Multiply, right);
        }

        public static SQLArithmeticExpression operator *(SQLExpression left, decimal right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Multiply, new SQLValueExpression(right));
        }

        public static SQLArithmeticExpression operator *(decimal left, SQLExpression right)
        {
            return new SQLArithmeticExpression(new SQLValueExpression(left), ArithmeticOperator.Multiply, right);
        }

        public static SQLArithmeticExpression operator /(SQLExpression left, SQLExpression right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Divide, right);
        }

        public static SQLArithmeticExpression operator /(SQLExpression left, decimal right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Divide, new SQLValueExpression(right));
        }

        public static SQLArithmeticExpression operator /(decimal left, SQLExpression right)
        {
            return new SQLArithmeticExpression(new SQLValueExpression(left), ArithmeticOperator.Divide, right);
        }

        public static SQLArithmeticExpression operator %(SQLExpression left, SQLExpression right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Modulus, right);
        }

        public static SQLArithmeticExpression operator %(SQLExpression left, decimal right)
        {
            return new SQLArithmeticExpression(left, ArithmeticOperator.Modulus, new SQLValueExpression(right));
        }

        public static SQLArithmeticExpression operator %(decimal left, SQLExpression right)
        {
            return new SQLArithmeticExpression(new SQLValueExpression(left), ArithmeticOperator.Modulus, right);
        }

        public static SQLBitwiseExpression operator |(SQLExpression left, SQLExpression right)
        {
            return new SQLBitwiseExpression(left, BitwiseOperator.Or, right);
        }

        public static SQLBitwiseExpression operator |(SQLExpression left, decimal right)
        {
            return new SQLBitwiseExpression(left, BitwiseOperator.Or, new SQLValueExpression(right));
        }

        public static SQLBitwiseExpression operator |(decimal left, SQLExpression right)
        {
            return new SQLBitwiseExpression(new SQLValueExpression(left), BitwiseOperator.Or, right);
        }

        public static SQLBitwiseExpression operator &(SQLExpression left, SQLExpression right)
        {
            return new SQLBitwiseExpression(left, BitwiseOperator.And, right);
        }

        public static SQLBitwiseExpression operator &(SQLExpression left, decimal right)
        {
            return new SQLBitwiseExpression(left, BitwiseOperator.And, new SQLValueExpression(right));
        }

        public static SQLBitwiseExpression operator &(decimal left, SQLExpression right)
        {
            return new SQLBitwiseExpression(new SQLValueExpression(left), BitwiseOperator.And, right);
        }
    }
}