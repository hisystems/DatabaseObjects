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

namespace DatabaseObjects
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// Extends the capabilities of the DatabaseObject by providing a set of routines
	/// that lock and unlock this object for exclusive access for a particular user.
	/// When Lock() is called a record is written to the lock table which includes the
	/// object's associated table name, the object's distinct value and the user ID
	/// specified in the DatabaseObjectLockController. When Unlock() is called this
	/// record is deleted. If another or the current user has locked the object then the
	/// IsLocked property will return true.
	/// The DatabseObjects library does not inhibit the loading and/or saving of any
	/// locked objects.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public abstract class DatabaseObjectLockable : DatabaseObject, IDatabaseObjectLockable
	{
		private DatabaseObjectLockController pobjLockController;
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new DatabaseObject with the parent collection that this object is
		/// associated with and the lock controller to be used with this object.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectLockable(DatabaseObjects objParent, DatabaseObjectLockController objLockController) 
            : base(objParent)
		{
			if (objLockController == null)
				throw new ArgumentNullException();
			
			pobjLockController = objLockController;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Indicates whether the current object is Locked either by the current user or
		/// another user.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public bool IsLocked
		{
			get
			{
				return pobjLockController.IsLocked(base.ParentCollection, this);
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the user identifier that currently has the object locked. Throws an
		/// exception if the object is not locked by a user.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public string LockedByUserID
		{
			get
			{
				return pobjLockController.LockedByUserID(base.ParentCollection, this);
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Locks this object.
		/// Throws an exception if this object is already locked.
		/// Throws an exception if this object has not been saved.
		/// It is possible that between calling IsLocked and Lock another
		/// user may have locked the object. Therefore, it is recommended calling Lock and then
		/// trapping the MethodAccessException to determine whether the lock actually succeeded.
		/// </summary>
		/// <exception cref="Exceptions.DatabaseObjectsException">Thrown if the object has not been saved or the object is already locked.</exception>
		/// --------------------------------------------------------------------------------
		public void Lock()
		{
			pobjLockController.Lock(base.ParentCollection, this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// UnLocks this object. Throws an exception if the object is not locked or the
		/// object has not been saved.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public void UnLock()
		{
			pobjLockController.UnLock(base.ParentCollection, this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether the current user has the object locked.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public bool IsLockedByCurrentUser
		{
			get
			{
				return pobjLockController.IsLockedByCurrentUser(base.ParentCollection, this);
			}
		}
	}
}
