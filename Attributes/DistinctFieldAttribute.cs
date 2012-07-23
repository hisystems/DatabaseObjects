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
namespace DatabaseObjects
{
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
