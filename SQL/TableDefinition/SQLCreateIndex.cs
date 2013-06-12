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
    public class SQLCreateIndex : SQLStatement
    {
        private string pstrName;
        private string pstrTableName;
        private bool pbIsUnique;
        private SQLIndexFields pobjFields = new SQLIndexFields();

        public SQLCreateIndex()
        {
        }

        public SQLCreateIndex(string strIndexName, string strTableName, string[] strFieldNames)
        {
            this.Name = strIndexName;
            this.TableName = strTableName;

            foreach (string strFieldName in strFieldNames)
                this.Fields.Add(strFieldName);
        }

        public SQLCreateIndex(string strIndexName, string strTableName, string[] strFieldNames, bool bIsUnique)
            : this(strIndexName, strTableName, strFieldNames)
        {
            pbIsUnique = bIsUnique;
        }

        public string Name
        {
            get
            {
                return pstrName;
            }

            set
            {
                pstrName = value;
            }
        }

        public string TableName
        {
            get
            {
                return pstrTableName;
            }

            set
            {
                pstrTableName = value;
            }
        }

        public bool IsUnique
        {
            get
            {
                return pbIsUnique;
            }

            set
            {
                pbIsUnique = value;
            }
        }

        public SQLIndexFields Fields
        {
            get
            {
                return pobjFields;
            }
        }

        public override string SQL
        {
            get
            {
				return base.Serializer.SerializeCreateIndex(this);
            }
        }
    }

    public class SQLIndexFields : IEnumerable<SQLIndexField>
    {
        private List<SQLIndexField> pobjFields = new List<SQLIndexField>();

        internal SQLIndexFields()
        {
        }

        public SQLIndexField Add()
        {
            return Add("", OrderBy.Ascending);
        }

        public SQLIndexField Add(string strFieldName)
        {
            return Add(strFieldName, OrderBy.Ascending);
        }

        public SQLIndexField Add(string strFieldName, OrderBy eOrder)
        {
            SQLIndexField objField = new SQLIndexField();

            objField.Name = strFieldName;
            objField.Order = eOrder;

            pobjFields.Add(objField);

            return objField;
        }

        IEnumerator<SQLIndexField> IEnumerable<SQLIndexField>.GetEnumerator()
        {
            return pobjFields.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return pobjFields.GetEnumerator();
        }
    }

    public class SQLIndexField
    {
        private string pstrName;
        private OrderBy peOrder;

        internal SQLIndexField()
        {
        }

        public string Name
        {
            get
            {
                return pstrName;
            }

            set
            {
                pstrName = value;
            }
        }

        public OrderBy Order
        {
            get
            {
                return peOrder;
            }

            set
            {
                peOrder = value;
            }
        }
    }
}
