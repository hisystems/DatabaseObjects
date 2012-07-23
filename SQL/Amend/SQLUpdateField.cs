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
	public class SQLUpdateField : SQLFieldValue
	{
		private SQLExpression pobjSourceValue;
			
		public SQLUpdateField(string strDestinationFieldName, string strSourceFieldName) 
            : this(strDestinationFieldName, new SQLFieldExpression(strSourceFieldName))
		{
		}
			
		public SQLUpdateField(string strDestinationFieldName, SQLExpression objSourceExpression)
		{
			if (String.IsNullOrEmpty(strDestinationFieldName))
				throw new ArgumentNullException("DestinationFieldName");
			else if (objSourceExpression == null)
				throw new ArgumentNullException("SourceExpression");
				
			base.Name = strDestinationFieldName;
			pobjSourceValue = objSourceExpression;
		}
			
		public new SQLExpression Value
		{
			get
			{
				return pobjSourceValue;
			}
		}
	}
}
