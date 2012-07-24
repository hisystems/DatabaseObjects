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
	public class SQLSetTransactionIsolationLevel : SQL.SQLStatement
	{
		private System.Data.IsolationLevel peIsolationLevel;
			
		public SQLSetTransactionIsolationLevel(System.Data.IsolationLevel eIsolationLevel)
		{
			peIsolationLevel = eIsolationLevel;
		}

		public System.Data.IsolationLevel Value
		{
			get
			{
				return peIsolationLevel;
			}
		}
			
		public override string SQL
		{
			get
			{
				return base.Serializer.SerializeSetTransactionIsolationLevel(this);
			}
		}
	}
}
