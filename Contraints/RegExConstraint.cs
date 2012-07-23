// ___________________________________________________
//
//  (c) Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace DatabaseObjects.Constraints
{
	/// <summary>
	/// Ensures that the associated binding value is a match for the regular expression.
	/// Utilises the System.Text.RegularExpressions.RegEx.IsMatch() function to determine whether the value matches.
	/// </summary>
	public class RegExConstraint : IConstraint<string>
	{
		private Regex pobjRegularExpression;
			
		public RegExConstraint(Regex objRegularExpression)
		{
			if (objRegularExpression == null)
				throw new ArgumentNullException();
				
			pobjRegularExpression = objRegularExpression;
		}
			
		public RegExConstraint(string strRegularExpression) 
            : this(new Regex(strRegularExpression))
		{
		}
			
		public bool ValueSatisfiesConstraint(string value)
		{
			return pobjRegularExpression.IsMatch(value);
		}
			
		public override string ToString()
		{
			return pobjRegularExpression.ToString();
		}
	}
}
