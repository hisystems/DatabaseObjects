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
using System.Linq;
using DatabaseObjects.SQL.Serializers;
	
namespace DatabaseObjects.SQL
{
	/// <summary>
	/// Allows an SQL function to utilised in a WHERE clause or as a calculated column.
	/// </summary>
	/// <remarks>
	/// Typically used to access a non database-agnostic function.
	/// </remarks>
	public class SQLFunctionExpression : SQLExpression
	{
		private SQLExpression[] pobjFunctionArguments;

		private string functionName;
		public bool IncludeParenthesesWhenArgumentsEmpty = false;

		public SQLFunctionExpression(string functionName, params SQLExpression[] arguments)
			: this(arguments)
		{
			this.functionName = functionName;
		}

		/// <summary>
		/// Used when the arguments are known but the function name is dependant on the connection type.
		/// Typically, in this scenario the FunctionName() will be overridden.
		/// </summary>
		/// <param name="arguments"></param>
		/// <remarks></remarks>
		protected SQLFunctionExpression(params SQLExpression[] arguments)
		{
			InitializeArguments(arguments);
		}
			
		private void InitializeArguments(params SQLExpression[] arguments)
		{
            if (arguments.Any(item => item == null))
				throw new ArgumentNullException();
				
			pobjFunctionArguments = arguments;
		}

		public SQLExpression[] Arguments
		{
			get
			{
				return pobjFunctionArguments;
			}
		}

		public bool HasArguments
		{
			get
			{
				return pobjFunctionArguments.Length > 0;
			}
		}

		internal override string SQL(Serializer serializer)
		{
			return this.functionName + serializer.SerializeFunctionExpressionArguments(this);
		}
	}
}
