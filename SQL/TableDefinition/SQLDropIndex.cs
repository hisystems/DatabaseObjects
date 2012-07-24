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
	public class SQLDropIndex : SQLStatement
	{
		private string pstrName;
		private string pstrTableName;
			
		public SQLDropIndex()
		{
		}
			
		public SQLDropIndex(string strIndexName, string strTableName)
		{
			this.Name = strIndexName;
			this.TableName = strTableName;
		}
			
		public string Name
		{
			get
			{
				return pstrName;
			}
				
			set
			{
                pstrName = value;
			}
		}
			
		public string TableName
		{
			get
			{
				return pstrTableName;
			}
				
			set
			{
                pstrTableName = value;
			}
		}
			
		public override string SQL
		{
			get
			{
				return base.Serializer.SerializeDropIndex(this);
			}
		}
	}
}
