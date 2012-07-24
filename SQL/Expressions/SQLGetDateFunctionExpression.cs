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
	public class SQLGetDateFunctionExpression : SQLExpression
	{
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeGetDateFunctionExpression(this);
		}
	}
}
