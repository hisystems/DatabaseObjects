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
	/// A constraint / piece of business logic / rule that is shared by the user interface and business logic layers.
	/// </summary>
	public interface IConstraint<T>
	{
		/// <summary>
		/// Indicates that the constraint passes, and no user error message should be displayed.
		/// </summary>
		bool ValueSatisfiesConstraint(T value);
	}
}
