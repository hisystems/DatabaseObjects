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
/// This class is deprecated. Use DatabaseObjects.DatabaseObject instead, it provides the same functionality.
/// </summary>
namespace DatabaseObjects
{
	[Obsolete("Use DatabaseObjects.DatabaseObject instead, it provides the same functionality.")]
    public abstract class DatabaseObjectUsingAttributes : DatabaseObject
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new DatabaseObject with the parent collection that this object is
		/// associated with.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectUsingAttributes(DatabaseObjects objParentCollection)
            : base(objParentCollection)
		{
		}
	}
}
