// ___________________________________________________
//
//  (c) Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.Constraints
{
	public class Constraint<T> : IConstraint<T>
	{
		private Func<T, bool> pobjPredicate;
			
		public Constraint(Func<T, bool> objPredicate)
		{
			if (objPredicate == null)
				throw new ArgumentNullException();
				
			pobjPredicate = objPredicate;
		}
			
		public bool ValueSatisfiesConstraint(T value)
		{
			return pobjPredicate.Invoke(value);
		}
	}
}
