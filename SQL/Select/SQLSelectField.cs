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
	public class SQLSelectField
	{
		private string pstrAlias;
		private SQLExpression pobjExpression;
			
		public SQLSelectField(SQLExpression objExpression)
		{
			if (objExpression == null)
				throw new ArgumentNullException();
				
			pobjExpression = objExpression;
		}
			
		internal SQLExpression Expression
		{
			get
			{
				return pobjExpression;
			}
		}
			
		public string Alias
		{
			get
			{
                return pstrAlias;
			}
				
			set
			{
				pstrAlias = value;
			}
		}
			
		internal string SQL(Database.ConnectionType eConnectionType)
		{
			string strSQL = pobjExpression.SQL(eConnectionType);
				
			if (!String.IsNullOrEmpty(this.Alias))
				strSQL += " AS " + Misc.SQLConvertIdentifierName(this.Alias, eConnectionType);
				
			return strSQL;
		}
	}
}
