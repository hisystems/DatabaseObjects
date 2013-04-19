// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// This class extends DatabaseObjects by storing all objects associated with this
	/// DatabaseObjects collection in memory. Any objects added via VolatileObjectAdd()
	/// or VolatileObjectDelete() only affect the memory list until VolatileObjectsSave() is called.
	/// VolatileObjectsSave() will delete any objects flagged for deletion via VolatileObjectDelete()
	/// and then save any pre-loaded or newly added objects via VolatileObjectsAdd()
	/// to the database.
	/// Item objects can implement IDatabaseObjectVolatile to override the default saving
	/// behaviour of VolatileObjectsSave().
	/// </summary>
	/// --------------------------------------------------------------------------------
	public abstract class DatabaseObjectsVolatile : DatabaseObjects, IEnumerable
	{
		private IList pobjItems;
		private IList pobjItemsToDelete = new ArrayList();
		private object pobjData;
		
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
		protected DatabaseObjectsVolatile(DatabaseObject objParent) 
            : base(objParent)
		{
			VolatileItemsLoad();
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
			VolatileItemsLoad();
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
			VolatileItemsLoad();
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
            : base(objDatabase)
		{
			pobjData = objData;
			VolatileItemsLoad();
		}
		
		/// <summary>
		/// Loads the items for this collection.
		/// This is usually overridden in base class so that code can be executed before the objects list is loaded.
		/// Because otherwise code cannot be executed before the MyBase.New or :base() call to this base class.
		/// as the items are loaded as part of the constructor.
		/// </summary>
		protected virtual void VolatileItemsLoad()
		{
			pobjItems = base.ObjectsList();
		}
		
		/// <summary>
		/// Returns the argument passed into the constructor New(Database, Object).
		/// </summary>
		protected object Data
		{
			get
			{
				return pobjData;
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Adds a new object (created via ItemInstance) to the in-memory list, and flags the
		/// item to be saved to the database when VolatileObjectsSave() is called.
		/// Returns the new object that has been added to the in-memory list.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected IDatabaseObject VolatileObjectAdd()
		{
			IDatabaseObject objNewItem = this.ItemInstance();
			
			this.VolatileObjectAdd(objNewItem);
			
			return objNewItem;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Adds an item to the in-memory list, and flags the item to be saved to the database
		/// when VolatileObjectsSave() is called.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected void VolatileObjectAdd(IDatabaseObject objItem)
		{
			pobjItems.Add(objItem);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an item at the specific index in the in-memory list.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected IDatabaseObject VolatileObjectByOrdinal(int intIndex)
		{
			return (IDatabaseObject)pobjItems[intIndex];
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the index at which the object exists in the in-memory list.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">ArgumentOutOfRangeException</exception>
		/// --------------------------------------------------------------------------------
		protected int VolatileObjectIndexOf(IDatabaseObject objObject)
		{
			int intIndex = 0;
			
			foreach (IDatabaseObject objItem in pobjItems)
			{
				if (objItem == objObject)
					return intIndex;
				
                intIndex++;
			}
			
			throw new ArgumentOutOfRangeException();
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Removes the item from the in-memory list, and flags the item to be deleted when
		/// VolatileObjectsSave() is called.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected void VolatileObjectDelete(IDatabaseObject objItem)
		{
			pobjItemsToDelete.Add(objItem);
			pobjItems.Remove(objItem);
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
		protected IDatabaseObject[] VolatileObjects
		{
			get
			{
				IDatabaseObject[] objItems = new IDatabaseObject[pobjItems.Count - 1 + 1];
				pobjItems.CopyTo(objItems, 0);
				
				return objItems;
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns all objects that have been flagged for deletion via VolatileObjectDelete().
		/// After VolatileObjectsSave() has been called this list is cleared and a zero length array
		/// would be returned.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected IDatabaseObject[] VolatileObjectsToDelete
		{
			get
			{
				IDatabaseObject[] objItems = new IDatabaseObject[pobjItemsToDelete.Count - 1 + 1];
				pobjItemsToDelete.CopyTo(objItems, 0);
				
				return objItems;
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Clears all items in the in-memory list. Items are NOT flagged for deletion and
		/// subsequently deleted in VolatileObjectsSave().
		/// Use VolatileObjectsDeleteAll to for this purpose.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected void VolatileObjectsClear()
		{
			pobjItems.Clear();
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Removes all items from the in-memory list, and flags them to be deleted on the next
		/// VolatileObjectsSave(). To clear the list without deleting the objects use
		/// VolatileObjectsClear().
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected void VolatileObjectsDeleteAll()
		{
			foreach (IDatabaseObject objItem in pobjItems)
				pobjItemsToDelete.Add(objItem);
			
			pobjItems.Clear();
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// All items flagged for deletion (via VolatileObjectDelete()) are deleted in the database.
		/// Then all items that have been added to the in-memory list (via VolatileObjectAdd())
		/// are added/saved to the database. If any of the objects implement IDatabaseObjectVolatile
		/// then the IDatabaseObjectVolatile.Save function is called instead of the default
		/// Database.ObjectSave() call. This allows the object to perform any additional
		/// checking and validation.
		/// IDatabaseObjectVolatile.Save is assumed to save itself to the database.
		/// i.e. via a call to MyBase.Save().
		/// Objects that do not implement IDatabaseObjectVolatile are still saved via the
		/// Database.ObjectSave() function.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Friend Sub SaveAll()
		///
		///     Mybase.VolatileObjectsSave()
		///
		/// End Sub
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected virtual void VolatileObjectsSave()
		{
            foreach (IDatabaseObject objItem in pobjItemsToDelete)
            {
                var objectToDelete = objItem;
                this.ParentDatabase.ObjectDelete(this, ref objectToDelete);
            }
			
			pobjItemsToDelete.Clear();
			
			foreach (IDatabaseObject objItem in pobjItems)
			{
				if (objItem is IDatabaseObjectVolatile)
					((IDatabaseObjectVolatile) objItem).Save();
				else
					this.ParentDatabase.ObjectSave(this, objItem);
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the number of items in the in-memory list.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected int VolatileObjectsCount
		{
			get
			{
				return pobjItems.Count;
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the enumerator for all objects currently in the in-memory list.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.VolatileObjectsEnumerator();
		}
		
		public System.Collections.IEnumerator VolatileObjectsEnumerator()
		{
			return pobjItems.GetEnumerator();
		}
	}
}
