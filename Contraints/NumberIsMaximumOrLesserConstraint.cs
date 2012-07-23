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
	public class NumberIsMaximumOrLesserConstraint : IConstraint<decimal>
	{
		private decimal maximumValue;
			
		public NumberIsMaximumOrLesserConstraint(decimal maximumValue)
		{
			this.maximumValue = maximumValue;
		}
			
		public bool ValueSatisfiesConstraint(decimal value)
		{
			return value <= maximumValue;
		}
			
		public override string ToString()
		{
			return "number must be " + maximumValue.ToString() + " or lower";
		}
	}
}
