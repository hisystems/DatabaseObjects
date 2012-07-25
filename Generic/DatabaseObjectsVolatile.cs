// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Collections.Generic;

namespace DatabaseObjects.Generic
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// This class is a generic version of DatabaseObjectsVolatile.
	/// This class stores all objects associated with this DatabaseObjects collection in
	/// memory. Any objects added via VolatileObjectAdd or VolatileObjectDelete only
	/// affect the memory list until VolatileObjectsSave() is called.
	/// VolatileObjectsSave() will delete any objects flagged for deletion via VolatileObjectDelete()
	/// and then save any pre-loaded or newly added objects via VolatileObjectsAdd()
	/// to the database.
	/// Item objects can implement IDatabaseObjectVolatile to override the default saving
	/// behaviour of VolatileObjectsSave().
	/// </summary>
	/// --------------------------------------------------------------------------------
	public abstract class DatabaseObjectsVolatile<T> : DatabaseObjectsVolatile, IEnumerable<T> where T : IDatabaseObject
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
		protected DatabaseObjectsVolatile(DatabaseObject objParent)
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
		protected DatabaseObjectsVolatile(Database objDatabase) 
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
		protected DatabaseObjectsVolatile(RootContainer rootContainer) 
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
		protected DatabaseObjectsVolatile(Database objDatabase, object objData) 
            : base(objDatabase, objData)
		{
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Adds a new object (created via ItemInstance) to the in-memory list, and flags the
		/// item to be saved to the database when VolatileObjectsSave() is called.
		/// Returns the new object that has been added to the in-memory list.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected new T VolatileObjectAdd()
		{
			T objNewItem = this.ItemInstance_();
				
			this.VolatileObjectAdd(objNewItem);
				
			return objNewItem;
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Adds an item to the in-memory list, and flags the item to be saved to the database
		/// when VolatileObjectsSave() is called.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected void VolatileObjectAdd(T objItem)
		{
			base.VolatileObjectAdd(objItem);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an item at the specific index in the in-memory list.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected new T VolatileObjectByOrdinal(int intIndex)
		{
			return (T)base.VolatileObjectByOrdinal(intIndex);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the index at which the object exists in the in-memory list.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">ArgumentOutOfRangeException</exception>
		/// --------------------------------------------------------------------------------
		protected int VolatileObjectIndexOf(T objObject)
		{
			return base.VolatileObjectIndexOf(objObject);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Removes the item from the in-memory list, and flags the item to be deleted when
		/// VolatileObjectsSave() is called.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected void VolatileObjectDelete(T objItem)
		{
			base.VolatileObjectDelete(objItem);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns all items that have been added or were part of the initial list of objects. If
		/// there are no items in the list then a zero length array is returned.
		/// This list does not include any objects that have been deleted via VolatileObjectDelete().
		/// After VolatileObjectsSave() has been called this list remains the same.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected new T[] VolatileObjects
		{
			get
			{
				IDatabaseObject[] objSourceObjects = base.VolatileObjects;
				T[] objDestinationObjects = new T[objSourceObjects.Length - 1 + 1];
					
				Array.Copy(objSourceObjects, objDestinationObjects, objSourceObjects.Length);
					
				return objDestinationObjects;
			}
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns all objects that have been flagged for deletion via VolatileObjectDelete().
		/// After VolatileObjectsSave() has been called this list is cleared and a zero length array
		/// would be returned.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected new T[] VolatileObjectsToDelete
		{
			get
			{
				IDatabaseObject[] objSourceObjects = base.VolatileObjectsToDelete;
				T[] objDestinationObjects = new T[objSourceObjects.Length - 1 + 1];
					
				Array.Copy(objSourceObjects, objDestinationObjects, objSourceObjects.Length);
					
				return objDestinationObjects;
			}
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return an instance of the class that is associated with this
		/// collection of objects. This is the generic version of the ItemInstance function.
		/// It is suffixed with an underscore so that it does not conflict with the underlying
		/// non-generic equivalent ItemInstance function. It's purpose is indentical to the
		/// non-generic version.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function ItemInstance_() As Product
		///
		///     Return New Product
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		protected virtual T ItemInstance_()
		{
			return (T)base.ItemInstance();
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the enumerator for all objects currently in the in-memory list.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected new IEnumerator<T> VolatileObjectsEnumerator()
		{
			return new List<T>(this.VolatileObjects).GetEnumerator();
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// For some reason - I believe it may be a bug/omission in the compiler, the above VolatileObjectsEnumerator
		/// cannot be used to also implement System.Collections.Generic.IEnumerable(Of T).GetEnumerator.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public new IEnumerator<T> GetEnumerator()
		{
			return this.VolatileObjectsEnumerator();
		}
	}
}
