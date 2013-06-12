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
	/// The SubsetAttribute class is used to indicate that the collection is a subset
	/// of the parent collection. The library uses the DatabaseObjects.Parent.DistinctValue
	/// property to determine the value to filter this table by using the field name specified
	/// in the attribute. For example if the collection was a set of InvoiceLines then
	/// the table would filter on the InvoiceID in the InvoicesLines table using the InvoiceID
	/// from the parent Invoice object.
	/// To further customer the subset (i.e. make it conditional)
	/// simply override the Subset function and do not specify a SubsetAttribute.
	/// </summary>
	/// <example>
	/// <code>
	///    &lt;Subset("InvoiceID")&gt;
	///    Public Class InvoiceLines
	///        ...
	/// </code>
	/// </example>
	/// --------------------------------------------------------------------------------
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SubsetAttribute : Attribute
	{
		private string pstrFieldName;
		
		/// <summary>
		/// Specifies the field name used to subset/filter the table so that the collection
		/// represents the appropriate records from the database table.
		/// For example, "InvoiceID" for an InvoiceLines collection.
		/// </summary>
		public SubsetAttribute(string strUsingFieldName)
		{
			if (String.IsNullOrEmpty(strUsingFieldName))
				throw new ArgumentNullException();
			
			pstrFieldName = strUsingFieldName;
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
