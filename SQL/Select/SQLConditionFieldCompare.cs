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
	public class SQLConditionFieldCompare
	{
		private SQLSelectTable pobjTable1;
		private string pstrFieldName1;
		private ComparisonOperator peCompare;
		private SQLSelectTable pobjTable2;
		private string pstrFieldName2;
			
		public SQLSelectTable Table1
		{
			get
			{
				return pobjTable1;
			}
				
			set
			{
				pobjTable1 = value;
			}
		}
			
		public string FieldName1
		{
			get
			{
				return pstrFieldName1;
			}
				
			set
			{
				pstrFieldName1 = value;
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
			
		public SQLSelectTable Table2
		{
			get
			{
				return pobjTable2;
			}
				
			set
			{
				pobjTable2 = value;
			}
		}
			
		public string FieldName2
		{
			get
			{
				return pstrFieldName2;
			}
				
			set
			{
				pstrFieldName2 = value;
			}
		}
	}
}
