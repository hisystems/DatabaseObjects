// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public enum BitwiseOperator
	{
		@And,
		@Or
	}
		
	public class SQLBitwiseExpression : SQLExpression
	{
		private SQLExpression pobjLeft;
		private BitwiseOperator peOperator;
		private SQLExpression pobjRight;
			
		public SQLBitwiseExpression()
		{
		}
			
		public SQLBitwiseExpression(string strLeftFieldName, BitwiseOperator eOperator, object objRightValue) 
            : this(new SQLFieldExpression(strLeftFieldName), eOperator, new SQLValueExpression(objRightValue))
		{
		}
			
		public SQLBitwiseExpression(string strLeftFieldName, BitwiseOperator eOperator, SQLExpression objRightExpression) 
            : this(new SQLFieldExpression(strLeftFieldName), eOperator, objRightExpression)
		{
		}
			
		public SQLBitwiseExpression(SQLExpression objLeftExpression, BitwiseOperator eOperator, SQLExpression objRightExpression)
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
			
		public BitwiseOperator Operator
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
			
		internal override string SQL(Database.ConnectionType eConnectionType)
		{
			if (pobjLeft == null)
				throw new ArgumentNullException(this.GetType().Name + ".LeftExpression");
			else if (pobjRight == null)
				throw new ArgumentNullException(this.GetType().Name + ".RightExpression");
				
			return "(" + pobjLeft.SQL(eConnectionType) + " " + OperatorString(peOperator) + " " + pobjRight.SQL(eConnectionType) + ")";
		}
			
		private string OperatorString(SQL.BitwiseOperator eOperator)
		{
			if (eOperator == BitwiseOperator.And)
				return "&";
			else if (eOperator == BitwiseOperator.Or)
				return "|";
			else
				throw new NotSupportedException(typeof(SQL.BitwiseOperator).Name);
		}
	}
}
