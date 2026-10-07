// _________________________________________________________________________
//
//  (c) Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//	    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// _________________________________________________________________________
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
