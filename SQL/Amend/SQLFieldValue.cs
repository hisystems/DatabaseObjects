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
	public class SQLFieldValue
	{
		private string pstrName;
		private object pobjValue;
			
		public SQLFieldValue()
		{
		}
			
		public SQLFieldValue(string strFieldName, object objNewValue)
		{
			this.Name = strFieldName;
			this.Value = objNewValue;
		}
			
		public string Name
		{
			get
			{
				return pstrName;
			}
				
			set
			{
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

				pstrName = value;
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
				pobjValue = value;
			}
		}
			
		public bool ValueIsDBNull
		{
			get
			{
				return DBNull.Value.Equals(this.Value);
			}
		}
	}
}
