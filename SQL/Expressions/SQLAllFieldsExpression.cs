// _________________________________________________________________________
//
//  (c) Hi-Integrity Systems 2012. All rights reserved.
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
	/// Represents '*' or 'T.*' when used to select all fields from a table or join.
	/// </summary>
	/// <remarks></remarks>
	public class SQLAllFieldsExpression : SQLExpression
	{
		private SQLSelectTable pobjTable;
			
		public SQLAllFieldsExpression()
		{
		}
			
		public SQLAllFieldsExpression(SQLSelectTable objTable)
		{
			this.Table = objTable;
		}
			
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
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			string strSQL = string.Empty;
				
			if (pobjTable != null)
				strSQL += serializer.SerializeTablePrefix(pobjTable) + ".";
				
			return strSQL + "*";
		}
	}
}
