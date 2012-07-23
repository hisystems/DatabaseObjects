// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.Generic
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// This class can be used to simplify the process of creating and returning late-bound
	/// IDatabaseObject or DatabaseObject objects. This is is particularly useful in decreasing
	/// load times of referenced objects by post-poning loading any objects until the property or
	/// method is called directly.
	/// The referenced object is only loaded on the first call to the Object property.
	/// Subsequent calls return the already loaded object.
	/// A variable of this type can be marked with the FieldMapping attribute so that the distinct
	/// values are automatically set and read by the library (this particular facility can only be
	/// used by an object that inherits from DatabaseObjectsUsingAttributes).
	/// </summary>
	/// --------------------------------------------------------------------------------
	public class ObjectReference<T> : ObjectReference where T : IDatabaseObject
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the class with the collection that the object is contained within.
		/// </summary>
		/// <param name="objCollection">
		/// The collection that contains the referenced object. The collection's
		/// MyBase.Object function is called to load the object.
		/// </param>
		/// --------------------------------------------------------------------------------
		public ObjectReference(DatabaseObjects objCollection) 
            : base(objCollection.ParentDatabase, objCollection)
		{
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the database and collection class that the object is contained within.
		/// </summary>
		/// <param name="objDatabase">
		/// The database that the collection is associated with.
		/// </param>
		/// <param name="objCollection">
		/// The collection that contains the referenced object. The class' MyBase.Object
		/// function is called to load the object.
		/// </param>
		/// --------------------------------------------------------------------------------
		public ObjectReference(Database objDatabase, IDatabaseObjects objCollection) 
            : base(objDatabase, objCollection)
		{
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Sets or returns the referenced object.
		/// When reading the value, if the object has not already been loaded then the object
		/// is loaded. On subsequent calls the already loaded object is returned.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public virtual new T Object
		{
			get
			{
				return (T)base.Object;
			}
				
			set
			{
				base.Object = value;
			}
		}
	}
}
