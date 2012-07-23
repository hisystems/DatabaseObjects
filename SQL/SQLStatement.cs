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
			
		public override string ToString()
		{
			return this.SQL;
		}
	}
}
