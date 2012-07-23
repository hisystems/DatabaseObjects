// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.IO;

namespace DatabaseObjects
{
	internal static class SystemReflectionExtensions
	{
		/// <summary>
		/// Returns the full class name and field name.
		/// </summary>
		public static string FullName(this System.Reflection.FieldInfo field)
		{
			return field.DeclaringType.FullName + System.Type.Delimiter + field.Name;
		}
		
		/// <summary>
		/// Returns the full class name and property name.
		/// </summary>
		public static string FullName(this System.Reflection.PropertyInfo property)
		{
			return property.DeclaringType.FullName + System.Type.Delimiter + property.Name;
		}
	}
}
