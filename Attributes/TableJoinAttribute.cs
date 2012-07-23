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
/// The TableJoinAttribute indicates that this collection's table should be joined
/// with another.
/// This function is useful in optimising database loading speeds by allowing multiple
/// tables to be joined into one data set. The resultant data set can then be used to load
/// objects from the associated tables avoiding subsequent SQL calls. For a complete
/// example, see the demonstration program.
/// To further customer the subset (i.e. make it conditional)
/// simply override the TableJoins function and do not specify a TableJoinAttribute.
/// </summary>
/// <example>
/// <code>
///    &lt;TableJoin("MainProductID", "Products", "ProductID")&gt;
///    Public Class Customers
///        ...
/// </code>
/// </example>
/// --------------------------------------------------------------------------------
namespace DatabaseObjects
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class TableJoinAttribute : Attribute
	{
		private string pstrJoinFieldName;
		private string pstrJoinToTableName;
		private string pstrJoinToFieldName;
		
		/// <summary>
		/// Specifies the field to join and the additional table and field name to which it is joined.
		/// </summary>
		/// <param name="strJoinFieldName">The name of the field in the collection's primary table that is to be joined to another table.</param>
		/// <param name="strJoinToTableName">The name of the table to join with the collection's primary table.</param>
		/// <param name="strJoinToFieldName">The name of the field in the table which is to be joined with the collection's primary table.</param>
		public TableJoinAttribute(string strJoinFieldName, string strJoinToTableName, string strJoinToFieldName)
		{
			if (String.IsNullOrEmpty(strJoinFieldName))
				throw new ArgumentNullException("Join Field Name");
			else if (String.IsNullOrEmpty(strJoinToTableName))
				throw new ArgumentNullException("Join To Table Name");
			else if (String.IsNullOrEmpty(strJoinToFieldName))
				throw new ArgumentNullException("Join To Field Name");
			
			pstrJoinFieldName = strJoinFieldName;
			pstrJoinToTableName = strJoinToTableName;
			pstrJoinToFieldName = strJoinToFieldName;
		}
		
		public string FieldName
		{
			get
			{
				return pstrJoinFieldName;
			}
		}
		
		public string ToTableName
		{
			get
			{
				return pstrJoinToTableName;
			}
		}
		
		public string ToFieldName
		{
			get
			{
				return pstrJoinToFieldName;
			}
		}
	}
}
