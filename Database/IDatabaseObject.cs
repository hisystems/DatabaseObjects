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
	/// An IDatabaseObject object represents a single record in a database table. The
	/// Database class provides the facility for classes implementing IDatabaseObject to
	/// copy data to and from the associated database record. Rather than directly
	/// implementing IDatabaseObject inherit from DatabaseObject as the DatabaseObject
	/// class provides the basic "plumbing" code required by IDatabaseObjects.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public interface IDatabaseObject
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return whether the object has been saved to the database. Generally,
		/// this is implemented using a private variable that stores the object's saved state.
		/// If a new object is saved or an existing object is loaded then this property
		/// is automatically set to true by the library.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		bool IsSaved { get; set; }
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return the distinct value that uniquely identifies this
		/// object in the database. If a new object is saved (which uses an auto increment
		/// field) or an existing object is loaded then this property is automatically
		/// set by the library.
		/// Typically, this is the value of an identity or auto increment database field.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		object DistinctValue { get; set; }
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// This subroutine should copy the database fields from objFields to the object's
		/// variables. objFields is populated with all of the fields from the associated record.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Sub LoadFields(ByVal objFields As SQL.SQLFieldValues) Implements IDatabaseObject.LoadFields
		///
		///     pstrCode = objFields("ProductCode").Value
		///     pstrDescription = objFields("ProductDescription").Value
		///
		/// End Sub
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		void LoadFields(SQL.SQLFieldValues objFields);
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return an SQLFieldValues object populated with the
		/// fields to be written to the database. The first argument of the SQLFieldValues.Add
		/// function is the database field name, the second is the field's value.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Function SaveFields() As SQL.SQLFieldValues Implements IDatabaseObject.SaveFields
		///
		///     SaveFields = New SQL.SQLFieldValues
		///     SaveFields.Add("ProductCode", pstrCode)
		///     SaveFields.Add("ProductDescription", pstrDescription)
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		SQL.SQLFieldValues SaveFields();
	}
}
