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
	public class SQLBeginTransaction : SQL.SQLStatement
	{
		public override string SQL
		{
			get
			{
				return base.Serializer.SerializeBeingTransaction(this);
			}
		}
	}
}
