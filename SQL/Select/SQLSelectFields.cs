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
	public class SQLSelectFields : IEnumerable<SQLSelectField>
	{
		private List<SQLSelectField> pobjFieldNames = new List<SQLSelectField>();
			
		public SQLSelectFields()
		{
		}
			
		public SQLSelectField Add()
		{
			return Add(string.Empty, string.Empty, 0, null);
		}
			
		public SQLSelectField Add(string strFieldName)
		{
			return Add(strFieldName, string.Empty, 0, null);
		}
			
		public SQLSelectField Add(string strFieldName, SQL.AggregateFunction eAggregateFunction)
		{
			return Add(strFieldName, string.Empty, eAggregateFunction, null);
		}
			
		public SQLSelectField Add(string strFieldName, string strAlias, SQL.AggregateFunction eAggregateFunction)
		{
			return Add(strFieldName, strAlias, eAggregateFunction, null);
		}
			
		public SQLSelectField Add(string strFieldName, string strAlias, SQLSelectTable objTable)
		{
			return Add(strFieldName, strAlias, 0, objTable);
		}
			
		public SQLSelectField Add(string strFieldName, SQLSelectTable objTable)
		{
			return Add(strFieldName, string.Empty, 0, objTable);
		}
			
		public SQLSelectField Add(string strFieldName, string strAlias, SQL.AggregateFunction eAggregateFunction, SQLSelectTable objTable)
		{
			if (eAggregateFunction == AggregateFunction.None)
				return Add(new SQLFieldExpression(objTable, strFieldName), strAlias);
			else
				return Add(new SQLFieldAggregateExpression(objTable, strFieldName, eAggregateFunction), strAlias);
		}
			
		public SQLSelectField Add(SQLExpression objExpression)
		{
			return Add(objExpression, string.Empty);
		}
			
		public SQLSelectField Add(SQLExpression objExpression, string strAlias)
		{
			SQLSelectField objSQLField = new SQLSelectField(objExpression);
			objSQLField.Alias = strAlias;
			pobjFieldNames.Add(objSQLField);
				
			return objSQLField;
		}
			
		public void Add(string[] objFieldNames)
		{
			for (int intIndex = 0; intIndex < objFieldNames.Length; intIndex++)
				this.Add(objFieldNames[intIndex]);
		}
			
		public SQLSelectField this[string strFieldName]
		{
			get
			{
				return this[FieldNameIndex(strFieldName)];
			}
		}
			
		public SQLSelectField this[int intIndex]
		{
			get
			{
				return pobjFieldNames[intIndex];
			}
		}
			
		public int Count
		{
			get
			{
				return pobjFieldNames.Count;
			}
		}
			
		public void Clear()
		{
			pobjFieldNames.Clear();
		}
			
		public bool Exists(string strFieldName)
		{
			return FieldNameIndex(strFieldName) >= 0;
		}
			
		public void Delete(ref SQLSelectField objSelectField)
		{
			if (!pobjFieldNames.Contains(objSelectField))
				throw new IndexOutOfRangeException();
				
			pobjFieldNames.Remove(objSelectField);
			objSelectField = null;
		}
			
		private int FieldNameIndex(string strFieldName)
		{
			for (int intIndex = 0; intIndex < this.Count; intIndex++)
			{
				if (this[intIndex].Expression is SQLFieldExpression)
				{
					SQLFieldExpression objFieldExpression = (SQLFieldExpression) (this[intIndex].Expression);
					if (string.Compare(strFieldName, objFieldExpression.Name, true) == 0)
						return intIndex;
				}
			}
				
			return -1;
		}
			
		public System.Collections.IEnumerator GetEnumerator()
		{
			return pobjFieldNames.GetEnumerator();
		}
			
		IEnumerator<SQLSelectField> IEnumerable<SQLSelectField>.GetEnumerator()
	    {
		    return pobjFieldNames.GetEnumerator();
	    }

	    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	    {
			return pobjFieldNames.GetEnumerator();
	    }
	}
}