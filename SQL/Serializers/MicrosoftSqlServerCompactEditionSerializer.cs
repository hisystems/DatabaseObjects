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
	internal class MicrosoftSqlServerCompactEditionSerializer : MicrosoftSerializer
	{
		public override Database.ConnectionType Type
		{
			get 
			{
				return Database.ConnectionType.SQLServerCompactEdition;
			}
		}

		public override string SerializeViewExists(SQLViewExists viewExists)
		{
			throw new NotSupportedException();
		}

		public override string SerializeTableExists(SQLTableExists tableExists)
		{
			var select = new SQLSelect();
			select.Tables.Add("TABLES").SchemaName = "INFORMATION_SCHEMA";
			select.Where.Add("TABLE_NAME", ComparisonOperator.EqualTo, tableExists.Name);

			return SerializeSelect(select);
		}

		public override string SerializeDataType(DataType dataType, int size, int precision, int scale)
		{
			switch (dataType)
			{
				case DataType.SmallDateTime:
					return "DATETIME";
				default:
					return base.SerializeDataType(dataType, size, precision, scale);
			}
		}

		public override string SerializeDropIndex(SQLDropIndex dropIndex)
		{
			if (String.IsNullOrEmpty(dropIndex.Name))
				throw new Exceptions.DatabaseObjectsException("IndexName has not been set.");
			else if (String.IsNullOrEmpty(dropIndex.TableName))
				throw new Exceptions.DatabaseObjectsException("TableName has not been set.");

			return "DROP INDEX " + SerializeIdentifier(dropIndex.TableName) + "." + SerializeIdentifier(dropIndex.Name);
		}
	}
}
