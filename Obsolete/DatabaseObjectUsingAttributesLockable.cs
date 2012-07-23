// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

/// <summary>
/// This class is deprecated. Use DatabaseObjects.DatabaseObjectLockable instead, it provides the same functionality.
/// </summary>
namespace DatabaseObjects
{
	[Obsolete("Use DatabaseObjects.DatabaseObjectLockable instead, it provides the same functionality.")]
    public abstract class DatabaseObjectUsingAttributesLockable : DatabaseObjectLockable
	{
		/// <summary>
		/// Initializes a new DatabaseObject with the parent collection that this object is
		/// associated with and the lock controller to be used with this object.
		/// </summary>
		protected DatabaseObjectUsingAttributesLockable(DatabaseObjects objParent, DatabaseObjectLockController objLockController) 
            : base(objParent, objLockController)
		{
		}
	}
}
