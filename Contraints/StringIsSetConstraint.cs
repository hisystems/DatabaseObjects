// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.Constraints
{
	public class StringIsSetConstraint : IConstraint<string>
	{
		public bool ValueSatisfiesConstraint(string value)
		{
			return !string.IsNullOrEmpty(value);
		}
			
		public override string ToString()
		{
			return "string must not be empty";
		}
	}
}