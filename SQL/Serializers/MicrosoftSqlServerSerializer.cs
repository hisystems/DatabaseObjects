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
	internal class MicrosoftSqlServerSerializer : MicrosoftSerializer
	{
		public override Database.ConnectionType Type
		{
			get 
			{
				return Database.ConnectionType.SQLServer;
			}
		}

		public override string SerializeViewExists(SQLViewExists viewExists)
		{
			var select = new SQLSelect();

			select.Tables.Add("sysobjects");
			select.Where.Add("Name", ComparisonOperator.EqualTo, viewExists.ViewName);
			select.Where.Add("XType", ComparisonOperator.EqualTo, "V");		//V = User defined view

			return SerializeSelect(select);
		}

		public override string SerializeIsolationLevel(System.Data.IsolationLevel isolationLevel)
		{
			if (isolationLevel == System.Data.IsolationLevel.Snapshot)
				return "SNAPSHOT";
			else
				return base.SerializeIsolationLevel(isolationLevel);
		}

		public override string SerializeTableExists(SQLTableExists tableExists)
		{
			var select = new SQLSelect();
			select.Tables.Add("sysobjects");
			select.Where.Add("Name", ComparisonOperator.EqualTo, tableExists.Name);
			select.Where.Add("XType", ComparisonOperator.EqualTo, "U");		// U = User defined table
			
			return SerializeSelect(select);
		}

		public override string SerializeAutoIncrementValue(SQLAutoIncrementValue autoIncrementValue)
		{
			return "SELECT SCOPE_IDENTITY() AS " + SerializeIdentifier(autoIncrementValue.ReturnFieldName);
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
