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
	public class SQLAlterTable : SQLStatement
	{
		private string pstrName;
		private SQLTableFields pobjFields = new SQLTableFields();
			
		public SQLAlterTable()
		{
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
			
		public SQLTableFields Fields
		{
			get
			{
				return pobjFields;
			}
		}
			
		public override string SQL
		{
			get
			{
				return base.Serializer.SerializeAlterTable(this);
			}
		}
	}
}
