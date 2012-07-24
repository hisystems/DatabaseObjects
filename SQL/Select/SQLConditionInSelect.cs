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
    /// <summary>
    /// This class represents an IN operation together with a SELECT statement
    /// i.e.  ... ProductID IN (SELECT ProductID FROM Product WHERE ...)
    /// </summary>
	public class SQLConditionInSelect
	{
		private SQLSelectTable pobjTable;
		private string pstrFieldName;
		private SQLSelect pobjSelect;
		private bool pbNotInSelect;
			
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
			
		public bool NotInSelect
		{
			get
			{
				return pbNotInSelect;
			}
				
			set
			{
				pbNotInSelect = value;
			}
		}
	}
}
