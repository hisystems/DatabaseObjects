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
	public class DateIsSetConstraint : IConstraint<DateTime>
	{
		private DateTime unsetDateValue;
			
		/// <summary>
		/// The unset / default value for the date is Date.MinValue.
		/// So if the date is not Date.MinValue then the constraint passes.
		/// </summary>
		public DateIsSetConstraint()
		{
			this.unsetDateValue = DateTime.MinValue;
		}
			
		public DateIsSetConstraint(DateTime unsetDateValue)
		{
			this.unsetDateValue = unsetDateValue;
		}
			
		public bool ValueSatisfiesConstraint(DateTime value)
		{
			return value != unsetDateValue;
		}
			
		public override string ToString()
		{
			return "date must not be " + unsetDateValue.ToString();
		}
	}
}
