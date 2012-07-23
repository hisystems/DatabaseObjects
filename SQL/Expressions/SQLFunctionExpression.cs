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
		private string pstrFunctionName;
		private SQLExpression[] pobjFunctionArguments;
			
		public bool IncludeParenthesesWhenArgumentsEmpty = false;
			
		public SQLFunctionExpression(string strFunctionName)
		{
			if (string.IsNullOrEmpty(strFunctionName))
				throw new ArgumentNullException();
				
			pstrFunctionName = strFunctionName;
		}
			
		public SQLFunctionExpression(string strFunctionName, params SQLExpression[] arguments) 
            : this(strFunctionName)
		{
			InitializeArguments(arguments);
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
			
		protected virtual string FunctionName(Database.ConnectionType eConnectionType)
		{
			return pstrFunctionName;
		}
			
		internal override string SQL(Database.ConnectionType eConnectionType)
		{
			string strArguments = string.Empty;
				
			if (pobjFunctionArguments != null)
				strArguments = String.Join(", ", pobjFunctionArguments.Select(argument => argument.SQL(eConnectionType)).ToArray());
				
			if ((IncludeParenthesesWhenArgumentsEmpty && pobjFunctionArguments == null) || pobjFunctionArguments != null)
				strArguments = "(" + strArguments + ")";
				
			return this.FunctionName(eConnectionType) + strArguments;
		}
	}
}
