// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// The OrderByFieldAttribute should specify the field that is sorted and whether it
	/// is ascending or descending. If the TableJoinAttribute has been specified then
	/// the sort order can be specified on the joined table field.
	/// To further customer the ordering (i.e. sort by more than one field)
	/// simply override the OrderBy function and do not specify an OrderByAttribute.
	/// Specifying this attribute is optional and if not specified the table is not
	/// sorted.
	/// </summary>
	/// <example>
	/// <code>
	///    &lt;OrderBy("CustomerName", SQL.OrderBy.Ascending)&gt;
	///    Public Class Customers
	///        ...
	/// </code>
	/// </example>
	/// --------------------------------------------------------------------------------
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class OrderByFieldAttribute : Attribute
	{
		private string pstrOrderByFieldName;
		private SQL.OrderBy peOrdering;
		private int pintPrecendence;
		
		/// <summary>
		/// Specifies the field to sort the table/collection by in ascending order.
		/// If the TableJoinAttribute has been specified then the sort order can be
		/// specified on the joined table field.
		/// </summary>
		public OrderByFieldAttribute(string strOrderByFieldName) 
            : this(strOrderByFieldName, SQL.OrderBy.Ascending)
		{
		}
		
		/// <summary>
		/// Specifies the field to sort the table/collection by in ascending or descending order.
		/// If the TableJoinAttribute has been specified then the sort order can be
		/// specified on the joined table field.
		/// </summary>
		public OrderByFieldAttribute(string strOrderByFieldName, SQL.OrderBy eOrder) 
            : this(strOrderByFieldName, eOrder, 1)
		{
		}
		
		/// <summary>
		/// Specifies the field to sort the table/collection by in ascending or descending order.
		/// If the TableJoinAttribute has been specified then the sort order can be
		/// specified on the joined table field.
		/// </summary>
		/// <param name="intPrecendence">
		/// Indicates the order precendence level when multiple OrderByAttributes are specified.
		/// OrderByAttributes are sorted for items with the lowest to the highest integer value.
		/// </param>
		public OrderByFieldAttribute(string strOrderByFieldName, SQL.OrderBy eOrder, int intPrecendence)
		{
			if (String.IsNullOrEmpty(strOrderByFieldName))
				throw new ArgumentNullException();
			
			pstrOrderByFieldName = strOrderByFieldName;
			peOrdering = eOrder;
			pintPrecendence = intPrecendence;
			
		}
		
		public string Name
		{
			get
			{
				return pstrOrderByFieldName;
			}
		}
		
		public SQL.OrderBy Ordering
		{
			get
			{
				return peOrdering;
			}
		}
		
		public int Precendence
		{
			get
			{
				return pintPrecendence;
			}
		}
	}
}
