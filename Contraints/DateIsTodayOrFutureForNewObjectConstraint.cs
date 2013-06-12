// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
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