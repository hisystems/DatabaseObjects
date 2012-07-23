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
	public class SQLValueExpression : SQLExpression
	{
		private object pobjValue;
			
		/// <summary>
		/// Specifies NULL.
		/// </summary>
		public SQLValueExpression()
		{
		}
			
		public SQLValueExpression(object objValue)
		{
			pobjValue = objValue;
		}
			
		public object Value
		{
			get
			{
				return pobjValue;
			}
		}
			
		internal override string SQL(Database.ConnectionType eConnectionType)
		{
			return Misc.SQLConvertValue(pobjValue, eConnectionType);
		}
	}
}
