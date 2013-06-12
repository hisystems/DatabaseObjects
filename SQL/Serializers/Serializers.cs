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
	internal static class Serializers
	{
		public readonly static Dictionary<Database.ConnectionType, Serializer> Items = new Dictionary<Database.ConnectionType, Serializer>();

		static Serializers()
		{
#if MONO_TOUCH
			Items.Add(Database.ConnectionType.SQLite, new SQLiteSerializer());
#else
			Items.Add(Database.ConnectionType.SQLServer, new MicrosoftSqlServerSerializer());
			Items.Add(Database.ConnectionType.SQLServerCompactEdition, new MicrosoftSqlServerCompactEditionSerializer());
			Items.Add(Database.ConnectionType.MicrosoftAccess, new MicrosoftAccessSerializer());
			Items.Add(Database.ConnectionType.MySQL, new MySqlSerializer());
			Items.Add(Database.ConnectionType.HyperSQL, new HyperSqlSerializer());
			Items.Add(Database.ConnectionType.Pervasive, new PervasiveSerializer());
			Items.Add(Database.ConnectionType.SQLite, new SQLiteSerializer());
#endif
		}
	}
}
