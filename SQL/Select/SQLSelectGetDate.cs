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
	public class SQLSelectGetDate : SQLSelectExpression
	{
		public SQLSelectGetDate() 
            : base(new SQLGetDateFunctionExpression())
		{
		}
	}
}
