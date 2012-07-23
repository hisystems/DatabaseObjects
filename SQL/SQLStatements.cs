// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Collections.Generic;

namespace DatabaseObjects.SQL
{
	public class SQLStatements : SQL.SQLStatement, IEnumerable<ISQLStatement>
	{
		private List<SQL.ISQLStatement> pobjStatements = new List<SQL.ISQLStatement>();
			
		public SQLStatements()
		{
		}
			
		public SQLStatements(SQL.ISQLStatement[] objStatements)
		{
			if (objStatements == null)
				throw new ArgumentNullException();
				
			pobjStatements.AddRange(objStatements);
		}
			
		public void Add(ISQLStatement objStatement)
		{
			pobjStatements.Add(objStatement);
		}
			
		public override string SQL
		{
			get
			{
				string strSQL = string.Empty;
					
				foreach (var objStatement in pobjStatements)
					strSQL += objStatement.SQL + "; ";
					
				return strSQL;
			}
		}
			
		IEnumerator<ISQLStatement> IEnumerable<ISQLStatement>.GetEnumerator()
	    {
			return pobjStatements.GetEnumerator();
	    }

	    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	    {
			return pobjStatements.GetEnumerator();
	    }
	}
}