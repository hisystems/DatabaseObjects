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
	/// Extends the functionality of IDatabaseObjects so that if a collection contains
	/// multiple subclasses of a particular class in the collection then the
	/// ItemInstanceForSubclass is used rather than ItemInstance to return the particular
	/// subclass to be created.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public interface IDatabaseObjectsMultipleSubclass : IDatabaseObjects
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
		/// Protected Function ItemInstanceForSubclass(ByVal objFieldValues As SQL.SQLFieldValues) As IDatabaseObject Implements IDatabaseObjects.ItemInstanceForSubclass
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
		IDatabaseObject ItemInstanceForSubclass(SQL.SQLFieldValues objFieldValues);
	}
}
