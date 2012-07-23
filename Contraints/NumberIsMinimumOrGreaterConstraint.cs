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
	public class NumberIsMinimumOrGreaterConstraint : IConstraint<decimal>
	{
		private decimal minimumValue;
			
		public NumberIsMinimumOrGreaterConstraint(decimal minimumValue)
		{
			this.minimumValue = minimumValue;
		}
			
		public bool ValueSatisfiesConstraint(decimal value)
		{
			return value >= minimumValue;
		}
			
		public override string ToString()
		{
			return "number must be " + minimumValue.ToString() + " or greater";
		}
	}
}