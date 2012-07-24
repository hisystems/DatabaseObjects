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
		public class CasesCollection : IEnumerable<Case>
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

			IEnumerator<Case> IEnumerable<Case>.GetEnumerator()
			{
				return pobjCases.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return pobjCases.GetEnumerator();
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

			public SQLExpression WhenCondition
			{
				get
				{
					return pobjWhenCondition;
				}
			}

			public SQLExpression Result
			{
				get
				{
					return pobjResult;
				}
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

		public SQLExpression InputExpression
		{
			get
			{
				return pobjInputExpression;
			}
		}
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeCaseExpression(this);
		}
	}
}
