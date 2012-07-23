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
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// This class extends Generic.DatabaseObjectsVolatile by providing by adding
	/// a public Add, Item, Count and Delete functions.
	/// For more information please see
	/// <see cref="Generic.DatabaseObjectsVolatile(Of T)">Generic.DatabaseObjectsVolatile</see>.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public abstract class DatabaseObjectsVolatileList<T> : DatabaseObjectsVolatile<T> where T : IDatabaseObject
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
		protected DatabaseObjectsVolatileList(DatabaseObject objParent) 
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
		protected DatabaseObjectsVolatileList(Database objDatabase) 
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
		protected DatabaseObjectsVolatileList(RootContainer rootContainer) 
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
		protected DatabaseObjectsVolatileList(Database objDatabase, object objData) 
            : base(objDatabase, objData)
		{
		}

		/// <summary>
		/// Creates and returns a new object which has been added to the in-memory list.
		/// </summary>
		public virtual T Add()
		{
			return base.VolatileObjectAdd();
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an item from the in-memory list at a specific ordinal index.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public virtual T this[int intIndex]
		{
			get
			{
				return base.VolatileObjectByOrdinal(intIndex);
			}
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Removes the item from the in-memory list, and flags the item to be deleted.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		public virtual void Delete(ref T objItem)
		{
			base.VolatileObjectDelete(objItem);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the number of items in the in-memory list.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public int Count
		{
			get
			{
				return base.VolatileObjectsCount;
			}
		}
	}
}
