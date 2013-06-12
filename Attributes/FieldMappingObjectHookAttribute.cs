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
	/// Used to mark a property or field that reference an object for which database
	/// fields need to be loaded. The referenced object then contain fields or properties
	/// marked with FieldMappingAttribute.
	/// The field must be marked on a reference type field not a value based field.
	/// This attribute is useful in situations where another class (usually an inner class)
	/// holds a reference to an object that contains additional properties that are stored
	/// in the same record as the main container class.
	/// </summary>
	/// --------------------------------------------------------------------------------
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class FieldMappingObjectHookAttribute : Attribute
	{
		public FieldMappingObjectHookAttribute()
		{
		}
	}
}
