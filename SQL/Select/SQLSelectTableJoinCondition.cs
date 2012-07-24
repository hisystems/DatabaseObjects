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
	public class SQLSelectTableJoinCondition
	{
		private SQLExpression pobjLeftExpression;
		private ComparisonOperator peCompare;
		private SQLExpression pobjRightExpression;
		private SQLSelectTableJoinConditions pobjParent;
			
		internal SQLSelectTableJoinCondition(SQLSelectTableJoinConditions objParent)
		{
			pobjParent = objParent;
		}
			
		public SQLExpression RightExpression
		{
			get
			{
				return pobjRightExpression;
			}
				
			set
			{
				pobjRightExpression = value;
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
			
		public SQLExpression LeftExpression
		{
			get
			{
				return pobjLeftExpression;
			}
				
			set
			{
				pobjLeftExpression = value;
			}
		}
	}
}
