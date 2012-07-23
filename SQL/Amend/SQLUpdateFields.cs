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
	public class SQLUpdateFields : SQLFieldValues
	{
		public SQLUpdateField AddCopy(string strDestinationFieldName, string strSourceFieldName)
		{
			SQLUpdateField objField = new SQLUpdateField(strDestinationFieldName, strSourceFieldName);
				
			this.Add(objField);
				
			return objField;
		}
			
		public SQLUpdateField Add(string strDestinationFieldName, SQLExpression objExpression)
		{
			SQLUpdateField objField = new SQLUpdateField(strDestinationFieldName, objExpression);
				
			pobjFields.Add(objField);
				
			return objField;
		}
			
		public void Add(SQLUpdateField objField)
		{
			pobjFields.Add(objField);
		}
			
		public void Add(SQLFieldValues objFieldValues)
		{
			foreach (SQLFieldValue objFieldValue in objFieldValues)
				base.Add(objFieldValue);
		}
	}
}
