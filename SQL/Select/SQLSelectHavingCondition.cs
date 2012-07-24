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
	public class SQLSelectHavingCondition : SQLExpression
	{
		private SQLExpression pobjLeftHandExpression;
		private ComparisonOperator peCompare;
		private SQLExpression pobjRightHandExpression;
			
		public SQLSelectHavingCondition(SQLExpression objLeftHandExpression, ComparisonOperator eCompare, SQLExpression objRightHandExpression)
		{
			if (objLeftHandExpression == null)
				throw new ArgumentNullException();
			else if (objRightHandExpression == null)
				throw new ArgumentNullException();
				
			pobjLeftHandExpression = objLeftHandExpression;
			peCompare = eCompare;
			pobjRightHandExpression = objRightHandExpression;
		}

		internal ComparisonOperator Compare
		{
			get
			{
				return peCompare;
			}
		}

		internal SQLExpression LeftExpression
		{
			get
			{
				return pobjLeftHandExpression;
			}
		}

		internal SQLExpression RightExpression
		{
			get
			{
				return pobjRightHandExpression;
			}
		}

		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeSelectHavingCondition(this);
		}
	}
}
