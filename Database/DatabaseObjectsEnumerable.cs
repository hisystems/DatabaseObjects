// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

/// --------------------------------------------------------------------------------
/// <summary>
/// Extends DatabaseObjects and implements the IEnumerable interface, therefore
/// providing support for the "For Each" construct.
/// </summary>
/// --------------------------------------------------------------------------------
///
namespace DatabaseObjects
{
	public abstract class DatabaseObjectsEnumerable : DatabaseObjects, IEnumerable
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
		protected DatabaseObjectsEnumerable(Database objDatabase)
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
		protected DatabaseObjectsEnumerable(RootContainer rootContainer)
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
		protected DatabaseObjectsEnumerable(DatabaseObject objParent)
            : base(objParent)
		{			
		}
		
		public System.Collections.IEnumerator GetEnumerator()
		{			
			return base.ObjectsList().GetEnumerator();
		}
	}
}
