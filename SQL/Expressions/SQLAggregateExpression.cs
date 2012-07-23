// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public class SQLAggregateExpression : SQLFunctionExpression
	{
		public SQLAggregateExpression(AggregateFunction aggregate, string fieldName) 
            : this(aggregate, new SQLFieldExpression(fieldName))
		{
		}
			
		public SQLAggregateExpression(AggregateFunction aggregate, SQLExpression expression) 
            : base(Misc.SQLConvertAggregate(aggregate), expression)
		{
		}
	}
}
