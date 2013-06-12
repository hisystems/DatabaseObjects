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
	public class SQLFieldValues : IEnumerable<SQLFieldValue>
	{
		protected List<SQLFieldValue> pobjFields = new List<SQLFieldValue>();
			
		public SQLFieldValues()
		{
		}
			
		public virtual SQLFieldValue Add()
		{
			return Add(string.Empty, null);
		}
			
		public virtual SQLFieldValue Add(SQLFieldValue objFieldAndValue)
		{
			return Add(objFieldAndValue.Name, objFieldAndValue.Value);
		}
			
		public virtual SQLFieldValue Add(string strDestinationFieldName, object objEqualToValue)
		{
			SQLFieldValue objSQLFieldValue = new SQLFieldValue(strDestinationFieldName, objEqualToValue);
				
			pobjFields.Add(objSQLFieldValue);
				
			return objSQLFieldValue;
		}
			
		public virtual void Add(SQLFieldValues objFieldValues)
		{
			foreach (SQLFieldValue objFieldValue in objFieldValues)
				this.Add(objFieldValue);
		}
			
		public SQLFieldValue this[string strFieldName]
		{
			get
			{
				if (!this.Exists(strFieldName))
					throw new ArgumentException("Field '" + strFieldName + "' does not exist.");

				return pobjFields.First(field => Equals(field, strFieldName));
			}
		}
			
		public SQLFieldValue this[int intIndex]
		{
			get
			{
				return (SQLFieldValue)pobjFields[intIndex];
			}
		}
			
		public int Count
		{
			get
			{
				return pobjFields.Count;
			}
		}
			
		public bool Exists(string strFieldName)
		{
			return pobjFields.FirstOrDefault(field => Equals(field, strFieldName)) != null;
		}
			
		public void Delete(ref SQLFieldValue objFieldValue)
		{
			if (!pobjFields.Contains(objFieldValue))
				throw new IndexOutOfRangeException();
				
			pobjFields.Remove(objFieldValue);
			objFieldValue = null;
		}
			
		private bool Equals(SQLFieldValue fieldValue, string strFieldName)
		{
			return fieldValue.Name.Equals(strFieldName, StringComparison.InvariantCultureIgnoreCase);
		}
			
		IEnumerator<SQLFieldValue> IEnumerable<SQLFieldValue>.GetEnumerator()
		{
			return pobjFields.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return pobjFields.GetEnumerator();
		}
	}
}
