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
	public class MicrosoftAccessDatabase : Database
	{
		/// <summary>
		/// Connects to a Microsoft Access mdb database file.
		/// Connects to a database with a blank password.
		/// </summary>
		/// <param name="strDatabaseFilePath">
		/// The full or relative path to the mdb file - including the mdb file name and extension.
		/// i.e. Data\database.mdb or C:\Data\database.mdb
		/// </param>
		/// <remarks></remarks>
		public MicrosoftAccessDatabase(string strDatabaseFilePath) 
            : this(strDatabaseFilePath, string.Empty)
		{
		}
		
		/// <summary>
		/// Connects to a Microsoft Access mdb database file.
		/// </summary>
		/// <param name="strDatabaseFilePath">
		/// The full or relative path to the mdb file - including the mdb file name and extension.
		/// i.e. Data\database.mdb or C:\Data\database.mdb
		/// </param>
		/// <remarks></remarks>
		public MicrosoftAccessDatabase(string strDatabaseFilePath, string strDatabasePassword)
            : base("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strDatabaseFilePath + ";Jet OLEDB:Database Password=" + strDatabasePassword + ";", ConnectionType.MicrosoftAccess)
		{
			if (string.IsNullOrEmpty(strDatabaseFilePath))
				throw new ArgumentNullException("Database File Path");
		}
	}
}
