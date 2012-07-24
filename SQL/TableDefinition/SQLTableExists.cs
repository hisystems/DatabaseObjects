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
				return base.Serializer.SerializeTableExists(this);
			}
		}
	}
}
