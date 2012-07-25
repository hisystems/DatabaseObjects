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
	internal static class Serializers
	{
		public readonly static Dictionary<Database.ConnectionType, Serializer> Items = new Dictionary<Database.ConnectionType, Serializer>();

		static Serializers()
		{
			Items.Add(Database.ConnectionType.SQLServer, new MicrosoftSqlServerSerializer());
			Items.Add(Database.ConnectionType.SQLServerCompactEdition, new MicrosoftSqlServerCompactEditionSerializer());
			Items.Add(Database.ConnectionType.MicrosoftAccess, new MicrosoftAccessSerializer());
			Items.Add(Database.ConnectionType.MySQL, new MySqlSerializer());
			Items.Add(Database.ConnectionType.HyperSQL, new HyperSqlSerializer());
			Items.Add(Database.ConnectionType.Pervasive, new PervasiveSerializer());
			Items.Add(Database.ConnectionType.SQLite, new SQLiteSerializer());
		}
	}
}
