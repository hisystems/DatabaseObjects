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
	public class SQLDropView : SQLStatement
	{
		private string pstrName;
			
		public SQLDropView(string strViewName)
		{
			if (string.IsNullOrEmpty(strViewName))
				throw new ArgumentNullException();
				
			pstrName = strViewName;
		}

		public string ViewName
		{
			get
			{
				return pstrName;
			}
		}
			
		public override string SQL
		{
			get
			{
				return base.Serializer.SerializeDropView(this);
			}
		}
	}
}
