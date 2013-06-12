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
