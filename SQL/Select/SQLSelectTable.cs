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
	public class SQLSelectTable : SQLSelectTableBase
	{
		private string pstrDatabaseName;
		private string pstrSchemaName;
		private string pstrName;
			
		public SQLSelectTable()
		{
		}
			
		public SQLSelectTable(string strName)
		{
			this.Name = strName;
		}
			
		public SQLSelectTable(string strName, string strAlias)
            : this(strName)
		{
			base.Alias = strAlias;
		}
			
		public string DatabaseName
		{
			get
			{
				return pstrDatabaseName;
			}
				
			set
			{
				pstrDatabaseName = value;
			}
		}
			
		public string SchemaName
		{
			get
			{
				return pstrSchemaName;
			}
				
			set
			{
				pstrSchemaName = value;
			}
		}
			
		public string Name
		{
			get
			{
				return pstrName;
			}
				
			set
			{
				if (String.IsNullOrEmpty(value))
					throw new ArgumentNullException();
				
				pstrName = value;
			}
		}
			
		protected override string Source(Database.ConnectionType eConnectionType)
		{
			if (String.IsNullOrEmpty(this.Name))
				throw new Exceptions.DatabaseObjectsException("Table has not been set.");
				
			string strSQL = string.Empty;
				
			if (!String.IsNullOrEmpty(pstrDatabaseName))
				strSQL += Misc.SQLConvertIdentifierName(pstrDatabaseName, eConnectionType) + ".";
				
			if (!String.IsNullOrEmpty(pstrSchemaName))
				strSQL += Misc.SQLConvertIdentifierName(pstrSchemaName, eConnectionType) + ".";
				
			return strSQL + Misc.SQLConvertIdentifierName(this.Name, eConnectionType);
		}
	}
}
