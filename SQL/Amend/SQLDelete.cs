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
	public class SQLDelete : SQLStatement
	{
		private SQLConditions pobjConditions = new SQLConditions();
		private string pstrTableName;
			
		public SQLDelete()
		{
		}
			
		public SQLDelete(string strTableName)
		{
			this.TableName = strTableName;
		}
			
		public SQLDelete(string strTableName, SQLCondition objWhereCondition)
		{
			this.TableName = strTableName;
			this.Where.Add(objWhereCondition);
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
				string strSQL;
					
				if (String.IsNullOrEmpty(TableName))
					throw new Exceptions.DatabaseObjectsException("TableName property has not been set.");
					
				strSQL = "DELETE FROM " + Misc.SQLConvertIdentifierName(this.TableName, this.ConnectionType);

				if (pobjConditions != null && !pobjConditions.IsEmpty)
					strSQL += " WHERE " + pobjConditions.SQL(this.ConnectionType);
					
				return strSQL;
			}
		}
	}
}
