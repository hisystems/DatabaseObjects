// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Collections.Generic;

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
				return this[TableNameIndex(strTableName)];
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
			return TableNameIndex(strTableName) >= 0;
		}
			
		public void Delete(ref SQLSelectTable objTable)
		{
			if (pobjTables.Contains(objTable))
				throw new IndexOutOfRangeException();
				
			pobjTables.Remove(objTable);
			objTable = null;
		}
			
		internal string SQL(Database.ConnectionType eConnectionType)
		{
			string strSQL = string.Empty;
			bool bAddTable;
				
			for (int intIndex = 0; intIndex < this.Count; intIndex++)
			{
				//Include the table if it's not being used in a join
				if (pobjJoins == null)
					bAddTable = true;
				else if (!pobjJoins.Exists(this[intIndex]))
					bAddTable = true;
				else
					bAddTable = false;
					
				if (bAddTable)
				{
					strSQL += this[intIndex].SQL(eConnectionType);
					if (intIndex != this.Count - 1)
						strSQL += ", ";
				}
			}
				
			if (pobjJoins != null)
			{
				string strJoinsSQL = pobjJoins.SQL(eConnectionType);

				if (strJoinsSQL != string.Empty && strSQL != string.Empty)
					strSQL += " ";
				
                strSQL += strJoinsSQL;
			}
				
			return strSQL;
		}
			
		private int TableNameIndex(string strTableName)
		{
			SQLSelectTable objTable;
				
			for (int intIndex = 0; intIndex < this.Count; intIndex++)
			{
				objTable = (SQLSelectTable)pobjTables[intIndex];
				if (string.Compare(strTableName, objTable.Name, true) == 0)
					return intIndex;
			}
				
			return -1;
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