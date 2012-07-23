// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects
{
	public class MySQLDatabase : Database
	{
		//Flags from http://dev.mysql.com/doc/refman/5.0/en/connector-odbc-configuration-connection-parameters.html
		private const int FLAG_MULTI_STATEMENTS = 67108864;
		private const int FLAG_FOUND_ROWS = 2;
		private const int Options = FLAG_MULTI_STATEMENTS | FLAG_FOUND_ROWS;
		
		/// <summary>
		/// Connects to a MySQL database using the ODBC driver.
		/// </summary>
		/// <remarks></remarks>
		public MySQLDatabase(string strDataSource, string strDatabaseName, string strUserName, string strPassword) 
            : base("Driver={MySQL ODBC 5.1 Driver}; Server=" + strDataSource + "; Database=" + strDatabaseName + "; UID=" + strUserName + "; PWD=" + strPassword + ";Option=" + Options, ConnectionType.MySQL)
		{
			if (string.IsNullOrEmpty(strDataSource))
				throw new ArgumentNullException("DataSource");
			else if (string.IsNullOrEmpty(strDatabaseName))
				throw new ArgumentNullException("DatabaseName");
			else if (string.IsNullOrEmpty(strUserName))
				throw new ArgumentNullException("UserName");
		}
	}
}
