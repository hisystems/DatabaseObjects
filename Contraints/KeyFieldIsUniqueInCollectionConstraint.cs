// ___________________________________________________
//
//  (c) Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.Constraints
{
	/// <summary>
	/// Ensures that the key field (KeyFieldName for parent collection class) value is unique within the collection.
	/// The value passed to the ConstraintBinding must by the key field value.
	/// The constraint ensure s that if the object is new then the key field value must be unique within the entire collection (the collection's SubSet applies).
	/// Otherwise, if the new object is already saved and the key field value has changed then it is ensured to be unique within the collection (the collection's SubSet applies).
	/// </summary>
	/// <remarks></remarks>
	/// <typeparam name="T">The key field data type (typically string)</typeparam>
	public class KeyFieldIsUniqueInCollectionConstraint<T> : IConstraint<T>
	{
		private DatabaseObject pobjDatabaseObject;
			
		/// <summary>
		/// Indicates the object that will be ensured to be unique within the collection based on the key field value.
		/// </summary>
		/// <param name="objDatabaseObject">
		/// The object that must be unique based on the key field value.
		/// </param>
		public KeyFieldIsUniqueInCollectionConstraint(DatabaseObject objDatabaseObject)
		{
			if (objDatabaseObject == null)
				throw new ArgumentNullException();
				
			pobjDatabaseObject = objDatabaseObject;
		}
			
		public bool ValueSatisfiesConstraint(T value)
		{
			IDatabaseObjects objParentCollection = pobjDatabaseObject.ParentCollection;
				
			SQL.SQLSelect objSelect = new SQL.SQLSelect();
			objSelect.Fields.Add(objParentCollection.DistinctFieldName());
			objSelect.Tables.Add(objParentCollection.TableName());
				
			SQL.SQLConditions objSubset = objParentCollection.Subset();
			if (objSubset != null && !objSubset.IsEmpty)
				objSelect.Where.Add(objSubset);

            objSelect.Where.Add(objParentCollection.KeyFieldName(), SQL.ComparisonOperator.EqualTo, value);
				
			//objFoundItem is nothing if the unique item is not found
			object objExistingObjectDistinctValue = pobjDatabaseObject.ParentDatabase.Connection.ExecuteScalarWithConnect(objSelect);
				
			//If a value was not found in the database then the value is unique
			if (objExistingObjectDistinctValue == null)
				return true;
			else
			{
				//If a value was found in the database then it should only be for this object (i.e. this field was not changed)
				if (((IDatabaseObject) pobjDatabaseObject).IsSaved)
					return objExistingObjectDistinctValue.Equals(((IDatabaseObject) pobjDatabaseObject).DistinctValue);
				else
					//If value was found in the database but the object has not been saved then the item found cannot be for this object
					//and therefore it is being used by another object
					return false;
			}
		}
	}
}
