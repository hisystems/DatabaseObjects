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
	public class SQLBeginTransaction : SQL.SQLStatement
	{
		public override string SQL
		{
			get
			{
				switch (base.ConnectionType)
				{
					case Database.ConnectionType.SQLServer:
					case Database.ConnectionType.SQLServerCompactEdition:
						return "BEGIN TRANSACTION";
					case Database.ConnectionType.MicrosoftAccess:
						return "BEGIN TRANSACTION";
					case Database.ConnectionType.MySQL:
						return "START TRANSACTION";
					case Database.ConnectionType.Pervasive:
						return "START TRANSACTION";
					case Database.ConnectionType.HyperSQL:
						return "START TRANSACTION";
					default:
						throw new NotImplementedException(this.ConnectionType.ToString());
				}
			}
		}
	}
}
