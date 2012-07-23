// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public class SQLFieldValues : IEnumerable
	{
		protected ArrayList pobjFields = new ArrayList();
			
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
					
				return (SQLFieldValue)pobjFields[FieldNameIndex(strFieldName)];
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
			return FieldNameIndex(strFieldName) >= 0;
		}
			
		public void Delete(ref SQLFieldValue objFieldValue)
		{
			if (!pobjFields.Contains(objFieldValue))
				throw new IndexOutOfRangeException();
				
			pobjFields.Remove(objFieldValue);
			objFieldValue = null;
		}
			
		internal int FieldNameIndex(string strFieldName)
		{
			SQLFieldValue objSQLFieldValue;
				
			for (int intIndex = 0; intIndex < this.Count; intIndex++)
			{
				objSQLFieldValue = (SQLFieldValue) (pobjFields[intIndex]);
				if (string.Compare(strFieldName, objSQLFieldValue.Name, true) == 0)
					return intIndex;
			}
				
			return -1;
		}
			
		public System.Collections.IEnumerator GetEnumerator()
		{
			return pobjFields.GetEnumerator();
		}
	}
}
