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
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// This class can be used to determine whether a view exists. This class can be
	/// used will all databases. If after running the SQL statement the data set is not
	/// empty then the view exists.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public class SQLViewExists : SQLStatement
	{
		private string pstrViewName;
			
		public SQLViewExists(string strViewName)
		{
			if (string.IsNullOrEmpty(strViewName))
				throw new ArgumentNullException();
				
			pstrViewName = strViewName;
		}
		
		public string ViewName
		{
			get
			{
				return pstrViewName;
			}
		}

		public override string SQL
		{
			get
			{
				return base.Serializer.SerializeViewExists(this);
			}
		}
	}
}
