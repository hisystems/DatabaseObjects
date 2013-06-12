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
	public class SQLConditionFieldCompare
	{
		private SQLSelectTable pobjTable1;
		private string pstrFieldName1;
		private ComparisonOperator peCompare;
		private SQLSelectTable pobjTable2;
		private string pstrFieldName2;
			
		public SQLSelectTable Table1
		{
			get
			{
				return pobjTable1;
			}
				
			set
			{
				pobjTable1 = value;
			}
		}
			
		public string FieldName1
		{
			get
			{
				return pstrFieldName1;
			}
				
			set
			{
				pstrFieldName1 = value;
			}
		}
			
		public ComparisonOperator Compare
		{
			get
			{
				return peCompare;
			}
				
			set
			{
				peCompare = value;
			}
		}
			
		public SQLSelectTable Table2
		{
			get
			{
				return pobjTable2;
			}
				
			set
			{
				pobjTable2 = value;
			}
		}
			
		public string FieldName2
		{
			get
			{
				return pstrFieldName2;
			}
				
			set
			{
				pstrFieldName2 = value;
			}
		}
	}
}
