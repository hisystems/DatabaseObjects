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
	internal static class Misc
	{
		public static string SQLConvertIdentifierName(string strIdentifierName, Database.ConnectionType eConnectionType)
		{
			//This function places tags around a field name or table name to ensure it doesn't
			//conflict with a reserved word or if it contains spaces it is not misinterpreted
				
			switch (eConnectionType)
			{
				case Database.ConnectionType.MicrosoftAccess:
				case Database.ConnectionType.SQLServer:
				case Database.ConnectionType.SQLServerCompactEdition:
					return "[" + strIdentifierName + "]";
				case Database.ConnectionType.MySQL:
					return "`" + strIdentifierName + "`";
				case Database.ConnectionType.Pervasive:
					return "\"" + strIdentifierName + "\"";
				case Database.ConnectionType.HyperSQL:
					return "\"" + strIdentifierName + "\"";
				default:
					throw new NotImplementedException(eConnectionType.ToString());
			}
		}
			
		public static string SQLConvertValue(object objValue, Database.ConnectionType eConnectionType)
		{
			if (SQLValueIsNull(objValue))
				return "NULL";
			else if (objValue is bool)
			{
				if ((bool)objValue)
					return "1";
				else
					return "0";
			}
			else if (objValue is char)
			{
				char chChar = (char) objValue;
					
				if (chChar == '\'')
				{
					switch (eConnectionType)
					{
						case Database.ConnectionType.MicrosoftAccess:
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
						case Database.ConnectionType.Pervasive:
						case Database.ConnectionType.HyperSQL:
							return @"''''";
						case Database.ConnectionType.MySQL:
							return @"'\''";
						default:
							throw new NotImplementedException(eConnectionType.ToString());
							break;
					}
				}
				else if (chChar.Equals(@"\"))
				{
					switch (eConnectionType)
					{
						case Database.ConnectionType.MicrosoftAccess:
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
						case Database.ConnectionType.Pervasive:
						case Database.ConnectionType.HyperSQL:
							return @"\";
						case Database.ConnectionType.MySQL:
							return @"\\";
						default:
							throw new NotImplementedException(eConnectionType.ToString());
							break;
					}
				}
				else
					return "'" + chChar + "'";
			}
			else if (objValue is DateTime)
			{
				DateTime dtDate = (DateTime)objValue;
				string strDate;
					
				//If the date hasn't been set then set to the 1899-12-30 so that
				//the date isn't set to 2001-01-01
				if (dtDate.Day == 1 && dtDate.Month == 1 && dtDate.Year == 1)
					strDate = "1899-12-30";
				else
					strDate = System.Convert.ToString(dtDate.Year + "-" + dtDate.Month + "-" + dtDate.Day);
					
				if (dtDate.Hour != 0 || dtDate.Minute != 0 || dtDate.Second != 0)
					strDate += " " + dtDate.Hour + ":" + dtDate.Minute + ":" + dtDate.Second + "." + dtDate.Millisecond;
					
				switch (eConnectionType)
				{
					case Database.ConnectionType.MicrosoftAccess:
						return "#" + strDate + "#";
					case Database.ConnectionType.MySQL:
					case Database.ConnectionType.SQLServer:
					case Database.ConnectionType.SQLServerCompactEdition:
					case Database.ConnectionType.Pervasive:
					case Database.ConnectionType.HyperSQL:
						return "\'" + strDate + "\'";
					default:
						throw new NotImplementedException(eConnectionType.ToString());
				}
			}
			else if (objValue is string)
			{
				switch (eConnectionType)
				{
					case Database.ConnectionType.MicrosoftAccess:
					case Database.ConnectionType.SQLServer:
					case Database.ConnectionType.SQLServerCompactEdition:
					case Database.ConnectionType.Pervasive:
					case Database.ConnectionType.HyperSQL:
						return "'" + ((string)objValue).Replace(@"'", @"''") + "'";
					case Database.ConnectionType.MySQL:
						//Replace the \ escape character with \\ and replace single quotes with two single quotes
						return "'" + ((string)objValue).Replace(@"\", @"\\").Replace(@"'", @"\'") + "'";
					default:
						throw new NotImplementedException(eConnectionType.ToString());
						break;
				}
			}
			else if (objValue is byte[])
			{
				switch (eConnectionType)
				{
					case Database.ConnectionType.MicrosoftAccess:
					case Database.ConnectionType.SQLServer:
					case Database.ConnectionType.SQLServerCompactEdition:
					case Database.ConnectionType.Pervasive:
					case Database.ConnectionType.MySQL:
						return SQLConvertBinaryArray("0x", (byte[])objValue, string.Empty);
					case Database.ConnectionType.HyperSQL:
						return SQLConvertBinaryArray("x'", (byte[])objValue, "'");
					default:
						throw new NotImplementedException(eConnectionType.ToString());
				}
			}
			else if (objValue is System.Guid)
			{
				return "'" + ((System.Guid) objValue).ToString("D") + "'";
			}
			else
			{
				return System.Convert.ToString(objValue);
			}
		}
			
		private static string SQLConvertBinaryArray(string strHexPrefix, byte[] bytData, string strHexSuffix)
		{
			System.Text.StringBuilder objHexData = new System.Text.StringBuilder(strHexPrefix, System.Convert.ToInt32((bytData.Length * 2) + strHexPrefix.Length + strHexSuffix.Length));
				
			foreach (byte bytByte in bytData)
				objHexData.Append(string.Format("{0:X2}", bytByte));
				
			objHexData.Append(strHexSuffix);
				
			return objHexData.ToString();
		}
			
		public static bool SQLValueIsNull(object objValue)
		{
			if (objValue == null)
				return true;
			else if (objValue == DBNull.Value)
				return true;
			else
				return false;
		}
			
		public static string SQLConvertAggregate(SQL.AggregateFunction eAggregate)
		{
            switch (eAggregate)
            {
                case AggregateFunction.Average:
				    return "AVG";
                case AggregateFunction.Count:
                    return "COUNT";
                case AggregateFunction.Sum:
                    return "SUM";
                case AggregateFunction.Minimum:
                    return "MIN";
                case AggregateFunction.Maximum:
                    return "MAX";
                case AggregateFunction.StandardDeviation:
                    return "STDEV";
                case AggregateFunction.Variance:
                    return "VAR";
                default:
				    throw new NotSupportedException();
            }
		}
			
		public static string SQLConvertCompare(SQL.ComparisonOperator eCompare)
		{
            switch (eCompare)
            {
                case ComparisonOperator.EqualTo:
				    return "=";
                case ComparisonOperator.NotEqualTo:
				    return "<>";
                case ComparisonOperator.LessThan:
				    return "<";
                case ComparisonOperator.LessThanOrEqualTo:
				    return "<=";
                case ComparisonOperator.GreaterThan:
				    return ">";
                case ComparisonOperator.GreaterThanOrEqualTo:
				    return ">=";
                case ComparisonOperator.Like:
				    return "LIKE";
                case ComparisonOperator.NotLike:
				    return "NOT LIKE";
                default:
				    throw new NotSupportedException();
            }
		}
			
		public static string SQLConvertLogicalOperator(SQL.LogicalOperator eLogicalOperator)
		{
			string strLogicalOperator;
				
			if (eLogicalOperator == LogicalOperator.And)
				strLogicalOperator = "AND";
			else if (eLogicalOperator == LogicalOperator.Or)
				strLogicalOperator = "OR";
			else
				throw new NotSupportedException();
				
			return strLogicalOperator;
		}
			
		public static string SQLFieldNameAndTablePrefix(SQLSelectTable objTable, string strFieldName, Database.ConnectionType eConnectionType)
		{
			string strTablePrefix = string.Empty;
				
			if (objTable != null)
				strTablePrefix = SQLTablePrefix(objTable, eConnectionType) + ".";
				
			return strTablePrefix + SQLConvertIdentifierName(strFieldName, eConnectionType);
		}
			
		/// <summary>
		/// Returns the table alias or if the alias is not set the table's name.
		/// </summary>
		public static string SQLTablePrefix(SQLSelectTable objTable, Database.ConnectionType eConnectionType)
		{
			if (!String.IsNullOrEmpty(objTable.Alias))
				return SQLConvertIdentifierName(objTable.Alias, eConnectionType);
			else
				return SQLConvertIdentifierName(objTable.Name, eConnectionType);
		}
			
		public static object SQLConditionValue(object objValue)
		{
			if (objValue is SQLFieldValue)
				return ((SQLFieldValue)objValue).Value;
			else
				return objValue;
		}
			
		public static void CompareValuePairAssertValid(SQL.ComparisonOperator eCompare, object objValue)
		{
			if (!(objValue is string) && (eCompare == ComparisonOperator.Like || eCompare == ComparisonOperator.NotLike))
				throw new Exceptions.DatabaseObjectsException("The LIKE operator cannot be used in conjunction with a non-string data type");
			else if (objValue is bool&& !(eCompare == ComparisonOperator.EqualTo || eCompare == ComparisonOperator.NotEqualTo))
				throw new Exceptions.DatabaseObjectsException("A boolean value can only be used in conjunction with the " + ComparisonOperator.EqualTo.ToString() + " or " + ComparisonOperator.NotEqualTo.ToString() + " operators");
		}
			
		public static void SQLConvertBooleanValue(ref object objValue, ref SQL.ComparisonOperator eCompare)
		{
			// If a boolean variable set to true then use the NOT
			// operator and compare it to 0. ie. if the condition is 'field = true' then
			// SQL code should be 'field <> 0'
			// -1 is true in MSAccess and 1 is true in SQLServer.
				
			if (objValue is bool)
			{
				if (((bool)objValue) == true)
				{
					if (eCompare == ComparisonOperator.EqualTo)
						eCompare = ComparisonOperator.NotEqualTo;
					else
						eCompare = ComparisonOperator.EqualTo;
				
                    objValue = false;
				}
			}
		}
			
		public static string SQLConvertCondition(ComparisonOperator eCompare, object objValue, Database.ConnectionType eConnectionType)
		{
			string strSQL = string.Empty;
				
			SQLConvertBooleanValue(ref objValue, ref eCompare);
				
			//Return 'IS NULL' rather than '= NULL'
			if (SQLValueIsNull(objValue))
			{
				if (eCompare == ComparisonOperator.EqualTo)
					strSQL += "IS " + SQLConvertValue(objValue, eConnectionType);
				else if (eCompare == ComparisonOperator.NotEqualTo)
					strSQL += "IS NOT " + SQLConvertValue(objValue, eConnectionType);
				else
					throw new Exceptions.DatabaseObjectsException("DBNull or Nothing/null specified as an SQLCondition value using the " + eCompare.ToString() + " operator");
			}
			else
				strSQL += SQLConvertCompare(eCompare) + " " + SQLConvertValue(objValue, eConnectionType);
				
			return strSQL;
		}
			
		public static string SQLConvertDataTypeString(Database.ConnectionType eConnection, DataType eDataType, int intSize, int intPrecision, int intScale)
		{
			string strDataType = string.Empty;
				
			switch (eDataType)
			{
				case DataType.TinyInteger:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "BYTE";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "TINYINT UNSIGNED";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "TINYINT";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "TINYINT";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "TINYINT";
							break;
					}
					break;
						
				case DataType.SmallInteger:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "SMALLINT";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "SMALLINT";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "SMALLINT";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "SMALLINT";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "SMALLINT";
							break;
					}
					break;
						
				case DataType.Integer:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "INTEGER";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "INT";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "INTEGER";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "INTEGER";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "INT";
							break;
					}
					break;
						
				case DataType.BigInteger:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "NUMERIC(19,0)";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "BIGINT";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "BIGINT";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "BIGINT";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "BIGINT";
							break;
					}
					break;
						
				case DataType.Character:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "TEXT(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "CHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "CHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "CHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "CHAR(" + intSize.ToString() + ")";
							break;
					}
					break;
						
				case DataType.UnicodeCharacter:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							//Unicode is only supported in Microsoft Access 2000+
							strDataType = "TEXT(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "NCHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "NCHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.Pervasive:
							//Unable to verify this is correct.
							strDataType = "CHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "CHAR(" + intSize.ToString() + ")";
							break;
					}
					break;
						
				case DataType.VariableCharacter:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "TEXT(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "VARCHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.SQLServer:
							strDataType = "VARCHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "VARCHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "VARCHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "VARCHAR(" + intSize.ToString() + ")";
							break;
					}
					break;
						
				case DataType.UnicodeVariableCharacter:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							//Unicode is only supported in Microsoft Access 2000+
							strDataType = "TEXT(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "NVARCHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "NVARCHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.Pervasive:
							//Unable to verify this is correct.
							strDataType = "VARCHAR(" + intSize.ToString() + ")";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "VARCHAR(" + intSize.ToString() + ")";
							break;
					}
					break;
						
				case DataType.Decimal:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "NUMERIC(" + intPrecision.ToString() + "," + intScale.ToString() + ")";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "DECIMAL(" + intPrecision.ToString() + "," + intScale.ToString() + ")";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "NUMERIC(" + intPrecision.ToString() + "," + intScale.ToString() + ")";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "NUMERIC(" + intPrecision.ToString() + "," + intScale.ToString() + ")";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "NUMBER(" + intPrecision.ToString() + "," + intScale.ToString() + ")";
							break;
					}
					break;
						
				case DataType.Real:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "REAL";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "FLOAT";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "REAL";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "REAL";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "DOUBLE";
							break;
					}
					break;
						
				case DataType.Float:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "FLOAT";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "DOUBLE";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "FLOAT";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "FLOAT";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "DOUBLE";
							break;
					}
					break;
						
				case DataType.SmallMoney:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "NUMERIC(10,4)";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "DECIMAL(10,4)";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "SMALLMONEY";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "DECIMAL(10,4)";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "NUMBER(10,4)";
							break;
					}
					break;
						
				case DataType.Money:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "NUMERIC(19,4)";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "DECIMAL(19,4)";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "MONEY";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "DECIMAL(19,4)";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "NUMBER(19,4)";
							break;
					}
					break;
						
				case DataType.Boolean:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "YESNO";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "BIT";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "BIT";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "BIT";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "BOOLEAN";
							break;
					}
					break;
						
				case DataType.SmallDateTime:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "DATETIME";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "DATETIME";
							break;
						case Database.ConnectionType.SQLServer:
							strDataType = "SMALLDATETIME";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "DATETIME";
							break;
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "DATETIME";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "DATE";
							break;
					}
					break;
						
				case DataType.DateTime:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "DATETIME";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "DATETIME";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "DATETIME";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "DATETIME";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "DATE";
							break;
					}
					break;
						
				case DataType.TimeStamp:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							throw new NotSupportedException("TIMESTAMP");
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "TIMESTAMP";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "TIMESTAMP";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "TIMESTAMP";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "TIMESTAMP";
							break;
					}
					break;
						
				case DataType.Text:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "MEMO";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "LONGTEXT";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "TEXT";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "LONGVARCHAR";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "CLOB(2G)";
							break;
					}
					break;
						
				case DataType.UnicodeText:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							//Unicode is only supported in Microsoft Access 2000+
							strDataType = "MEMO";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "LONGTEXT CHARACTER SET UTF8";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "NTEXT";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "LONGVARCHAR";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "CLOB(2G)";
							break;
					}
					break;
						
				case DataType.Binary:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "OLEOBJECT";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "BLOB";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "BINARY";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "BINARY";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "BINARY(8000)";
							break;
					}
					break;
						
				case DataType.VariableBinary:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "OLEOBJECT";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "BLOB";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "VARBINARY";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "LONGVARBINARY";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "VARBINARY(8000)";
							break;
					}
					break;
						
				case DataType.Image:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "IMAGE";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "LONGBLOB";
							break;
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "IMAGE";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "LONGVARBINARY";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "BLOB(2G)";
							break;
					}
					break;
						
				case DataType.UniqueIdentifier:
					switch (eConnection)
					{
						case Database.ConnectionType.MicrosoftAccess:
							strDataType = "UNIQUEIDENTIFIER";
							break;
						case Database.ConnectionType.MySQL:
							strDataType = "UNIQUEIDENTIFIER";
							break;
						case Database.ConnectionType.Pervasive:
							strDataType = "UNIQUEIDENTIFIER";
							break;
						case Database.ConnectionType.SQLServer:
							strDataType = "UNIQUEIDENTIFIER";
							break;
						case Database.ConnectionType.SQLServerCompactEdition:
							strDataType = "UNIQUEIDENTIFIER";
							break;
						case Database.ConnectionType.HyperSQL:
							strDataType = "UNIQUEIDENTIFIER";
							break;
					}
					break;
						
				default:
					throw new NotImplementedException("Data type " + eDataType.ToString());
					break;
			}
				
			if (String.IsNullOrEmpty(strDataType))
				throw new NotImplementedException("Data type " + eDataType.ToString() + " is not implemented for connection type " + eConnection.ToString());
				
			return strDataType;
		}
			
		public static void DataTypeEnsureIsDecimal(DataType eDataType)
		{
			if (eDataType != DataType.Decimal)
				throw new Exceptions.MethodLockedException("First set Type to " + DataType.Decimal.ToString());
		}
			
		public static void DataTypeEnsureIsCharacter(SQL.DataType eDataType)
		{
			if (!DataTypeIsCharacter(eDataType))
				throw new Exceptions.MethodLockedException("Data type is not character based");
		}
			
		public static void DataTypeEnsureIsInteger(SQL.DataType eDataType)
		{
			if (!DataTypeIsInteger(eDataType))
				throw new Exceptions.MethodLockedException();
		}
			
		public static bool DataTypeIsInteger(SQL.DataType eDataType)
		{
            switch (eDataType)
            {
                case DataType.BigInteger:
                case DataType.Integer:
                case DataType.SmallInteger:
                case DataType.TinyInteger:
                    return true;
                default:
                    return false;
            }
		}
			
		public static bool DataTypeIsCharacter(DataType eDataType)
		{
			switch (eDataType)
			{
				case DataType.Character:
				case DataType.UnicodeCharacter:
				case DataType.VariableCharacter:
				case DataType.UnicodeVariableCharacter:
					return true;
				default:
					return false;
			}
		}
			
		/// <summary>
		/// All keys are returned in lower case.
		/// </summary>
		/// <exception cref="FormatException">If the connection string is in an invalid format.</exception>
		internal static IDictionary<string, string> GetDictionaryFromConnectionString(string strConnectionString)
		{
			var objDictionary = new Dictionary<string, string>();
			string[] strPropertyValueArray;
				
			foreach (string strPropertyValue in strConnectionString.Split(';'))
			{
				if (!String.IsNullOrEmpty(strPropertyValue))
				{
					strPropertyValueArray = strPropertyValue.Split('=');
					if (strPropertyValueArray.Length == 2)
						objDictionary.Add(strPropertyValueArray[0].Trim().ToLower(), strPropertyValueArray[1].Trim());
					else
						throw new FormatException("Invalid key property definition for '" + strPropertyValue + "' from '" + strConnectionString + "'");
				}
			}
				
			return objDictionary;
		}
	}
}
