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
	public class MicrosoftSQLCompactEditionDatabase : Database
	{
		/// <summary>
		/// Connects to an SQL Compact Edition 3.5 database.
		/// Assumes the database has a blank password.
		/// </summary>
		/// <param name="strDatabaseFilePath">The full or relative path to the SQL compact edition database. i.e. Data\database.sdf or C:\Data\database.sdf</param>
		public MicrosoftSQLCompactEditionDatabase(string strDatabaseFilePath) 
            : this(strDatabaseFilePath, string.Empty)
		{
		}
		
		/// <summary>
		/// Connects to an SQL Compact Edition 3.5 database.
		/// </summary>
		/// <param name="strDatabaseFilePath">The full or relative path to the SQL compact edition database. i.e. Data\database.sdf or C:\Data\database.sdf</param>
		public MicrosoftSQLCompactEditionDatabase(string strDatabaseFilePath, string strPassword) 
            : base("Provider=Microsoft.SQLSERVER.CE.OLEDB.3.5;Data Source=" + strDatabaseFilePath + ";SSCE:Database Password='" + strPassword + "';OLE DB Services=-3;", ConnectionType.SQLServerCompactEdition)
		{
			if (string.IsNullOrEmpty(strDatabaseFilePath))
				throw new ArgumentNullException("Database File Path");
		}
	}
}
