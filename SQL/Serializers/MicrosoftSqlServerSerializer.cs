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
