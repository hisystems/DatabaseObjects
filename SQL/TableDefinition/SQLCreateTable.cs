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
	public class SQLCreateTable : SQLStatement
	{
		private string pstrName;
		private SQLTableFields pobjFields;
			
		public SQLCreateTable()
		{
			pobjFields = new SQLTableFields();
			pobjFields.AlterMode = SQLTableFields.AlterModeType.Add; //set that fields can only be added
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
				return base.Serializer.SerializeCreateTable(this);
			}
		}
	}
}
