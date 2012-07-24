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
	public class SQLSelectOrderByField
	{
		private string pstrName;
		private SQLSelectTable pobjTable;
		private OrderBy peOrder = OrderBy.Ascending;
		private AggregateFunction peAggregateFunction = AggregateFunction.None;
			
		internal SQLSelectOrderByField()
		{
		}
			
		public string Name
		{
			get
			{
				return pstrName;
			}
				
			set
			{
                pstrName = value;
			}
		}
			
		public OrderBy Order
		{
			get
			{
				return peOrder;
			}
				
			set
			{
				peOrder = value;
			}
		}
			
		public void OrderingReverse()
		{
			if (peOrder != OrderBy.None)
				peOrder = ~peOrder;
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
			
		public SQLSelectTable Table
		{
			get
			{
				return pobjTable;
			}
				
			set
			{
				pobjTable = value;
			}
		}
	}
}
