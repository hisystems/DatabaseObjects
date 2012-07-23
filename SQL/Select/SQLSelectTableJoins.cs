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
	public class SQLSelectTableJoins : IEnumerable<SQLSelectTableJoin>
	{
		private List<SQLSelectTableJoin> pobjJoins = new List<SQLSelectTableJoin>();
			
		public SQLSelectTableJoins()
		{
		}
			
		public SQLSelectTableJoin Add()
		{
			SQLSelectTableJoin objJoin = new SQLSelectTableJoin(this);
				
			pobjJoins.Add(objJoin);
				
			return objJoin;
		}
			
		public SQLSelectTableJoin Add(SQLSelectTableBase objLeftTable, SQLSelectTableJoin.Type eJoin, SQLSelectTableBase objRightTable)
		{
			SQLSelectTableJoin objJoin = new SQLSelectTableJoin(this);
				
			objJoin.LeftTable = objLeftTable;
			objJoin.TheType = eJoin;
			objJoin.RightTable = objRightTable;
				
			pobjJoins.Add(objJoin);
				
			return objJoin;
		}
			
		public SQLSelectTableJoin this[int intIndex]
		{
			get
			{
				return pobjJoins[intIndex];
			}
		}
			
		public int Count
		{
			get
			{
				return pobjJoins.Count;
			}
		}
			
		public bool Exists(SQLSelectTableBase objTable)
		{
			for (int intIndex = 0; intIndex < pobjJoins.Count; intIndex++)
			{
				var table = this[intIndex];
                if (table.LeftTable == objTable || table.RightTable == objTable)
					return true;
			}

            return false;
		}
			
		public void Delete(SQLSelectTableJoin objJoin)
		{
			if (!pobjJoins.Contains(objJoin))
				throw new IndexOutOfRangeException();
				
			pobjJoins.Remove(objJoin);
			objJoin = null;
		}
			
		internal string SQL(Database.ConnectionType eConnectionType)
		{
			if (this.Count > 0)
				//recurse through the joins from right to left
				return this[this.Count - 1].SQL(eConnectionType);
			else
				return string.Empty;
		}
			
		public System.Collections.IEnumerator GetEnumerator()
		{
			return pobjJoins.GetEnumerator();
		}
			
		IEnumerator<SQLSelectTableJoin> IEnumerable<SQLSelectTableJoin>.GetEnumerator()
	    {
			return pobjJoins.GetEnumerator();
	    }

	    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	    {
			return pobjJoins.GetEnumerator();
	    }
	}
}