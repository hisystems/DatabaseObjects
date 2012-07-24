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
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeLengthFunctionExpression(this);
		}
	}
}
