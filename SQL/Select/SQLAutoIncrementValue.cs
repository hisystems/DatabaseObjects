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
	public class SQLAutoIncrementValue : SQLStatement
	{
		private string pstrReturnFieldName = "AutoIncrementValue";
			
		public SQLAutoIncrementValue()
		{
		}
			
		public string ReturnFieldName
		{
			get
			{
				return pstrReturnFieldName;
			}
				
			set
			{
				if (String.IsNullOrEmpty(value))
					throw new ArgumentNullException();
					
				pstrReturnFieldName = value;
			}
		}
			
		public override string SQL
		{
			get
			{
				switch (this.ConnectionType)
				{
					case Database.ConnectionType.SQLServer:
						return "SELECT SCOPE_IDENTITY() AS " + Misc.SQLConvertIdentifierName(this.ReturnFieldName, this.ConnectionType);
					case Database.ConnectionType.MicrosoftAccess:
					case Database.ConnectionType.Pervasive:
					case Database.ConnectionType.SQLServerCompactEdition:
						return "SELECT @@IDENTITY AS " + Misc.SQLConvertIdentifierName(this.ReturnFieldName, this.ConnectionType);
					case Database.ConnectionType.MySQL:
						//The @@IDENTITY function is supported by MySQL from version 3.23.25
						//but I've put the original function here just in case
						return "SELECT LAST_INSERT_ID() AS " + Misc.SQLConvertIdentifierName(this.ReturnFieldName, this.ConnectionType);
					case Database.ConnectionType.HyperSQL:
						return "SELECT IDENTITY() AS " + Misc.SQLConvertIdentifierName(this.ReturnFieldName, this.ConnectionType);
					default:
						throw new NotImplementedException(this.ConnectionType.ToString());
						break;
				}
			}
		}
	}
}
