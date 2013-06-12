// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
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
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace DatabaseObjects
{
	internal static class ObjectReferenceEarlyBinding
	{
		/// <summary>
		/// Used to maintain a cache of collection assemblies that can be traversed
		/// </summary>
		private class CollectionTypesInAssemblies
		{
			private List<CollectionTypesInAssembly> items = new List<CollectionTypesInAssembly>();
			
			public CollectionTypesInAssembly this[Assembly Assembly]
			{
				get
				{
					var existingItem = items.SingleOrDefault(workingItem => workingItem.Assembly.Equals(Assembly));
					
					if (existingItem == null)
					{
						var newItem = new CollectionTypesInAssembly(Assembly);
						items.Add(newItem);
						return newItem;
					}
					else
					{
						return existingItem;
					}
				}
			}
		}
		
		private class CollectionTypesInAssembly
		{
			public readonly Assembly Assembly;
			private readonly CollectionType[] items;
			
			public CollectionTypesInAssembly(Assembly Assembly)
			{
				this.Assembly = Assembly;
				this.items = GetCollectionTypesUsingTableAndDistinctFieldAttributes(Assembly);
			}
			
			public CollectionType this[Type itemInstanceType]
			{
				get
				{
					return this.items.Single(type => type.ItemInstanceType.Equals(itemInstanceType));
				}
			}
			
			/// <summary>
			/// Indicates whether there is associated collection type information for the specified item instance type.
			/// </summary>
			public bool Exists(Type itemInstanceType)
			{
				return this.items.FirstOrDefault(type => type.ItemInstanceType.Equals(itemInstanceType)) != null;
			}
			
			private CollectionType[] GetCollectionTypesUsingTableAndDistinctFieldAttributes(Assembly assemblyToSearch)
			{
				
				List<CollectionType> collectionTypes = new List<CollectionType>();
				
				foreach (var collectionType in GetRealCollectionTypes(assemblyToSearch))
				{
					var customAttributes = collectionType.GetCustomAttributes(true);
					if (customAttributes != null)
					{
						var tableAttribute = (TableAttribute) (customAttributes.SingleOrDefault(attribute => attribute is TableAttribute));
						if (tableAttribute != null)
						{
                            var distinctFieldAttribute = (DistinctFieldAttribute)(customAttributes.SingleOrDefault(attribute => attribute is DistinctFieldAttribute));
							if (distinctFieldAttribute != null) // Technically, a DistinctFieldAttribute is always defined with a TableAttribute so this check is somewhat unnecessary
							{
                                var itemInstanceAttribute = (ItemInstanceAttribute)(customAttributes.SingleOrDefault(attribute => attribute is ItemInstanceAttribute));
								Type itemInstanceType = null;
								if (itemInstanceAttribute == null)
								{
									itemInstanceType = DatabaseObjectsItemInstance.GetGenericCollectionTArgument(collectionType);
								}
								else
								{
									itemInstanceType = itemInstanceAttribute.Type;
								}
								collectionTypes.Add(new CollectionType() {TableName = tableAttribute.Name, DistinctFieldName = distinctFieldAttribute.Name, ItemInstanceType = itemInstanceType});
							}
						}
					}
				}
				
				return collectionTypes.ToArray();
			}
			
			private IEnumerable<Type> GetRealCollectionTypes(Assembly assemblyToSearch)
			{
				List<string> excludeCollectionTypeNames = new List<string>();
				
#if DEBUG
				excludeCollectionTypeNames.Add("DatabaseObjects.UnitTests.ItemInstanceTests+TableWithNoItemInstanceAttribute");
				excludeCollectionTypeNames.Add("DatabaseObjects.UnitTests.AttributesInvalidTests+InvalidItemInstanceTypeCollection");
#endif
				return assemblyToSearch.GetTypes()
                    .Where(type => !type.IsAbstract)
                    .Where(type => type.IsSubclassOf(typeof(DatabaseObjects)))
                    .Where(type => !excludeCollectionTypeNames.Contains(type.FullName));
			}
		}
		
		private class CollectionType
		{
			public string TableName;
			public string DistinctFieldName;
			public Type ItemInstanceType;
		}
		
		/// <summary>
		/// </summary>
		/// <returns>Returns an empty list if there are no early binding required for the item instance type, otherwise the table joins required.</returns>
		private static CollectionTypesInAssemblies collectionTypesInAssemblies = new CollectionTypesInAssemblies();

		internal static SQL.SQLSelectTableJoins GetTableJoins(SQL.SQLSelectTable collectionTable, Type itemInstanceType)
		{
			var tableJoins = new SQL.SQLSelectTableJoins();

			GetTableJoins(collectionTable, itemInstanceType,  collectionTable, tableJoins);

			return tableJoins;
		}

		/// <summary>
		/// Creates all of the table joins by traversing the item instance type and finding ObjectReferenceEarlyBinding and FieldMapping attributed fields
		/// and then finding the corresponding item instance type and the associated collection / table that it should be joined to.
		/// This applies to all of the fields in the item instance type and also to any other table joins that the fields refer to.
		/// Essentially, ensuring that all table joins are created for the collection.
		/// </summary>
		/// <param name="leftTable">The joined table which contains all of the calculated table joins</param>
		/// <returns>The joined table which contains all of the calculated table joins</returns>
		internal static SQL.SQLSelectTableBase GetTableJoins(SQL.SQLSelectTable collectionTable, Type itemInstanceType, SQL.SQLSelectTableBase leftTable, SQL.SQLSelectTableJoins tableJoins)
		{
			foreach (var fieldInfo in GetObjectReferenceEarlyBindingFieldsAndForBaseTypes(itemInstanceType))
			{
				var fieldTypeAssembly = fieldInfo.ObjectReferenceFieldType.Assembly; // assume that the collection type is defined in the same assembly as the item instance type
				
				if (!collectionTypesInAssemblies[fieldTypeAssembly].Exists(fieldInfo.ObjectReferenceFieldType))
					throw new InvalidOperationException("The associated collection type for " + fieldInfo.ObjectReferenceFieldType.Name + " could not be found. The collection type must; inherit from DatabaseObjects.Generic.DatabaseObjects or have an ItemInstanceAttribute that specifies the type, have a TableAttribute and DistinctFieldAttributes and be defined in assembly " + fieldTypeAssembly.FullName);
				
				var fieldAssociatedCollectionInfo = collectionTypesInAssemblies[fieldTypeAssembly][fieldInfo.ObjectReferenceFieldType];
				
				SQL.SQLSelectTable rightTable = new SQL.SQLSelectTable(fieldAssociatedCollectionInfo.TableName);
				var tableJoin = tableJoins.Add(leftTable, SQL.SQLSelectTableJoin.Type.Inner, rightTable);
				tableJoin.Where.Add(new SQL.SQLFieldExpression(collectionTable, fieldInfo.FieldMappingName), SQL.ComparisonOperator.EqualTo, new SQL.SQLFieldExpression(new SQL.SQLSelectTable(fieldAssociatedCollectionInfo.TableName), fieldAssociatedCollectionInfo.DistinctFieldName));
				leftTable = tableJoin;

				// Also traverse "horizontally" / to a greater depth along the reference chain to ensure that referenced objects which also reference other objects are added to the table joins
				leftTable = GetTableJoins(new SQL.SQLSelectTable(fieldAssociatedCollectionInfo.TableName), fieldAssociatedCollectionInfo.ItemInstanceType, leftTable, tableJoins);
			}

			return leftTable;
		}

		/// <summary>
		/// Traverses the item instance type and any base classes for reference to fields marked with the
		/// ObjectReferenceEarlyBindingAttribute.
		/// </summary>
		private static ObjectReferenceEarlyBindingField[] GetObjectReferenceEarlyBindingFieldsAndForBaseTypes(Type itemInstanceType)
		{
			List<ObjectReferenceEarlyBindingField> fields = new List<ObjectReferenceEarlyBindingField>();
			
			//Loop through all of the base classes and stop when the base class is in the DatabaseObjects assembly or the root Object type is reached.
			//The root Object type could be reached if the IDatabaseObject is implemented directly and no classes are inherited from DatabaseObjects
			while (!itemInstanceType.Assembly.Equals(System.Reflection.Assembly.GetExecutingAssembly()) && !itemInstanceType.Equals(typeof(object)))
			{
				//Find ObjectReference fields with a FieldMappingAttribute - if no FieldMappingAttribute then the source field id cannot be found
				foreach (var fieldInfo in GetObjectReferenceEarlyBindingFields(itemInstanceType))
					fields.Add(fieldInfo);

				itemInstanceType = itemInstanceType.BaseType;
			}
			
			return fields.ToArray();
		}
		
		internal class ObjectReferenceEarlyBindingField
		{
			/// <summary>
			/// The type T that is defined on the DatabaseObjects.Generic.ObjectReference class.
			/// </summary>
			/// <remarks></remarks>
			public Type ObjectReferenceFieldType;
            public FieldInfo Field;
			public string FieldMappingName;
		}
		
		/// <summary>
		/// Returns all of the fields on an item instance type that have:
		/// 1. ObjectReferenceEarlyBindingAttribute specified.
		/// 2. FieldMappingAttribute specified
		/// 3. Attached to a type of DatabaseObjects.Generic.ObjectReference class
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// When ObjectReferenceEarlyBindingAttribute is associated incorrectly.
		/// </exception>
		internal static ObjectReferenceEarlyBindingField[] GetObjectReferenceEarlyBindingFields(Type itemInstanceType)
		{
			List<ObjectReferenceEarlyBindingField> fieldTypes = new List<ObjectReferenceEarlyBindingField>();
			
			foreach (FieldInfo field in itemInstanceType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly))
			{
				var customAttributes = field.GetCustomAttributes(inherit: true).Cast<Attribute>();
				if (customAttributes != null && customAttributes.SingleOrDefault(attribute => attribute is ObjectReferenceEarlyBindingAttribute) != null)
				{
					var fieldMappingAttribute = (FieldMappingAttribute) (customAttributes.SingleOrDefault(attribute => attribute is FieldMappingAttribute));
					if (fieldMappingAttribute != null)
					{
						if (GenericTypesAreEqual(field.FieldType, typeof(Generic.ObjectReference<IDatabaseObject>)))
							fieldTypes.Add(new ObjectReferenceEarlyBindingField() {Field = field, ObjectReferenceFieldType = field.FieldType.GetGenericArguments()[0], FieldMappingName = fieldMappingAttribute.FieldName});
						else
							throw new InvalidOperationException("ObjectReferenceEarlyBindingAttribute specified on " + field.DeclaringType.Name + "." + field.Name + " is invalid because it must be specified on a field of type " + typeof(Generic.ObjectReference<IDatabaseObject>).FullName);
					}
					else
						throw new InvalidOperationException("ObjectReferenceEarlyBindingAttribute specified on " + field.DeclaringType.Name + "." + field.Name + " is invalid because it must also be specified with a " + typeof(FieldMappingAttribute).FullName);
				}
			}
			
			return fieldTypes.ToArray();
		}
		
		/// <summary>
		/// Determines whether the base classes are equal, ignoring any type arguments specified on the types.
		/// </summary>
		internal static bool GenericTypesAreEqual(Type type1, Type type2)
		{
			return type1.FullName.Split('`')[0].Equals(type2.FullName.Split('`')[0]) && type1.Module.Equals(type2.Module);
		}
	}
}
