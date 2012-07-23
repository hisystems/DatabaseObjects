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
	public class SQLRollbackTransaction : SQL.SQLStatement
	{
		public override string SQL
		{
			get
			{
				switch (base.ConnectionType)
				{
					case Database.ConnectionType.SQLServer:
					case Database.ConnectionType.SQLServerCompactEdition:
						return "ROLLBACK TRANSACTION";
					case Database.ConnectionType.MicrosoftAccess:
						return "ROLLBACK TRANSACTION";
					case Database.ConnectionType.MySQL:
						return "ROLLBACK";
					case Database.ConnectionType.Pervasive:
						return "ROLLBACK";
					case Database.ConnectionType.HyperSQL:
						return "ROLLBACK";
					default:
						throw new NotImplementedException(base.ConnectionType.ToString());
				}
			}
		}
	}
}
