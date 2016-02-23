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
	internal class MySqlSerializer : Serializer 
	{
		public override Database.ConnectionType Type
		{
			get 
			{
				return Database.ConnectionType.MySQL;
			}
		}

		public override string SerializeViewExists(SQLViewExists viewExists)
		{
			var select = new SQLSelect();

			select.Tables.Add("Tables").SchemaName = "INFORMATION_SCHEMA";
			select.Where.Add("Table_Type", ComparisonOperator.EqualTo, "View");
			select.Where.Add("TABLE_NAME", ComparisonOperator.Like, viewExists.ViewName);

			return SerializeSelect(select);
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
			return "SHOW TABLES LIKE " + SerializeValue(tableExists.Name);
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
			return "`" + strIdentifierName + "`";
		}

		public override string SerializeSingleQuoteCharacter()
		{
			return @"'\''";
		}

		public override string SerializeBackslashCharacter()
		{
			return @"\\";
		}

		public override string SerializeString(string stringValue)
		{
			return "'" + stringValue.Replace(@"\", @"\\").Replace(@"'", @"\'") + "'";
		}

		public override string SerializeGetDateFunctionExpression(SQLGetDateFunctionExpression expression)
		{
			return "CURDATE()";
		}

		public override string SerializeLengthFunctionExpression(SQLLengthFunctionExpression expression)
		{
			return "LENGTH" + SerializeFunctionExpressionArguments(expression);
		}

		public override string SerializeAlterTableFieldsModifier(SQLTableFields.AlterModeType alterMode)
		{
			// For MySQL the modifiers are added for each column not for all of the columns.
			// So override the SerializeAlterTableFieldModifier instead.
			return String.Empty;
		}

		public override string SerializeAlterTableFieldModifier(SQLTableFields.AlterModeType alterMode)
		{
			// For MySQL the modifiers are added for each column not for all of the columns.
			return SerializeColumnAlterMode(alterMode);
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

		public override string SerializeAutoIncrementValue(SQLAutoIncrementValue autoIncrementValue)
		{
			// The @@IDENTITY function is supported by MySQL from version 3.23.25.
			// The original function is being used just in case.
			return "SELECT LAST_INSERT_ID() AS " + SerializeIdentifier(autoIncrementValue.ReturnFieldName);
		}

		public override string SerializeTableFieldDataType(SQLTableField field)
		{
			if (field.AutoIncrements)
				return base.SerializeTableFieldDataType(field) + " AUTO_INCREMENT";
			else
				return base.SerializeTableFieldDataType(field);
		}

		public override string SerializeTableFieldKeyTypeOption(SQLTableField field)
		{
			// must be set to a key type if it is an auto increment field
			if (field.AutoIncrements && field.KeyType == KeyType.None)
				return "UNIQUE";
			else
				return base.SerializeTableFieldKeyTypeOption(field);
		}

		public override string SerializeDataType(DataType dataType, int size, int precision, int scale)
		{
			switch (dataType)
			{
				case DataType.TinyInteger:
					return "TINYINT UNSIGNED";
				case DataType.SmallInteger:
					return "SMALLINT";
				case DataType.Integer:
					return "INT";
				case DataType.BigInteger:
					return "BIGINT";
				case DataType.Character:
					return "CHAR(" + size.ToString() + ")";
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
					return "BIT";
				case DataType.SmallDateTime:
					return "DATETIME";
				case DataType.DateTime:
					return "DATETIME";
				case DataType.TimeStamp:
					return "TIMESTAMP";
				case DataType.Text:
					return "LONGTEXT";
				case DataType.UnicodeText:
					return "LONGTEXT CHARACTER SET UTF8";
				case DataType.Binary:
					return "BLOB";
				case DataType.VariableBinary:
					return "BLOB";
				case DataType.Image:
					return "LONGBLOB";
				case DataType.UniqueIdentifier:
                    return "CHAR(38)";
				default:
					return base.SerializeDataType(dataType, size, precision, scale);
			}
		}
	}
}
