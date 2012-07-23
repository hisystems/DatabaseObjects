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
	/// This constraint will ensure that the date is either today or a future
	/// when the associated object is not saved.
	/// If the object is not new, then the date constraint does not apply.
	/// </summary>
	/// <remarks></remarks>
	public class DateIsTodayOrFutureForNewObjectConstraint : IConstraint<DateTime>
	{
		private IDatabaseObject associatedObject;
			
		public DateIsTodayOrFutureForNewObjectConstraint(IDatabaseObject associatedObject)
		{
			if (associatedObject == null)
				throw new ArgumentNullException();
				
			this.associatedObject = associatedObject;
		}
			
		public bool ValueSatisfiesConstraint(DateTime value)
		{
			return associatedObject.IsSaved || value >= DateTime.Today;
		}
			
		public override string ToString()
		{
			return "date must be today or a future date for a new object";
		}
	}
}