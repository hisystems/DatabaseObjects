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
using System.Collections.Generic;
using System.Linq;

namespace DatabaseObjects.SQL
{
	public class SQLSelectTables : IEnumerable<SQLSelectTableBase>
	{
		private SQLSelect pobjParent;
		private List<SQLSelectTableBase> pobjTables = new List<SQLSelectTableBase>();
		private SQLSelectTableJoins pobjJoins = new SQLSelectTableJoins();
			
		public SQLSelectTables()
		{
		}
			
		internal SQLSelectTables(SQLSelect objParent)
		{
			pobjParent = objParent;
		}
			
		/// <summary>
		/// Returns the associated parent SQLSelect object associated with this table collection.
		/// Returns nothing if the object is not assigned to an SQLSelect object.
		/// </summary>
		public SQLSelect Parent
		{
			get
			{
				return pobjParent;
			}
				
			internal set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjParent = value;
			}
		}
			
		public SQLSelectTable Add()
		{
			return Add(string.Empty);
		}
			
		public SQLSelectTable Add(string strTableName)
		{
			SQLSelectTable objTable = new SQLSelectTable(strTableName);
				
			pobjTables.Add(objTable);
				
			return objTable;
		}
			
		public SQLSelectTable Add(string strTableName, string strAlias)
		{
			SQLSelectTable objTable = new SQLSelectTable(strTableName, strAlias);
				
			pobjTables.Add(objTable);
				
			return objTable;
		}
			
		public void Add(SQL.SQLSelectTable objTable)
		{
			pobjTables.Add(objTable);
		}
			
		public void Add(SQL.SQLSelectTableFromSelect objTable)
		{
			pobjTables.Add(objTable);
		}
			
		public SQLSelectTableBase this[string strTableName]
		{
			get
			{
				if (!Exists(strTableName))
					throw new ArgumentException(strTableName + " does not exist");

				return pobjTables.Single(table => Equals(table, strTableName));
			}
		}
			
		public SQLSelectTableBase this[int intIndex]
		{
			get
			{
				return pobjTables[intIndex];
			}
		}
			
		public int Count
		{
			get
			{
				return pobjTables.Count;
			}
		}
			
		public SQLSelectTableJoins Joins
		{
			get
			{
				return pobjJoins;
			}
				
			set
			{
				pobjJoins = value;
			}
		}
			
		public bool Exists(string strTableName)
		{
			return pobjTables.SingleOrDefault(table => Equals(table, strTableName)) != null;
		}
			
		public void Delete(ref SQLSelectTable objTable)
		{
			if (pobjTables.Contains(objTable))
				throw new IndexOutOfRangeException();
				
			pobjTables.Remove(objTable);
			objTable = null;
		}
						
		private bool Equals(SQLSelectTable table, string strTableName)
		{
			return table.Name.Equals(strTableName, StringComparison.InvariantCultureIgnoreCase);
		}
			
		public System.Collections.IEnumerator GetEnumerator()
		{
			return pobjTables.GetEnumerator();
		}
			
		IEnumerator<SQLSelectTableBase> IEnumerable<SQLSelectTableBase>.GetEnumerator()
	    {
			return pobjTables.GetEnumerator();
	    }

	    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	    {
			return pobjTables.GetEnumerator();
	    }
	}
}