// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//	    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// _________________________________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
    public abstract class SQLExpression
    {
        internal abstract string SQL(Serializers.Serializer serializer);

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