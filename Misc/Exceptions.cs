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

namespace DatabaseObjects.Exceptions
{
	/// <summary>
	/// Represents a general DatabaseObjects library exception
	/// </summary>
	[Serializable()]
    public class DatabaseObjectsException : ApplicationException
	{
		public DatabaseObjectsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) 
            : base(info, context)
		{
		}
			
		public DatabaseObjectsException(string strMessage) 
            : base(strMessage)
		{
		}
			
		public DatabaseObjectsException(string strMessage, Exception objInnerException)
            : base(strMessage, objInnerException)
		{
		}
	}
		
	public class ObjectAlreadyExistsException : ApplicationException
	{
		private IDatabaseObject pobjItem;
		private object pobjKey;
			
		public ObjectAlreadyExistsException(IDatabaseObject objItem, object objKey) 
            : base(objItem.GetType().Name + ": '" + objKey.ToString() + "'")
		{
			pobjItem = objItem;
			pobjKey = objKey;
		}
			
		public IDatabaseObject Item
		{
			get
			{
				return pobjItem;
			}
		}
			
		public object Key
		{
			get
			{
				return pobjKey;
			}
		}
	}
		
	public class ObjectDoesNotExistException : ApplicationException
	{
		private object pobjDistinctOrKeyValue;
			
		public ObjectDoesNotExistException(IDatabaseObjects objCollection, object objDistinctOrKeyValue) 
            : base(objCollection.GetType().Name + ": '" + objDistinctOrKeyValue.ToString() + "'")
		{
			pobjDistinctOrKeyValue = objDistinctOrKeyValue;
		}
			
		public ObjectDoesNotExistException(IDatabaseObject objItem) 
            : base(objItem.GetType().Name + ": '" + objItem.DistinctValue.ToString() + "'")
		{
			pobjDistinctOrKeyValue = objItem.DistinctValue;
		}
			
		public object DistinctOrKeyValue
		{
			get
			{
				return pobjDistinctOrKeyValue;
			}
		}
	}
		
	public class MethodLockedException : ApplicationException
	{
		public MethodLockedException()
		{
		}
			
		public MethodLockedException(string strMessage) 
            : base(strMessage)
		{
				
		}
	}
		
	/// <summary>
	/// Thrown when an object tries to be locked - but it is already locked.
	/// </summary>
	/// <remarks>
	/// Originally the DatabaseObjectsLockController.Lock would throw a DatabaseObjectsException but now it throws a ObjectAlreadyLockedException.
	/// </remarks>
	public class ObjectAlreadyLockedException : DatabaseObjectsException
	{
		public ObjectAlreadyLockedException(IDatabaseObjects objCollection, IDatabaseObject objObject) 
            : base(objObject.GetType().Name + "." + objCollection.DistinctFieldName() + " " + objObject.DistinctValue.ToString() + " is already locked")
		{
		}
	}
}
