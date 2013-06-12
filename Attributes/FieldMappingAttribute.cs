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
	/// The FieldMappingAttribute class is used to define a mapping between a database
	/// field and a class property. This attribute is used with the
	/// DatabaseObjectUsingAttributes class.
	/// This field can also be used for loading fields that are of type
	/// DatabaseObjects.ObjectReference or DatabaseObjects.Generic.ObjectReference.
	/// </summary>
	/// --------------------------------------------------------------------------------
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class FieldMappingAttribute : Attribute
	{
		private string pstrFieldName;
		
		/// --------------------------------------------------------------------------------
		/// <param name="strFieldName">
		/// The name of the database field associated with this property or field.
		/// </param>
		/// <example>
		/// Loads a field:
		/// <code>
		///
		/// &lt;DatabaseObjects.FieldMapping("Name")&gt; _
		/// Private pstrName As String
		///
		/// </code>
		/// Loads an object:
		/// <code>
		///
		/// &lt;DatabaseObjects.FieldMapping("ProductGroupID")&gt; _
		/// Private pobjGroup As New Generic.ObjectReference(Of ProductGroup, Integer)(Database.ProductGroups)
		///
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		public FieldMappingAttribute(string strFieldName)
		{
			if (String.IsNullOrEmpty(strFieldName))
				throw new ArgumentNullException();
			
			pstrFieldName = strFieldName;
		}
		
		public string FieldName
		{
			get
			{
				return pstrFieldName;
			}
		}
	}
}
