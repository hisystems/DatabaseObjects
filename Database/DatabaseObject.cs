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
	/// Represents a single database record. Implements IDatabaseObject and provides
	/// the basic plumbing code required for the interface.
	/// Mapping of fields from the database record to the fields in the inheriting class
	/// can be defined by using the FieldMappingAttribute.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public abstract class DatabaseObject : IDatabaseObject
	{
		private bool pbIsSaved;
		private object pobjDistinctValue;
		private DatabaseObjects pobjParentCollection;
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new DatabaseObject with the parent collection that this object is
		/// associated with.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// 'Code from a class that has inherited from DatabaseObjects
		/// 'So that this object has a reference to the parent
		/// Protected Overrides Function ItemInstance() As DatabaseObjects.IDatabaseObject
		///
		///     Return New Product(Me)
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		protected DatabaseObject(DatabaseObjects objParentCollection)
		{
			if (objParentCollection == null)
				throw new ArgumentNullException();
			
			pobjParentCollection = objParentCollection;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Loads this object with the object's fields and properties with the fields from the database.
		/// Automatically sets the IDatabaseObject.DistinctValue and IDatabaseObject.IsSaved properties so that the
		/// object is correctly initialized.
		/// Onforwards a call to Database.ObjectLoad()
		/// </summary>
		/// <remarks>
		/// Typically used from within the overridden LoadFields function when completely loading and initializing an
		/// IDatabaseObject object from the database fields. This is different to LoadFieldsForObject in
		/// that the IDatabaseObject.DistinctValue and IDatabaseObject.IsSaved properties are not initialized.
		/// Furthermore the object typically does not implement IDatabaseObject
		/// and therefore these properties cannot be set.
		/// Onforwards a call to Database.ObjectLoad()
		/// </remarks>
		/// --------------------------------------------------------------------------------
		protected void Load(SQL.SQLFieldValues objFields)
		{
			Database.ObjectLoad(this, objFields);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Loads this object with the object's fields and properties with the fields from the database.
		/// Automatically sets the IDatabaseObject.DistinctValue and IDatabaseObject.IsSaved properties so that the
		/// object is correctly initialized.
		/// Onforwards a call to Database.ObjectLoad()
		/// </summary>
		/// <remarks>
		/// Typically used from within the overridden LoadFields function when completely loading and initializing an
		/// IDatabaseObject object from the database fields. This is different to LoadFieldsForObject in
		/// that the IDatabaseObject.DistinctValue and IDatabaseObject.IsSaved properties are not initialized.
		/// Furthermore the object typically does not implement IDatabaseObject
		/// and therefore these properties cannot be set.
		/// Onforwards a call to Database.ObjectLoad()
		/// </remarks>
		/// --------------------------------------------------------------------------------
		protected void Load(IDataReader objData)
		{
			Database.ObjectLoad(this, objData);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Sets/returns the parent collection (DatabaseObjects instance) that this object is
		/// associated with.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected internal DatabaseObjects ParentCollection
		{
			get
			{
				return pobjParentCollection;
			}
			
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				
				pobjParentCollection = value;
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the database associated with this object.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected internal Database ParentDatabase
		{
			get
			{
				return pobjParentCollection.ParentDatabase;
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the grand parent of this object. This is usually the object that
		/// contains the collection that this object is contained with in.
		/// For example, a GrandParent object of an InvoiceDetail would be an Invoice.
		/// </summary>
		/// --------------------------------------------------------------------------------
		/// <exception cref="InvalidCastException">
		/// Throws an exception if the grand parent object is not of type DatabaseObject and has
		/// only implemented IDatabaseObject.
		/// </exception>
		protected internal DatabaseObject GrandParent
		{
			get
			{
				return this.ParentCollection.Parent;
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the distinct value from the grand parent of this object.
		/// This is usually the object that contains the collection that this object is contained within.
		/// For example, a GrandParent object of an InvoiceDetail would be an Invoice.
		/// </summary>
		/// --------------------------------------------------------------------------------
		/// <exception cref="InvalidCastException">
		/// Throws an exception if the grand parent object is not of type DatabaseObject and has
		/// only implemented IDatabaseObject.
		/// </exception>
		protected internal object GrandParentDistinctValue
		{
			get
			{
				return this.ParentCollection.Parent.DistinctValue;
			}
		}
		
		/// <summary>
		/// Returns the root container object that this object is a child of.
		/// </summary>
		/// <remarks>
		/// Traverses up the object heirarchy to find the root container class.
		/// </remarks>
		protected internal TRootContainer RootContainer<TRootContainer>() where TRootContainer : RootContainer
		{
			return pobjParentCollection.RootContainer<TRootContainer>();
		}
		
		/// <summary>
		/// Deletes the record from the database associated with this record.
		/// After which this object becomes invalid.
		/// The IsSaved property is automtically set to false.
		/// Performs the same function as IDatabaseObjects.ObjectDelete().
		/// </summary>
		protected void Delete()
		{
			IDatabaseObject objMeReference = this;
			this.ParentDatabase.ObjectDelete(this.ParentCollection, ref objMeReference);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Extracts the fields to save to the database from the objItem.SaveFields function.
		/// The fields are then written to the database using either an SQL INSERT or UPDATE
		/// depending on whether the object has already been saved. If the collection has
		/// implemented IDatabaseObjects.KeyFieldName then objItem's key is also validated to
		/// ensure it is not null and unique within the collection.
		/// If the parent collection has implemented Subset then this object should exist
		/// within the parent collection. If not, a duplicate key error may occur if the key
		/// is being used in another subset in the same table. If a record is being amended
		/// (MyBase.IsSaved is True) then the function will "AND" the parent collection's
		/// Subset conditions and the DistinctValue value to create the WHERE clause in the
		/// UPDATE statement. Therefore, the combination of the IDatabaseObjects.Subset and
		/// IDatabaseObject.DistinctValue conditions MUST identify only one record in the
		/// table. Otherwise multiple records will be updated with the same data. If data is
		/// only inserted and not amended (usually a rare occurance) then this requirement
		/// is unnecessary.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// 'Make the inherited "Protected Sub Save" public
		/// Public Overrides Sub Save()
		///
		///     MyBase.Save()
		///
		/// End Sub
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		protected virtual void Save()
		{
			this.ParentDatabase.ObjectSave(this.ParentCollection, this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the distinct value that uniquely identifies this object in the
		/// database. If a new object is saved or an existing object is loaded then this
		/// property is automatically set by the library.
		/// Typically, this is the value of an identity or auto increment database field.
		/// </summary>
		/// --------------------------------------------------------------------------------
        protected virtual object DistinctValue
		{
			get
			{
				return pobjDistinctValue;
			}
			
			set
			{
				pobjDistinctValue = value;
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether this object has been saved to the database. If a new object is
		/// saved (which uses an auto increment field) or an existing object is loaded then
		/// this property is automatically set to true by the library.
		/// </summary>
		/// --------------------------------------------------------------------------------
        protected virtual bool IsSaved
		{
			get
			{
				return pbIsSaved;
			}
			
			set
			{
				pbIsSaved = value;
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
		protected SQL.SQLFieldValues LockRecord()
		{
			return this.ParentDatabase.ObjectLockRecord(this.ParentCollection, this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets and returns the field value from the database record associated with this object.
		/// Returns DBNull.Value if the field is NULL.
		/// </summary>
		/// <param name="strFieldName">
		/// The name of the database field that is to be read.
		/// Must be a field in the table associated with this object's record.
		/// </param>
		/// <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved.</exception>
		/// --------------------------------------------------------------------------------
		protected object GetFieldValue(string strFieldName)
		{
			return this.ParentDatabase.ObjectGetFieldValue(this, strFieldName);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Sets the field value for the database record associated with this object.
		/// </summary>
		/// <param name="strFieldName">
		/// The name of the database field that is to be set.
		/// Must be a field in the table associated with this object's record.
		/// </param>
		/// <param name="objNewValue">
		/// The new value that the database field it to be set to.
		/// If Nothing/null then the field is set to NULL.
		/// </param>
		/// <exception cref="Exceptions.ObjectDoesNotExistException">If the object has not already been saved.</exception>
		/// --------------------------------------------------------------------------------
		protected void SetFieldValue(string strFieldName, object objNewValue)
		{
			this.ParentDatabase.ObjectSetFieldValue(this, strFieldName, objNewValue);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Performs a shallow memberwise copy of fields in this and all base classes,
		/// but does not copy any of the DatabaseObjects fields ensuring that the
		/// objects are not considered equal.
		/// </summary>
		/// <remarks>
		/// This type and the objCopyTo must be of the same type.
		/// </remarks>
		/// --------------------------------------------------------------------------------
		protected virtual void MemberwiseCopy(DatabaseObject objCopyTo)
		{
			this.MemberwiseCopy(objCopyTo, true);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Performs a shallow memberwise copy of fields in this and all base classes,
		/// but does not copy any of the DatabaseObjects fields ensuring that the
		/// objects are not considered equal.
		/// </summary>
		/// <remarks>
		/// This type and the objCopyTo must be of the same type.
		/// </remarks>
		/// <param name="bCopyReferenceTypes">
		/// Indicates whether reference/object types are also copied.
		/// </param>
		/// --------------------------------------------------------------------------------
		protected virtual void MemberwiseCopy(DatabaseObject objCopyTo, bool bCopyReferenceTypes)
		{
			if (!objCopyTo.GetType().Equals(this.GetType()))
				throw new ArgumentException("Type '" + this.GetType().Name + "' does not equal type '" + objCopyTo.GetType().Name + "'");
			
			foreach (System.Reflection.FieldInfo objField in this.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
			{
				//Check that the field is not defined in the DatabaseObjects library
				if (!objField.DeclaringType.Assembly.Equals(System.Reflection.Assembly.GetExecutingAssembly()))
				{
					if (bCopyReferenceTypes || (!bCopyReferenceTypes && !objField.FieldType.IsValueType))
					{
						objField.SetValue(objCopyTo, objField.GetValue(this));
					}
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Sets the properties and fields marked with the FieldMappingAttribute with the
		/// values from the database record. Properties or fields that are an enum data
		/// type are automatically converted from the database integer value to the equivalent
		/// enum. For properties and fields marked with the FieldMappingObjectHookAttribute
		/// the property's or field's object is also traversed for properties or fields marked
		/// with the FieldMappingAttribute.
		/// Loads the lowest order base class that does not exist in the
		/// DatabaseObjects assembly first up through to the highest order class.
		/// This function should generally not be called from an inheritor.
		/// Use LoadFieldValues() to correctly load this object from a set of field values.
		/// </summary>
		/// <remarks>
		/// Generally, this function should only be overriden in order to perform any custom
		/// loading of data from the database. To completely initialize and load the object
		/// from an SQL.SQLFieldValues object call the protected LoadFieldValues(SQL.SQLFieldValues)
		/// sub.
		/// </remarks>
		/// <example>
		/// <code>
		///
		/// &lt;DatabaseObjects.FieldMapping("Name")&gt; _
		/// Private pstrName As String
		///
		/// OR
		///
		/// &lt;DatabaseObjects.FieldMapping("Name")&gt; _
		/// Public Property Name() As String
		///     Get
		///
		///         Return pstrName
		///
		///     End Get
		///
		///     Set(ByVal Value As String)
		///
		///         pstrName = Value
		///
		///     End Set
		///
		/// End Property
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
        protected virtual void LoadFields(SQL.SQLFieldValues objFields)
		{
			DatabaseObjectUsingAttributesHelper.LoadFieldsForBaseTypes(this, this.GetType(), objFields);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets the values from the properties and fields marked with the FieldMappingAttribute
		/// to be saved to the database. Properties or fields that return an enum data type are
		/// automatically converted from the enum to the equivalent integer value for database
		/// storage. For properties and fields marked with the FieldMappingObjectHookAttribute
		/// the property's or field's object is also traversed for properties or fields marked
		/// with the FieldMappingAttribute.
		/// </summary>
		/// <example>
		/// <code>
		///
		/// &lt;DatabaseObjects.FieldMapping("Name")&gt; _
		/// Private pstrName As String
		///
		/// OR
		///
		/// &lt;DatabaseObjects.FieldMapping("Name")&gt; _
		/// Public Property Name() As String
		///     Get
		///
		///         Return pstrName
		///
		///     End Get
		///
		///     Set(ByVal Value As String)
		///
		///         pstrName = Value
		///
		///     End Set
		///
		/// End Property
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		protected virtual SQL.SQLFieldValues SaveFields()
		{
			return DatabaseObjectUsingAttributesHelper.SaveFieldsForBaseTypes(this, this.GetType());
		}

        /// <summary>
        /// Private implementation of interface for protected method.. 
        /// Onforwards call to protected method.
        /// </summary>
        bool IDatabaseObject.IsSaved
        {
            get
            {
                return this.IsSaved;
            }

            set
            {
                this.IsSaved = value;
            }
        }

        /// <summary>
        /// Private implementation of interface for protected method.. 
        /// Onforwards call to protected method.
        /// </summary>
        object IDatabaseObject.DistinctValue
        {
            get
            {
                return this.DistinctValue;
            }
            set
            {
                this.DistinctValue = value;
            }
        }

        /// <summary>
        /// Private implementation of interface for protected method.. 
        /// Onforwards call to protected method.
        /// </summary>
        void IDatabaseObject.LoadFields(SQL.SQLFieldValues objFields)
        {
            LoadFields(objFields);
        }

        /// <summary>
        /// Private implementation of interface for protected method.. 
        /// Onforwards call to protected method.
        /// </summary>
        SQL.SQLFieldValues IDatabaseObject.SaveFields()
        {
            return this.SaveFields();
        }

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Compares whether two objects are equal using the distinct values of each object.
		/// Specifically, true is returned if the distinct values match, both objects are
		/// not Nothing and both object types are the same.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public override bool Equals(object obj)
		{
			if (obj is IDatabaseObject)
				return AreEqual(this, (IDatabaseObject) obj);
			else
				return false;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Compares whether two objects are equal using the distinct values of each object.
		/// Specifically, true is returned if the distinct values match, both objects are
		/// not Nothing and both object types are the same.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static bool operator ==(DatabaseObject objItem1, DatabaseObject objItem2)
		{
			return AreEqual(objItem1, objItem2);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Compares whether two objects are equal using the distinct values of each object.
		/// Specifically, true is returned if the distinct values match, both objects are
		/// not Nothing and both object types are the same.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static bool operator ==(DatabaseObject objItem1, IDatabaseObject objItem2)
		{
			return AreEqual(objItem1, objItem2);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Compares whether two objects are equal using the distinct values of each object.
		/// Specifically, true is returned if the distinct values match, both objects are
		/// not Nothing and both object types are the same.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static bool operator ==(IDatabaseObject objItem1, DatabaseObject objItem2)
		{
			return AreEqual(objItem1, objItem2);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Compares whether two objects are not equal using the distinct values of each object.
		/// Specifically, true is returned if the distinct values do not match, an object is
		/// Nothing or both object types are not the same.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static bool operator !=(DatabaseObject objItem1, DatabaseObject objItem2)
		{
			return !AreEqual(objItem1, objItem2);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Compares whether two objects are not equal using the distinct values of each object.
		/// Specifically, true is returned if the distinct values do not match, an object is
		/// Nothing or both object types are not the same.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static bool operator !=(DatabaseObject objItem1, IDatabaseObject objItem2)
		{
			return !AreEqual(objItem1, objItem2);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Compares whether two objects are not equal using the distinct values of each object.
		/// Specifically, true is returned if the distinct values do not match, an object is
		/// Nothing or both object types are not the same.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static bool operator !=(IDatabaseObject objItem1, DatabaseObject objItem2)
		{
			return !AreEqual(objItem1, objItem2);
		}
		
		private static bool AreEqual(IDatabaseObject objItem1, IDatabaseObject objItem2)
		{
			if (objItem1 == null && objItem2 == null)
			{
				//If both of the objects are nothing then they are considered equal
				return true;
			}
			else if (objItem1 == null || objItem2 == null)
			{
				//If one of the objects is nothing then they cannot be equal
				return false;
			}
			else if (objItem1 == objItem2)
			{
				//If both objects are the same. Test this before the type and distinct value check
				//because the object may not have been saved, but true should still be returned.
				return true;
			}
			else if (!(objItem1.IsSaved && objItem2.IsSaved))
			{
				//If one of the objects have not been saved then the objects must be different.
				//Because the object equality would have returned true in the "objItem1 Is objItem2" test above.
				return false;
			}
			else if (((object) objItem1).GetType().Equals(((object) objItem2).GetType()))
			{
				if (IsNumeric(objItem1.DistinctValue))
				{
					//If the objects are the same type then check whether their distinct numeric values are equal
					//casting to decimal because INT IDENTITY fields in SQL SERVER returns DECIMAL
					//whereas the actual value when read from a SELECT is INT
					return System.Convert.ToDecimal(objItem1.DistinctValue) == (System.Convert.ToDecimal(objItem2.DistinctValue));
				}
				else
				{
					//If the objects are the same type then check whether their distinct values are equal
					return objItem1.DistinctValue.Equals(objItem2.DistinctValue);
				}
			}
			else
			{
				//Return false because the object types are not equal
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		private static bool IsNumeric(object objValue)
		{
			if (objValue is IConvertible)
			{
				System.TypeCode eTypeCode = ((IConvertible) objValue).GetTypeCode();
				return TypeCode.Char <= eTypeCode && eTypeCode <= TypeCode.Decimal;
			}
			else
			{
				return false;
			}
		}
	}
}
