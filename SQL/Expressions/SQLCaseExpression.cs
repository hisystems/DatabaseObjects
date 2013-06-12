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
