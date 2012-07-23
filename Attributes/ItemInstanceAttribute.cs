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

/// --------------------------------------------------------------------------------
/// <summary>
/// Specifies the type of class instance that will represent each database record / object.
/// The type must have a constructor with argument of type DatabaseObjects.DatabaseObjects.
/// This is the same arguments that are required for a class that inherits from DatabaseObject.
/// Alternatively, an empty constructor if available will be used.
/// The type must implement IDatabaseObject or inherit from DatabaseObject.
/// Using this attribute is logically equivalent to overridding the ItemInstance function.
/// </summary>
/// --------------------------------------------------------------------------------
namespace DatabaseObjects
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ItemInstanceAttribute : Attribute
	{
		private Type pobjType;
		
	    /// <summary>
	    /// Indicates the type that inherits DatabaseObject or implements IDatabaseObject.
	    /// </summary>
	    public ItemInstanceAttribute(Type objType)
	    {
		    if (!objType.GetInterfaces().Contains(typeof(IDatabaseObject)))
			    throw new ArgumentException("Type " + objType.FullName + " passed to ItemInstanceAttribute does not implement " + typeof(IDatabaseObject).FullName);
			
		    pobjType = objType;
	    }
		
	    public Type Type
	    {
		    get
		    {
			    return pobjType;
		    }
	    }
    }
}
