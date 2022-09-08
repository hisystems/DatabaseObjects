// _________________________________________________________________________
//
//  � Hi-Integrity Systems 2010. All rights reserved.
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
