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
	public class SQLCommitTransaction : SQL.SQLStatement
	{
		public override string SQL
		{
			get
			{
				switch (base.ConnectionType)
				{
					case Database.ConnectionType.SQLServer:
					case Database.ConnectionType.SQLServerCompactEdition:
						return "COMMIT TRANSACTION";
					case Database.ConnectionType.MicrosoftAccess:
						return "COMMIT TRANSACTION";
					case Database.ConnectionType.MySQL:
						return "COMMIT";
					case Database.ConnectionType.Pervasive:
						return "COMMIT";
					case Database.ConnectionType.HyperSQL:
						return "COMMIT";
					default:
						throw new NotImplementedException(this.ConnectionType.ToString());
				}
			}
		}
	}
}
