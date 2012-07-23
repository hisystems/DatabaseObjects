// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public class SQLCreateView : SQLStatement
	{
		private string pstrName;
		private SQLSelect pobjSelect;
			
		public SQLCreateView()
		{
		}
			
		public SQLCreateView(string strViewName, SQLSelect objSelectStatement)
		{
			this.Name = strViewName;
			this.Select = objSelectStatement;
		}
			
		public string Name
		{
			get
			{
				return pstrName;
			}
				
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException();
					
				pstrName = value;
			}
		}
			
		public SQLSelect Select
		{
			get
			{
				return pobjSelect;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjSelect = value;
			}
		}
			
		public override string SQL
		{
			get
			{
				if (string.IsNullOrEmpty(this.Name))
					throw new Exceptions.DatabaseObjectsException("View name has not been set");
				else if (pobjSelect == null)
					throw new Exceptions.DatabaseObjectsException("Select statement has not been set");
					
				return "CREATE VIEW " + Misc.SQLConvertIdentifierName(this.Name, this.ConnectionType) + " AS " + pobjSelect.SQL;
			}
		}
	}
}
	
