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
	public class SQLFieldExpression : SQLExpression
	{
		private string pstrFieldName;
		private SQLSelectTable pobjTable;
			
		public SQLFieldExpression()
		{
		}
			
		public SQLFieldExpression(string strFieldName)
		{
			this.Name = strFieldName;
		}
			
		public SQLFieldExpression(SQLSelectTable objTable, string strFieldName)
		{
			this.Name = strFieldName;
			this.Table = objTable;
		}
			
		public string Name
		{
			get
			{
				return pstrFieldName;
			}
				
			set
			{
				pstrFieldName = value;
			}
		}
			
		public SQLSelectTable Table
		{
			get
			{
				return pobjTable;
			}
				
			set
			{
				pobjTable = value;
			}
		}
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeFieldExpression(this);
		}
	}
}
