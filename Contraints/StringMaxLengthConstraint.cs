// ___________________________________________________
//
//  (c) Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.Constraints
{
	public class StringMaxLengthConstraint : IConstraint<string>
	{
		private int pintMaxLength;
			
		public StringMaxLengthConstraint(int intMaxLength)
		{
			if (intMaxLength < 1)
				throw new ArgumentException("Maxlength cannot be less than 1 " + intMaxLength.ToString());
				
			pintMaxLength = intMaxLength;
		}
			
		public int MaxLength
		{
			get
			{
				return pintMaxLength;
			}
		}
			
		public bool ValueSatisfiesConstraint(string value)
		{
			return (value != null && value.Length <= pintMaxLength) || value == null;
		}
			
		public override string ToString()
		{
			return "string length is less than or equal to " + pintMaxLength.ToString() + " characters";
		}
	}
}
