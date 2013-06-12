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
	/// The TableAttribute class specifies the name of the database table that this collection
	/// is associated with.
	/// This attribute must be specified for all DatabaseObjects*UsingAttributes classes.
	/// </summary>
	/// <example>
	/// <code>
	///    &lt;Table("Customers")&gt;
	///    Public Class Customers
	///        ...
	/// </code>
	/// </example>
	/// --------------------------------------------------------------------------------
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class TableAttribute : Attribute
	{
		private string pstrTableName;
		
		/// <summary>
		/// Specifies the name of the database table that is the source of this collection.
		/// </summary>
		public TableAttribute(string strTableName)
		{
			if (String.IsNullOrEmpty(strTableName))
				throw new ArgumentNullException();
			
			pstrTableName = strTableName;
		}
		
		public string Name
		{
			get
			{
				return pstrTableName;
			}
		}
	}
}
