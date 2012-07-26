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
	public enum ArithmeticOperator
	{
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus
	}
		
	public class SQLArithmeticExpression : SQLExpression
	{
		private SQLExpression pobjLeft;
		private ArithmeticOperator peOperator;
		private SQLExpression pobjRight;
			
		public SQLArithmeticExpression()
		{
		}
			
		public SQLArithmeticExpression(string strLeftFieldName, ArithmeticOperator eOperator, object objRightValue) 
            : this(new SQLFieldExpression(strLeftFieldName), eOperator, new SQLValueExpression(objRightValue))
		{
		}
			
		public SQLArithmeticExpression(string strLeftFieldName, ArithmeticOperator eOperator, SQLExpression objRightExpression) 
            : this(new SQLFieldExpression(strLeftFieldName), eOperator, objRightExpression)
		{
		}
			
		public SQLArithmeticExpression(SQLExpression objLeftExpression, ArithmeticOperator eOperator, SQLExpression objRightExpression)
		{
			pobjLeft = objLeftExpression;
			peOperator = eOperator;
			pobjRight = objRightExpression;
		}
			
		public SQLExpression LeftExpression
		{
			get
			{
				return pobjLeft;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjLeft = value;
			}
		}
			
		public SQLExpression RightExpression
		{
			get
			{
				return pobjRight;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjRight = value;
			}
		}
			
		public ArithmeticOperator Operator
		{
			get
			{
				return peOperator;
			}
				
			set
			{
				peOperator = value;
			}
		}
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeArithmeticExpression(this);
		}
	}
}