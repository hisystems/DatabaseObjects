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
                case DataType.Character:
                    return "NCHAR(" + size.ToString() + ")";
                case DataType.VariableCharacter:
                    return "NVARCHAR(" + size.ToString() + ")";
                case DataType.Text:
                    return "NTEXT";
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
