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

namespace DatabaseObjects
{
	internal sealed class DatabaseObjectsItemInstance
	{		
		/// <summary>
		/// </summary>
		/// <param name="itemInstanceTypeToCreate">The type of DatabaseObjects.DatabaseObject to create.</param>
		/// <param name="databaseObjects">Parameter that is passed to the constructor of the DatabaseObject to create. If there is a default constructor then this argument is not used.</param>
		public static IDatabaseObject CreateItemInstance(Type itemInstanceTypeToCreate, DatabaseObjects databaseObjects)
		{			
			object objObjectInstance = null;
			
			foreach (ConstructorInfo objConstructor in itemInstanceTypeToCreate.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				ParameterInfo[] objConstructorParameters = objConstructor.GetParameters();
				if (objConstructorParameters.Length == 0)
				{
					objObjectInstance = objConstructor.Invoke(null);
					break;
				}
				else if (objConstructorParameters.Length == 1 && (objConstructorParameters[0].ParameterType.IsSubclassOf(typeof(DatabaseObjects)) || objConstructorParameters[0].ParameterType.Equals(typeof(DatabaseObjects))))
				{
					objObjectInstance = objConstructor.Invoke(new[] {databaseObjects});
					break;
				}
			}
			
			if (objObjectInstance == null)
				throw new Exceptions.DatabaseObjectsException("An empty constructor or constructor with argument DatabaseObjects.DatabaseObjects (or subclass) could not be found for type '" + itemInstanceTypeToCreate.FullName + "'. This type has been specified by the ItemInstanceAttribute for the type '" + databaseObjects.GetType().FullName + "' or as the T argument.");
			else if (!(objObjectInstance is IDatabaseObject))
				throw new Exceptions.DatabaseObjectsException("'" + itemInstanceTypeToCreate.FullName + "' does not implement IDatabaseObject or inherit from DatabaseObject. Type was specified for use by the ItemInstanceAttribute on the type '" + databaseObjects.GetType().FullName + "'");
			else
				return (IDatabaseObject)objObjectInstance;
		}
		
		/// <summary>
		/// Returns the first generic argument that is passed to the DatabaseObjects.Generic.DatabaseObjects class (or super class).
		/// This is used to determine the type of item that the collection represents and the type of class that should be returned from
		/// the IDatabaseObjects.ItemInstance function.
		/// </summary>
		public static Type GetGenericCollectionTArgument(Type collectionType)
		{			
			Type currentCollectionType = collectionType;
			
			while (currentCollectionType != null && !(currentCollectionType.IsGenericType && currentCollectionType.GetGenericArguments()[0].IsSubclassOf(typeof(DatabaseObject))))
				currentCollectionType = currentCollectionType.BaseType;
			
			if (currentCollectionType != null)
				return currentCollectionType.GetGenericArguments()[0];
			else
				throw new Exceptions.DatabaseObjectsException("The ItemInstance type could not be found for '" + collectionType.FullName + "' because it does not inherit from DatabaseObjects.Generic.DatabaseObjects");
		}
	}
}
