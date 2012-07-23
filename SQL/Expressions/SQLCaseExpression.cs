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
using System.Collections.Generic;

namespace DatabaseObjects.SQL
{
	/// <summary>
	/// Allows the use of CASE statements. Specifically:
	///
	/// Simple CASE expression:
	/// CASE input_expression
	/// WHEN when_expression THEN result_expression [ ...n ]
	/// [ ELSE else_result_expression ]
	/// END
	///
	/// Searched CASE expression:
	/// CASE
	/// WHEN Boolean_expression THEN result_expression [ ...n ]
	/// [ ELSE else_result_expression ]
	/// END
	/// </summary>
	public class SQLCaseExpression : SQLExpression
	{
		public class CasesCollection
		{
			private List<Case> pobjCases = new List<Case>();
				
			public void Add(SQLExpression objWhenCondition, SQLExpression objResult)
			{
				pobjCases.Add(new Case(objWhenCondition, objResult));
			}
				
			public void Add(Case objCase)
			{
				pobjCases.Add(objCase);
			}
				
			internal string SQL(Database.ConnectionType eConnectionType)
			{
				if (pobjCases.Count == 0)
					throw new InvalidOperationException("There are no cases for the CASE statement");
					
				return string.Join(" ", pobjCases.Select(objCase => objCase.SQL(eConnectionType)).ToArray());
			}
		}
			
		public class Case
		{
			private SQLExpression pobjWhenCondition;
			private SQLExpression pobjResult;
				
			/// <summary>
			///
			/// </summary>
			public Case(SQLExpression objWhenCondition, SQLExpression objResult)
			{
				if (objWhenCondition == null)
					throw new ArgumentNullException("When condition");
				else if (objResult == null)
					throw new ArgumentNullException("Result");
					
				pobjWhenCondition = objWhenCondition;
				pobjResult = objResult;
			}
				
			internal string SQL(Database.ConnectionType eConnectionType)
			{
				return "WHEN " + pobjWhenCondition.SQL(eConnectionType) + " THEN " + pobjResult.SQL(eConnectionType);
			}
		}
			
		private SQLExpression pobjInputExpression;
		private CasesCollection pobjCases = new CasesCollection();
		private SQLExpression pobjElseResult;
			
		public SQLCaseExpression()
		{
		}
			
		public SQLCaseExpression(SQLExpression objInputExpression)
		{
			if (objInputExpression == null)
				throw new ArgumentNullException();
				
			pobjInputExpression = objInputExpression;
		}
			
		public CasesCollection Cases
		{
			get
			{
				return pobjCases;
			}
		}
			
		public SQLExpression ElseResult
		{
			get
			{
				return pobjElseResult;
			}
				
			set
			{
				pobjElseResult = value;
			}
		}
			
		internal override string SQL(Database.ConnectionType eConnectionType)
		{
			string strSQL = "CASE";
				
			if (pobjInputExpression != null)
				strSQL += " " + pobjInputExpression.SQL(eConnectionType);
				
			strSQL += " " + pobjCases.SQL(eConnectionType);
				
			if (pobjElseResult != null)
				strSQL += " ELSE " + pobjElseResult.SQL(eConnectionType);
				
			strSQL += " END";
				
			return strSQL;
		}
	}
}
