// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseObjects.SQL.Serializers
{
	internal class MicrosoftAccessSerializer : MicrosoftSerializer
	{
        public MicrosoftAccessSerializer()
        {
            base.SerializeTopClauseWithParentheses = false;
        }
		
        public override Database.ConnectionType Type
		{
			get 
			{
				return Database.ConnectionType.MicrosoftAccess;
			}
		}

		public override string SerializeViewExists(SQLViewExists viewExists)
		{
			throw new NotSupportedException();
		}

		public override string SerializeTableExists(SQLTableExists tableExists)
		{
			var select = new SQLSelect();

			select.Tables.Add("msysobjects");
			select.Where.Add("Name", ComparisonOperator.EqualTo, tableExists.Name);
			select.Where.Add("Type", ComparisonOperator.EqualTo, 1);

			return SerializeSelect(select);
		}

		public override string SerializeDateTime(DateTime dateTime)
		{
			return "#" + base.SerializeDateTimeValue(dateTime) + "#";
		}

		public override string SerializeGetDateFunctionExpression(SQLGetDateFunctionExpression expression)
		{
			return "Now()";
		}

		public override string SerializeAfterSelectTables(SQLSelect select)
		{
			if (select.PerformLocking)
				throw new NotSupportedException("Locking is not supported for " + this.Type.ToString());

			return base.SerializeAfterSelectTables(select);
		}

		public override string SerializeBoolean(bool value)
		{
			if (value)
				return "-1";
			else
				return "0";
		}

		public override string SerializeDataType(SQL.DataType dataType, int size, int precision, int scale)
		{
			switch (dataType)
			{
				case DataType.TinyInteger:
					return "BYTE";
				case DataType.BigInteger:
					return "NUMERIC(19,0)";
				case DataType.Character:
					return "TEXT(" + size.ToString() + ")";
				case DataType.UnicodeCharacter:
					//Unicode is only supported in Microsoft Access 2000+
					return "TEXT(" + size.ToString() + ")";
				case DataType.VariableCharacter:
					return "TEXT(" + size.ToString() + ")";
				case DataType.UnicodeVariableCharacter:
					//Unicode is only supported in Microsoft Access 2000+
					return "TEXT(" + size.ToString() + ")";
				case DataType.SmallMoney:
					return "NUMERIC(10,4)";
				case DataType.Money:
					return "NUMERIC(19,4)";
				case DataType.Boolean:
					return "YESNO";
				case DataType.SmallDateTime:
					return "DATETIME";
				case DataType.TimeStamp:
					throw new NotSupportedException("TIMESTAMP");
				case DataType.Text:
					return "MEMO";
				case DataType.UnicodeText:
					//Unicode is only supported in Microsoft Access 2000+
					return "MEMO";
				case DataType.Binary:
					return "OLEOBJECT";
				case DataType.VariableBinary:
					return "OLEOBJECT";
				default:
					return base.SerializeDataType(dataType, size, precision, scale);
			}
		}
	}
}
