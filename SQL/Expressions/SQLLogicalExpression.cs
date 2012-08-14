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
	public enum LogicalOperator
	{
		@And,
		@Or
	}

	public class SQLLogicalExpression : SQLExpression
	{
		private SQLExpression left;
		private LogicalOperator @operator;
		private SQLExpression right;
			
		public SQLLogicalExpression()
		{
		}
			
		public SQLLogicalExpression(SQLExpression leftExpression, LogicalOperator @operator, SQLExpression rightExpression)
		{
			this.LeftExpression = leftExpression;
			this.@operator = @operator;
			this.RightExpression = rightExpression;
		}
			
		public SQLExpression LeftExpression
		{
			get
			{
				return this.left;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				this.left = value;
			}
		}
			
		public SQLExpression RightExpression
		{
			get
			{
				return this.right;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				this.right = value;
			}
		}
			
		public LogicalOperator Operator
		{
			get
			{
				return this.@operator;
			}
				
			set
			{
				this.@operator = value;
			}
		}
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeLogicalExpression(this);
		}
	}
}
