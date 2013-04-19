// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects
{
	/// <summary>
	/// Attribute for a field of type DatabaseObjects.Generic.ObjectReference class.
	/// Attribute cannot be applied to a DatabaseObjects.ObjectReference class - only the generic version.
	///
	/// Indicates whether the foreign / referenced object should be loaded when the containing
	/// object is loaded (early binding).
	///
	/// If ommitted then the late binding is applied. Specifically, only the reference object's
	/// distinct value will be assigned to the ObjectReference.DistinctValue property.
	/// Subsequent calls to ObjectReference.Object will load the foreign / referenced.
	///
	/// Early binding is achieved by joining the table with the foreign table and creating
	/// the main object and the foreign / referenced object from the joined data.
	/// The table joins are created automatically and are automatically implemented in the
	/// DatabaseObjects.TableJoins function.
	///
	/// ObjectReferenceEarlyBindingAttribute must be used with the FieldMappingAttribute to indicate
	/// the name of the field in the primary collection's table that references the
	/// foreign field / distinct value in the foreign collection.
	/// The field specified MUST link to the foreign collection's distinct field and table name as indicated
	/// by the TableAttribute and DistinctFieldAttribute values.
	/// </summary>
	/// <remarks>
	/// The foreign collection table is determined by utilising the type T defined by
	/// DatabaseObjects.Generic.ObjectReference and then obtaining the table name and distinct
	/// field from the TableAttribute and DistinctFieldAttribute values.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ObjectReferenceEarlyBindingAttribute : Attribute
	{
		/// <summary>
		/// Indicates that the referenced object should be loaded when the main object is loaded.
		/// This is achieved using table joins.
		/// Can only be used with a DatabaseObjects.Generic.ObjectReference class.
		/// The foreign collection must specify the TableAttribute and DistinctFieldAttribute values.
		/// </summary>
		public ObjectReferenceEarlyBindingAttribute()
		{
		}
	}
}
