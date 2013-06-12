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
	/// The KeyFieldAttribute should specify the field name that uniquely identifies each object
	/// within the collection. As opposed to the ordinal/index position or distinct field, the key field
	/// provides another method of accessing a particular object within the collection.
	/// The key field must be unique within the collection. If the SubsetAttribute
	/// has been specified then the key field only needs to be unique within
	/// the subset, not the entire table. Specifying this attribute is optional.
	/// </summary>
	/// <example>
	/// <code>
	///    &lt;KeyField("CustomerName")&gt;
	///    Public Class Customers
	///        ...
	/// </code>
	/// </example>
	/// --------------------------------------------------------------------------------
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class KeyFieldAttribute : Attribute
	{
		private string pstrKeyFieldName;
		
		/// <summary>
		/// Specifies the field name that uniquely identifies each object
		/// within the collection. As opposed to the ordinal/index position or distinct field,
		/// the key field provides another method of accessing a particular object within the collection.
		/// The key field must be unique within the collection. If the SubsetAttribute
		/// has been specified then the key field only needs to be unique within
		/// the subset, not the entire table. Specifying this attribute is optional.
		/// </summary>
		public KeyFieldAttribute(string strKeyFieldName)
		{
			if (String.IsNullOrEmpty(strKeyFieldName))
				throw new ArgumentNullException();
			
			pstrKeyFieldName = strKeyFieldName;
		}
		
		public string Name
		{
			get
			{
				return pstrKeyFieldName;
			}
		}
	}
}
