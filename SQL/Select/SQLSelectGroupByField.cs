// ___________________________________________________
//
//  (c) Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au<http://www.hisystems.com.au> - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public class SQLSelectGroupByField
	{
		private SQLExpression pobjExpression;
			
		internal SQLSelectGroupByField(SQLExpression objExpression)
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
			
		internal string SQL(Database.ConnectionType eConnectionType)
		{
			return pobjExpression.SQL(eConnectionType);
		}
	}
}
