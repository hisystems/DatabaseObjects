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
	public class SQLUpdate : SQLStatement
	{
		private SQLUpdateFields pobjFields = new SQLUpdateFields();
		private SQLConditions pobjConditions = new SQLConditions();
		private string pstrTableName = string.Empty;
			
		public SQLUpdate()
		{
		}
			
		public SQLUpdate(string strTableName)
		{
			this.TableName = strTableName;
		}
			
		public SQLUpdate(string strTableName, SQLFieldValue objValue, SQLCondition objWhere)
		{
			this.TableName = strTableName;
			this.Fields.Add(objValue);
			this.Where.Add(objWhere);
		}
			
		public string TableName
		{
			get
			{
				return pstrTableName;
			}
				
			set
			{
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException();
					
				pstrTableName = value;
			}
		}
			
		public SQLUpdateFields Fields
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
			
		public SQLConditions Where
		{
			get
			{
				return pobjConditions;
			}
				
			set
			{
				pobjConditions = value;
			}
		}

		public override string SQL
		{
			get 
			{
				return base.Serializer.SerializeUpdate(this);
			}
		}
	}
}
