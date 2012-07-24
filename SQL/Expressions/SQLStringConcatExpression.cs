// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Linq;
	
namespace DatabaseObjects.SQL
{
	public class SQLStringConcatExpression : SQLExpression
	{
		private SQLExpression left;
		private SQLExpression right;
			
		public SQLStringConcatExpression()
		{
		}
			
		public SQLStringConcatExpression(string leftString, string rightString) 
            : this(new SQLValueExpression(leftString), new SQLValueExpression(rightString))
		{
		}
			
		public SQLStringConcatExpression(SQLExpression leftExpression, SQLExpression rightExpression)
		{
			this.LeftExpression = leftExpression;
			this.RightExpression = rightExpression;
		}
			
		public SQLExpression LeftExpression
		{
			get
			{
				return left;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				left = value;
			}
		}
			
		public SQLExpression RightExpression
		{
			get
			{
				return right;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				right = value;
			}
		}
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeStringContactExpression(this);
		}

		public static SQLExpression ConcatenateAll(params SQLExpression[] sqlExpressions)
		{
			if (sqlExpressions.Length < 2)
				throw new ArgumentException("Two or more expressions are required for string concatenation");
				
			SQLStringConcatExpression currentExpression = null;
				
			currentExpression = new SQLStringConcatExpression();
			currentExpression.LeftExpression = sqlExpressions[0];
			currentExpression.RightExpression = sqlExpressions[1];
				
			foreach (var sqlExpression in sqlExpressions.Skip(2))
			{
				SQLStringConcatExpression newExpression = new SQLStringConcatExpression();
				newExpression.LeftExpression = currentExpression;
				newExpression.RightExpression = sqlExpression;
					
				currentExpression = newExpression;
			}
				
			return currentExpression;
		}
	}
}
