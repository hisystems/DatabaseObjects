// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public class SQLSetTransactionIsolationLevel : SQL.SQLStatement
	{
		private System.Data.IsolationLevel peIsolationLevel;
			
		public SQLSetTransactionIsolationLevel(System.Data.IsolationLevel eIsolationLevel)
		{
			peIsolationLevel = eIsolationLevel;
		}
			
		public override string SQL
		{
			get
			{
				switch (base.ConnectionType)
				{
					case Database.ConnectionType.SQLServer:
					case Database.ConnectionType.MySQL:
					case Database.ConnectionType.Pervasive:
					case Database.ConnectionType.SQLServerCompactEdition:
					case Database.ConnectionType.HyperSQL:
						return "SET TRANSACTION ISOLATION LEVEL " + GetIsolationLevelString(peIsolationLevel);
					default:
						throw new NotImplementedException(this.ConnectionType.ToString() + "; Transaction isolation levels");
				}
			}
		}
			
		private string GetIsolationLevelString(System.Data.IsolationLevel eIsolationLevel)
		{
			switch (eIsolationLevel)
			{
				case IsolationLevel.ReadCommitted:
					return "READ COMMITTED";
				case IsolationLevel.ReadUncommitted:
					return "READ UNCOMMITTED";
				case IsolationLevel.RepeatableRead:
					return "REPEATABLE READ";
				case IsolationLevel.Serializable:
					return "SERIALIZABLE";
				case IsolationLevel.Snapshot:
					//Only SQL Server 2005+ supports snapshot isolation levels
					if (base.ConnectionType == Database.ConnectionType.SQLServer)
						return "SNAPSHOT";
					else
						throw new NotSupportedException("Snapshots isolation level is not supported for " + base.ConnectionType);
				default:
					throw new NotImplementedException("Isolation level");
			}
		}
	}
}
