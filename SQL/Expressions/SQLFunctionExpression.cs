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
