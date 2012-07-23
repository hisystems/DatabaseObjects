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
	/// This class extends DatabaseObjects.Generic.DatabaseObjectsEnumerable by providing public
	/// Add, Item and Count properties.
	/// For more information please see DatabaseObjects.DatabaseObjects.
	/// </summary>
	/// --------------------------------------------------------------------------------
	///
	public abstract class DatabaseObjectsList<T> : DatabaseObjectsEnumerable<T> where T : IDatabaseObject
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
		protected DatabaseObjectsList(Database objDatabase) 
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
		protected DatabaseObjectsList(RootContainer rootContainer) 
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
		protected DatabaseObjectsList(DatabaseObject objParent)
            : base(objParent)
		{
		}
			
		/// <summary>
		/// Creates and returns a new object associated with this collection.
		/// </summary>
		public virtual T Add()
		{
			return this.ItemInstance_();
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the number of items in this collection.
		/// </summary>
		/// <remarks>
		/// This property onforwards a call to DatabaseObjects.ObjectsCount.
		/// </remarks>
		/// --------------------------------------------------------------------------------
		///
		public int Count
		{
			get
			{
				return base.ObjectsCount();
			}
		}
	}
}
