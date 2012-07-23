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
	public class ObjectIsSetConstraint : IConstraint<object>
	{
		public bool ValueSatisfiesConstraint(object value)
		{
			return value != null;
		}
			
		public override string ToString()
		{
			return "object must not be null";
		}
	}
}
