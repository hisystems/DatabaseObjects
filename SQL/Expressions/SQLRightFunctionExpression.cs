// ___________________________________________________
//
//  (c) Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au<http://www.hisystems.com.au> - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public class SQLRightFunctionExpression : SQLFunctionExpression
	{
		public SQLRightFunctionExpression(string strFieldName, int intLength) 
            : this(new SQLFieldExpression(strFieldName), intLength)
		{
		}
			
		public SQLRightFunctionExpression(SQLExpression objExpression, int intLength) 
            : base(objExpression, new SQLValueExpression(intLength))
		{
			if (intLength < 0)
				throw new ArgumentException("Length: " + intLength.ToString());
		}

		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeRightFunctionExpression(this);
		}
	}
}
