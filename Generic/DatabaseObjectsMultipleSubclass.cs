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
	/// <summary>
	/// Extends DatabaseObjects.Generic.DatabaseObjects but provides a additional override ItemInstanceForSubclass_
	/// which allows different subclasses to be created (that inherit from T) based on the contents of a database record
	/// as specified by the SQLFieldValues argument.
	/// </summary>
	public abstract class DatabaseObjectsMultipleSubclass<T> : DatabaseObjects<T>, IDatabaseObjectsMultipleSubclass where T : IDatabaseObject
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return an instance of the class that is associated with this collection of objects.
		/// The associated class must implement the IDatabaseObjectMultipleSubclass interface.
		/// </summary>
		/// <param name="objFieldValues">
		/// The database record field values that can be used to determine the type of subclass to be loaded.
		/// Do NOT call ObjectFromFieldValues() or ObjectLoad(). These functions are called after
		/// ItemInstanceForSubclass returns.
		/// from this function.
		/// </param>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function ItemInstanceForSubclass_(ByVal objFieldValues As SQL.SQLFieldValues)
		///
		///     If objSubclassRecord("Type") = "Special" Then
		///         Return New SpecialisedProduct
		///     Else
		///         Return New Product
		///     End If
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		protected abstract T ItemInstanceForSubclass_(SQL.SQLFieldValues objFieldValues);
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new DatabaseObjects with it's associated database.
		/// </summary>
		///
		/// <param name="objDatabase">
		/// The database that this collection is associated with.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjectsMultipleSubclass(Database objDatabase)
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
		protected DatabaseObjectsMultipleSubclass(RootContainer rootContainer)
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
		protected DatabaseObjectsMultipleSubclass(DatabaseObject objParent) 
            : base(objParent)
		{
		}
			
		protected IDatabaseObject ItemInstanceForSubclass(SQL.SQLFieldValues objFieldValues)
		{
			return this.ItemInstanceForSubclass_(objFieldValues);
		}

        IDatabaseObject IDatabaseObjectsMultipleSubclass.ItemInstanceForSubclass(SQL.SQLFieldValues objFieldValues)
        {
            return this.ItemInstanceForSubclass(objFieldValues);
        }

		protected override T ItemInstance_()
		{
			throw new NotSupportedException("ItemInstance_ is not supported for IDatabaseObjectsMultipleSubclass objects");
		}
			
		protected override IDatabaseObject ItemInstance()
		{
			throw new NotSupportedException("ItemInstance is not supported for IDatabaseObjectsMultipleSubclass objects");
		}
	}
}
