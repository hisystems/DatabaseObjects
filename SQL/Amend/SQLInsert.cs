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
	public class SQLInsert : SQLStatement
	{
		private SQLFieldValues pobjFields = new SQLFieldValues();
		private string pstrTableName = string.Empty;
			
		public SQLInsert()
		{
		}
			
		public string TableName
		{
			get
			{
				return pstrTableName;
			}
				
			set
			{
				pstrTableName = value;
			}
		}
			
		public SQLFieldValues Fields
		{
			get
			{
				return pobjFields;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjFields = value;
			}
		}

		public override string SQL
		{
			get 
			{
				return base.Serializer.SerializeInsert(this);
			}
		}
	}
}
