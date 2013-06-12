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
using System.Collections.Generic;

namespace DatabaseObjects.Generic
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// Extends DatabaseObjects and wraps all calls with the type T associated
	/// this collection. For more information see DatabaseObjects.DatabaseObjects.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public abstract class DatabaseObjects<T> : DatabaseObjects where T : IDatabaseObject
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new DatabaseObjects with it's associated database.
		/// </summary>
		///
		/// <param name="objDatabase">
		/// The database that this collection is associated with.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjects(Database objDatabase) 
            : base(objDatabase)
		{
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes with it the associated root container and database.
		/// </summary>
		///
		/// <param name="rootContainer">
		/// The root object that this collection is associated with.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjects(RootContainer rootContainer) 
            : base(rootContainer)
		{
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new DatabaseObjects with it's associated parent object.
		/// The Parent property can be used to access the parent variable passed into this constructor.
		/// </summary>
		///
		/// <param name="objParent">
		/// The parent object that this collection is associated with.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected DatabaseObjects(DatabaseObject objParent)
            : base(objParent)
		{
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an instance of an object from this collection using a distinct value as
		/// specified by DistinctFieldName. If Subset has been implemented then the objDistinctValue
		/// need only be unique within the subset specified, not the entire database table.
		/// </summary>
		///
		/// <param name="objDistinctValue">
		/// The value that uniquely identifies the object within this collection. This is the value
		/// of the field defined by this collection's DistinctFieldName function.
		/// </param>
		///
		/// <example> Loads a product using a product ID of 123
		/// <code>
		/// Dim objProduct As Product = MyBase.Object(123)
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected new T Object(object objDistinctValue)
		{
			return (T)base.Object(objDistinctValue);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an instance of an object from this collection using a distinct value as
		/// specified by DistinctFieldName. If Subset has been implemented then the objDistinctValue
		/// need only be unique within the subset specified, not the entire database table.
		/// Returns Nothing/null if the distinct value does not exist in the database.
		/// This feature is what differentiates DatabaseObjects.Object() from DatabaseObjects.ObjectIfExists().
		/// </summary>
		///
		/// <param name="objDistinctValue">
		/// The value that uniquely identifies the object within this collection. This is the value
		/// of the field defined by this collection's DistinctFieldName function.
		/// </param>
		///
		/// <example> Loads a product using a product ID of 123
		/// <code>
		/// Dim objProduct As Product = MyBase.Object(123)
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected new T ObjectIfExists(object objDistinctValue)
		{
			return (T)base.ObjectIfExists(objDistinctValue);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an object using a unique key value.
		/// The key must be unique within this collection. If the collection's DatabaseObjects.Subset
		/// has been implemented then the key need only be unique within the subset specified, not the
		/// entire database table.
		/// </summary>
		///
		/// <param name="objKey">
		/// The key that identifies the object with this collection. The key is the value of the field
		/// defined by this collection's KeyFieldName.
		/// </param>
		///
		/// <example>
		/// <code>
		/// Default Public ReadOnly Property Item(ByVal strProductCode As String) As Product
		///     Get
		///
		///         Return MyBase.ObjectByKey(strProductCode)
		///
		///     End Get
		/// End Property
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected new T ObjectByKey(object objKey)
		{
			return (T)base.ObjectByKey(objKey);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an object using a unique key value.
		/// The key must be unique within this collection. If the collection's DatabaseObjects.Subset
		/// has been implemented then the key need only be unique within the subset specified, not the
		/// entire database table.
		/// Returns Nothing/null if the object does exist with the specified key.
		/// This feature is what differentiates DatabaseObjects.ObjectByKey() from DatabaseObjects.ObjectByKeyExists().
		/// </summary>
		///
		/// <param name="objKey">
		/// The key that identifies the object with this collection. The key is the value of the field
		/// defined by this collection's KeyFieldName.
		/// </param>
		///
		/// <example>
		/// <code>
		/// Default Public ReadOnly Property Item(ByVal strProductCode As String) As Product
		///     Get
		///
		///         Return MyBase.ObjectByKey(strProductCode)
		///
		///     End Get
		/// End Property
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected new T ObjectByKeyIfExists(object objKey)
		{
			return (T)base.ObjectByKeyIfExists(objKey);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Extracts the fields to save to the database from the objItem.SaveFields function.
		/// The fields are then written to the database using either an SQL INSERT or UPDATE
		/// depending on whether the object has already been saved.
		/// </summary>
		///
		/// <param name="objItem">
		/// The object to save to the database. The values saved to the database are extracted from the
		/// SQLFieldValues object returned from IDatabaseObject.SaveFields.
		/// </param>
		/// --------------------------------------------------------------------------------
		///
		protected void ObjectSave(T objItem)
		{
			this.ParentDatabase.ObjectSave(this, objItem);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// ObjectByOrdinalFirst returns the first object in the collection respectively
		/// filtered and sorted by the collection's Subset and OrderBy values. It differs
		/// from ObjectByOrdinal in that it only loads the first record from the database
		/// table not the entire table.
		/// </summary>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example>
		/// <code>
		/// 'Assuming this class is the Suppliers class
		///
		/// 'Ideal for loading default objects
		/// Dim objDefaultSupplier As Supplier = MyBase.ObjectByOrdinalFirst
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected new T ObjectByOrdinalFirst()
		{
			return (T)base.ObjectByOrdinalFirst();
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the last object in the collection respectively
		/// filtered and sorted by the collection's Subset and OrderBy values. It differs
		/// from ObjectByOrdinal in that it only loads the first record from the database
		/// table not the entire table.
		/// </summary>
		///
		/// <returns><see cref="IDatabaseObject" />    (DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example>
		/// <code>
		/// 'Assuming this class is the Suppliers class
		///
		/// 'Ideal for loading default objects
		/// Dim objDefaultSupplier As Supplier = MyBase.ObjectByOrdinalFirst
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected new T ObjectByOrdinalLast()
		{
			return (T)base.ObjectByOrdinalLast();
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Deletes an object's database record. If this collection's Subset has been
		/// implemented then the object must exist within the subset, otherwise the object
		/// will not be deleted. If the object has not been saved to the database the function
		/// will exit without executing an SQL DELETE command. After deleting the database
		/// record the object is set to Nothing. The calling function should receive the
		/// object ByRef for this to have any affect. Setting the object to Nothing
		/// minimises the possibility of the deleted object being used in code after
		/// ObjectDelete has been called.
		/// </summary>
		///
		/// <param name="objItem">
		/// The object to delete. The calling function should receive this object ByRef
		/// as the object is set to Nothing after deletion.
		/// </param>
		///
		/// <example>
		/// <code>
		/// Public Sub Delete(ByRef objProduct As Product)
		///
		///     MyBase.ObjectDelete(objProduct)
		///
		/// End Sub
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected void ObjectDelete(T objItem)
		{
            var objectToDelete = (IDatabaseObject)objItem;
            base.ObjectDelete(ref objectToDelete);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Loads an object from the current record of an IDataReader object.
		/// </summary>
		///
		/// <param name="objReader">
		/// The data to be copied into a new DatabaseObject object.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		/// --------------------------------------------------------------------------------
		///
		protected new T ObjectFromDataReader(IDataReader objReader)
		{
			return (T)base.ObjectFromDataReader(objReader);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Loads an object from the values contained in an SQLFieldValues object.
		/// This function is generally used from within an DatabaseObject.Load function when
		/// the TableJoins function has been implemented.
		/// </summary>
		///
		/// <param name="objFieldValues">
		/// The data container from which to load a new object.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		/// --------------------------------------------------------------------------------
		///
		protected new T ObjectFromFieldValues(SQL.SQLFieldValues objFieldValues)
		{
			return (T)base.ObjectFromFieldValues(objFieldValues);
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an array of objects contained within this collection.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected new T[] ObjectsArray()
		{
			IList<T> objList = this.ObjectsList();
			T[] objArray = new T[objList.Count - 1 + 1];
				
			objList.CopyTo(objArray, 0);
				
			return objArray;
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an IList object containing all of this collection's objects. This
		/// function is useful when loading a set of objects for a subset or for use with
		/// the IEnumerable interface.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected new IList<T> ObjectsList()
		{
			return this.ObjectsListConvert(base.ObjectsList());
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an Generic.Dictionary object. Each key/value pair contains a key and
		/// the object associated with the key.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected new IDictionary<object, T> ObjectsDictionary()
		{
			return this.ObjectsDictionaryConvert(base.ObjectsDictionary());
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns a Generic.Dictionary object. Each key/value pair contains a distinct
		/// value and the object associated with the distinct value.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected new IDictionary<object, T> ObjectsDictionaryByDistinctValue()
		{
			return this.ObjectsDictionaryConvert(base.ObjectsDictionaryByDistinctValue());
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Converts a non-generic IList to a generic IList.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		private IList<T> ObjectsListConvert(IList objSourceList)
		{
			var objList = new List<T>();
				
			foreach (IDatabaseObject objItem in objSourceList)
				objList.Add((T) objItem);
				
			return objList;
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Converts a non-generic IDictionary to a generic IDictionary.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		private IDictionary<object, T> ObjectsDictionaryConvert(IDictionary objSourceDictionary)
		{
			var objDictionary = new Dictionary<object, T>();
				
			foreach (DictionaryEntry objItem in objSourceDictionary)
				objDictionary.Add(objItem.Key, ((T) objItem.Value));
				
			return objDictionary;
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns a collection of objects that match the specified search criteria.
		/// This function utilises any subsets, ordering or table joins specified in this
		/// collection. To add a set of conditions to the objSearchCriteria object with
		/// higher precendance use the "Add(SQLConditions)" overloaded as this will wrap
		/// the conditions within parentheses.
		/// </summary>
		///
		/// <param name="objSearchCriteria">
		/// The criteria to search for within this collection. To add set a of conditions with
		/// with higher precendance use the "Add(SQLConditions)" overloaded function as this
		/// will wrap the conditions within parentheses.
		/// </param>
		///
		/// <returns><see cref="Collections.IList" />	(System.Collections.IList)</returns>
		///
		/// <remarks>
		/// The following wildcard characters are used when using the LIKE operator (extract
		/// from Microsoft Transact-SQL Reference):
		///
		/// <table width="659" border="1" cellspacing="1" cellpadding="4">
		///   <tr>
		///     <th width="16%" height="20">Wildcard character</th>
		///     <th width="22%">Description</th>
		///     <th width="62%">Example</th>
		///   </tr>
		///   <tr>
		///     <td>%</td>
		///     <td>Any string of zero or more characters.</td>
		///     <td>WHERE title LIKE '%computer%' finds all book titles with the word
		///         'computer' anywhere in the book title.</td>
		///   </tr>
		///   <tr>
		///     <td>_ (underscore)</td>
		///     <td>Any single character.</td>
		///     <td>WHERE au_fname LIKE '_ean' finds all four-letter first names that end
		///       with ean (Dean, Sean, and so on).</td>
		///   </tr>
		/// </table>
		/// </remarks>
		///
		/// <example>
		/// <code>
		/// Public Function Search(ByVal objSearchCriteria As Object, ByVal eType As SearchType) As IList
		///
		///     Dim objConditions As SQL.SQLConditions = New SQL.SQLConditions
		///
		///     Select Case eType
		///         Case SearchType.DescriptionPrefix
		///             objConditions.Add("ProductName", SQL.ComparisonOperator.Like, objSearchCriteria &amp; "%")
		///         Case SearchType.Description
		///             objConditions.Add("ProductName", SQL.ComparisonOperator.Like, "%" &amp; objSearchCriteria &amp; "%")
		///     End Select
		///
		///     Return MyBase.ObjectsSearch(objConditions)
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected new IList<T> ObjectsSearch(SQL.SQLConditions objSearchCriteria)
		{
			return this.ObjectsListConvert(base.ObjectsSearch(objSearchCriteria));
		}
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return an instance of the class that is associated with this
		/// collection of objects. This is the generic version of the ItemInstance function.
		/// It is suffixed with an underscore so that it does not conflict with the underlying
		/// non-generic equivalent ItemInstance function. It's purpose is indentical to the
		/// non-generic version.
		/// If this function is not overriden an object of type T will be created.
		/// The type T must have a default constructor or a constructor that accepts a type of
		/// DatabaseObjects.Generic.DatabaseObjects or a subclass.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function ItemInstance_() As Product
		///
		///     Return New Product
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected virtual T ItemInstance_()
		{
			return (T)base.ItemInstance();
		}
	}
}
	
