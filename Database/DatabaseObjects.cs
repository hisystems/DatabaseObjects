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
/// Represents a collection of database records. Implements IDatabaseObjects and provides
/// the basic plumbing code required for the interface.
/// Attributes can be used to specify the database specific information using
/// class attributes DistinctFieldAttribute, TableAttribute, KeyFieldAttribute, ItemInstanceAttribute,
/// OrderByAttribute, SubsetAttribute and TableJoinAttribute.
/// </summary>
/// --------------------------------------------------------------------------------
namespace DatabaseObjects
{
	public abstract class DatabaseObjects : IDatabaseObjects
	{
		internal const string DistinctFieldAutoIncrementsObsoleteWarningMessage = "Obsolete and replaced by DistinctFieldAutoAssignment to support both auto increment and automatically assigned globally unique identifiers.";
		
		private Database pobjDatabase;
		private DatabaseObject pobjParent;
		/// <summary>
		/// May optionally be set to the container object that this object is a child of.
		/// </summary>
		/// <remarks></remarks>
		private RootContainer rootContainer;
		private DatabaseObjectsUsingAttributesHelper pobjAttributeHelper;
		
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
		{
			if (objDatabase == null)
				throw new ArgumentNullException();
			
			pobjDatabase = objDatabase;
			pobjAttributeHelper = new DatabaseObjectsUsingAttributesHelper(this);
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
		{
			if (rootContainer == null)
				throw new ArgumentNullException();
			
			this.rootContainer = rootContainer;
			pobjDatabase = rootContainer.Database;
			pobjAttributeHelper = new DatabaseObjectsUsingAttributesHelper(this);
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
		{
			if (objParent == null)
				throw new ArgumentNullException();
			
			pobjDatabase = objParent.ParentDatabase;
			pobjParent = objParent;
			pobjAttributeHelper = new DatabaseObjectsUsingAttributesHelper(this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the parent object that this collection is associated with.
		/// This property will return Nothing if the 'New(DatabaseObjects.Database)'
		/// constructor is used.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected internal DatabaseObject Parent
		{
			get
			{
				return pobjParent;
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
			if (rootContainer != null)
				return (TRootContainer)this.rootContainer;
			else if (pobjParent != null)
				return pobjParent.RootContainer<TRootContainer>();
			else
				return null;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the distinct value of the parent i.e. Parent.DistinctValue
		/// Throws a NullReferenceException if there is no parent.
		/// </summary>
		/// <exception cref="NullReferenceException">Parent is nothing</exception>
		/// --------------------------------------------------------------------------------
		protected internal object ParentDistinctValue
		{
			get
			{
				return ((IDatabaseObject)this.Parent).DistinctValue;
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the database associated with this collection/database table.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected internal Database ParentDatabase
		{
			get
			{
				return pobjDatabase;
			}
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
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example> Loads a product using a product ID of 123
		/// <code>
		/// Dim objProduct As Product = MyBase.Object(123)
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected IDatabaseObject Object(object objDistinctValue)
		{
			return this.ParentDatabase.Object(this, objDistinctValue);
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
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		///
		/// <example> Loads a product using a product ID of 123
		/// <code>
		/// Dim objProduct As Product = MyBase.Object(123)
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected IDatabaseObject ObjectIfExists(object objDistinctValue)
		{
			return this.ParentDatabase.ObjectIfExists(this, objDistinctValue);
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
		/// <param name="objDistinctValue">
		/// The value that uniquely identifies the object within the collection. This is the value
		/// of the field defined by the collection's IDatabaseObjects.DistinctFieldName function.
		/// </param>
		///
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
		/// --------------------------------------------------------------------------------
		protected SQL.SQLFieldValues ObjectFieldValues(object objDistinctValue)
		{
			return this.ParentDatabase.ObjectFieldValues(this, objDistinctValue);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether an object exists for the specified distinct value in the collection.
		/// </summary>
		///
		/// <param name="objDistinctValue">
		/// The value to search for in the collection. This is the value of the field defined
		/// by the collection's IDatabaseObjects.DistinctFieldName function.
		/// </param>
		/// --------------------------------------------------------------------------------
		///
		protected bool ObjectExistsByDistinctValue(object objDistinctValue)
		{
			return this.ParentDatabase.ObjectExistsByDistinctValue(this, objDistinctValue);
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
		protected void ObjectSave(IDatabaseObject objItem)
		{
			this.ParentDatabase.ObjectSave(this, objItem);
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
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
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
		protected IDatabaseObject ObjectByKey(object objKey)
		{
			return this.ParentDatabase.ObjectByKey(this, objKey);
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
		/// <returns><see cref="IDatabaseObject" />	(DatabaseObjects.IDatabaseObject)</returns>
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
		protected IDatabaseObject ObjectByKeyIfExists(object objKey)
		{
			return this.ParentDatabase.ObjectByKeyIfExists(this, objKey);
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
		protected IDatabaseObject ObjectByOrdinalFirst()
		{
			return this.ParentDatabase.ObjectByOrdinalFirst(this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the last object in the collection respectively
		/// filtered and sorted by the collection's Subset and OrderBy values. It differs
		/// from ObjectByOrdinal in that it only loads the first record from the database
		/// table not the entire table.
		/// </summary>
		///
		/// <returns><see cref="IDatabaseObject" /> (DatabaseObjects.IDatabaseObject)</returns>
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
		protected IDatabaseObject ObjectByOrdinalLast()
		{
			return this.ParentDatabase.ObjectByOrdinalLast(this);
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
		protected void ObjectDelete(ref IDatabaseObject objItem)
		{
			this.ParentDatabase.ObjectDelete(this, ref objItem);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether the key exists within the collection. If this collection's Subset
		/// has been set then only the subset is searched not the entire table.
		/// </summary>
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
		///     Return MyBase.ObjectExists(strProductCode)
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected bool ObjectExists(object objKey)
		{
			return this.ParentDatabase.ObjectExists(this, objKey);
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
		protected IDatabaseObject ObjectFromDataReader(IDataReader objReader)
		{
			return Database.ObjectFromDataReader(this, objReader);
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
		protected IDatabaseObject ObjectFromFieldValues(SQL.SQLFieldValues objFieldValues)
		{
			return Database.ObjectFromFieldValues(this, objFieldValues);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an IList object containing all of this collection's objects. This
		/// function is useful when loading a set of objects for a subset or for use with
		/// the IEnumerable interface.
		/// </summary>
		///
		/// <returns><see cref="Collections.IList" />	(System.Collections.IList)</returns>
		///
		/// <example>
		/// <code>
		/// 'Alternatively, the DatabaseObjectsEnumerable class can be used which
		/// 'automatically incorporates an enumerator
		/// Private Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		///
		///     Return MyBase.ObjectsList.GetEnumerator
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected IList ObjectsList()
		{
			return this.ParentDatabase.ObjectsList(this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an array of IDatabaseObject objects contained within this collection.
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected IDatabaseObject[] ObjectsArray()
		{			
			return this.ParentDatabase.ObjectsArray(this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an IDictionary object. Each key/value pair contains a key and
		/// the object associated with the key.
		/// </summary>
		///
		/// <returns><see cref="Collections.IDictionary" />	(System.Collections.IDictionary)</returns>
		/// --------------------------------------------------------------------------------
		///
		protected IDictionary ObjectsDictionary()
		{			
			return this.ParentDatabase.ObjectsDictionary(this);
			
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns an IDictionary object. Each key/value pair contains a distinct
		/// value and the object associated with the distinct value.
		/// </summary>
		///
		/// <returns><see cref="Collections.IDictionary" />	(System.Collections.IDictionary)</returns>
		/// --------------------------------------------------------------------------------
		///
		protected IDictionary ObjectsDictionaryByDistinctValue()
		{			
			return this.ParentDatabase.ObjectsDictionaryByDistinctValue(this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Returns the number of items in this collection. If this collection's Subset
		/// has been implemented then this function returns the number of records within the
		/// subset, not the entire table.
		/// Also utilises the table joins so that any filters specified on the subset
		/// can be used.
		/// </summary>
		///
		/// <returns><see cref="Int32" />	(System.Int32)</returns>
		///
		/// <example>
		/// <code>
		/// 'Return the number of items in this collection.
		/// Public ReadOnly Property Count() As Integer
		///     Get
		///
		///         Return MyBase.ObjectsCount
		///
		///     End Get
		/// End Property
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		///
		protected int ObjectsCount()
		{			
			return this.ParentDatabase.ObjectsCount(this);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Deletes all of the objects in this collection. If Subset has been implemented
		/// then only the objects within the subset are deleted, not the table's entire
		/// contents.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		protected void ObjectsDeleteAll()
		{			
			this.ParentDatabase.ObjectsDeleteAll(this);
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
		protected IList ObjectsSearch(SQL.SQLConditions objSearchCriteria)
		{			
			return this.ParentDatabase.ObjectsSearch(this, objSearchCriteria);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return whether the Distinct field as specified in the
		/// associated collection's DatabaseObject.DistinctField is an identity field
		/// (Autonumber in Microsoft Access) or is a unique identifier field.
		/// If set to either value then the IDatabaseObject.DistinctValue value is
		/// automatically set when a new object is saved.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function DistinctFieldAutoAssignment() As SQL.FieldValueAutoAssignmentType
		///
		///     Return SQL.FieldValueAutoAssignmentType.AutoIncrement
		///
		/// End Function
		/// </code>
		/// </example>
        protected virtual SQL.FieldValueAutoAssignmentType DistinctFieldAutoAssignment()
		{			
			return pobjAttributeHelper.DistinctFieldAutoAssignment;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return whether the Distinct field as specified in the
		/// associated collection's DatabaseObject.DistinctField is an identity field
		/// (Autonumber in Microsoft Access). If set to true, then the
		/// DatabaseObject.DistinctValue value is set when a new object is saved.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function DistinctFieldAutoIncrements() As Boolean
		///
		///     Return True
		///
		/// End Function
		/// </code>
		/// </example>
		/// --------------------------------------------------------------------------------
		[Obsolete(DatabaseObjects.DistinctFieldAutoIncrementsObsoleteWarningMessage)]
        protected virtual bool DistinctFieldAutoIncrements()
		{			
			//Indicates to defer logic to the DistinctFieldAutoAssignment function as this function is obsolete.
			return false;
		}
		
		/// <summary>
		/// Should return the field name that uniquely identifies each object
		/// within the collection. Typically, this is the field name of an identity or auto
		/// increment field. If the DatabaseObjects.SubSet function has been implemented
		/// then the DistinctFieldName need only be unique within the subset not the
		/// entire table. The DistinctFieldName and KeyFieldName can be identical. This
		/// function should almost always be implemented.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function DistinctFieldName() As String
		///
		///     Return "ProductID"
		///
		/// End Function
		/// </code>
		/// </example>
		protected virtual string DistinctFieldName()
		{			
			return pobjAttributeHelper.DistinctFieldName;
		}
		
		/// <summary>
		/// This property should return the field name that uniquely identifies each object
		/// within the collection. As opposed to the ordinal/index position, the key field
		/// provides another method of accessing a particular object within the collection.
		/// The key field must be unique within the collection. If the DatabaseObjects.Subset
		/// function has been implemented then the key field only needs to be unique within
		/// the specified subset, not the entire table. Implementing this function is optional.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function KeyFieldName() As String
		///
		///     Return "ProductCode"
		///
		/// End Function
		/// </code>
		/// </example>
        protected virtual string KeyFieldName()
		{			
			return pobjAttributeHelper.KeyFieldName;
		}
		
		/// <summary>
		/// Should return an SQLSelectOrderByFields object containing the list
		/// of fields the collection will be sorted by. Just as with an SQL statement, the
		/// order of the fields added to the collection indicates the group sorting. If
		/// DatabaseObjects.TableJoins has been implemented then fields from the adjoining
		/// table or tables can be utilized. The sort order is used by the ObjectByOrdinal,
		/// ObjectByOrdinalFirst and ObjectsSearch functions.
		/// Should return Nothing if no ordering is required.
		/// Implementing this function is optional.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function OrderBy() As SQL.SQLSelectOrderByFields
		///
		///     OrderBy = New SQL.SQLSelectOrderByFields
		///     OrderBy.Add("ProductCode", SQL.OrderBy.Ascending)
		///
		/// End Function
		/// </code>
		/// </example>
        protected virtual SQL.SQLSelectOrderByFields OrderBy()
		{			
			return pobjAttributeHelper.OrderBy;
		}
		
		/// <summary>
		/// Should return the conditions that define the collection's subset.
		/// If the collection should include the entire table then this function should return Nothing.
		/// Implementing this function is optional.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function Subset() As SQL.SQLConditions
		///
		///     Dim objConditions As New SQL.SQLConditions
		///
		///     'Only include products that are in group ID 1234
		///     objConditions.Add("GroupID", SQL.ComparisonOperator.EqualTo, 1234)
		///
		///     Return objConditions
		///
		/// End Function
		/// </code>
		/// </example>
        protected virtual SQL.SQLConditions Subset()
		{			
			return pobjAttributeHelper.Subset;
		}
		
		/// <summary>
		/// Should return an SQLSelectTableJoins object containing the table
		/// or tables to be joined to the primary table. This function is useful in
		/// optimising database loading speeds by allowing multiple tables to be joined into
		/// one data set. The resultant data set can then be used to load
		/// objects from the associated tables avoiding subsequent SQL calls. For a complete
		/// example, see the demonstration program.
		/// Should return Nothing if no table joins are required.
		/// Implementing this function is optional.
		/// </summary>
        protected virtual SQL.SQLSelectTableJoins TableJoins(SQL.SQLSelectTable objPrimaryTable, SQL.SQLSelectTables objTables)
		{			
			return pobjAttributeHelper.TableJoins(objPrimaryTable, objTables);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return the name of the table associated with this collection.
		/// This function should almost always be implemented.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function TableName() As String
		///
		///     Return "Products"
		///
		/// End Function
		/// </code>
		/// </example>
        protected virtual string TableName()
		{			
			return pobjAttributeHelper.TableName;
		}
		
		/// <summary>
		/// Should return an instance of the class that is associated with this
		/// collection of objects. The associated class must implement the IDatabaseObject
		/// interface. Typically, a DatabaseObject (implements IDatabaseObject) instance is
		/// returned from this function.
		/// </summary>
		///
		/// <example>
		/// <code>
		/// Protected Overrides Function ItemInstance() As IDatabaseObject
		///
		///     Return New Product
		///
		/// End Function
		/// </code>
		/// </example>
        protected virtual IDatabaseObject ItemInstance()
		{			
			return pobjAttributeHelper.ItemInstance();
		}

        /// <summary>
        /// Private implementation of interface for protected method.
        /// Onforwards call to protected method.
        /// </summary>
        IDatabaseObject IDatabaseObjects.ItemInstance()
        {
            return ItemInstance();
        }

        /// <summary>
        /// Private implementation of interface for protected method.
        /// Onforwards call to protected method.
        /// </summary>
        SQL.SQLConditions IDatabaseObjects.Subset()
        {
            return Subset();
        }

        /// <summary>
        /// Private implementation of interface for protected method.
        /// Onforwards call to protected method.
        /// </summary>
        string IDatabaseObjects.TableName()
        {
            return TableName();
        }

        /// <summary>
        /// Private implementation of interface for protected method.
        /// Onforwards call to protected method.
        /// </summary>
        string IDatabaseObjects.KeyFieldName()
        {
            return KeyFieldName();
        }

        /// <summary>
        /// Private implementation of interface for protected method.
        /// Onforwards call to protected method.
        /// </summary>
        string IDatabaseObjects.DistinctFieldName()
        {
            return DistinctFieldName();
        }

        /// <summary>
        /// Private implementation of interface for protected method.
        /// Onforwards call to protected method.
        /// </summary>
        SQL.FieldValueAutoAssignmentType IDatabaseObjects.DistinctFieldAutoAssignment()
        {
            return DistinctFieldAutoAssignment();
        }

        /// <summary>
        /// Private implementation of interface for protected method.
        /// Onforwards call to protected method.
        /// </summary>
        bool IDatabaseObjects.DistinctFieldAutoIncrements()
        {
            return DistinctFieldAutoIncrements();
        }

        /// <summary>
        /// Private implementation of interface for protected method.
        /// Onforwards call to protected method.
        /// </summary>
        SQL.SQLSelectOrderByFields IDatabaseObjects.OrderBy()
        {
            return OrderBy();
        }

        /// <summary>
        /// Private implementation of interface for protected method.
        /// Onforwards call to protected method.
        /// </summary>
        SQL.SQLSelectTableJoins IDatabaseObjects.TableJoins(SQL.SQLSelectTable objPrimaryTable, SQL.SQLSelectTables objTables)
        {
            return TableJoins(objPrimaryTable, objTables);
        }
	}
}
