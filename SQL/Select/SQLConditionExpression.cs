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
	public class SQLConditionExpression : SQLExpression
	{
		private SQLExpression pobjLeftExpression;
		private ComparisonOperator peCompare;
		private SQLExpression pobjRightExpression;
			
		internal SQLConditionExpression()
		{
		}
			
		public SQLConditionExpression(SQLExpression objLeftExpression, ComparisonOperator eCompare, SQLExpression objRightExpression)
		{
			this.LeftExpression = objLeftExpression;
			this.Compare = eCompare;
			this.RightExpression = objRightExpression;
		}
			
		public SQLExpression LeftExpression
		{
			get
			{
				return pobjLeftExpression;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjLeftExpression = value;
			}
		}
			
		public ComparisonOperator Compare
		{
			get
			{
				return peCompare;
			}
				
			set
			{
				peCompare = value;
			}
		}
			
		public SQLExpression RightExpression
		{
			get
			{
				return pobjRightExpression;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjRightExpression = value;
			}
		}
			
		internal override string SQL(Database.ConnectionType eConnectionType)
		{
			if (pobjLeftExpression == null)
				throw new InvalidOperationException("Left expression is not set");
			else if (pobjRightExpression == null)
				throw new InvalidOperationException("Right expression is not set");
				
			return Condition(pobjLeftExpression, peCompare, pobjRightExpression, eConnectionType);
		}
			
		private string Condition(SQLExpression objLeftExpression, ComparisonOperator eCompare, SQLExpression objRightExpression, Database.ConnectionType eConnectionType)
		{
			string strSQL = pobjLeftExpression.SQL(eConnectionType) + " ";
				
			if (objRightExpression is SQLValueExpression)
				strSQL += Misc.SQLConvertCondition(eCompare, ((SQLValueExpression) objRightExpression).Value, eConnectionType);
			else
				strSQL += Misc.SQLConvertCompare(eCompare) + " " + objRightExpression.SQL(eConnectionType);
				
			return strSQL;
		}
	}
}
