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
	public class SQLSelectTableFromSelect : SQLSelectTableBase
	{
		private SQLSelect pobjSelect;
			
		public SQLSelectTableFromSelect(SQLSelect objSelect, string strAlias)
		{
			if (String.IsNullOrEmpty(strAlias))
				throw new ArgumentNullException();
			else if (objSelect == null)
				throw new ArgumentNullException();
				
			pobjSelect = objSelect;
			base.Alias = strAlias;
		}

		public SQLSelect Select
		{
			get
			{
				return pobjSelect;
			}
		}
					
		internal override string Source(Serializers.Serializer serializer)
		{
			return serializer.SerializeSelectTableFromSelect(this);
		}
	}
}
