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
	public class SQLDropTable : SQLStatement
	{
		private string pstrName;
			
		public SQLDropTable()
		{
		}
			
		public SQLDropTable(string strTableName)
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
				if (String.IsNullOrEmpty(this.Name))
					throw new Exceptions.DatabaseObjectsException("TableName has not been set.");
					
				return "DROP TABLE " + Misc.SQLConvertIdentifierName(this.Name, this.ConnectionType);
			}
		}
	}
}
