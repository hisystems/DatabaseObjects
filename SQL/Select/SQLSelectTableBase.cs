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
	public abstract class SQLSelectTableBase
	{
        internal abstract string Source(Serializers.Serializer serializer);

		private string pstrAlias = string.Empty;
			
		public SQLSelectTableBase()
		{
		}
			
		public virtual string Alias
		{
			get
			{
				return pstrAlias;
			}
				
			set
			{
				pstrAlias = value;
			}
		}
	}
}
