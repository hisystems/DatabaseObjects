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
	public class SQLDropIndex : SQLStatement
	{
		private string pstrName;
		private string pstrTableName;
			
		public SQLDropIndex()
		{
		}
			
		public SQLDropIndex(string strIndexName, string strTableName)
		{
			this.Name = strIndexName;
			this.TableName = strTableName;
		}
			
		public string Name
		{
			get
			{
				return pstrName;
			}
				
			set
			{
                pstrName = value;
			}
		}
			
		public string TableName
		{
			get
			{
				return pstrTableName;
			}
				
			set
			{
                pstrTableName = value;
			}
		}
			
		public override string SQL
		{
			get
			{
				if (String.IsNullOrEmpty(this.Name))
					throw new Exceptions.DatabaseObjectsException("IndexName has not been set.");
				else if (String.IsNullOrEmpty(this.TableName))
					throw new Exceptions.DatabaseObjectsException("TableName has not been set.");
					
				switch (this.ConnectionType)
				{
					case Database.ConnectionType.SQLServer:
					case Database.ConnectionType.SQLServerCompactEdition:
						return "DROP INDEX " + Misc.SQLConvertIdentifierName(this.TableName, this.ConnectionType) + "." + Misc.SQLConvertIdentifierName(this.Name, this.ConnectionType);
					case Database.ConnectionType.MicrosoftAccess:
					case Database.ConnectionType.MySQL:
					case Database.ConnectionType.Pervasive:
						return "DROP INDEX " + Misc.SQLConvertIdentifierName(this.Name, this.ConnectionType) + " ON " + Misc.SQLConvertIdentifierName(this.TableName, this.ConnectionType);
					default:
						throw new NotSupportedException();
						break;
				}
			}
		}
	}
}
