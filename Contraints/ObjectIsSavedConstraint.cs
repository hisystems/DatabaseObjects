// ___________________________________________________
//
//  © Hi-Integrity Systems 2013. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.Constraints
{
	public class ObjectIsSavedConstraint : IConstraint<IDatabaseObject>
	{
		public bool ValueSatisfiesConstraint(IDatabaseObject value)
		{
			return value.IsSaved;
		}
		
		public override string ToString()
		{
			return "object must be saved";
		}
	}
}
