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
