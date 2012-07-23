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
        protected abstract string Source(Database.ConnectionType eConnectionType);

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
			
		internal string SQL(Database.ConnectionType eConnectionType)
		{
			string strSQL = this.Source(eConnectionType);
				
			if (!String.IsNullOrEmpty(this.Alias))
				strSQL += " " + Misc.SQLConvertIdentifierName(this.Alias, eConnectionType);
				
			return strSQL;
		}
	}
}
