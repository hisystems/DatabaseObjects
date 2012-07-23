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
				return this[FieldNameIndex(strFieldName)];
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
			
		internal string SQL(Database.ConnectionType eConnectionType)
		{
			string strSQL = string.Empty;
				
			for (int intIndex = 0; intIndex < this.Count; intIndex++)
			{
				strSQL += this[intIndex].SQL(eConnectionType);
				if (intIndex != this.Count - 1)
					strSQL += ", ";
			}
				
			return strSQL;
		}
			
		public bool Exists(string strFieldName)
		{
			return FieldNameIndex(strFieldName) >= 0;
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
			
		private int FieldNameIndex(string strFieldName)
		{
			SQLSelectOrderByField objOrderByField;
				
			for (int intIndex = 0; intIndex < this.Count; intIndex++)
			{
				objOrderByField = (SQLSelectOrderByField) (pobjOrderByFields[intIndex]);
				if (string.Compare(strFieldName, objOrderByField.Name, true) == 0)
					return intIndex;
			}
				
			return -1;
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
