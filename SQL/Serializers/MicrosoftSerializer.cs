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
	/// <summary>
	/// Serializes an DatabaseObjects.SQL.* object into an SQL string ready for execution.
	/// This class manages Microsoft oriented database systems; SQL Server, Access, Compact Edition.
	/// Methods can be overridden if a database system serializes differently.
	/// </summary>
	internal abstract class MicrosoftSerializer : Serializer
	{
        protected bool SerializeTopClauseWithParentheses = true;

		public override string SerializeBeforeSelectFields(SQLSelect select)
		{
            var topClause = String.Empty;

            if (select.Top > 0)
            {
                if (SerializeTopClauseWithParentheses)
                    topClause = "TOP(" + select.Top.ToString() + ") ";
                else
                    topClause = "TOP " + select.Top.ToString() + " ";
            }

			return base.SerializeBeforeSelectFields(select) + topClause;
		}

		public override string SerializeTableFieldDataType(SQLTableField field)
		{
			if (field.AutoIncrements)
				return base.SerializeTableFieldDataType(field) + " IDENTITY";
			else
				return base.SerializeTableFieldDataType(field);
		}

		public override string SerializeIdentifier(string strIdentifierName)
		{
			return "[" + strIdentifierName + "]";
		}

		public override string SerializeAfterSelectTables(SQLSelect select)
		{
			if (select.PerformLocking)
				return "WITH (HOLDLOCK, ROWLOCK)";
			else
				return String.Empty;
		}

		public override string SerializeGetDateFunctionExpression(SQLGetDateFunctionExpression expression)
		{
			return "GetDate()";
		}

		public override string SerializeSelectExpression(SQLSelectExpression selectExpression)
		{
			return "SELECT " + selectExpression.Expression.SQL(this);
		}

		public override string SerializeLengthFunctionExpression(SQLLengthFunctionExpression expression)
		{
			return "LEN" + SerializeFunctionExpressionArguments(expression);
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
					return "NCHAR(" + size.ToString() + ")";
				case DataType.VariableCharacter:
					return "VARCHAR(" + size.ToString() + ")";
				case DataType.UnicodeVariableCharacter:
					return "NVARCHAR(" + size.ToString() + ")";
				case DataType.Decimal:
					return "NUMERIC(" + precision.ToString() + "," + scale.ToString() + ")";
				case DataType.Real:
					return "REAL";
				case DataType.Float:
					return "FLOAT";
				case DataType.SmallMoney:
					return "SMALLMONEY";
				case DataType.Money:
					return "MONEY";
				case DataType.Boolean:
					return "BIT";
				case DataType.SmallDateTime:
					return "SMALLDATETIME";
				case DataType.DateTime:
					return "DATETIME";
				case DataType.TimeStamp:
					return "TIMESTAMP";
				case DataType.Text:
					return "TEXT";
				case DataType.UnicodeText:
					return "NTEXT";
				case DataType.Binary:
					return "BINARY";
				case DataType.VariableBinary:
					return "VARBINARY";
				case DataType.Image:
					return "IMAGE";
				case DataType.UniqueIdentifier:
					return "UNIQUEIDENTIFIER";
				default:
					return base.SerializeDataType(dataType, size, precision, scale);
			}
		}
	}
}
