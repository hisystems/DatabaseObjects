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
	/// <summary>
	/// Indicates that the number must fall in the range (inclusive)
	/// of the numbers specified.
	/// </summary>
	public class NumberIsInRangeConstraint : IConstraint<decimal>
	{
		private decimal startingNumber;
		private decimal endingNumber;
			
		public NumberIsInRangeConstraint(decimal startingNumber, decimal endingNumber)
		{
			this.startingNumber = startingNumber;
			this.endingNumber = endingNumber;
		}
			
		public bool ValueSatisfiesConstraint(decimal value)
		{
			return value >= startingNumber && value <= endingNumber;
		}
			
		public override string ToString()
		{
			return "number must be in range " + startingNumber.ToString() + " to " + endingNumber.ToString() + " (inclusive)";
		}
	}
}