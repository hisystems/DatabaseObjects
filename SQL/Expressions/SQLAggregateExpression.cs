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
		private AggregateFunction aggregate;

		public SQLAggregateExpression(AggregateFunction aggregate, string fieldName) 
            : this(aggregate, new SQLFieldExpression(fieldName))
		{
		}
			
		public SQLAggregateExpression(AggregateFunction aggregate, SQLExpression expression) 
            : base(expression)
		{
			this.aggregate = aggregate;
		}

		public AggregateFunction Aggregate
		{
			get
			{
				return aggregate;
			}
		}

		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeAggregateExpression(this);
		}
	}
}
