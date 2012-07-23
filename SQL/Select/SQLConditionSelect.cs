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
	public class SQLConditionSelect
	{
		//This class allows a conditional value generated from an SELECT statement to be added as an SQL condition.
		//i.e.  ... WHERE (SELECT MAX(StockOnHand) FROM Product WHERE Supplier.ProductID = Product.ProductID) > 1000
			
		private object pobjValue;
		private SQLSelect pobjSelect;
		private ComparisonOperator peCompare;
			
		public SQLConditionSelect()
		{
		}
			
		public SQLSelect Select
		{
			get
			{
				return pobjSelect;
			}
				
			set
			{
				pobjSelect = value;
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
			
		public object Value
		{
			get
			{
				return pobjValue;
			}
				
			set
			{
				pobjValue = Misc.SQLConditionValue(value);
			}
		}
			
		internal string SQL(Database.ConnectionType eConnectionType)
		{
			if (this.Select == null)
				throw new Exceptions.DatabaseObjectsException("Select is not set.");
				
			Misc.CompareValuePairAssertValid(this.Compare, pobjValue);
				
			this.Select.ConnectionType = eConnectionType;
			return Condition(this.Select, this.Compare, pobjValue, eConnectionType);
		}
			
		private string Condition(SQLSelect objSelect, ComparisonOperator eCompare, object vValue, Database.ConnectionType eConnectionType)
		{
			Misc.SQLConvertBooleanValue(ref vValue, ref eCompare);
				
			return "(" + objSelect.SQL + ") " + Misc.SQLConvertCompare(eCompare) + " " + Misc.SQLConvertValue(vValue, eConnectionType);
		}
	}
}
