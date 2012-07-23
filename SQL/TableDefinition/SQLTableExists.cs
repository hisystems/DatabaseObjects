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
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// This class can be used to determine whether a table exists. This class can be
	/// used will all databases. If after running the SQL statement the data set is not
	/// empty then the table exists.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public class SQLTableExists : SQLStatement
	{
		private string pstrName;
			
		public SQLTableExists()
		{
		}
			
		public SQLTableExists(string strTableName)
		{
			this.Name = strTableName;
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
			
		public override string SQL
		{
			get
			{
				string strSQL = string.Empty;
				SQLSelect objSelect;
					
				switch (this.ConnectionType)
				{
					case Database.ConnectionType.MicrosoftAccess:
						objSelect = new SQLSelect();
						objSelect.ConnectionType = this.ConnectionType;
						objSelect.Tables.Add("msysobjects");
						objSelect.Where.Add("Name", ComparisonOperator.EqualTo, this.Name);
						objSelect.Where.Add("Type", ComparisonOperator.EqualTo, 1);
						strSQL = objSelect.SQL;
						break;
					case Database.ConnectionType.MySQL:
						strSQL = "SHOW TABLES LIKE " + Misc.SQLConvertValue(this.Name, this.ConnectionType);
						break;
					case Database.ConnectionType.SQLServer:
						objSelect = new SQLSelect();
						objSelect.ConnectionType = this.ConnectionType;
						objSelect.Tables.Add("sysobjects");
						objSelect.Where.Add("Name", ComparisonOperator.EqualTo, this.Name);
						objSelect.Where.Add("XType", ComparisonOperator.EqualTo, "U"); //U = User defined table
						strSQL = objSelect.SQL;
						break;
					case Database.ConnectionType.Pervasive:
						objSelect = new SQLSelect();
						objSelect.ConnectionType = this.ConnectionType;
						objSelect.Tables.Add("X$FILE");
						objSelect.Where.Add("Xf$name", ComparisonOperator.EqualTo, this.Name);
						strSQL = objSelect.SQL;
						break;
					case Database.ConnectionType.HyperSQL:
						objSelect = new SQLSelect();
						objSelect.ConnectionType = this.ConnectionType;
						objSelect.Tables.Add("SYSTEM_TABLES").SchemaName = "INFORMATION_SCHEMA";
						objSelect.Where.Add("TABLE_SCHEM", ComparisonOperator.EqualTo, "PUBLIC");
						objSelect.Where.Add("TABLE_NAME", ComparisonOperator.EqualTo, this.Name);
						strSQL = objSelect.SQL;
						break;
					case Database.ConnectionType.SQLServerCompactEdition:
						throw new NotSupportedException("Use SELECT COUNT(*) FROM table to determine if a record exists");
						break;
					default:
						throw new NotImplementedException(this.ConnectionType.ToString());
						break;
				}
					
				return strSQL;
			}
		}
	}
}
