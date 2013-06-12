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
			SQLSelectTableJoin objJoin = new SQLSelectTableJoin();
				
			pobjJoins.Add(objJoin);
				
			return objJoin;
		}
			
		public SQLSelectTableJoin Add(SQLSelectTableBase objLeftTable, SQLSelectTableJoin.Type eJoin, SQLSelectTableBase objRightTable)
		{
			SQLSelectTableJoin objJoin = new SQLSelectTableJoin();
				
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