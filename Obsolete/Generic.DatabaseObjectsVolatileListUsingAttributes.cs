// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.Generic
{
	/// <summary>
	/// This class is deprecated. Use DatabaseObjects.Generic.DatabaseObjectsVolatileList instead, it provides the same functionality.
	/// </summary>
	[Obsolete("Use DatabaseObjects.Generic.DatabaseObjectsVolatileList instead, it provides the same functionality.")]
    public abstract class DatabaseObjectsVolatileListUsingAttributes<T> : Generic.DatabaseObjectsVolatileList<T> where T : IDatabaseObject
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
		///
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectsVolatileListUsingAttributes(DatabaseObject objParent) 
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
		protected DatabaseObjectsVolatileListUsingAttributes(Database objDatabase) 
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
		protected DatabaseObjectsVolatileListUsingAttributes(RootContainer rootContainer) 
            : base(rootContainer)
		{
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance with it's associated database and data that
		/// can be used for specifying a subset.
		/// </summary>
		/// <remarks>
		/// The Data propety is set before the items list is loaded.
		/// </remarks>
		///
		/// <param name="objDatabase">
		/// The database that this collection is associated with.
		/// </param>
		///
		/// <param name="objData">
		/// An additional object that is usually required so that it can be used
		/// as a filter in the SubSet function.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectsVolatileListUsingAttributes(Database objDatabase, object objData) 
            : base(objDatabase, objData)
		{
		}
	}
}
