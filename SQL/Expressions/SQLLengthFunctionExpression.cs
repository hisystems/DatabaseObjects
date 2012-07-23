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
	/// <summary>
	/// SQL function that returns the length of a string.
	/// </summary>
	public class SQLLengthFunctionExpression : SQLFunctionExpression
	{
		public SQLLengthFunctionExpression(SQLExpression objExpression) 
            : base(objExpression)
		{
		}
			
		protected override string FunctionName(Database.ConnectionType eConnectionType)
		{
			switch (eConnectionType)
			{
				case Database.ConnectionType.MySQL:
					return "LENGTH";
				case Database.ConnectionType.SQLServer:
				case Database.ConnectionType.MicrosoftAccess:
					return "LEN";
				default:
					throw new NotImplementedException(eConnectionType.ToString());
					break;
			}
				
			return base.FunctionName(eConnectionType);
		}
	}
}
