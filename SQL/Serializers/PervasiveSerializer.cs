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
	internal class PervasiveSerializer : Serializer
	{
		public override Database.ConnectionType Type
		{
			get 
			{
				return Database.ConnectionType.Pervasive;
			}
		}

		public override string SerializeViewExists(SQLViewExists viewExists)
		{
			throw new NotSupportedException();
		}

		public override string SerializeRollbackTransaction(SQLRollbackTransaction rollbackTransaction)
		{
			return "ROLLBACK";
		}

		public override string SerializeCommitTransaction(SQLCommitTransaction commitTransaction)
		{
			return "COMMIT";
		}

		public override string SerializeBeingTransaction(SQLBeginTransaction beginTransaction)
		{
			return "START TRANSACTION";
		}

		public override string SerializeTableExists(SQLTableExists tableExists)
		{
			var select = new SQLSelect();
			select.Tables.Add("X$FILE");
			select.Where.Add("Xf$name", ComparisonOperator.EqualTo, tableExists.Name);

			return SerializeSelect(select);
		}

		public override string SerializeSelect(SQLSelect select)
		{
			var tokens = new TokenSerializer();

			tokens.Add(base.SerializeSelect(select));

			if (select.PerformLocking)
				tokens.Add("FOR UPDATE");

			if (select.Top > 0)
				tokens.Add("LIMIT " + select.Top.ToString());

			return tokens.ToString();
		}

		public override string SerializeIdentifier(string strIdentifierName)
		{
			return "\"" + strIdentifierName + "\"";
		}

		public override string SerializeColumnAlterMode(SQLTableFields.AlterModeType alterMode)
		{
			switch (alterMode)
			{
				case SQLTableFields.AlterModeType.Modify:
					return "MODIFY COLUMN";
				default:
					return base.SerializeColumnAlterMode(alterMode);
			}
		}

		public override string SerializeTableFieldNullableOption(SQLTableField field)
		{
			if (field.AutoIncrements)
				return String.Empty;		// NULL/NOT NULL is not required if it's an IDENTITY field
			else
				return base.SerializeTableFieldNullableOption(field);
		}

		public override string SerializeTableFieldDataType(SQLTableField field)
		{
			if (field.AutoIncrements)
				return "IDENTITY";
			else
				return base.SerializeTableFieldDataType(field);
		}

		public override string SerializeDataType(DataType dataType, int size, int precision, int scale)
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
					return "CHAR(" + size.ToString() + ")";
				case DataType.UnicodeCharacter:
					//Unable to verify this is correct.
					return "CHAR(" + size.ToString() + ")";
				case DataType.VariableCharacter:
					return "VARCHAR(" + size.ToString() + ")";
				case DataType.UnicodeVariableCharacter:
					//Unable to verify this is correct.
					return "VARCHAR(" + size.ToString() + ")";
				case DataType.Decimal:
					return "NUMERIC(" + precision.ToString() + "," + scale.ToString() + ")";
				case DataType.Real:
					return "REAL";
				case DataType.Float:
					return "FLOAT";
				case DataType.SmallMoney:
					return "DECIMAL(10,4)";
				case DataType.Money:
					return "DECIMAL(19,4)";
				case DataType.Boolean:
					return "BIT";
				case DataType.SmallDateTime:
					return "DATETIME";
				case DataType.DateTime:
					return "DATETIME";
				case DataType.TimeStamp:
					return "TIMESTAMP";
				case DataType.Text:
					return "LONGVARCHAR";
				case DataType.UnicodeText:
					return "LONGVARCHAR";
				case DataType.Binary:
					return "BINARY";
				case DataType.VariableBinary:
					return "LONGVARBINARY";
				case DataType.Image:
					return "LONGVARBINARY";
				case DataType.UniqueIdentifier:
					return "UNIQUEIDENTIFIER";
				default:
					return base.SerializeDataType(dataType, size, precision, scale);
			}
		}
	}
}
