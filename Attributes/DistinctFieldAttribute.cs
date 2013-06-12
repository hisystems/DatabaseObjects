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
	/// Specifies the field name that uniquely identifies each object
	/// within the collection. Typically, this is the field name of an identity or auto
	/// increment field. If the SubSetAttribute has been specified
	/// then the strDistinctFieldName need only be unique within the subset not the
	/// entire table. The strDistinctFieldName and can be identical to the field name
	/// specified with a KeyField attribute.
	/// This attribute must be specified on a DatabaseObjects*UsingAttributes class.
	/// This attribute is used to implement the IDatabaseObjects.DistinctFieldName
	/// and IDatabaseObjects.DistinctFieldAutoIncrements functions.
	/// </summary>
	/// <example>
	/// <code>
	///    &lt;DistinctField("CustomerID", bAutoIncrements:=True)&gt;
	///    Public Class Customers
	///        ...
	/// </code>
	/// </example>
	/// --------------------------------------------------------------------------------
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DistinctFieldAttribute : Attribute
	{
		private string pstrDistinctFieldName;
		private SQL.FieldValueAutoAssignmentType peFieldValueAutomaticAssignment = SQL.FieldValueAutoAssignmentType.None;
		
		/// <summary>
		/// Specifies the field name that uniquely identifies each object
		/// within the collection.
		/// </summary>
		public DistinctFieldAttribute(string strDistinctFieldName)
            : this(strDistinctFieldName, bAutoIncrements: false)
		{
		}
		
		/// <summary>
		/// Specifies the field name that uniquely identifies each object
		/// within the collection. Typically, this is the field name of an identity or auto
		/// increment field in which case the bAutoIncrements value should be set to true.
		/// </summary>
		public DistinctFieldAttribute(string strDistinctFieldName, bool bAutoIncrements)
		{
			if (String.IsNullOrEmpty(strDistinctFieldName))
				throw new ArgumentNullException();
			
			pstrDistinctFieldName = strDistinctFieldName;

			if (bAutoIncrements)
				peFieldValueAutomaticAssignment = SQL.FieldValueAutoAssignmentType.AutoIncrement;
		}
		
		/// <summary>
		/// Specifies the field name that uniquely identifies each object
		/// within the collection. Typically, this is the field name of an identity or auto
		/// increment field in which case the bAutoIncrements value should be set to true.
		/// </summary>
		public DistinctFieldAttribute(string strDistinctFieldName, SQL.FieldValueAutoAssignmentType eAutomaticAssignment)
		{
			if (String.IsNullOrEmpty(strDistinctFieldName))
				throw new ArgumentNullException();
			
			pstrDistinctFieldName = strDistinctFieldName;
			peFieldValueAutomaticAssignment = eAutomaticAssignment;
		}
		
		public string Name
		{
			get
			{
				return pstrDistinctFieldName;
			}
		}
		
		public SQL.FieldValueAutoAssignmentType AutomaticAssignment
		{
			get
			{
				return peFieldValueAutomaticAssignment;
			}
		}
	}
	
}
