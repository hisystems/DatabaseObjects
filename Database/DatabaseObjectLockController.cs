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
	/// This is the controller class that initializes the lock table and the user
	/// ID that is to be associated with all locking operations. In most situations,
	/// only one instance of this class is ever created and this instance is passed into
	/// the constructor for all DatabaseObjectLockable and DatabaseObjectUsingAttributesLockable
	/// instances.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public class DatabaseObjectLockController
	{
		private string pstrCurrentUserID;
		private string pstrLockTableName;
		private Database pobjDatabase;
		
		public DatabaseObjectLockController(Database objDatabase, string strLockTableName, string strCurrentUserID)
		{
			if (objDatabase == null)
				throw new ArgumentNullException("Database");
			else if (String.IsNullOrEmpty(strCurrentUserID))
				throw new ArgumentNullException("User");
			else if (String.IsNullOrEmpty(strLockTableName))
				throw new ArgumentNullException("Lock table name");
			
			pobjDatabase = objDatabase;
			pstrCurrentUserID = strCurrentUserID;
			pstrLockTableName = strLockTableName;
			
			EnsureTableExists();
		}
		
		private void EnsureTableExists()
		{
			SQL.SQLTableExists objTableExists = new SQL.SQLTableExists(pstrLockTableName);
			
			using (ConnectionScope connection = new ConnectionScope(pobjDatabase))
			{
				IDataReader with_1 = connection.Execute(objTableExists);
				//If table does not exist
				if (!with_1.Read())
				{
					connection.ExecuteNonQuery(CreateTable());
					connection.ExecuteNonQuery(CreateTableIndex());
				}
			}
		}
		
		/// <summary>
		/// Returns whether the object is locked.
		/// </summary>
		public bool IsLocked(IDatabaseObjects objCollection, IDatabaseObject objObject)
		{
			return this.LockRecordExists(objCollection.TableName(), objObject);
		}
		
		/// <summary>
		/// Returns whether the object is locked by the current user. Specifically, the user that was specified
		/// in the constructor.
		/// </summary>
		public bool IsLockedByCurrentUser(IDatabaseObjects objCollection, IDatabaseObject objObject)
		{
			return this.LockRecordExists(objCollection.TableName(), objObject, new SQL.SQLCondition("UserID", SQL.ComparisonOperator.EqualTo, pstrCurrentUserID));
		}
		
		/// <summary>
		/// Returns the user ID that has the object locked.
		/// Throws an exception if the object is not locked.
		/// </summary>
		public string LockedByUserID(IDatabaseObjects objCollection, IDatabaseObject objObject)
		{
			SQL.SQLSelect objSelect = new SQL.SQLSelect();
			
			objSelect.Fields.Add("UserID");
			objSelect.Tables.Add(pstrLockTableName);
			objSelect.Where.Add("TableName", SQL.ComparisonOperator.EqualTo, objCollection.TableName());
            objSelect.Where.Add("RecordID", SQL.ComparisonOperator.EqualTo, objObject.DistinctValue.ToString());
			
			using (ConnectionScope connection = new ConnectionScope(pobjDatabase))
			{
				using (IDataReader objReader = connection.Execute(objSelect))
				{
					if (objReader.Read())
						return objReader[0].ToString();
					else
						throw new Exceptions.DatabaseObjectsException("Object is not locked");
				}
			}
		}
		
		private bool LockRecordExists(string strTableName, IDatabaseObject objObject, SQL.SQLCondition objAdditionalCondition = null)
		{
			SQL.SQLSelect objSelect = new SQL.SQLSelect();
			
			objSelect.Fields.Add(string.Empty, SQL.AggregateFunction.Count);
			objSelect.Tables.Add(pstrLockTableName);
			objSelect.Where.Add("TableName", SQL.ComparisonOperator.EqualTo, strTableName);
			objSelect.Where.Add("RecordID", SQL.ComparisonOperator.EqualTo, objObject.DistinctValue.ToString());
			if (objAdditionalCondition != null)
				objSelect.Where.Add(objAdditionalCondition);
			
			using (ConnectionScope connection = new ConnectionScope(pobjDatabase))
			{
				using (IDataReader objReader = connection.Execute(objSelect))
				{
					objReader.Read();
					return System.Convert.ToInt32(objReader[0]) != 0;
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Locks an object.
		/// Throws an exception if the object is already locked.
		/// Throws an exception if the object is not been saved.
		/// Because it is possible that between calling IsLocked and calling Lock another
		/// user may have locked the object. Therefore, it is recommended calling Lock and then
		/// trapping the Exceptions.ObjectAlreadyExistsException to determine whether the object is already locked.
		/// </summary>
		/// <exception cref="Exceptions.DatabaseObjectsException">Thrown if the object has not been saved.</exception>
		/// <exception cref="Exceptions.ObjectAlreadyExistsException">Thrown if the object has already been locked.</exception>
		/// --------------------------------------------------------------------------------
		public void Lock(IDatabaseObjects objCollection, IDatabaseObject objObject)
		{
			if (!objObject.IsSaved)
				throw new Exceptions.DatabaseObjectsException("Object is not saved and cannot be locked");
			
			SQL.SQLInsert objInsert = new SQL.SQLInsert();
			objInsert.TableName = pstrLockTableName;
			objInsert.Fields.Add("TableName", objCollection.TableName());
			objInsert.Fields.Add("RecordID", objObject.DistinctValue.ToString());
			objInsert.Fields.Add("UserID", pstrCurrentUserID);
			
			//If another user/connection has managed to add a record to the database just before
			//this connection has a DatabaseObjectsException will be thrown because duplicate keys will
			//be added to the table.
			
			using (ConnectionScope connection = new ConnectionScope(pobjDatabase))
			{
				try
				{
					connection.ExecuteNonQuery(objInsert);
				}
				catch (Exceptions.DatabaseObjectsException)
				{
					throw new Exceptions.ObjectAlreadyLockedException(objCollection, objObject);
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// UnLocks this object. Throws an exception if the object is not locked by the current
		/// user or the object has not been saved.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public void UnLock(IDatabaseObjects objCollection, IDatabaseObject objObject)
		{
			//If the table is locked by someone else
			if (!this.IsLockedByCurrentUser(objCollection, objObject))
				throw new MethodAccessException("Object already locked");
			else if (!objObject.IsSaved)
				throw new MethodAccessException("Object is not saved and cannot be unlocked");
			
			SQL.SQLDelete objDelete = new SQL.SQLDelete();
			objDelete.TableName = pstrLockTableName;
			objDelete.Where.Add("TableName", SQL.ComparisonOperator.EqualTo, objCollection.TableName());
			objDelete.Where.Add("RecordID", SQL.ComparisonOperator.EqualTo, objObject.DistinctValue.ToString());
			objDelete.Where.Add("UserID", SQL.ComparisonOperator.EqualTo, pstrCurrentUserID);
			
			using (ConnectionScope connection = new ConnectionScope(pobjDatabase))
				connection.ExecuteNonQuery(objDelete);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Provides a means by which to ensure all locks have been removed for this user
		/// in situations where an unexpected exception occurs and/or the user logs out of
		/// system.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public void UnlockAll()
		{
			SQL.SQLDelete objDelete = new SQL.SQLDelete();
			objDelete.TableName = pstrLockTableName;
			objDelete.Where.Add("UserID", SQL.ComparisonOperator.EqualTo, pstrCurrentUserID);
			
			using (ConnectionScope connection = new ConnectionScope(pobjDatabase))
				connection.ExecuteNonQuery(objDelete);
		}
		
		private SQL.SQLStatement CreateTable()
		{
			SQL.SQLCreateTable objTable = new SQL.SQLCreateTable();
			
			objTable.Name = pstrLockTableName;
			objTable.Fields.Add("TableName", SQL.DataType.VariableCharacter, 50);
			objTable.Fields.Add("RecordID", SQL.DataType.VariableCharacter, 20);
			objTable.Fields.Add("UserID", SQL.DataType.VariableCharacter, 255); //Accounts for windows user names
			
			return objTable;
		}
		
		private SQL.SQLStatement CreateTableIndex()
		{
			SQL.SQLCreateIndex objIndex = new SQL.SQLCreateIndex();
			
			objIndex.Name = "Primary";
			objIndex.IsUnique = true;
			objIndex.TableName = pstrLockTableName;
			objIndex.Fields.Add("TableName");
			objIndex.Fields.Add("RecordID");
			
			return objIndex;
		}
	}
}
