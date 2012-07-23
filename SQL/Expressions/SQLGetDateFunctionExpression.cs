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
	public class SQLGetDateFunctionExpression : SQLExpression
	{
		internal override string SQL(Database.ConnectionType eConnectionType)
		{
			switch (eConnectionType)
			{
				case Database.ConnectionType.MicrosoftAccess:
					return "Now()";
				case Database.ConnectionType.SQLServer:
					return "GetDate()";
				case Database.ConnectionType.SQLServerCompactEdition:
					return "GetDate()";
				case Database.ConnectionType.MySQL:
					return "CURDATE()";
				case Database.ConnectionType.Pervasive:
					throw new NotImplementedException();
					break;
				default:
					throw new NotSupportedException();
					break;
			}
		}
	}
}
