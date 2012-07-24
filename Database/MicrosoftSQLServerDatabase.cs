// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Collections.Generic;

namespace DatabaseObjects
{
	public class MicrosoftSQLServerDatabase : Database
	{
		private string pstrDataSource;
		private string pstrDatabase;
		
		/// <summary>
		/// Connects to a Microsoft SQL Server database.
		/// Uses SSPI security to connect to the database.
		/// </summary>
		/// <remarks></remarks>
		public MicrosoftSQLServerDatabase(string strDataSource, string strDatabaseName)
            : base(ConnectionStringRoot(strDataSource, strDatabaseName) + "Integrated Security=SSPI;", ConnectionType.SQLServer)
		{
			EnsureDatabaseDetailsValid(strDataSource, strDatabaseName);
			
			pstrDatabase = strDatabaseName;
			pstrDataSource = strDataSource;
		}
		
		/// <summary>
		/// Connects to a Microsoft SQL Server database.
		/// </summary>
		/// <remarks></remarks>
		public MicrosoftSQLServerDatabase(string strDataSource, string strDatabaseName, string strUserName, string strPassword)
            : base(ConnectionStringRoot(strDataSource, strDatabaseName) + "UID=" + strUserName + ";pwd=" + strPassword + ";", ConnectionType.SQLServer)
		{
			EnsureDatabaseDetailsValid(strDataSource, strDatabaseName);
			
			if (string.IsNullOrEmpty(strUserName))
				throw new ArgumentNullException("UserName");
			
			pstrDatabase = strDatabaseName;
			pstrDataSource = strDataSource;
		}
		
		public string Source
		{
			get
			{
				return pstrDataSource;
			}
		}
		
		public string Name
		{
			get
			{
				return pstrDatabase;
			}
		}
		
		/// <summary>
		/// If parsing fails then the FormatException message will contain useful information as to the cause.
		/// </summary>
		/// <exception cref="FormatException">If the connection string is in an invalid format.</exception>
		public static MicrosoftSQLServerDatabase Parse(string strConnectionString)
		{
			var objDictionary = GetDictionaryFromConnectionString(strConnectionString);
			
			if (objDictionary.ContainsKey("integrated security"))
				return new MicrosoftSQLServerDatabase(GetDataSource(objDictionary), GetDatabase(objDictionary));
			else
				return new MicrosoftSQLServerDatabase(GetDataSource(objDictionary), GetDatabase(objDictionary), GetUserID(objDictionary), GetPassword(objDictionary));
		}
		
		private static string GetDataSource(IDictionary<string, string> objConnectionDictionary)
		{
			if (objConnectionDictionary.ContainsKey("data source"))
				return objConnectionDictionary["data source"];
			else
				throw new FormatException("Could not find 'Data Source=' definition");
		}
		
		private static string GetDatabase(IDictionary<string, string> objConnectionDictionary)
		{
			if (objConnectionDictionary.ContainsKey("database"))
				return objConnectionDictionary["database"];
			else if (objConnectionDictionary.ContainsKey("initial catalog"))
				return objConnectionDictionary["initial catalog"];
			else
				throw new FormatException("Could not find 'Database=' or 'Initial Catalog=' definition");
		}
		
		private static string GetUserID(IDictionary<string, string> objConnectionDictionary)
		{
			if (objConnectionDictionary.ContainsKey("uid"))
				return objConnectionDictionary["uid"];
			else if (objConnectionDictionary.ContainsKey("user id"))
				return objConnectionDictionary["user id"];
			else
				throw new FormatException("Could not find 'UID=' or 'User ID=' definition");
		}
		
		private static string GetPassword(IDictionary<string, string> objConnectionDictionary)
		{
			if (objConnectionDictionary.ContainsKey("pwd"))
				return objConnectionDictionary["pwd"];
			else if (objConnectionDictionary.ContainsKey("password"))
				return objConnectionDictionary["password"];
			else
				throw new FormatException("Could not find 'Password=' or 'Pwd=' definition");
		}
		
		private void EnsureDatabaseDetailsValid(string strDataSource, string strDatabaseName)
		{
			if (string.IsNullOrEmpty(strDataSource))
				throw new ArgumentNullException("DataSource");
			else if (string.IsNullOrEmpty(strDatabaseName))
				throw new ArgumentNullException("DatabaseName");
		}
		
		private static string ConnectionStringRoot(string strDataSource, string strDatabaseName)
		{
			return "Provider=SQLOLEDB;Data Source=" + strDataSource + ";" + "Database=" + strDatabaseName + ";";
		}
	}
}
