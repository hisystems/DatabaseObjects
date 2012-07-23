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
/// Specifies the interface of all classes that are considered lockable by the library.
/// See DatabaseObjectLockable, DatabaseObjectUsingAttributesLockable for further
/// details.
/// </summary>
/// --------------------------------------------------------------------------------
namespace DatabaseObjects
{
	public interface IDatabaseObjectLockable
	{
		void Lock();
		void UnLock();
		string LockedByUserID { get; }
		bool IsLocked { get; }
		bool IsLockedByCurrentUser { get; }
	}
}
