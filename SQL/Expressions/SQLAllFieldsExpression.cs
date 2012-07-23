// ___________________________________________________
//
//  (c) Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	/// <summary>
	/// Represents '*' or 'T.*' when used to select all fields from a table or join.
	/// </summary>
	/// <remarks></remarks>
	public class SQLAllFieldsExpression : SQLExpression
	{
		private SQLSelectTable pobjTable;
			
		public SQLAllFieldsExpression()
		{
		}
			
		public SQLAllFieldsExpression(SQLSelectTable objTable)
		{
			this.Table = objTable;
		}
			
		public SQLSelectTable Table
		{
			get
			{
				return pobjTable;
			}
				
			set
			{
				pobjTable = value;
			}
		}
			
		internal override string SQL(Database.ConnectionType eConnectionType)
		{
			string strSQL = string.Empty;
				
			if (pobjTable != null)
				strSQL += Misc.SQLTablePrefix(pobjTable, eConnectionType) + ".";
				
			return strSQL + "*";
		}
	}
}
