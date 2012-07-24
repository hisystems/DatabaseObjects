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
	public class SQLAutoIncrementValue : SQLStatement
	{
		private string pstrReturnFieldName = "AutoIncrementValue";
			
		public SQLAutoIncrementValue()
		{
		}
			
		public string ReturnFieldName
		{
			get
			{
				return pstrReturnFieldName;
			}
				
			set
			{
				if (String.IsNullOrEmpty(value))
					throw new ArgumentNullException();
					
				pstrReturnFieldName = value;
			}
		}
			
		public override string SQL
		{
			get
			{
				return base.Serializer.SerializeAutoIncrementValue(this);
			}
		}
	}
}
