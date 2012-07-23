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
/// This class is deprecated. Use DatabaseObjects.DatabaseObjectsVolatile instead, it provides the same functionality.
/// </summary>
namespace DatabaseObjects
{
	[Obsolete("Use DatabaseObjects.DatabaseObjectsVolatile instead, it provides the same functionality.")]
    public abstract class DatabaseObjectsVolatileUsingAttributes : DatabaseObjectsVolatile
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance with the parent that it is associated with.
		/// </summary>
		///
		/// <param name="objParent">
		/// The parent collection that this collection is associated with. This is often
		/// useful so that the SubSet property can use the Parent to filter
		/// by a particular value pertinent to the parent object.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectsVolatileUsingAttributes(DatabaseObject objParent) 
            : base(objParent)
		{
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance with it's associated database.
		/// </summary>
		///
		/// <param name="objDatabase">
		/// The database that this collection is associated with.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectsVolatileUsingAttributes(Database objDatabase)
            : base(objDatabase)
		{
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes with it the associated root container and database.
		/// </summary>
		///
		/// <param name="rootContainer">
		/// The root object that this collection is associated with.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectsVolatileUsingAttributes(RootContainer rootContainer)
            : base(rootContainer)
		{
		}
	}
}
