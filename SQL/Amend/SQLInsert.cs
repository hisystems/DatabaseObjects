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
				string strFields = string.Empty;
				string strFieldValues = string.Empty;
					
				if (String.IsNullOrEmpty(TableName))
					throw new Exceptions.DatabaseObjectsException("TableName property has not been set.");
					
				if (pobjFields.Count == 0)
					throw new Exceptions.DatabaseObjectsException("Field values have not been set.");
					
				for (int intIndex = 0; intIndex < pobjFields.Count; intIndex++)
				{
					strFields += Misc.SQLConvertIdentifierName(pobjFields[intIndex].Name, this.ConnectionType);
					if (intIndex != pobjFields.Count - 1)
						strFields += ",";
				}

				SQLFieldValue objField;
					
				for (int intIndex = 0; intIndex < pobjFields.Count; intIndex++)
				{
					objField = pobjFields[intIndex];
						
					if (objField.Value is SQLExpression)
						strFieldValues += ((SQLExpression) objField.Value).SQL(this.ConnectionType);
					else
						strFieldValues += Misc.SQLConvertValue(pobjFields[intIndex].Value, this.ConnectionType);
						
					if (intIndex != pobjFields.Count - 1)
						strFieldValues += ",";
				}
					
				return "INSERT INTO " + Misc.SQLConvertIdentifierName(this.TableName, this.ConnectionType) + " " + "(" + strFields + ") VALUES (" + strFieldValues + ")";
			}
		}
	}
}
