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
	public class SQLFieldAggregateExpression : SQLFieldExpression
	{
		private AggregateFunction peAggregateFunction = AggregateFunction.None;
			
		public SQLFieldAggregateExpression()
		{
		}
			
		public SQLFieldAggregateExpression(string strFieldName) 
            : base(strFieldName)
		{
		}
			
		public SQLFieldAggregateExpression(SQLSelectTable objTable, string strFieldName) 
            : base(objTable, strFieldName)
		{
		}
			
		public SQLFieldAggregateExpression(SQLSelectTable objTable, string strFieldName, AggregateFunction eAggregate) 
            : base(objTable, strFieldName)
		{
			this.AggregateFunction = eAggregate;
		}
			
		public SQL.AggregateFunction AggregateFunction
		{
			get
			{
				return peAggregateFunction;
			}
				
			set
			{
				peAggregateFunction = value;
			}
		}
			
		internal override string SQL(Database.ConnectionType eConnectionType)
		{
			if (this.AggregateFunction == AggregateFunction.None)
				throw new InvalidOperationException("AggregateFunction unspecified for " + this.GetType().Name);
				
			string strFieldName = string.Empty;
				
			if (this.AggregateFunction == AggregateFunction.Count)
				strFieldName = "*";
			else
				strFieldName = base.SQL(eConnectionType);
				
			return Misc.SQLConvertAggregate(this.AggregateFunction) + "(" + strFieldName + ")";
		}
	}
}
