// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
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
	/// This class can be used to determine whether a view exists. This class can be
	/// used will all databases. If after running the SQL statement the data set is not
	/// empty then the view exists.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public class SQLViewExists : SQLStatement
	{
		private string pstrViewName;
			
		public SQLViewExists(string strViewName)
		{
			if (string.IsNullOrEmpty(strViewName))
				throw new ArgumentNullException();
				
			pstrViewName = strViewName;
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
						throw new NotSupportedException();
					case Database.ConnectionType.MySQL:
						objSelect = new SQLSelect();
						objSelect.ConnectionType = this.ConnectionType;
						objSelect.Tables.Add("Tables").SchemaName = "INFORMATION_SCHEMA";
						objSelect.Where.Add("Table_Type", ComparisonOperator.EqualTo, "View");
						objSelect.Where.Add("TABLE_NAME", ComparisonOperator.Like, pstrViewName);
						strSQL = objSelect.SQL;
						break;
					case Database.ConnectionType.SQLServer:
						objSelect = new SQLSelect();
						objSelect.ConnectionType = this.ConnectionType;
						objSelect.Tables.Add("sysobjects");
						objSelect.Where.Add("Name", ComparisonOperator.EqualTo, pstrViewName);
						objSelect.Where.Add("XType", ComparisonOperator.EqualTo, "V"); //V = User defined view
						strSQL = objSelect.SQL;
						break;
					case Database.ConnectionType.Pervasive:
						throw new NotSupportedException();
					case Database.ConnectionType.HyperSQL:
						objSelect = new SQLSelect();
						objSelect.ConnectionType = this.ConnectionType;
						objSelect.Tables.Add("VIEWS").SchemaName = "INFORMATION_SCHEMA";
						objSelect.Where.Add("TABLE_SCHEMA", ComparisonOperator.EqualTo, "PUBLIC");
						objSelect.Where.Add("TABLE_NAME", ComparisonOperator.EqualTo, pstrViewName);
						strSQL = objSelect.SQL;
						break;
					case Database.ConnectionType.SQLServerCompactEdition:
						throw new NotSupportedException();
					default:
						throw new NotImplementedException(this.ConnectionType.ToString());
				}
					
				return strSQL;
			}
		}
	}
}
