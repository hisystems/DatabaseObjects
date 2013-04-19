// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DatabaseObjects
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// Used by all of the DatabaseObjects*UsingAttributes classes.
	/// All DatabaseObjects*UsingAttributes classes can use attributes to specify the
	/// IDatabaseObjects.* functions that are normally overridden.
	/// But using attributes provides a nice and cleaner way of doing this - although
	/// sometimes it is a bit limiting and the functions sometimes need to be overriden
	/// and attributes not used.
	/// </summary>
	/// --------------------------------------------------------------------------------
	internal class DatabaseObjectsUsingAttributesHelper
	{
		private DatabaseObjects pobjDatabaseObjects;
		private DistinctFieldAttribute pobjDistinctField = null;
		private TableAttribute pobjTable = null;
		private KeyFieldAttribute pobjKeyField = null;
		private List<OrderByFieldAttribute> pobjOrderByAttributes = new List<OrderByFieldAttribute>();
		private SubsetAttribute pobjSubset = null;
		private List<TableJoinAttribute> pobjTableJoins = new List<TableJoinAttribute>();
		private ItemInstanceAttribute pobjItemInstance = null;
		
		public DatabaseObjectsUsingAttributesHelper(DatabaseObjects objDatabaseObjects)
		{
			if (objDatabaseObjects == null)
				throw new ArgumentNullException();
			
			pobjDatabaseObjects = objDatabaseObjects;
			
			object[] objAttributes = objDatabaseObjects.GetType().GetCustomAttributes(true);
			
			if (objAttributes != null)
			{
				foreach (Attribute objAttribute in objAttributes)
				{
					if (objAttribute is DistinctFieldAttribute)
						pobjDistinctField = (DistinctFieldAttribute) objAttribute;
					else if (objAttribute is KeyFieldAttribute)
						pobjKeyField = (KeyFieldAttribute) objAttribute;
					else if (objAttribute is OrderByFieldAttribute)
						pobjOrderByAttributes.Add((OrderByFieldAttribute) objAttribute);
					else if (objAttribute is SubsetAttribute)
						pobjSubset = (SubsetAttribute) objAttribute;
					else if (objAttribute is TableAttribute)
						pobjTable = (TableAttribute) objAttribute;
					else if (objAttribute is TableJoinAttribute)
						pobjTableJoins.Add((TableJoinAttribute) objAttribute);
					else if (objAttribute is ItemInstanceAttribute)
						pobjItemInstance = (ItemInstanceAttribute) objAttribute;
				}
			}
		}
		
		public IDatabaseObject ItemInstance()
		{
			var itemInstanceType = GetItemInstanceType();
			
			return DatabaseObjectsItemInstance.CreateItemInstance(itemInstanceType, pobjDatabaseObjects);
		}
		
		/// <summary>
		/// Returns the item instance type to instantiate.
		/// It may be defined as a T argument on a generic collection or explicitly
		/// via the ItemInstance attribute.
		/// </summary>
		private Type GetItemInstanceType()
		{
			if (pobjItemInstance == null)
			{
				try
				{
					return DatabaseObjectsItemInstance.GetGenericCollectionTArgument(pobjDatabaseObjects.GetType());
				}
				catch (Exceptions.DatabaseObjectsException ex)
				{
					throw new Exceptions.DatabaseObjectsException("ItemInstanceAttribute has not been specified and " + ex.Message);
				}
			}
			else
				return pobjItemInstance.Type;
		}
		
		public string DistinctFieldName
		{
			get
			{
				if (pobjDistinctField == null)
					throw new Exceptions.DatabaseObjectsException("DistinctFieldAttribute has not been specified on " + pobjDatabaseObjects.GetType().FullName);
				
				return pobjDistinctField.Name;
			}
		}
		
		public SQL.FieldValueAutoAssignmentType DistinctFieldAutoAssignment
		{
			get
			{
				if (pobjDistinctField == null)
					throw new Exceptions.DatabaseObjectsException("DistinctFieldAttribute has not been specified on " + pobjDatabaseObjects.GetType().FullName);
				
				return pobjDistinctField.AutomaticAssignment;
			}
		}
		
		public string TableName
		{
			get
			{
				if (pobjTable == null)
					throw new Exceptions.DatabaseObjectsException("TableAttribute has not been specified on " + pobjDatabaseObjects.GetType().FullName);
				
				return pobjTable.Name;
			}
		}
		
		public string KeyFieldName
		{
			get
			{
				//If attribute was not specified
				if (pobjKeyField == null)
					return string.Empty;
				else
					return pobjKeyField.Name;
			}
		}
		
		public SQL.SQLSelectOrderByFields OrderBy
		{
			get
			{
				//If no attributes were specified
				if (pobjOrderByAttributes.Count == 0)
					return null;
				else
				{
					SQL.SQLSelectOrderByFields objOrderBy = new SQL.SQLSelectOrderByFields();
					
					foreach (OrderByFieldAttribute objOrderByAttribute in pobjOrderByAttributes.OrderBy(item => item.Precendence))
						objOrderBy.Add(objOrderByAttribute.Name, objOrderByAttribute.Ordering);
					
					return objOrderBy;
				}
			}
		}
		
		public SQL.SQLConditions Subset
		{
			get
			{
				//If attribute was not specified
				if (pobjSubset == null)
					return null;
				else
				{
					if (pobjDatabaseObjects.Parent == null)
						throw new Exceptions.DatabaseObjectsException("Subset attribute requires the collection to have valid parent in order to obtain the value to subset the collection. Calls DatabaseObjects.Parent.DistinctValue");
					
					SQL.SQLConditions objConditions = new SQL.SQLConditions();

                    objConditions.Add(pobjSubset.FieldName, SQL.ComparisonOperator.EqualTo, ((IDatabaseObject)pobjDatabaseObjects.Parent).DistinctValue);
					
					return objConditions;
				}
			}
		}
		
		public SQL.SQLSelectTableJoins TableJoins(SQL.SQLSelectTable objPrimaryTable, SQL.SQLSelectTables objTables)
		{
			SQL.SQLSelectTableJoins tableJoinsCollection;
			
			//If attribute was not specified
			if (pobjTableJoins.Count > 0)
				tableJoinsCollection = TableJoinsFromAttributes(pobjTableJoins.ToArray(), objPrimaryTable, objTables);
			else
			{
				//If the ObjectReferenceEarlyBindingAttribute is specified on the item instance then create the table joins that will be required
				var earlyBindingTableJoins = ObjectReferenceEarlyBinding.GetTableJoins(objPrimaryTable, this.GetItemInstanceType());
				
				if (earlyBindingTableJoins.Count > 0)
					tableJoinsCollection = earlyBindingTableJoins;
				else
					tableJoinsCollection = null;
			}
			
			return tableJoinsCollection;
		}
		
		private static SQL.SQLSelectTableJoins TableJoinsFromAttributes(TableJoinAttribute[] tableJoinAttributes, SQL.SQLSelectTable primaryTable, SQL.SQLSelectTables tables)
		{
			SQL.SQLSelectTableJoins tableJoinsCollection = new SQL.SQLSelectTableJoins();
			
			SQL.SQLSelectTableBase leftTable = primaryTable;
			string leftTableName = primaryTable.Name;
			
			foreach (var tableJoinAttribute in tableJoinAttributes)
			{
				SQL.SQLSelectTableBase rightTable = tables.Add(tableJoinAttribute.ToTableName);
				var tableJoin = tableJoinsCollection.Add(leftTable, SQL.SQLSelectTableJoin.Type.Inner, rightTable);
				tableJoin.Where.Add(new SQL.SQLFieldExpression(new SQL.SQLSelectTable(leftTableName), tableJoinAttribute.FieldName), SQL.ComparisonOperator.EqualTo, new SQL.SQLFieldExpression(new SQL.SQLSelectTable(tableJoinAttribute.ToTableName), tableJoinAttribute.ToFieldName));
				leftTable = tableJoin;
				leftTableName = tableJoinAttribute.ToTableName;
			}
			
			return tableJoinsCollection;
		}
	}
}
