// _________________________________________________________________________
//
//  (c) Hi-Integrity Systems 2010. All rights reserved.
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
    public class SQLSelectGroupByFields : IEnumerable<SQLSelectGroupByField>
    {
        private List<SQLSelectGroupByField> pobjGroupByFields = new List<SQLSelectGroupByField>();

        public SQLSelectGroupByFields()
        {
        }

        public SQLSelectGroupByField Add()
        {
            return Add("", null);
        }

        public SQLSelectGroupByField Add(string strFieldName)
        {
            return Add(strFieldName, null);
        }

        public SQLSelectGroupByField Add(string strFieldName, SQLSelectTable objTable)
        {
            SQLSelectGroupByField objFieldOrder = new SQLSelectGroupByField(new SQLFieldExpression(objTable, strFieldName));

            pobjGroupByFields.Add(objFieldOrder);

            return objFieldOrder;
        }

        public SQLSelectGroupByField Add(SQLExpression objExpression)
        {
            SQLSelectGroupByField objFieldOrder = new SQLSelectGroupByField(objExpression);

            pobjGroupByFields.Add(objFieldOrder);

            return objFieldOrder;
        }

        public SQLSelectGroupByField this[string strFieldName]
        {
            get
            {
                return this[FieldNameIndex(strFieldName)];
            }
        }

        public SQLSelectGroupByField this[int intIndex]
        {
            get
            {
                return pobjGroupByFields[intIndex];
            }
        }

        public int Count
        {
            get
            {
                return pobjGroupByFields.Count;
            }
        }
		
        public bool Exists(string strFieldName)
        {
            return FieldNameIndex(strFieldName) >= 0;
        }

        public void Delete(ref SQLSelectGroupByField objGroupByField)
		{
			if (!pobjGroupByFields.Contains(objGroupByField))
				throw new IndexOutOfRangeException();
				
			pobjGroupByFields.Remove(objGroupByField);
			objGroupByField = null;
		}

        private int FieldNameIndex(string strFieldName)
        {
            SQLSelectGroupByField objGroupByField;

            for (int intIndex = 0; intIndex < this.Count; intIndex++)
            {
                objGroupByField = (SQLSelectGroupByField)(pobjGroupByFields[intIndex]);
                if (objGroupByField.Expression is SQLFieldExpression)
                {
                    if (string.Compare(strFieldName, ((SQLFieldExpression)objGroupByField.Expression).Name, true) == 0)
                        return intIndex;
                }
            }

            return -1;
        }

        public bool IsEmpty
        {
            get
            {
                return pobjGroupByFields.Count == 0;
            }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return pobjGroupByFields.GetEnumerator();
        }

        IEnumerator<SQLSelectGroupByField> IEnumerable<SQLSelectGroupByField>.GetEnumerator()
        {
            return pobjGroupByFields.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return pobjGroupByFields.GetEnumerator();
        }
    }
}