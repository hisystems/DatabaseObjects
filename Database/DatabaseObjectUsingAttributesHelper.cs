// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// Populates all fields and properties marked with the
	/// FieldMapping and FieldMappingObjectHook attributes with a set of
	/// </summary>
	/// --------------------------------------------------------------------------------
	public sealed class DatabaseObjectUsingAttributesHelper
	{
		private const System.Reflection.BindingFlags pcePropertyFieldScope = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly;
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Saves an object with fields or properties marked with DatabaseObjects.FieldMapping attributes
		/// to the SQL.SQLFieldValues object for saving to the database.
		/// </summary>
		/// <param name="objObject">
		/// The generic object that does not implement IDatabaseObject
		/// but contains FieldMapping attributes.
		/// </param>
		/// <remarks>
		/// Typically used from within the overridden SaveFields function when loading a generic object normally
		/// marked with the FieldMappingObjectHook attribute.
		/// </remarks>
		/// --------------------------------------------------------------------------------
		public static SQL.SQLFieldValues SaveFieldsForObject(object objObject)
		{
			if (objObject == null)
				throw new ArgumentNullException();
			
			return SaveFieldsForObject(objObject, objObject.GetType());
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Recurses through the lowest order base classes up to the highest order classes loading fields
		/// and hooked objects on each object.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static SQL.SQLFieldValues SaveFieldsForBaseTypes(object objObject, System.Type objType)
		{
			SQL.SQLFieldValues objFieldValues = new SQL.SQLFieldValues();
			
			//Need to check that the type is not at the object level which can occur when traversing through hooked objects
			if (!objType.Assembly.Equals(System.Reflection.Assembly.GetExecutingAssembly()) && !objType.Equals(typeof(object)))
			{
				objFieldValues.Add(SaveFieldsForBaseTypes(objObject, objType.BaseType));
				objFieldValues.Add(SaveFieldsForObject(objObject, objType));
				objFieldValues.Add(SaveFieldsForHookedObjects(objObject, objType));
			}
			
			return objFieldValues;
		}
		
		private static SQL.SQLFieldValues SaveFieldsForHookedObjects(object objObject, System.Type objType)
		{
			object[] objAttributes;
			object objFieldObject;
			SQL.SQLFieldValues objFieldValues = new SQL.SQLFieldValues();
			
			//Search for fields that have the FieldMappingObjectHookAttribute
			foreach (System.Reflection.FieldInfo objField in objType.GetFields(pcePropertyFieldScope))
			{
				objAttributes = objField.GetCustomAttributes(typeof(FieldMappingObjectHookAttribute), true);
				if (objAttributes != null)
				{
					foreach (FieldMappingObjectHookAttribute objFieldMappingObjectHook in objAttributes)
					{
						if (objField.FieldType.IsValueType)
                            throw new Exceptions.DatabaseObjectsException("Field " + objField.FullName() + " marked with FieldMappingObjectHook attribute on value type - must be a class type");
						else
						{
							objFieldObject = objField.GetValue(objObject);
							if (objFieldObject == null)
                                throw new Exceptions.DatabaseObjectsException("Field " + objField.FullName() + " marked with " + typeof(FieldMappingObjectHookAttribute).Name + " is Nothing");

							objFieldValues.Add(SaveFieldsForBaseTypes(objFieldObject, objFieldObject.GetType()));
						}
					}
				}
			}
			
			//Search for properties that have the FieldMappingObjectHookAttribute
			foreach (System.Reflection.PropertyInfo objProperty in objType.GetProperties(pcePropertyFieldScope))
			{
				objAttributes = objProperty.GetCustomAttributes(typeof(FieldMappingObjectHookAttribute), true);
				if (objAttributes != null)
				{
					foreach (FieldMappingObjectHookAttribute objFieldMappingObjectHook in objAttributes)
					{
						if (objProperty.CanRead)
						{
							if (objProperty.PropertyType.IsValueType)
								throw new Exceptions.DatabaseObjectsException("Property " + objProperty.FullName() + " marked with FieldMappingObjectHook attribute on value type - must be a class type");
							else
							{
								objFieldObject = objProperty.GetValue(objObject, null);
								if (objFieldObject == null)
                                    throw new Exceptions.DatabaseObjectsException("Property " + objProperty.FullName() + " marked with " + typeof(FieldMappingObjectHookAttribute).Name + " is Nothing");

								objFieldValues.Add(SaveFieldsForBaseTypes(objFieldObject, objFieldObject.GetType()));
							}
						}
					}
				}
			}
			
			return objFieldValues;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Saves an object with fields or properties marked with DatabaseObjects.FieldMapping attributes
		/// to the fields values, which can be used to save to the database.
		/// </summary>
		/// --------------------------------------------------------------------------------
		private static SQL.SQLFieldValues SaveFieldsForObject(object objObject, System.Type objType)
		{
			object[] objAttributes;
			SQL.SQLFieldValues objFieldValues = new SQL.SQLFieldValues();
			
			//Search for fields that have the FieldMappingAttribute
			foreach (System.Reflection.FieldInfo objField in objType.GetFields(pcePropertyFieldScope))
			{
				objAttributes = objField.GetCustomAttributes(typeof(FieldMappingAttribute), true);
				if (objAttributes != null)
				{
					foreach (FieldMappingAttribute objFieldMapping in objAttributes)
					{
						try
						{
							//If an enum field then convert the enum to an integer
							if (objField.FieldType.IsEnum)
								objFieldValues.Add(objFieldMapping.FieldName, System.Convert.ToInt32(objField.GetValue(objObject)));
                            else if (objField.GetValue(objObject) is ObjectReference)   //Get the distinct value if this is an ObjectReference field
								objFieldValues.Add(objFieldMapping.FieldName, ((ObjectReference) (objField.GetValue(objObject))).DistinctValue);
							else
								objFieldValues.Add(objFieldMapping.FieldName, objField.GetValue(objObject));
						}
						catch (Exception ex)
						{
							throw new Exceptions.DatabaseObjectsException("Field '" + objField.FullName() + "' could not be read.", ex);
						}
					}
				}
			}
			
			//Search for properties that have the FieldMappingAttribute
			foreach (System.Reflection.PropertyInfo objProperty in objType.GetProperties(pcePropertyFieldScope))
			{
				objAttributes = objProperty.GetCustomAttributes(typeof(FieldMappingAttribute), true);
				if (objAttributes != null)
				{
					foreach (FieldMappingAttribute objFieldMapping in objAttributes)
					{
						if (objProperty.CanRead)
						{
							try
							{
								//If an enum field then convert the enum to an integer
								if (objProperty.PropertyType.IsEnum)
									objFieldValues.Add(objFieldMapping.FieldName, System.Convert.ToInt32(objProperty.GetValue(objObject, null)));
								else
									objFieldValues.Add(objFieldMapping.FieldName, objProperty.GetValue(objObject, null));
							}
							catch (Exception ex)
							{
								throw new Exceptions.DatabaseObjectsException("Property '" + objProperty.FullName() + "' could not be read.", ex);
							}
						}
					}
				}
			}
			
			return objFieldValues;
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Performs loading an object marked with the FieldMappingObjectHook attribute.
		/// Loads an object with fields or properties marked with DatabaseObjects.FieldMapping attributes
		/// with the fields from the database.
		/// </summary>
		/// <param name="objObject">
		/// The generic object that does not implement IDatabaseObject
		/// but contains FieldMapping attributes.
		/// </param>
		/// <remarks>
		/// Typically used from within the overridden LoadFields function when loading a generic object normally
		/// marked with the FieldMappingObjectHook attribute.
		/// </remarks>
		/// --------------------------------------------------------------------------------
		public static void LoadFieldsForObject(object objObject, SQL.SQLFieldValues objFields)
		{
			if (objObject == null)
				throw new ArgumentNullException();
			
			LoadFieldsForObject(objObject, objObject.GetType(), objFields);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Recurses through the lowest order base classes up to the highest order classes loading fields
		/// and hooked objects on each object.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static void LoadFieldsForBaseTypes(object objObject, System.Type objType, SQL.SQLFieldValues objFields)
		{
			//Skip classes in the DatabaseObjects assembly or class instances of type Object
			//Need to check that the type is not at the object level which can occur when traversing through hooked objects
			if (!objType.Assembly.Equals(System.Reflection.Assembly.GetExecutingAssembly()) && !objType.Equals(typeof(object)))
			{
				LoadFieldsForBaseTypes(objObject, objType.BaseType, objFields);
				LoadFieldsForObject(objObject, objType, objFields);
				LoadFieldsForHookedObjects(objObject, objType, objFields);
				LoadFieldsForObjectReferenceEarlyBinding(objObject, objType, objFields);
			}
		}
		
		private static void LoadFieldsForObjectReferenceEarlyBinding(object objectToLoad, System.Type type, SQL.SQLFieldValues fieldValues)
		{
			foreach (var earlyBindingField in ObjectReferenceEarlyBinding.GetObjectReferenceEarlyBindingFields(type))
			{
				var objectReference = (ObjectReference) (earlyBindingField.Field.GetValue(objectToLoad));
				objectReference.Object = Database.ObjectFromFieldValues(objectReference.ParentCollection, fieldValues);
			}
		}
		
		private static void LoadFieldsForHookedObjects(object objObject, System.Type objType, SQL.SQLFieldValues objFields)
		{
			object[] objAttributes;
			object objFieldObject;
			
			//Search for fields that have the FieldMappingObjectHookAttribute
			foreach (System.Reflection.FieldInfo objField in objType.GetFields(pcePropertyFieldScope))
			{
				objAttributes = objField.GetCustomAttributes(typeof(FieldMappingObjectHookAttribute), true);
				if (objAttributes != null)
				{
					foreach (FieldMappingObjectHookAttribute objFieldMappingObjectHook in objAttributes)
					{
						if (objField.FieldType.IsValueType)
						{
							throw new Exceptions.DatabaseObjectsException("Field " + objField.FullName() + " marked with FieldMappingObjectHook attribute on value type - must be a class type");
						}
						else
						{
							objFieldObject = objField.GetValue(objObject);
							if (objFieldObject == null)
                                throw new Exceptions.DatabaseObjectsException("Field " + objField.FullName() + " marked with " + typeof(FieldMappingObjectHookAttribute).Name + " is Nothing");

							LoadFieldsForBaseTypes(objFieldObject, objFieldObject.GetType(), objFields);
						}
					}
				}
			}
			
			//Search for properties that have the FieldMappingObjectHookAttribute
			foreach (System.Reflection.PropertyInfo objProperty in objType.GetProperties(pcePropertyFieldScope))
			{
				objAttributes = objProperty.GetCustomAttributes(typeof(FieldMappingObjectHookAttribute), true);
				if (objAttributes != null)
				{
					foreach (FieldMappingObjectHookAttribute objFieldMappingObjectHook in objAttributes)
					{
						if (objProperty.CanRead)
						{
							if (objProperty.PropertyType.IsValueType)
                                throw new Exceptions.DatabaseObjectsException("Property " + objProperty.FullName() + " marked with FieldMappingObjectHook attribute on value type - must be a class type");
							else
							{
								objFieldObject = objProperty.GetValue(objObject, null);
								if (objFieldObject == null)
                                    throw new Exceptions.DatabaseObjectsException("Property " + objProperty.FullName() + " marked with " + typeof(FieldMappingObjectHookAttribute).Name + " is Nothing");

								LoadFieldsForBaseTypes(objFieldObject, objFieldObject.GetType(), objFields);
							}
						}
					}
				}
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Loads an object with fields or properties marked with DatabaseObjects.FieldMapping attributes
		/// with the fields from the database.
		/// </summary>
		/// --------------------------------------------------------------------------------
		private static void LoadFieldsForObject(object objObject, System.Type objType, SQL.SQLFieldValues objFields)
		{
			object[] objAttributes;
			
			//Search for fields that have the FieldMappingAttribute
			foreach (System.Reflection.FieldInfo objField in objType.GetFields(pcePropertyFieldScope))
			{
				objAttributes = objField.GetCustomAttributes(typeof(FieldMappingAttribute), true);
				if (objAttributes != null)
				{
					foreach (FieldMappingAttribute objFieldMapping in objAttributes)
					{
						if (!(objFields[objFieldMapping.FieldName].Value == DBNull.Value))
						{
							try
							{
								//If an enum field then convert the integer to the enum equivalent
								if (objField.FieldType.IsEnum)
									objField.SetValue(objObject, System.Enum.ToObject(objField.FieldType, objFields[objFieldMapping.FieldName].Value));
                                else if (objField.GetValue(objObject) is ObjectReference)   //Set the distinct value if this is an ObjectReference field
									((ObjectReference) (objField.GetValue(objObject))).DistinctValue = objFields[objFieldMapping.FieldName].Value;
								else if (objField.FieldType.Equals(typeof(bool)) && !objFields[objFieldMapping.FieldName].Value.GetType().Equals(typeof(bool)))
									//MySQL connection provides BIT data as a ULong data type, so convert to boolean.
									objField.SetValue(objObject, System.Convert.ToInt32(objFields[objFieldMapping.FieldName].Value) != 0);
								else
									objField.SetValue(objObject, objFields[objFieldMapping.FieldName].Value);
							}
							catch (Exception ex)
							{
								throw new Exceptions.DatabaseObjectsException("Field '" + objField.FullName() + "' could not be set.", ex);
							}
						}
					}
				}
			}
			
			//Search for properties that have the FieldMappingAttribute
			foreach (System.Reflection.PropertyInfo objProperty in objType.GetProperties(pcePropertyFieldScope))
			{
				objAttributes = objProperty.GetCustomAttributes(typeof(FieldMappingAttribute), true);
				//Diagnostics.Debug.Print("Traversing : " & objType.Name & "." & objProperty.Name)
				if (objAttributes != null)
				{
					foreach (FieldMappingAttribute objFieldMapping in objAttributes)
					{
						if (objProperty.CanWrite)
						{
							if (!(objFields[objFieldMapping.FieldName].Value == DBNull.Value))
							{
								try
								{
									//If an enum field then convert the integer to the enum equivalent
									if (objProperty.PropertyType.IsEnum)
									{
										objProperty.SetValue(objObject, System.Enum.ToObject(objProperty.PropertyType, objFields[objFieldMapping.FieldName].Value), null);
									}
									else if (objProperty.PropertyType.Equals(typeof(bool)) && !objFields[objFieldMapping.FieldName].Value.GetType().Equals(typeof(bool)))
									{
										//MySQL connection provides BIT data as a ULong data type, so convert to boolean.
										objProperty.SetValue(objObject, System.Convert.ToInt32(objFields[objFieldMapping.FieldName].Value) != 0, null);
									}
									else
									{
										objProperty.SetValue(objObject, objFields[objFieldMapping.FieldName].Value, null);
									}
								}
								catch (Exception ex)
								{
									throw new Exceptions.DatabaseObjectsException("Property '" + objProperty.FullName() + "' could not be set.", ex);
								}
							}
						}
					}
				}
			}
		}
	}
}
