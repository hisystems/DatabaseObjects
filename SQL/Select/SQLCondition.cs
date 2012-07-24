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
	public class SQLCondition
	{
		private string pstrFieldName = string.Empty;
		private object pobjValue;
		private ComparisonOperator peCompare;
		private SQLSelectTable pobjTable;
			
		internal SQLCondition()
		{
		}
			
		public SQLCondition(string strFieldName, ComparisonOperator eCompare, object objValue)
		{
			this.FieldName = strFieldName;
			this.Compare = eCompare;
			pobjValue = objValue;
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
			
		public string FieldName
		{
			get
			{
				return pstrFieldName;
			}
				
			set
			{
				pstrFieldName = value;
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
				pobjValue = GetConditionValue(value);
			}
		}

		internal static object GetConditionValue(object objValue)
		{
			if (objValue is SQLFieldValue)
				return ((SQLFieldValue)objValue).Value;
			else
				return objValue;
		}
	}
}
