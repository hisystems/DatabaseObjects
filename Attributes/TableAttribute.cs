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
