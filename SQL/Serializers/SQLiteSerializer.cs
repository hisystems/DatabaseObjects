// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseObjects.SQL.Serializers
{
	internal class SQLiteSerializer : Serializer 
	{
		public SQLiteSerializer()
		{
			base.fractionalSecondsFormat = ".fffffff";
		}

		public override Database.ConnectionType Type
		{
			get 
			{
				return Database.ConnectionType.SQLite;
			}
		}

		public override string SerializeByteArray(byte[] bytData)
		{
			return SerializeByteArray("X'", bytData, "'");
		}
		
		public override string SerializeAlterTableFieldsModifier(SQLTableFields.AlterModeType alterMode)
		{
			if (alterMode == SQLTableFields.AlterModeType.Drop)
				throw new NotSupportedException("SQLite does not support dropping a column");
			else
				return base.SerializeAlterTableFieldsModifier(alterMode);
		}

		public override string SerializeDropIndex(SQLDropIndex dropIndex)
		{
			if (String.IsNullOrEmpty(dropIndex.Name))
				throw new Exceptions.DatabaseObjectsException("IndexName has not been set.");

			// Table name is NOT used as in other database systems - it must be a globably unique index name
			return "DROP INDEX " + SerializeIdentifier(dropIndex.Name);
		}

		public override string SerializeSelect(SQLSelect select)
		{
			var tokens = new TokenSerializer();

			tokens.Add(base.SerializeSelect(select));

			if (select.Top > 0)
				tokens.Add("LIMIT " + select.Top.ToString());

			return tokens.ToString();
		}

		public override string SerializeTableField(SQLTableField field, SQLTableFields.AlterModeType alterMode)
		{
			// if altering or creating a field append the AUTOINCREMENT. Field format: name datatype default nullable keytype AUTOINCREMENT
			if (alterMode != SQLTableFields.AlterModeType.Drop && field.AutoIncrements)
				return base.SerializeTableField(field, alterMode) + " AUTOINCREMENT";
			else
				return base.SerializeTableField(field, alterMode);
		}

		public override string SerializeAutoIncrementValue(SQLAutoIncrementValue autoIncrementValue)
		{
			return "SELECT last_insert_rowid() AS " + autoIncrementValue.ReturnFieldName;
		}

		public override string SerializeTableExists(SQLTableExists tableExists)
		{
			var select = new SQLSelect();

			select.Tables.Add("sqlite_master");
			select.Where.Add("type", ComparisonOperator.EqualTo, "table");
			select.Where.Add("name", ComparisonOperator.EqualTo, tableExists.Name);

			return SerializeSelect(select);
		}

		public override string SerializeViewExists(SQLViewExists viewExists)
		{
			var select = new SQLSelect();

			select.Tables.Add("sqlite_master");
			select.Where.Add("Type", ComparisonOperator.EqualTo, "View");
			select.Where.Add("Name", ComparisonOperator.EqualTo, viewExists.ViewName);

			return SerializeSelect(select);
		}

		public override string SerializeIdentifier(string strIdentifierName)
		{
			return "\"" + strIdentifierName + "\"";
		}

		public override string SerializeDataType(SQL.DataType dataType, int size, int precision, int scale)
		{
			switch (dataType)
			{
				case DataType.TinyInteger:
					return "TINYINT";
				case DataType.SmallInteger:
					return "SMALLINT";
				case DataType.Integer:
					return "INTEGER";
				case DataType.BigInteger:
					return "BIGINT";
				case DataType.Character:
					return "CHARACTER(" + size.ToString() + ")";
				case DataType.UnicodeCharacter:
					return "NCHAR(" + size.ToString() + ")";
				case DataType.VariableCharacter:
					return "VARCHAR(" + size.ToString() + ")";
				case DataType.UnicodeVariableCharacter:
					return "NVARCHAR(" + size.ToString() + ")";
				case DataType.Decimal:
					return "DECIMAL(" + precision.ToString() + "," + scale.ToString() + ")";
				case DataType.Real:
					return "FLOAT";
				case DataType.Float:
					return "DOUBLE";
				case DataType.SmallMoney:
					return "DECIMAL(10,4)";
				case DataType.Money:
					return "DECIMAL(19,4)";
				case DataType.Boolean:
					return "INTEGER";
				case DataType.SmallDateTime:
					// TEXT as ISO8601 strings ("YYYY-MM-DD HH:MM:SS.SSS")
					return "DATETIME ";
				case DataType.DateTime:
					// TEXT as ISO8601 strings ("YYYY-MM-DD HH:MM:SS.SSS")
					return "DATETIME";
				case DataType.TimeStamp:
					return "DATETIME";
				case DataType.Text:
					return "TEXT";
				case DataType.UnicodeText:
					return "TEXT";
				case DataType.Binary:
					return "BLOB";
				case DataType.VariableBinary:
					return "BLOB";
				case DataType.Image:
					return "BLOB";
				case DataType.UniqueIdentifier:
					return "GUID";
				default:
					return base.SerializeDataType(dataType, size, precision, scale);
			}

		}
	}
}
