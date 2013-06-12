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
using DatabaseObjects;
using System.Diagnostics;
using System.Transactions;
using System.Collections.Generic;

namespace DatabaseObjects
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// Represents a database connection and provides a set of functions that work
	/// in conjunction with classes implementing IDatabaseObjects and IDatabaseObject.
	/// The Database class automatically generates and executes the required SQL
	/// statements to perform common database operations such as saving, deleting
	/// searching etc. based on the values returned via the IDatabaseObjects and
	/// IDatabaseObject interfaces.
	/// Typically, this class is only used when explicitly implementing the IDatabaseObjects
	/// and IDatabaseObject interfaces rather than inheriting from DatabaseObjects (or
	/// DatabaseObjectsEnumerable) and DatabaseObject.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public class Database
	{
		public enum ConnectionType
		{
#if MONO_TOUCH
            SQLite
#else
			SQLServer,
			MicrosoftAccess,
			MySQL,
			Pervasive,
			SQLServerCompactEdition,
			HyperSQL,
			SQLite
#endif
		}
		
		private ConnectionController pobjConnection;
		private TransactionsClass pobjTransactions;
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Creates a new Database instance specifying the database to connect to and SQL
		/// syntax to use with the database. Each function call of the Database class opens
		/// and closes a connection. Therefore, connection pooling should be enabled
		/// for optimal performance.
		/// </summary>
		///
		/// <param name="strConnectionString">
		/// A database connection string to either a Microsoft Access, SQLServer, Pervasive or MySQL
		/// database. For example, 'Provider=Microsoft.Jet.OLEDB.4.0;Data
		/// Source=northwind.mdb;Jet OLEDB:Database Password=;'.
		/// </param>
		///
		/// <param name="eConnectionType">
		/// Indicates the SQL syntax to generate for the database specified in strConnectionString.
		/// </param>
		/// --------------------------------------------------------------------------------
		///
		public Database(string strConnectionString, ConnectionType eConnectionType)
		{
			pobjConnection = new ConnectionController(strConnectionString, eConnectionType);
			pobjTransactions = new TransactionsClass(pobjConnection);
		}
		
		/// <summary>
		/// Initializes the Database instance with the database connection to utilise.
		/// The connection is not opened until it is required.
		/// The supplied connection should not be opened.
		/// </summary>
		/// <param name="objDatabaseConnection">An unopened connection to the database.</param>
		/// <remarks></remarks>
		public Database(IDbConnection objDatabaseConnection, ConnectionType eConnectionType)
		{
			pobjConnection = new ConnectionController(objDatabaseConnection, eConnectionType);
			pobjTransactions = new TransactionsClass(pobjConnection);
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an instance of an object from the collection using a distinct value (see
		/// IDatabaseObjects.DistinctFieldName). If the collection has implemented the
		/// IDatabaseObjects.Subset function then the objDistinctValue need only be unique
		/// within the collection's subset, not the entire database table.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection that contains the object.
		/// </param>
		///
		/// <param name="objDistinctValue">
		/// The value that uniquely identifies the object within the collection. This is the value
		/// of the field defined by the collection's IDatabaseObjects.DistinctFieldName function.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example> Loads a product using a product ID of 123
		/// <code>
		/// objProduct = objDatabase.Object(NorthwindDB.Products, 123)
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public IDatabaseObject Object(IDatabaseObjects objCollection, object objDistinctValue)
		{
			return ObjectFromFieldValues(objCollection, this.ObjectFieldValues(objCollection, objDistinctValue));
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an instance of an object from the collection using a distinct value (see
		/// IDatabaseObjects.DistinctFieldName). If the collection has implemented the
		/// IDatabaseObjects.Subset function then the objDistinctValue need only be unique
		/// within the collection's subset, not the entire database table.
		/// Returns Nothing/null if the distinct value does not exist in the database.
		/// This feature is what differentiates Database.Object() from Database.ObjectIfExists().
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection that contains the object.
		/// </param>
		///
		/// <param name="objDistinctValue">
		/// The value that uniquely identifies the object within the collection. This is the value
		/// of the field defined by the collection's IDatabaseObjects.DistinctFieldName function.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example> Loads a product using a product ID of 123
		/// <code>
		/// objProduct = objDatabase.Object(NorthwindDB.Products, 123)
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public IDatabaseObject ObjectIfExists(IDatabaseObjects objCollection, object objDistinctValue)
		{
			SQL.SQLFieldValues objFieldValues = this.ObjectFieldValuesIfExists(objCollection, objDistinctValue);
			
			if (objFieldValues == null)
				return null;
			else
				return ObjectFromFieldValues(objCollection, objFieldValues);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the database fields for an object from the collection using a distinct value
		/// (see IDatabaseObjects.DistinctFieldName). If the collection has implemented the
		/// IDatabaseObjects.Subset function then the objDistinctValue need only be unique
		/// within the collection's subset, not the entire database table.
		/// This is typically used to interogate the database fields before loading the
		/// object with a call to ObjectFromFieldValues.
		/// This function is rarely used and generally the Object function suffices.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection that contains the object.
		/// </param>
		///
		/// <param name="objDistinctValue">
		/// The value that uniquely identifies the object within the collection. This is the value
		/// of the field defined by the collection's IDatabaseObjects.DistinctFieldName function.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		/// --------------------------------------------------------------------------------
		///
		public SQL.SQLFieldValues ObjectFieldValues(IDatabaseObjects objCollection, object objDistinctValue)
		{
			SQL.SQLFieldValues objFieldValues = ObjectFieldValuesIfExists(objCollection, objDistinctValue);
			
			if (objFieldValues == null)
				throw new Exceptions.ObjectDoesNotExistException(objCollection, objDistinctValue);
			
			return objFieldValues;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the database fields for an object from the collection using a distinct value
		/// (see IDatabaseObjects.DistinctFieldName).
		/// Returns Nothing/null if the distinct value does not exist.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		private SQL.SQLFieldValues ObjectFieldValuesIfExists(IDatabaseObjects objCollection, object objDistinctValue)
		{
			SQL.SQLSelect objSelect = new SQL.SQLSelect();
			SQL.SQLConditions objSubset;

            SQL.SQLSelectTable objPrimaryTable = objSelect.Tables.Add(objCollection.TableName());
			objSelect.Tables.Joins = objCollection.TableJoins(objPrimaryTable, objSelect.Tables);
			objSelect.Where.Add(objCollection.DistinctFieldName(), SQL.ComparisonOperator.EqualTo, objDistinctValue);
			objSubset = objCollection.Subset();
			if (objSubset != null && !objSubset.IsEmpty)
				objSelect.Where.Add(objSubset);
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				using (IDataReader objReader = objConnection.Execute(objSelect))
				{
					if (objReader.Read())
						return FieldValuesFromDataReader(objCollection, objReader);
					else
						return null;
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether an object exists for the specified distinct value in the collection.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection that is searched for the distinct value.
		/// </param>
		///
		/// <param name="objDistinctValue">
		/// The value to search for in the collection. This is the value of the field defined
		/// by the collection's IDatabaseObjects.DistinctFieldName function.
		/// </param>
		/// --------------------------------------------------------------------------------
		///
		public bool ObjectExistsByDistinctValue(IDatabaseObjects objCollection, object objDistinctValue)
		{
			SQL.SQLSelect objSelect = new SQL.SQLSelect();
            SQL.SQLConditions objSubset;
			
			objSelect.Tables.Add(objCollection.TableName());
			objSelect.Where.Add(objCollection.DistinctFieldName(), SQL.ComparisonOperator.EqualTo, objDistinctValue);
			objSubset = objCollection.Subset();
			if (objSubset != null && !objSubset.IsEmpty)
				objSelect.Where.Add(objSubset);
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
				using (IDataReader objReader = objConnection.Execute(objSelect))
					return objReader.Read();
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Extracts the fields to save to the database from the objItem.SaveFields function.
		/// The fields are then written to the database using either an SQL INSERT or UPDATE
		/// depending on whether the object has already been saved. If the collection has
		/// implemented IDatabaseObjects.KeyFieldName then objItem's key is also validated to
		/// ensure it is not null and unique within the collection. If objCollection has
		/// implemented IDatabaseObjects.Subset then objItem should exist within objCollection.
		/// If not, a duplicate key error may occur if the obItem's key is being used in
		/// another subset in the same table. If a record is being amended
		/// (IDatabaseObject.IsSaved returns true) then the function will "AND" the collection's
		/// IDatabaseObjects.Subset conditions and the objItem's IDatabaseObject.DistinctValue
		/// value to create the WHERE clause in the UPDATE statement. Therefore, the
		/// combination of the IDatabaseObjects.Subset and IDatabaseObject.DistinctValue
		/// conditions MUST identify only one record in the table. Otherwise multiple records
		/// will be updated with the same data. If data is only inserted and not amended
		/// (usually a rare occurance) then this requirement is unnecessary.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection which contains or will contain the object to save.
		/// </param>
		///
		/// <param name="objItem">
		/// The object to save to the database. The values saved to the database are extracted from the
		/// SQLFieldValues object returned from IDatabaseObject.SaveFields.
		/// </param>
		///
		/// <example> Saves a product object (Me) to the database.
		/// <code>
		/// Public Sub Save()
		///
		///     objDatabase.ObjectSave(NorthwindDB.Products, Me)
		///
		/// End Sub
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public void ObjectSave(IDatabaseObjects objCollection, IDatabaseObject objItem)
		{
			SQL.SQLFieldValues objFieldValues;
			var objNewGUID = Guid.Empty;
			var autoAssignment = MergeDistinctFieldAutoAssignmentAndDistinctFieldAutoIncrements(objCollection);
			
			objFieldValues = objItem.SaveFields();
			
			if (objFieldValues == null)
				throw new Exceptions.DatabaseObjectsException(objItem.GetType().Name + " IDatabaseObject.SaveFields not implemented");
			
			//Add the distinct field value if it hasn't been added via the SaveFields sub
			if (!objFieldValues.Exists(objCollection.DistinctFieldName()))
			{
				if (autoAssignment == SQL.FieldValueAutoAssignmentType.None)
					objFieldValues.Add(objCollection.DistinctFieldName(), objItem.DistinctValue);
				else if (autoAssignment == SQL.FieldValueAutoAssignmentType.NewUniqueIdentifier)
				{
					//For a new object, with a GUID that should be automatically assigned
					//Create a new GUID for the distinct field so that it saved for the INSERT
					if (!objItem.IsSaved)
					{
						objNewGUID = System.Guid.NewGuid();
						objFieldValues.Add(objCollection.DistinctFieldName(), objNewGUID);
					}
				}
			}
			
#if !DEBUG
			ItemKeyEnsureValid(objCollection, objItem, objFieldValues);
#endif
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				
				if (objItem.IsSaved)
				{
					var objUpdate = new SQL.SQLUpdate();
					objUpdate.TableName = objCollection.TableName();
					objUpdate.Fields.Add(objFieldValues);
					objUpdate.Where.Add(objCollection.DistinctFieldName(), SQL.ComparisonOperator.EqualTo, objItem.DistinctValue);
					var objSubset = objCollection.Subset();
					if (objSubset != null && !objSubset.IsEmpty)
					{
						objUpdate.Where.Add(objSubset);
					}
					
					objConnection.ExecuteNonQuery(objUpdate);
				}
				else
				{
					var objInsert = new SQL.SQLInsert();
					objInsert.TableName = objCollection.TableName();
					objInsert.Fields = objFieldValues;
					objConnection.ExecuteNonQuery(objInsert);
					
					if (autoAssignment == SQL.FieldValueAutoAssignmentType.NewUniqueIdentifier)
						objItem.DistinctValue = objNewGUID;
					else if (autoAssignment == SQL.FieldValueAutoAssignmentType.AutoIncrement)
						objItem.DistinctValue = Connection.ExecuteScalar(new SQL.SQLAutoIncrementValue());
					
					object objRollbackDistinctValue = objItem.DistinctValue;
					objItem.IsSaved = true;
					
					if (Transaction.Current != null)
					{
						Transaction.Current.EnlistVolatile(new TransactionExecuteActionOnRollback(() => objItem.IsSaved = false), EnlistmentOptions.None);
						Transaction.Current.EnlistVolatile(new TransactionExecuteActionOnRollback(() => objItem.DistinctValue = objRollbackDistinctValue), EnlistmentOptions.None);
					}
				}
			}
		}
		
		/// <summary>
		/// Merges the obsolete function DistinctFieldAutoIncrements with the new DistinctFieldAutoAssignment function.
		/// </summary>
		private SQL.FieldValueAutoAssignmentType MergeDistinctFieldAutoAssignmentAndDistinctFieldAutoIncrements(IDatabaseObjects collection)
		{
			if (collection.DistinctFieldAutoIncrements())
				return SQL.FieldValueAutoAssignmentType.AutoIncrement;
			else
				return collection.DistinctFieldAutoAssignment();
		}
		
		private void ItemKeyEnsureValid(IDatabaseObjects objCollection, IDatabaseObject objItem, SQL.SQLFieldValues objFieldValues)
		{
			SQL.SQLSelect objSelect;
			object objKeyFieldValue;
            SQL.SQLConditions objSubset;
			
			//If the key field is set and the key field is specified in the object
			if (objCollection.KeyFieldName() != string.Empty && objFieldValues.Exists(objCollection.KeyFieldName()))
			{
				objKeyFieldValue = ItemKeyFieldValue(objCollection, objItem, objFieldValues);
				
				if (objKeyFieldValue is string)
				{
					if (String.IsNullOrEmpty((string)objKeyFieldValue))
						throw new Exceptions.DatabaseObjectsException(objItem.GetType().Name + " " + objCollection.KeyFieldName() + " field is Null");
				}
				
				objSelect = new SQL.SQLSelect();
				
				objSelect.Tables.Add(objCollection.TableName());
				objSelect.Fields.Add(objCollection.KeyFieldName());
				objSelect.Where.Add(objCollection.KeyFieldName(), SQL.ComparisonOperator.EqualTo, objKeyFieldValue);
				objSubset = objCollection.Subset();
				if (objSubset != null && !objSubset.IsEmpty)
					objSelect.Where.Add(objSubset);
				
				if (objItem.IsSaved)
					objSelect.Where.Add(objCollection.DistinctFieldName(), SQL.ComparisonOperator.NotEqualTo, objItem.DistinctValue);
				
				using (ConnectionScope objConnection = new ConnectionScope(this))
					using (IDataReader objReader = objConnection.Execute(objSelect))
						if (objReader.Read())
							throw new Exceptions.ObjectAlreadyExistsException(objItem, objKeyFieldValue);
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an object from the collection using a unique key value.
		/// The key must be unique within the collection. If the collection's
		/// IDatabaseObjects.Subset has been implemented then the key need only be unique
		/// within the subset specified, not the entire database table.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection which contains the object.
		/// </param>
		///
		/// <param name="objKey">
		/// The key that identifies the object with the collection. The key is the value of
		/// the field defined by the collection's IDatabaseObjects.KeyFieldName.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example>
		/// <code>
		/// Default Public ReadOnly Property Item(ByVal strProductCode As String) As Product
		///     Get
		///
		///         Return objDatabase.ObjectByKey(Me, strProductCode)
		///
		///     End Get
		/// End Property
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public IDatabaseObject ObjectByKey(IDatabaseObjects objCollection, object objKey)
		{
			IDatabaseObject objObject = ObjectByKeyIfExists(objCollection, objKey);
			
			if (objObject == null)
				throw new Exceptions.ObjectDoesNotExistException(objCollection, objKey);
			
			return objObject;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an object from the collection using a unique key value.
		/// The key must be unique within the collection. If the collection's
		/// IDatabaseObjects.Subset has been implemented then the key need only be unique
		/// within the subset specified, not the entire database table.
		/// Returns Nothing/null if the object does exist with the specified key.
		/// This feature is what differentiates Database.ObjectByKey() from Database.ObjectByKeyExists().
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection which contains the object.
		/// </param>
		///
		/// <param name="objKey">
		/// The key that identifies the object with the collection. The key is the value of
		/// the field defined by the collection's IDatabaseObjects.KeyFieldName.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example>
		/// <code>
		/// Default Public ReadOnly Property Item(ByVal strProductCode As String) As Product
		///     Get
		///
		///         Return objDatabase.ObjectByKey(Me, strProductCode)
		///
		///     End Get
		/// End Property
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public IDatabaseObject ObjectByKeyIfExists(IDatabaseObjects objCollection, object objKey)
		{
			var objSelect = new SQL.SQLSelect();
			string keyFieldName = objCollection.KeyFieldName();
			
			EnsureKeyFieldNameIsSet(keyFieldName, objCollection);
			
			SQL.SQLSelectTable objPrimaryTable = objSelect.Tables.Add(objCollection.TableName());
			objSelect.Tables.Joins = objCollection.TableJoins(objPrimaryTable, objSelect.Tables);
			objSelect.Where.Add(keyFieldName, SQL.ComparisonOperator.EqualTo, objKey);
			var objSubset = objCollection.Subset();
			if (objSubset != null && !objSubset.IsEmpty)
			{
				objSelect.Where.Add(objSubset);
			}
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				using (IDataReader objReader = objConnection.Execute(objSelect))
				{
					if (objReader.Read())
						return ObjectFromDataReader(objCollection, objReader);
					else
						return null;
				}
			}
		}
		
		/// <summary>
		/// Throwns an exception if the key field name is "".
		/// </summary>
		private void EnsureKeyFieldNameIsSet(string keyFieldName, IDatabaseObjects collection)
		{
			if (String.IsNullOrEmpty(keyFieldName))
				throw new InvalidOperationException("The KeyFieldAttribute has not been specified or the KeyFieldName function overridden for " + collection.GetType().FullName);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// ObjectByOrdinalFirst returns the first object in the collection respectively
		/// filtered and sorted by the collection's IDatabaseObjects.Subset and
		/// IDatabaseObjects.OrderBy values. It differs from ObjectByOrdinal in that it only
		/// loads the first record from the database table not the entire table.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection which contains the object.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example>
		/// <code>
		/// 'Ideal for loading default objects
		/// Dim objDefaultSupplier As Supplier = objDatabase.ObjectByOrdinalFirst(objGlobalSuppliersInstance)
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public IDatabaseObject ObjectByOrdinalFirst(IDatabaseObjects objCollection)
		{
			var objSelect = new SQL.SQLSelect();
			
			//only select the first row of the recordset
			objSelect.Top = 1;
			SQL.SQLSelectTable objPrimaryTable = objSelect.Tables.Add(objCollection.TableName());
			objSelect.Tables.Joins = objCollection.TableJoins(objPrimaryTable, objSelect.Tables);
			objSelect.Where = objCollection.Subset();
			objSelect.OrderBy = objCollection.OrderBy();
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				using (IDataReader objReader = objConnection.Execute(objSelect))
				{
					if (objReader.Read())
						return ObjectFromDataReader(objCollection, objReader);
					else
						throw new Exceptions.ObjectDoesNotExistException(objCollection, "TOP 1");
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the last object in the collection respectively
		/// filtered and sorted by the collection's IDatabaseObjects.Subset and
		/// IDatabaseObjects.OrderBy values. It differs from ObjectByOrdinal in that it only
		/// loads the first record from the database table not the entire table.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection which contains the object.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" /> (DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example>
		/// <code>
		/// 'Ideal for loading default objects
		/// Dim objDefaultSupplier As Supplier = objDatabase.ObjectByOrdinalFirst(objGlobalSuppliersInstance)
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public IDatabaseObject ObjectByOrdinalLast(IDatabaseObjects objCollection)
		{
			var objSelect = new SQL.SQLSelect();
			
			//only select the first row of the recordset
			objSelect.Top = 1;
			SQL.SQLSelectTable objPrimaryTable = objSelect.Tables.Add(objCollection.TableName());
			objSelect.Tables.Joins = objCollection.TableJoins(objPrimaryTable, objSelect.Tables);
			objSelect.Where = objCollection.Subset();

            SQL.SQLSelectOrderByFields objOrderBy = objCollection.OrderBy();
			if (objOrderBy != null)
			{
				objOrderBy.OrderingReverseAll();
				objSelect.OrderBy = objOrderBy;
			}
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				using (IDataReader objReader = objConnection.Execute(objSelect))
				{
					if (objReader.Read())
						return ObjectFromDataReader(objCollection, objReader);
					else
						throw new Exceptions.ObjectDoesNotExistException(objCollection, "TOP 1 with reversed ordering");
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the number of items in the collection. If the collection's
		/// IDatabaseObjects.Subset has been implemented then this function returns the
		/// number of records within the subset, not the entire table.
		/// Also utilises the table joins so that any filters specified on the subset
		/// can be used.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The object for which the number of records are returned.
		/// </param>
		///
		/// <returns><see cref="Int32" />	(System.Int32)</returns>
		///
		/// <example>
		/// <code>
		/// 'Return the number of items in this collection.
		/// Public ReadOnly Property Count() As Integer
		///     Get
		///
		///         Return objDatabase.ObjectsCount(Me)
		///
		///     End Get
		/// End Property
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public int ObjectsCount(IDatabaseObjects objCollection)
		{
			var objSelect = new SQL.SQLSelect();
			
			objSelect.Where = objCollection.Subset();
			objSelect.Fields.Add(string.Empty, SQL.AggregateFunction.Count);
			objSelect.Tables.Add(objCollection.TableName());
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				using (IDataReader objReader = objConnection.Execute(objSelect))
				{
					objReader.Read();
					return System.Convert.ToInt32(objReader[0]);
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether the key exists within the collection. If the collection's
		/// IDatabaseObjects.Subset has been set then only the subset is searched not the
		/// entire table.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection to search within.
		/// </param>
		///
		/// <param name="objKey">
		/// The key value to search by.
		/// </param>
		///
		/// <returns><see cref="Boolean" />	(System.Boolean)</returns>
		///
		/// <example>
		/// <code>
		/// Public Function Exists(ByVal strProductCode As String) As Boolean
		///
		///     Return objDatabase.ObjectExists(Me, strProductCode)
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public bool ObjectExists(IDatabaseObjects objCollection, object objKey)
		{
			var objSelect = new SQL.SQLSelect();
			string keyFieldName = objCollection.KeyFieldName();
			
			EnsureKeyFieldNameIsSet(keyFieldName, objCollection);
			
			objSelect.Tables.Add(objCollection.TableName());
			//.Fields.Add objCollection.DistinctFieldName
			objSelect.Where.Add(keyFieldName, SQL.ComparisonOperator.EqualTo, objKey);
			var objSubset = objCollection.Subset();
			if (objSubset != null && !objSubset.IsEmpty)
			{
				objSelect.Where.Add(objSubset);
			}
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
				using (IDataReader objReader = objConnection.Execute(objSelect))
					return objReader.Read();
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Deletes an object's database record. If the collection's IDatabaseObjects.Subset
		/// has been implemented then the object must exist within the subset, otherwise the
		/// object will not be deleted. If the object has not been saved to the database the
		/// function will exit without executing an SQL DELETE command. After deleting the
		/// database record the object is set to Nothing. The calling function should receive
		/// the object ByRef for this to have any affect. Setting the object to Nothing
		/// minimises the possibility of the deleted object being used in code after
		/// ObjectDelete has been called.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection that contains the object to delete. If the item does not exist
		/// within the collection then the object will not be deleted.
		/// </param>
		///
		/// <param name="objItem">
		/// The object to delete. The calling function should receive this object ByRef
		/// as the object is set to Nothing after deletion.
		/// Reference Type: <see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)
		/// </param>
		///
		/// <example>
		/// <code>
		/// Public Sub Delete(ByRef objProduct As Product)
		///
		///     objDatabase.ObjectDelete(Me, objProduct)
		///     'objProduct will now be Nothing
		///
		/// End Sub
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public void ObjectDelete(IDatabaseObjects objCollection, ref IDatabaseObject objItem)
		{
			if (objItem.IsSaved)
			{
				SQL.SQLDelete objDelete = new SQL.SQLDelete();
                SQL.SQLConditions objSubset;
				
				objDelete.TableName = objCollection.TableName();
				objDelete.Where.Add(objCollection.DistinctFieldName(), SQL.ComparisonOperator.EqualTo, objItem.DistinctValue);
				objSubset = objCollection.Subset();
				if (objSubset != null && !objSubset.IsEmpty)
					objDelete.Where.Add(objSubset);
				
				using (ConnectionScope objConnection = new ConnectionScope(this))
					objConnection.ExecuteNonQuery(objDelete);
				
				objItem.IsSaved = false;
				
				if (Transaction.Current != null)
				{
					IDatabaseObject objItemCopy = objItem;
					Transaction.Current.EnlistVolatile(new TransactionExecuteActionOnRollback(() => objItemCopy.IsSaved = true), EnlistmentOptions.None);
				}
			}
			
			//The function that calls ObjectDelete objItem MUST be ByRef for this to have any effect
			objItem = null;
		}

		private class TransactionExecuteActionOnRollback : IEnlistmentNotification
		{
			private Action pobjAction;
			
			public TransactionExecuteActionOnRollback(Action objAction)
			{
				if (objAction == null)
					throw new ArgumentNullException();
				
				pobjAction = objAction;
			}
			
			public void Commit(System.Transactions.Enlistment enlistment)
			{
			}
			
			public void InDoubt(System.Transactions.Enlistment enlistment)
			{
			}
			
			public void Prepare(System.Transactions.PreparingEnlistment preparingEnlistment)
			{
				preparingEnlistment.Prepared();
			}
			
			public void Rollback(System.Transactions.Enlistment enlistment)
			{
				pobjAction.Invoke();
			}
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Deletes all of the objects in the collection. If IDatabaseObjects.Subset
		/// has been implemented then only the objects within the subset are deleted, not
		/// the table's entire contents.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection from which all objects are to be deleted.
		/// </param>
		/// --------------------------------------------------------------------------------
		///
		public void ObjectsDeleteAll(IDatabaseObjects objCollection)
		{
			SQL.SQLDelete objDelete = new SQL.SQLDelete();
			
			objDelete.TableName = objCollection.TableName();
			objDelete.Where = objCollection.Subset();
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
				objConnection.ExecuteNonQuery(objDelete);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an IList object containing all of the collection's associated child
		/// objects. This function is useful when loading a set of objects for a subset or
		/// for use with the IEnumerable interface.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection which contains the objects to load.
		/// </param>
		///
		/// <returns><see cref="Collections.IList" />	(System.Collections.IList)</returns>
		///
		/// <example>
		/// <code>
		/// 'Can be used to provide an enumerator for use with the "For Each" clause
		/// Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		///
		///     Return objDatabase.ObjectsList(objGlobalProductsInstance).GetEnumerator
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public IList ObjectsList(IDatabaseObjects objCollection)
		{
			IList objArrayList = new ArrayList();
			SQL.SQLSelect objSelect = new SQL.SQLSelect();
			
			SQL.SQLSelectTable objPrimaryTable = objSelect.Tables.Add(objCollection.TableName());
			objSelect.Tables.Joins = objCollection.TableJoins(objPrimaryTable, objSelect.Tables);
			objSelect.Where = objCollection.Subset();
			objSelect.OrderBy = objCollection.OrderBy();
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				using (IDataReader objReader = objConnection.Execute(objSelect))
				{
					while (objReader.Read())
						objArrayList.Add(ObjectFromDataReader(objCollection, objReader));
					
					return objArrayList;
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an array of IDatabaseObject objects contained within this collection.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public IDatabaseObject[] ObjectsArray(IDatabaseObjects objCollection)
		{
			IList objList = this.ObjectsList(objCollection);
			IDatabaseObject[] objArray = new IDatabaseObject[objList.Count - 1 + 1];
			
			objList.CopyTo(objArray, 0);
			
			return objArray;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an IDictionary object. Each key/value pair contains a key and
		/// the object associated with the key.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection which specifies the objects to load.
		/// </param>
		///
		/// <returns><see cref="Collections.IDictionary" />	(System.Collections.IDictionary)</returns>
		/// --------------------------------------------------------------------------------
		///
		public IDictionary ObjectsDictionary(IDatabaseObjects objCollection)
		{
			return ObjectsDictionaryBase(objCollection);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an IDictionary object. Each key/value pair contains a distinct
		/// value and the object associated with the distinct value.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection which specifies the objects to load.
		/// </param>
		///
		/// <returns><see cref="Collections.IDictionary" />	(System.Collections.IDictionary)</returns>
		/// --------------------------------------------------------------------------------
		///
		public IDictionary ObjectsDictionaryByDistinctValue(IDatabaseObjects objCollection)
		{
			return ObjectsDictionaryBase(objCollection, true);
		}
		
		private IDictionary ObjectsDictionaryBase(IDatabaseObjects objCollection, bool bKeyIsDistinctField = false)
		{
			//Returns an IDictionary with the key being either the DistinctField or KeyField
			
			IDictionary objDictionary = new Hashtable();
			SQL.SQLSelect objSelect = new SQL.SQLSelect();
			string strKeyField;
			
			SQL.SQLSelectTable objPrimaryTable = objSelect.Tables.Add(objCollection.TableName());
			objSelect.Tables.Joins = objCollection.TableJoins(objPrimaryTable, objSelect.Tables);
			objSelect.Where = objCollection.Subset();
			objSelect.OrderBy = objCollection.OrderBy();
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				using (IDataReader objReader = objConnection.Execute(objSelect))
				{
					if (bKeyIsDistinctField)
						strKeyField = objCollection.DistinctFieldName();
					else
						strKeyField = objCollection.KeyFieldName();
					
					while (objReader.Read())
						objDictionary.Add(objReader[strKeyField], ObjectFromDataReader(objCollection, objReader));
					
					return objDictionary;
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns a collection of objects that match the specified search criteria.
		/// This function utilises any subsets, ordering or table joins specified in the
		/// collection. To add a set of conditions to the objSearchCriteria object with
		/// higher precendance use the "Add(SQLConditions)" overloaded function as this will
		/// wrap the conditions within parentheses.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection to search within.
		/// </param>
		///
		/// <param name="objSearchCriteria">
		/// The criteria to search for within the collection. To add a set of conditions with
		/// with higher precendance use the "Add(SQLConditions)" overloaded function as this
		/// will wrap the conditions within parentheses.
		/// </param>
		///
		/// <returns><see cref="Collections.IList" />	(System.Collections.IList)</returns>
		///
		/// <remarks>
		/// The following wildcard characters are used when using the LIKE operator (extract
		/// from Microsoft Transact-SQL Reference)
		///
		///
		/// <font size="1">
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
		/// </font>
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
		///     Return objDatabase.ObjectsSearch(objGlobalProductsInstance, objConditions)
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		public IList ObjectsSearch(IDatabaseObjects objCollection, SQL.SQLConditions objSearchCriteria)
		{
			var objSelect = new SQL.SQLSelect();
			ArrayList objResults = new ArrayList();
			
			if (objSearchCriteria.IsEmpty)
				throw new ArgumentException("Search criteria is empty");
			
			SQL.SQLSelectTable objPrimaryTable = objSelect.Tables.Add(objCollection.TableName());
			objSelect.Tables.Joins = objCollection.TableJoins(objPrimaryTable, objSelect.Tables);
			objSelect.OrderBy = objCollection.OrderBy();
			objSelect.Where = objCollection.Subset();
			
			if (objSearchCriteria != null)
			{
				if (objSelect.Where == null)
					objSelect.Where = new SQL.SQLConditions();

				objSelect.Where.Add(objSearchCriteria);
			}
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				using (IDataReader objReader = objConnection.Execute(objSelect))
				{
					while (objReader.Read())
						objResults.Add(ObjectFromDataReader(objCollection, objReader));
					
					return objResults;
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Locks the database record associated with this object by selecting and locking
		/// the row in the database. Supported in Microsoft SQLServer, Pervasive and MySQL.
		/// The record lock is released when the transaction is committed or rolled back.
		/// Throws an exception if not in transaction mode.
		/// Returns the field values from the record that has been locked.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public SQL.SQLFieldValues ObjectLockRecord(IDatabaseObjects objCollection, IDatabaseObject objItem)
		{
			SQL.SQLSelect objSelect = new SQL.SQLSelect();
            SQL.SQLConditions objSubset;
			
			objSelect.PerformLocking = true;
			objSelect.Tables.Add(objCollection.TableName());
			objSelect.Where.Add(objCollection.DistinctFieldName(), SQL.ComparisonOperator.EqualTo, objItem.DistinctValue);
			objSubset = objCollection.Subset();
			if (objSubset != null && !objSubset.IsEmpty)
				objSelect.Where.Add(objSubset);
			
			using (var objReader = this.Transactions.Execute(objSelect))
			{
				if (objReader.Read())
					return FieldValuesFromDataReader(objCollection, objReader);
				else
					throw new Exceptions.ObjectDoesNotExistException(objCollection, objItem.DistinctValue);
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets and returns the field value from the database record associated with the
		/// object and collection.
		/// </summary>
		/// <param name="objItem">
		/// The object which represents the database record to be read. Specifically,
		/// the object's distinct field name is used to determine which record to read.
		/// </param>
		/// <param name="strFieldName">
		/// The name of the database field that is to be read.
		/// </param>
		/// <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved.</exception>
		/// --------------------------------------------------------------------------------
		public object ObjectGetFieldValue(DatabaseObject objItem, string strFieldName)
		{
			return this.ObjectGetFieldValue(objItem.ParentCollection, objItem, strFieldName);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets and returns the field value from the database record associated with the
		/// object and collection.
		/// </summary>
		/// <param name="objCollection">
		/// The collection that the object exists within.
		/// The function utilises the collection's subset and tablename to determine which
		/// table and record to read.
		/// Returns DBNull.Value if the field is NULL.
		/// </param>
		/// <param name="objItem">
		/// The object which represents the database record to be read. Specifically,
		/// the object's distinct field name is used to determine which record to read.
		/// </param>
		/// <param name="strFieldName">
		/// The name of the database field that is to be read.
		/// </param>
		/// <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved.</exception>
		/// --------------------------------------------------------------------------------
		public object ObjectGetFieldValue(IDatabaseObjects objCollection, IDatabaseObject objItem, string strFieldName)
		{
			if (!objItem.IsSaved)
				throw new Exceptions.ObjectDoesNotExistException(objItem);
			
			SQL.SQLSelect objSelect = new SQL.SQLSelect();
            SQL.SQLConditions objSubset;
			
			objSelect.Fields.Add(strFieldName);
			objSelect.Tables.Add(objCollection.TableName());
			objSelect.Where.Add(objCollection.DistinctFieldName(), SQL.ComparisonOperator.EqualTo, objItem.DistinctValue);
			objSubset = objCollection.Subset();
			if (objSubset != null && !objSubset.IsEmpty)
				objSelect.Where.Add(objSubset);
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
			{
				using (IDataReader objReader = objConnection.Execute(objSelect))
				{
					if (objReader.Read())
						return objReader[0];
					else
						throw new Exceptions.ObjectDoesNotExistException(objCollection, objItem.DistinctValue);
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Sets the field value for the database record associated with the object and
		/// collection.
		/// </summary>
		/// <param name="objItem">
		/// The object which represents the database record to be set. Specifically,
		/// the object's distinct field name is used to determine which record to modify.
		/// </param>
		/// <param name="strFieldName">
		/// The name of the database field that is to be set.
		/// </param>
		/// <param name="objNewValue">
		/// The new value that the database field it to be set to.
		/// If Nothing/null then the field is set to NULL.
		/// </param>
		/// <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved</exception>
		/// --------------------------------------------------------------------------------
		public void ObjectSetFieldValue(DatabaseObject objItem, string strFieldName, object objNewValue)
		{
			this.ObjectSetFieldValue(objItem.ParentCollection, objItem, strFieldName, objNewValue);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Sets the field value for the database record associated with the object and
		/// collection.
		/// </summary>
		/// <param name="objCollection">
		/// The collection that the object exists within.
		/// The function utilises the collection's subset and tablename to determine which
		/// table and record to modify.
		/// </param>
		/// <param name="objItem">
		/// The object which represents the database record to be set. Specifically,
		/// the object's distinct field name is used to determine which record to modify.
		/// </param>
		/// <param name="strFieldName">
		/// The name of the database field that is to be set.
		/// </param>
		/// <param name="objNewValue">
		/// The new value that the database field it to be set to.
		/// If Nothing/null then the field is set to NULL.
		/// </param>
		/// <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved</exception>
		/// --------------------------------------------------------------------------------
		public void ObjectSetFieldValue(IDatabaseObjects objCollection, IDatabaseObject objItem, string strFieldName, object objNewValue)
		{
			if (!objItem.IsSaved)
				throw new Exceptions.ObjectDoesNotExistException(objItem);
			
            SQL.SQLUpdate objUpdate = new SQL.SQLUpdate();
			objUpdate.TableName = objCollection.TableName();
			objUpdate.Fields.Add(strFieldName, objNewValue);
			objUpdate.Where.Add(objCollection.DistinctFieldName(), SQL.ComparisonOperator.EqualTo, objItem.DistinctValue);

			var objSubset = objCollection.Subset();
			if (objSubset != null && !objSubset.IsEmpty)
				objUpdate.Where.Add(objSubset);
			
			using (ConnectionScope objConnection = new ConnectionScope(this))
				objConnection.ExecuteNonQuery(objUpdate);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Creates and initializes an object from the current record of a DataRow object.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection associated with the IDataReader object.
		/// </param>
		///
		/// <param name="objRow">
		/// The data to be copied into a new IDatabaseObject object.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		/// --------------------------------------------------------------------------------
		///
		public static IDatabaseObject ObjectFromDataRow(IDatabaseObjects objCollection, System.Data.DataRow objRow)
		{
			System.Data.DataTable objTable = objRow.Table.Clone();
			objTable.Rows.Add(objRow);

			return ObjectFromFieldValues(objCollection, FieldValuesFromDataReader(objCollection, objTable.CreateDataReader()));
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Creates and initializes an object from the current record of an IDataReader object.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection associated with the IDataReader object.
		/// </param>
		///
		/// <param name="objReader">
		/// The data to be copied into a new IDatabaseObject object.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		/// --------------------------------------------------------------------------------
		///
		public static IDatabaseObject ObjectFromDataReader(IDatabaseObjects objCollection, IDataReader objReader)
		{
			return ObjectFromFieldValues(objCollection, FieldValuesFromDataReader(objCollection, objReader));
		}
		
		private static SQL.SQLFieldValues FieldValuesFromDataReader(IDatabaseObjects objCollection, IDataReader objReader)
		{
			string strFieldName;
			string strTablePrefix;
			SQL.SQLFieldValues objFieldValues;
			
			//check that the distinct field name exists
			if (!FieldExists(objReader, objCollection.DistinctFieldName()))
				throw new Exceptions.DatabaseObjectsException(System.Convert.ToString(((object) objCollection).GetType().Name + ".DistinctFieldName '" + objCollection.DistinctFieldName() + "' is invalid"));
			
			objFieldValues = new SQL.SQLFieldValues();
			strTablePrefix = objCollection.TableName() + ".";
			
			//Copy the recordset values into the SQL.SQLFieldValues object
			for (int intIndex = 0; intIndex < objReader.FieldCount; intIndex++)
			{
				//If the recordset has been loaded with a join then it may be prefixed with
				//the table name - this is the case with Microsoft Access
				//If so remove the table name if the table prefix is the same as objCollection.TableName
				//All of the other joined fields with tablename prefixes on the fields will remain. This is ok considering
				//most of the time an inner join has been performed where the field names are equal in the 2 joined tables
				strFieldName = objReader.GetName(intIndex);
				if (strFieldName.IndexOf(strTablePrefix) == 0)
					objFieldValues.Add(strFieldName.Substring(strTablePrefix.Length), objReader[intIndex]);
				else
					objFieldValues.Add(strFieldName, objReader[intIndex]);
			}
			
			return objFieldValues;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Creates and initializes an object from the values contained in an SQLFieldValues object.
		/// This function is generally used from within an IDatabaseObject.Load function when
		/// the IDatabaseObjects.TableJoins function has been implemented.
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection associated with the field values.
		/// </param>
		///
		/// <param name="objFieldValues">
		/// The data container from which to load a new object.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		/// --------------------------------------------------------------------------------
		public static IDatabaseObject ObjectFromFieldValues(IDatabaseObjects objCollection, SQL.SQLFieldValues objFieldValues)
		{
			IDatabaseObject objItem;
			
			if (objCollection is IDatabaseObjectsMultipleSubclass)
				objItem = ((IDatabaseObjectsMultipleSubclass) objCollection).ItemInstanceForSubclass(objFieldValues);
			else
				objItem = objCollection.ItemInstance();
			
			ObjectLoad(objCollection, objItem, objFieldValues);
			
			return objItem;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes an existing object with values from a set of database fields.
		/// Specifically, sets the IDatbaseObject.IsSaved property to true,
		/// sets the IDatbaseObject.DistinctValue using the provided data and
		/// calls IDatbaseObject.LoadFields().
		/// </summary>
		///
		/// <param name="objItem">
		/// The object into which the data should be copied into.
		/// </param>
		///
		/// <param name="objFieldValues">
		/// The data container that contains the data to be copied into the object.
		/// </param>
		/// --------------------------------------------------------------------------------
		public static void ObjectLoad(DatabaseObject objItem, SQL.SQLFieldValues objFieldValues)
		{
			ObjectLoad(objItem.ParentCollection, objItem, objFieldValues);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes an existing object with values from a set of database fields.
		/// Specifically, sets the IDatbaseObject.IsSaved property to true,
		/// sets the IDatbaseObject.DistinctValue using the provided data and
		/// calls IDatbaseObject.LoadFields().
		/// </summary>
		///
		/// <param name="objItem">
		/// The object into which the data should be copied into.
		/// </param>
		///
		/// <param name="objData">
		/// The data container that contains the data to be copied into the object.
		/// </param>
		/// --------------------------------------------------------------------------------
		public static void ObjectLoad(DatabaseObject objItem, IDataReader objData)
		{
			ObjectLoad(objItem, FieldValuesFromDataReader(objItem.ParentCollection, objData));
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes an existing object with values from a set of database fields.
		/// Specifically, sets the IDatbaseObject.IsSaved property to true,
		/// sets the IDatbaseObject.DistinctValue using the provided data and
		/// calls IDatbaseObject.LoadFields().
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection associated with the field values. This is not used
		/// to create an object - but to set the distinct field for the object using the
		/// IDatabaseObjects.DistinctFieldName property.
		/// </param>
		///
		/// <param name="objFieldValues">
		/// The data container that contains the data to be copied into the object.
		/// </param>
		/// --------------------------------------------------------------------------------
		public static void ObjectLoad(IDatabaseObjects objCollection, IDatabaseObject objItem, SQL.SQLFieldValues objFieldValues)
		{
			if (objFieldValues == null)
				throw new ArgumentNullException();
			
			objItem.IsSaved = true;
			objItem.DistinctValue = objFieldValues[objCollection.DistinctFieldName()].Value;
			objItem.LoadFields(objFieldValues);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes an existing object with values from a set of database fields.
		/// Specifically, sets the IDatbaseObject.IsSaved property to true,
		/// sets the IDatbaseObject.DistinctValue using the provided data and
		/// calls IDatbaseObject.LoadFields().
		/// </summary>
		///
		/// <param name="objCollection">
		/// The collection associated with the field values. This is not used
		/// to create an object - but to set the distinct field for the object using the
		/// IDatabaseObjects.DistinctFieldName property.
		/// </param>
		///
		/// <param name="objData">
		/// The data container that contains the data to be copied into the object.
		/// </param>
		/// --------------------------------------------------------------------------------
		public static void ObjectLoad(IDatabaseObjects objCollection, IDatabaseObject objItem, IDataReader objData)
		{
			ObjectLoad(objCollection, objItem, FieldValuesFromDataReader(objCollection, objData));
		}
		
		private static bool FieldExists(IDataReader objReader, string strFieldName)
		{
			bool bExists = false;
			string strReaderFieldName;
			
			for (int intIndex = 0; intIndex < objReader.FieldCount; intIndex++)
			{
				strReaderFieldName = objReader.GetName(intIndex);

				if (strReaderFieldName.IndexOf('.') >= 0)
					strReaderFieldName = strReaderFieldName.Split('.')[1];

				if (string.Compare(strReaderFieldName, strFieldName, true) == 0)
				{
					bExists = true;
					break;
				}
			}
			
			return bExists;
		}
		
		private object ItemKeyFieldValue(IDatabaseObjects objCollection, IDatabaseObject objItem, SQL.SQLFieldValues objFieldValues)
		{
			//On the rare occurance that the KeyField is the same as the DistinctField
			//then the key value may not have been set in the Save and therefore be
			//available in the objFieldValues collection. In which case the
			//key has to be extracted from the objItem.DistinctField.
			object objKeyFieldValue;
			
			if (string.Compare(objCollection.DistinctFieldName(), objCollection.KeyFieldName(), true) == 0)
				objKeyFieldValue = objItem.DistinctValue;
			else
				objKeyFieldValue = objFieldValues[objCollection.KeyFieldName()].Value;
			
			return objKeyFieldValue;
		}
		
		public TransactionsClass Transactions
		{
			get
			{
				return pobjTransactions;
			}
		}
		
		public ConnectionController Connection
		{
			get
			{
				return pobjConnection;
			}
		}

		/// <summary>
		/// All keys are returned in lower case.
		/// </summary>
		/// <exception cref="FormatException">If the connection string is in an invalid format.</exception>
		protected static IDictionary<string, string> GetDictionaryFromConnectionString(string strConnectionString)
		{
			var objDictionary = new Dictionary<string, string>();
			string[] strPropertyValueArray;

			foreach (string strPropertyValue in strConnectionString.Split(';'))
			{
				if (!String.IsNullOrEmpty(strPropertyValue))
				{
					strPropertyValueArray = strPropertyValue.Split('=');
					if (strPropertyValueArray.Length == 2)
						objDictionary.Add(strPropertyValueArray[0].Trim().ToLower(), strPropertyValueArray[1].Trim());
					else
						throw new FormatException("Invalid key property definition for '" + strPropertyValue + "' from '" + strConnectionString + "'");
				}
			}

			return objDictionary;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Provides a mechanism for starting beginning, commiting and rolling back transactions.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public class TransactionsClass
		{
			private ConnectionController pobjConnectionController;
			
			internal TransactionsClass(ConnectionController objConnection)
			{
				pobjConnectionController = objConnection;
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Notifies that a transaction has begun and that all modifications to the database
			/// are only committed after a call to Commit. Alternatively, if
			/// Rollback is called then all changes are aborted. To execute other
			/// statements for the transaction call the Execute and ExecuteNonQuery functions.
			/// Because all changes to the database must be executed on the same connection
			/// DatabaseObjects maintains an open connection until the Commit or Rollback functions
			/// are called. When transactions are not being used connections are opened and closed
			/// for each SQL statement executed i.e. (INSERT/UPDATE/SELECT...).
			/// </summary>
			///
			/// <example>
			/// <code>
			/// Public Sub Shadows Save()
			///
			///     Mybase.ParentDatabase.Transactions.Begin()
			///
			///     MyBase.Save
			///     Me.Details.Save
			///
			///     'Execute any other statements here via
			///     'MyBase.ParentDatabase.Transactions.Execute()...
			///
			///     Mybase.ParentDatabase.Transactions.Commit()
			///
			/// End sub
			/// </code>
			/// </example>
			/// --------------------------------------------------------------------------------
			public void Begin()
			{
				pobjConnectionController.BeginTransaction(System.Data.IsolationLevel.Unspecified);
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Notifies that a transaction has begun and that all modifications to the database
			/// are only committed after a call to Commit. Alternatively, if
			/// Rollback is called then all changes are aborted. To execute other
			/// statements for the transaction call the Execute and ExecuteNonQuery functions.
			/// Because all changes to the database must be executed on the same connection
			/// DatabaseObjects maintains an open connection until the Commit or Rollback functions
			/// are called. When transactions are not being used connections are opened and closed
			/// for each SQL statement executed i.e. (INSERT/UPDATE/SELECT...).
			/// </summary>
			///
			/// <example>
			/// <code>
			/// Public Sub Shadows Save()
			///
			///     Mybase.ParentDatabase.Transactions.Begin()
			///
			///     MyBase.Save
			///     Me.Details.Save
			///
			///     'Execute any other statements here via
			///     'MyBase.ParentDatabase.Transactions.Execute()...
			///
			///     Mybase.ParentDatabase.Transactions.Commit()
			///
			/// End sub
			/// </code>
			/// </example>
			/// --------------------------------------------------------------------------------
			public void Begin(System.Data.IsolationLevel eIsolationLevel)
			{
				pobjConnectionController.BeginTransaction(eIsolationLevel);
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Indicates whether a transaction is currently in progress, caused by a call to Begin.
			/// Once the final, outer Commit or Rollback has been called false is returned.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public bool InProgress
			{
				get
				{
					return pobjConnectionController.InTransactionMode;
				}
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Commits all statements that have been executed after the Begin() call.
			/// The database connection is closed after the transaction has been committed.
			/// </summary>
			///
			/// <example>
			/// <code>
			/// Public Sub Shadows Save()
			///
			///     Mybase.ParentDatabase.Transactions.Begin()
			///
			///     MyBase.Save
			///     Me.Details.Save
			///
			///     'Execute any other statements here via
			///     'MyBase.ParentDatabase.Transactions.Execute()...
			///
			///     Mybase.ParentDatabase.Transactions.Commit()
			///
			/// End sub
			/// </code>
			/// </example>
			/// --------------------------------------------------------------------------------
			public void Commit()
			{
				pobjConnectionController.CommitTransaction();
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Rollsback all statements that have been executed after the Begin() call.
			/// The database connection is closed after the transaction has been rolled back.
			/// Subsequent calls are ignored.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public void Rollback()
			{
				pobjConnectionController.RollbackTransaction();
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Allows an SQL statement to be executed on the current transaction connection.
			/// If a transaction is not in progress an exception will occur.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public int ExecuteNonQuery(SQL.ISQLStatement objStatement)
			{
				if (!pobjConnectionController.InTransactionMode)
					throw new Exceptions.DatabaseObjectsException("Not in transaction mode");
				
				return pobjConnectionController.ExecuteNonQuery(objStatement);
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Allows the SQL statements to be executed on the current transaction connection.
			/// If a transaction is not in progress an exception will occur.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public int ExecuteNonQuery(SQL.ISQLStatement[] objStatements)
			{
				return this.ExecuteNonQuery(new SQL.SQLStatements(objStatements));
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Allows an SQL statement to be executed on the current transaction connection.
			/// If a transaction is not in progress an exception will occur.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public IDataReader Execute(SQL.ISQLStatement objStatement)
			{
				if (!pobjConnectionController.InTransactionMode)
					throw new Exceptions.DatabaseObjectsException("Not in transaction mode");
				
				return pobjConnectionController.Execute(objStatement);
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Allows the SQL statements to be executed on the current transaction connection.
			/// If a transaction is not in progress an exception will occur.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public IDataReader Execute(SQL.ISQLStatement[] objStatements)
			{
				return this.Execute(new SQL.SQLStatements(objStatements));
			}
		}
		
		public class ConnectionController
		{
			/// <summary>
			/// Fired after an SQL statement has been executed.
			/// Useful for trace logging of SQL statements executed.
			/// </summary>
			public delegate void StatementExecutedEventHandler(SQL.ISQLStatement objStatement);
			public event StatementExecutedEventHandler StatementExecuted;
			
			private IDbConnection pobjConnection;
			//This is only used for SQLServerCompact databases
			private Stack<IDbTransaction> pobjTransactions = new Stack<IDbTransaction>();
			
			private ConnectionType peConnectionType;
			private int pintConnectionCount = 0;
			private int pintTransactionLevel = 0;
			
			internal ConnectionController(string strConnectionString, ConnectionType eConnectionType)
                : this(CreateConnection(strConnectionString), eConnectionType)
			{
			}
			
			internal ConnectionController(IDbConnection objConnection, ConnectionType eConnectionType)
			{
				if (objConnection == null)
					throw new ArgumentNullException("Connection");
				
				pobjConnection = objConnection;
				peConnectionType = eConnectionType;
				SQL.SQLStatement.DefaultConnectionType = eConnectionType;
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Indicates that either Execute or ExecuteNonQuery is going to be used
			/// and that a connection needs to be opened if one is not already.
			/// If in transaction mode (Transactions.Begin has been called) then the
			/// current connection is left opened.
			/// If not in transaction mode then a new connection is opened.
			/// Always call Start before using Execute or ExecuteNonQuery whether in
			/// transaction mode or not as the library will open the connection if necessary.
			/// Once the connection is no longer required, call Database.Connection.Finished()
			/// or Database.Transactions.Commit().
			/// If called within a TransactionScope() then a database transaction is automatically started.
			/// </summary>
			/// <remarks>
			/// This feature is particularly relevant when database records are locked
			/// during transactions. If a second connection outside of the DatabaseObjects
			/// library is used then a possible deadlock could occur. Using the Execute
			/// and ExecuteNonQuery functions means that a new connection is opened if not
			/// in transaction mode or the current transaction connection is used - thereby
			/// avoiding potential deadlocks.
			/// </remarks>
			/// --------------------------------------------------------------------------------
			[Obsolete("Use DatabaseObjects.ConnectionScope")]
			public void Start()
			{
				ConnectionStart();
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Indicates that either Execute or ExecuteNonQuery have been called and are not
			/// going to be called again.
			/// If in transaction mode (Transactions.Begin has been called) then the
			/// connection is left open until Transactions.Commit or Rollback is called.
			/// If not in transaction mode then the connection is closed.
			/// Always call Finished when finished using the connection whether in
			/// transaction mode or not as the library will close the connection if necessary.
			/// </summary>
			/// <remarks>
			/// This feature is particularly relevant when database records are locked
			/// during transactions. If a second connection outside of the DatabaseObjects
			/// library is used then a possible deadlock could occur. Using the Execute
			/// and ExecuteNonQuery functions means that a new connection is opened if not
			/// in transaction mode or the current transaction connection is used - thereby
			/// avoiding potential deadlocks.
			/// </remarks>
			/// --------------------------------------------------------------------------------
			[Obsolete("Use DatabaseObjects.ConnectionScope")]
			public void Finished()
			{
				ConnectionFinished();
			}

            /// <summary>
            /// Only used for unit tests.
            /// </summary>
            internal ConnectionType Type
            {
                get
                {
                    return peConnectionType;
                }
            }
			
			private void ConnectionStart()
			{
				if (pintConnectionCount == 0)
					pobjConnection.Open();
				
				pintConnectionCount++;
			}
			
			private void ConnectionFinished()
			{
				pintConnectionCount--;
				
				if (pintConnectionCount <= 0)
				{
					pintConnectionCount = 0;
					if (pobjConnection.State == ConnectionState.Closed)
						throw new Exceptions.DatabaseObjectsException("Connection has already been closed.");
					
                    pobjConnection.Close();
				}
			}
			
			internal void BeginTransaction(System.Data.IsolationLevel eIsolationLevel)
			{
				ConnectionStart();
				
				switch (peConnectionType)
				{
#if !MONO
					case ConnectionType.SQLServerCompactEdition:
						IDbTransaction objTransaction = pobjConnection.BeginTransaction(eIsolationLevel);
						//Simulate that SET TRANSACTION ISOLATION LEVEL has been called
						pobjTransactions.Push(objTransaction);
						
						if (eIsolationLevel != System.Data.IsolationLevel.Unspecified)
                            OnStatementExecuted(new SQL.SQLSetTransactionIsolationLevel(eIsolationLevel));

						//Simulate that BEGIN TRANSACTION has been called
                        OnStatementExecuted(new SQL.SQLBeginTransaction());
						break;
#endif
                    default:
						if (eIsolationLevel != System.Data.IsolationLevel.Unspecified)
						{
							ExecuteNonQuery(new SQL.SQLSetTransactionIsolationLevel(eIsolationLevel));
						}
						ExecuteNonQuery(new SQL.SQLBeginTransaction());
						break;
				}
				
				pintTransactionLevel++;
			}
			
			internal void CommitTransaction()
			{
				switch (peConnectionType)
				{
#if !MONO
					case ConnectionType.SQLServerCompactEdition:
						// Compact edition does not directly support use of COMMIT TRANSACTION
						IDbTransaction objTransaction = pobjTransactions.Pop();
						objTransaction.Commit();
						//Simulate that COMMIT TRANSACTION has been called
                        OnStatementExecuted(new SQL.SQLCommitTransaction());
						break;
#endif 
                    default:
						ExecuteNonQuery(new SQL.SQLCommitTransaction());
						break;
				}
				
				ConnectionFinished();
				pintTransactionLevel--;
			}
			
			internal void RollbackTransaction()
			{
				if (pintTransactionLevel <= 0)
					throw new InvalidOperationException("A transaction has not been started");
				
				switch (peConnectionType)
				{
#if !MONO
					case ConnectionType.SQLServerCompactEdition:
						// Compact edition does not directly support use of ROLLBACK TRANSACTION
						IDbTransaction objTransaction = pobjTransactions.Pop();
						objTransaction.Rollback();
						//Simulate that COMMIT TRANSACTION has been called
                        OnStatementExecuted(new SQL.SQLRollbackTransaction());
						break;
#endif
					default:
						ExecuteNonQuery(new SQL.SQLRollbackTransaction());
						break;
				}
				
				ConnectionFinished();
				pintTransactionLevel--;
			}

            private void OnStatementExecuted(SQL.ISQLStatement statement)
            {
                if (StatementExecuted != null)
                    StatementExecuted(statement);
            }

			internal bool InTransactionMode
			{
				get
				{
					return pintTransactionLevel > 0;
				}
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Executes the SQL statement.
			/// Returns Nothing/null if no record was selected, otherwise the first field from the
			/// returned result.
			/// ConnectionController.Start must be called prior to and ConnectionController.Finished afterwards,
			/// otherwise the connection will not be correctly closed.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public object ExecuteScalar(SQL.ISQLStatement objSQLStatement)
            {
				using (var objDataReader = ExecuteInternal(pobjConnection, objSQLStatement))
				{
					if (objDataReader.Read())
						return objDataReader[0];
					else
						return null;
				}
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Executes the SQL statement.
			/// Returns Nothing/null if no record was selected, otherwise the first field from the
			/// returned result.
			/// ConnectionController.Start and
			/// ConnectionController.Finished are automatically called.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public object ExecuteScalarWithConnect(SQL.ISQLStatement objSQLStatement)
            {
				using (var connectionScope = new ConnectionScope(this))
				{
					using (var objDataReader = connectionScope.Execute(objSQLStatement))
					{
						if (objDataReader.Read())
							return objDataReader[0];
						else
							return null;
					}
				}
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Executes the SQL statement.
			/// ConnectionController.Start must be called prior to and ConnectionController.Finished afterwards.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public IDataReader Execute(SQL.ISQLStatement objSQLStatement)
			{
				return ExecuteInternal(pobjConnection, objSQLStatement);
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Executes the SQL statements.
			/// ConnectionController.Start must be called prior to and ConnectionController.Finished afterwards.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public IDataReader Execute(SQL.ISQLStatement[] objSQLStatements)
			{
				return ExecuteInternal(pobjConnection, new SQL.SQLStatements(objSQLStatements));
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Executes the SQL statement.
			/// ConnectionController.Start must be called prior to and ConnectionController.Finished afterwards.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public int ExecuteNonQuery(SQL.ISQLStatement objSQLStatement)
			{
				return ExecuteNonQueryInternal(pobjConnection, objSQLStatement);
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Executes the SQL statements.
			/// ConnectionController.Start must be called prior to and ConnectionController.Finished afterwards.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public int ExecuteNonQuery(SQL.ISQLStatement[] objSQLStatements)
			{
				return ExecuteNonQueryInternal(pobjConnection, new SQL.SQLStatements(objSQLStatements));
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Executes an SQL statement on a new connection from the connection pool.
			/// ConnectionController.Start or ConnectionController.Finished do not have to be called.
			/// </summary>
			/// --------------------------------------------------------------------------------
			[Obsolete("Use DatabaseObjects.ConnectionScope")]
            public int ExecuteNonQueryWithConnect(SQL.ISQLStatement objSQLStatement)
			{
                using (var connectionScope = new ConnectionScope(this))
                    return connectionScope.ExecuteNonQuery(objSQLStatement);
			}
			
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Executes the SQL statements on a new connection from the connection pool.
			/// ConnectionController.Start or ConnectionController.Finished do not have to be called.
			/// </summary>
			/// --------------------------------------------------------------------------------
			[Obsolete("Use DatabaseObjects.ConnectionScope")]
			public int ExecuteNonQueryWithConnect(SQL.ISQLStatement[] objSQLStatements)
			{
                using (var connectionScope = new ConnectionScope(this))
				    return connectionScope.ExecuteNonQuery(new SQL.SQLStatements(objSQLStatements));
			}
			
			protected virtual IDataReader ExecuteInternal(IDbConnection objConnection, SQL.ISQLStatement objSQLStatement)
			{
				if (objConnection == null)
					throw new Exceptions.DatabaseObjectsException("Connection is not open, call Database.Connection.Start() or Database.Transactions.Begin()");
				
				objSQLStatement.ConnectionType = peConnectionType;
				string strSQL = objSQLStatement.SQL;

				IDataReader reader;
                
				using (var command = objConnection.CreateCommand())
				{
					command.CommandText = strSQL;

					if (pobjTransactions.Count > 0)
						command.Transaction = pobjTransactions.Peek(); //Only used for SQLServerCompactEdition

					try
					{
						reader = command.ExecuteReader();
					}
					catch (Exception ex)
					{
						throw new Exceptions.DatabaseObjectsException("Execute failed: " + strSQL, ex);
					}
				}

				OnStatementExecuted(objSQLStatement);
				
				return reader;
			}
			
			protected virtual int ExecuteNonQueryInternal(IDbConnection objConnection, SQL.ISQLStatement objSQLStatement)
			{
				if (objConnection == null)
					throw new Exceptions.DatabaseObjectsException("Connection is not open, call Database.Connection.Start() or Database.Transactions.Begin()");

				objSQLStatement.ConnectionType = peConnectionType;
				string strSQL = objSQLStatement.SQL;
				int rowAffected;
				
				using (var command = objConnection.CreateCommand())
				{
					command.CommandText = strSQL;

					if (pobjTransactions.Count > 0)
						command.Transaction = pobjTransactions.Peek(); //Only used for SQLServerCompactEdition

					try
					{
						rowAffected = command.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						throw new Exceptions.DatabaseObjectsException("ExecuteNonQuery failed: " + strSQL, ex);
					}
				}
                
                OnStatementExecuted(objSQLStatement);

                return rowAffected;
			}
			
			private static IDbConnection CreateConnection(string strConnectionString)
			{
#if MONO
				throw new NotImplementedException("Pass an IDbConnection object to the constructor");
#else
				//Searches for an occurance of 'Provider='
				//If found then it is assumed to be an OLEDB connection otherwise an ODBC connection
				if (GetDictionaryFromConnectionString(strConnectionString).ContainsKey("provider"))
					return new System.Data.OleDb.OleDbConnection(strConnectionString);
				else
					return new System.Data.Odbc.OdbcConnection(strConnectionString);
#endif
			}
		}
	}
}
