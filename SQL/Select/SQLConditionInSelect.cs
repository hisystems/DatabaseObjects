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
    /// <summary>
    /// This class represents an IN operation together with a SELECT statement
    /// i.e.  ... ProductID IN (SELECT ProductID FROM Product WHERE ...)
    /// </summary>
	public class SQLConditionInSelect
	{
		private SQLSelectTable pobjTable;
		private string pstrFieldName;
		private SQLSelect pobjSelect;
		private bool pbNotInSelect;
			
		public SQLSelectTable Table
		{
			get
			{
				return pobjTable;
			}
				
			set
			{
				pobjTable = value;
			}
		}
			
		public string FieldName
		{
			get
			{
				return pstrFieldName;
			}
				
			set
			{
				pstrFieldName = value;
			}
		}
			
		public SQLSelect Select
		{
			get
			{
				return pobjSelect;
			}
				
			set
			{
				pobjSelect = value;
			}
		}
			
		public bool NotInSelect
		{
			get
			{
				return pbNotInSelect;
			}
				
			set
			{
				pbNotInSelect = value;
			}
		}
	}
}
