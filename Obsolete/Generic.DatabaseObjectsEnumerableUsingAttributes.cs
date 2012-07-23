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
	/// This class is deprecated. Use DatabaseObjects.Generic.DatabaseObjectsEnumerable instead, it provides the same functionality.
	/// </summary>
	[Obsolete("Use DatabaseObjects.Generic.DatabaseObjectsEnumerable instead, it provides the same functionality.")]
    public abstract class DatabaseObjectsEnumerableUsingAttributes<T> : Generic.DatabaseObjectsEnumerable<T> where T : IDatabaseObject
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new DatabaseObjects with it's associated database.
		/// </summary>
		///
		/// <param name="objDatabase">
		/// The database that this collection is associated with.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectsEnumerableUsingAttributes(Database objDatabase) 
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
		protected DatabaseObjectsEnumerableUsingAttributes(RootContainer rootContainer)
            : base(rootContainer)
		{
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new DatabaseObjects with it's associated parent object.
		/// The Parent property can be used to access the parent variable passed into this constructor.
		/// </summary>
		///
		/// <param name="objParent">
		/// The parent object that this collection is associated with.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectsEnumerableUsingAttributes(DatabaseObject objParent) 
            : base(objParent)
		{
		}
	}
}
