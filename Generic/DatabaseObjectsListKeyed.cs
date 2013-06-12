// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//	    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// _________________________________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.Generic
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// Extends DatabaseObjects.Generic.DatabaseObjectsList by providing a public
	/// Add(inherited) Item by key, Count (inherited) and Exists properties and function.
	/// For more information please see DatabaseObjects.DatabaseObjects.
	/// </summary>
	/// --------------------------------------------------------------------------------
	///
	public abstract class DatabaseObjectsListKeyed<T, TKey> : DatabaseObjectsList<T> where T : IDatabaseObject
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
		protected DatabaseObjectsListKeyed(Database objDatabase)
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
		protected DatabaseObjectsListKeyed(RootContainer rootContainer) 
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
		protected DatabaseObjectsListKeyed(DatabaseObject objParent) 
            : base(objParent)
		{
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an object for a key value.
		/// </summary>
		/// <remarks>
		/// This function onforwards a call to DatabaseObjects.ObjectByKey().
		/// </remarks>
		/// --------------------------------------------------------------------------------
		///
		public virtual T this[TKey Key]
		{
			get
			{
				return base.ObjectByKey(Key);
			}
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether the key exists within the collection.
		/// </summary>
		/// <remarks>
		/// This function onforwards a call to DatabaseObjects.ObjectExists().
		/// </remarks>
		/// --------------------------------------------------------------------------------
		///
		public virtual bool Exists(TKey Key)
		{
			return base.ObjectExists(Key);
		}
	}
}
