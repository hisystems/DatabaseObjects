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
using System.Linq;

namespace DatabaseObjects.SQL
{
	public class SQLFieldValues : IEnumerable
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
			
		public System.Collections.IEnumerator GetEnumerator()
		{
			return pobjFields.GetEnumerator();
		}
	}
}
