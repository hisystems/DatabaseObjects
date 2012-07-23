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
				string strSQL;
				string strFieldValues = string.Empty;
					
				if (String.IsNullOrEmpty(TableName))
					throw new Exceptions.DatabaseObjectsException("TableName property has not been set.");
				else if (pobjFields.Count == 0)
					throw new Exceptions.DatabaseObjectsException("Field values have not been set.");
					
				SQLFieldValue objField;
					
				for (int intIndex = 0; intIndex < pobjFields.Count; intIndex++)
				{
					objField = pobjFields[intIndex];
					//Check the field name has been set. Can't really check whether the value has been set or not.
					if (String.IsNullOrEmpty(objField.Name))
						throw new Exceptions.DatabaseObjectsException("Field Name has not been set.");
						
					if (objField is SQLUpdateField)
						strFieldValues += Misc.SQLConvertIdentifierName(objField.Name, this.ConnectionType) + " = " + ((SQLUpdateField) objField).Value.SQL(this.ConnectionType);
					else if (objField is SQLFieldValue)
						strFieldValues += Misc.SQLConvertIdentifierName(objField.Name, this.ConnectionType) + " = " + Misc.SQLConvertValue(objField.Value, this.ConnectionType);
					else
						throw new NotSupportedException(objField.GetType().Name);
						
					if (intIndex != pobjFields.Count - 1)
						strFieldValues += ", ";
				}
					
				strSQL = "UPDATE " + Misc.SQLConvertIdentifierName(this.TableName, this.ConnectionType) + " " + "SET " + strFieldValues;
					
				if (pobjConditions != null && !pobjConditions.IsEmpty)
					strSQL += " WHERE " + pobjConditions.SQL(this.ConnectionType);
					
				return strSQL;
			}
		}
	}
}
