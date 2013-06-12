// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
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

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public abstract class SQLStatement : ISQLStatement
	{
		private static Database.ConnectionType peDefaultConnectionType;
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// This is used as the default connection type when manually instantiating an
		/// SQLSelect, SQLDelete, SQLUpdate or SQLInsert command (which all inherit from
		/// SQLStatement) and is set by the last Database.Connect function's ConnectionType
		/// argument. However, the Database class does not rely on this property as the
		/// ConnectionType property is set before any SQL statements are executed. This
		/// allows different Database instances to use different databases.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static Database.ConnectionType DefaultConnectionType
		{
			get
			{
				return peDefaultConnectionType;
			}
				
			set
			{
				peDefaultConnectionType = value;
			}
		}

        public abstract string SQL { get; }
			
		private Database.ConnectionType peConnectionType = peDefaultConnectionType; 

        public SQLStatement()
        {
        }

		public Database.ConnectionType ConnectionType
		{
			get
			{
				return peConnectionType;
			}
				
			set
			{
				peConnectionType = value;
			}
		}

		/// <summary>
		/// Returns the serializer to be used based on the connection type.
		/// </summary>
		internal Serializers.Serializer Serializer
		{
			get
			{
				return Serializers.Serializers.Items[peConnectionType];
			}
		}
			
		public override string ToString()
		{
			return this.SQL;
		}
	}
}
