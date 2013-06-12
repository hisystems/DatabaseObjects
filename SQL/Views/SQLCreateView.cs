// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
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
	public class SQLCreateView : SQLStatement
	{
		private string pstrName;
		private SQLSelect pobjSelect;
			
		public SQLCreateView()
		{
		}
			
		public SQLCreateView(string strViewName, SQLSelect objSelectStatement)
		{
			this.Name = strViewName;
			this.Select = objSelectStatement;
		}
			
		public string Name
		{
			get
			{
				return pstrName;
			}
				
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException();
					
				pstrName = value;
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
				if (value == null)
					throw new ArgumentNullException();
					
				pobjSelect = value;
			}
		}
			
		public override string SQL
		{
			get
			{
				return base.Serializer.SerializeCreateView(this);
			}
		}
	}
}
	
