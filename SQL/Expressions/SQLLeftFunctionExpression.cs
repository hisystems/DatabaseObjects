// ___________________________________________________
//
//  (c) Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public class SQLLeftFunctionExpression : SQLFunctionExpression
	{
		private SQLExpression pobjExpression;
		private int pintLength;
			
		public SQLLeftFunctionExpression(string strFieldName, int intLength) 
            : this(new SQLFieldExpression(strFieldName), intLength)
		{
		}
			
		public SQLLeftFunctionExpression(SQLExpression objExpression, int intLength) 
            : base(objExpression, new SQLValueExpression(intLength))
		{
			if (intLength < 0)
				throw new ArgumentException("Length: " + intLength.ToString());
		}

		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeLeftFunctionExpression(this);
		}
	}
}
