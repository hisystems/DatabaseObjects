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
