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
	public class SQLSelectExpression : SQLStatement
	{
		private SQL.SQLExpression pobjExpression;
			
		public SQLSelectExpression(SQL.SQLExpression objExpression)
		{
			if (objExpression == null)
				throw new ArgumentNullException();
				
			pobjExpression = objExpression;
		}

		public SQL.SQLExpression Expression
		{
			get
			{
				return pobjExpression;
			}
		}
			
		public override string SQL
		{
			get
			{
				return base.Serializer.SerializeSelectExpression(this);
			}
		}
	}
}
