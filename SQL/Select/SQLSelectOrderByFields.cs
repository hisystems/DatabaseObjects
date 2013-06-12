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
	public class SQLSelectOrderByFields : IEnumerable<SQLSelectOrderByField>
	{
		private List<SQLSelectOrderByField> pobjOrderByFields = new List<SQLSelectOrderByField>();
			
		public SQLSelectOrderByFields()
		{
		}
			
		public SQLSelectOrderByField Add()
		{
			return Add(string.Empty, 0, OrderBy.Ascending, null);
		}
			
		public SQLSelectOrderByField Add(string strFieldName)
		{
			return Add(strFieldName, 0, OrderBy.Ascending, null);
		}
			
		public SQLSelectOrderByField Add(string strFieldName, OrderBy eOrder)
		{
			return Add(strFieldName, 0, eOrder, null);
		}
			
		public SQLSelectOrderByField Add(string strFieldName, OrderBy eOrder, SQL.AggregateFunction eAggregate)
		{
			return Add(strFieldName, eAggregate, eOrder, null);
		}
			
		public SQLSelectOrderByField Add(string strFieldName, OrderBy eOrder, SQLSelectTable objTable)
		{
			return Add(strFieldName, 0, eOrder, objTable);
		}
			
		public SQLSelectOrderByField Add(string strFieldName, SQL.AggregateFunction eAggregate)
		{
			return Add(strFieldName, eAggregate, OrderBy.Ascending, null);
		}
			
		public SQLSelectOrderByField Add(string strFieldName, SQL.AggregateFunction eAggregate, OrderBy eOrder, SQLSelectTable objTable)
		{
			var objFieldOrder = new SQLSelectOrderByField();
				
			objFieldOrder.Table = objTable;
			objFieldOrder.Name = strFieldName;
			objFieldOrder.Order = eOrder;
			objFieldOrder.AggregateFunction = eAggregate;
				
			pobjOrderByFields.Add(objFieldOrder);
				
			return objFieldOrder;
		}
			
		public SQLSelectOrderByField this[int intIndex]
		{
			get
			{
				return pobjOrderByFields[intIndex];
			}
		}
			
		public SQLSelectOrderByField this[string strFieldName]
		{
			get
			{
				if (!Exists(strFieldName))
					throw new ArgumentException(strFieldName + " does not exist");

				return this.Single(field => Equals(field, strFieldName));
			}
		}
			
		public int Count
		{
			get
			{
				return pobjOrderByFields.Count;
			}
		}
			
		public bool IsEmpty
		{
			get
			{
				return pobjOrderByFields.Count == 0;
			}
		}
			
		public bool Exists(string strFieldName)
		{
			return this.SingleOrDefault(field => Equals(field, strFieldName)) != null;
		}
			
		public void Delete(ref SQLSelectOrderByField objOrderByField)
		{
			if (!pobjOrderByFields.Contains(objOrderByField))
				throw new IndexOutOfRangeException();
				
			pobjOrderByFields.Remove(objOrderByField);
			objOrderByField = null;
		}
			
		public void OrderingReverseAll()
		{
			foreach (SQLSelectOrderByField objOrderBy in this)
				objOrderBy.OrderingReverse();
		}
			
		private bool Equals(SQLSelectOrderByField field, string strFieldName)
		{
			return field.Name.Equals(strFieldName, StringComparison.InvariantCultureIgnoreCase);
		}
			
		public System.Collections.IEnumerator GetEnumerator()
		{
			return pobjOrderByFields.GetEnumerator();
		}
			
		IEnumerator<SQLSelectOrderByField> IEnumerable<SQLSelectOrderByField>.GetEnumerator()
	    {
			return pobjOrderByFields.GetEnumerator();
	    }

	    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	    {
			return pobjOrderByFields.GetEnumerator();
	    }
	}
}
