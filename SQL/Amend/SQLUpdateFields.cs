// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//	    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// _________________________________________________________________________
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
