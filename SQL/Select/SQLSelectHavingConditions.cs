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
using System.Collections.Generic;

namespace DatabaseObjects.SQL
{
	public class SQLSelectHavingConditions : IEnumerable
	{
		private ArrayList pobjConditions = new ArrayList();
		private List<LogicalOperator> pobjLogicalOperators = new List<LogicalOperator>();
			
		public SQLSelectHavingConditions()
		{
		}
			
		public SQLSelectHavingCondition Add(AggregateFunction eAggregate, string strFieldName, ComparisonOperator eCompare, object objValue)
		{
			return Add(null, eAggregate, strFieldName, eCompare, objValue);
		}
			
		public SQLSelectHavingCondition Add(SQLSelectTable objTable, AggregateFunction eAggregate, string strFieldName, ComparisonOperator eCompare, object objValue)
		{
			if (String.IsNullOrEmpty(strFieldName))
				throw new ArgumentNullException("Fieldname is null");
				
			SQLFieldExpression objLeftHandExpression;
				
			if (eAggregate == AggregateFunction.None)
				objLeftHandExpression = new SQLFieldExpression(objTable, strFieldName);
			else
				objLeftHandExpression = new SQLFieldAggregateExpression(objTable, strFieldName, eAggregate);
				
			return Add(objLeftHandExpression, eCompare, new SQLValueExpression(objValue));
		}
			
		public void Add(SQLSelectHavingConditions objConditions)
		{
			if (objConditions.IsEmpty)
				throw new ArgumentException("SQLConditions does not contain any conditions.");
				
			EnsurePreviousLogicalOperatorExists();
			pobjConditions.Add(objConditions);
		}
			
		public SQLSelectHavingCondition Add(SQLExpression objLeftExpression, ComparisonOperator eCompare, SQLExpression objRightExpression)
		{
			var condition = new SQLSelectHavingCondition(objLeftExpression, eCompare, objRightExpression);

			EnsurePreviousLogicalOperatorExists();
            pobjConditions.Add(condition);

            return condition;
		}
			
		public bool IsEmpty
		{
			get
			{
				return pobjConditions.Count == 0;
			}
		}

		internal LogicalOperator[] LogicalOperators
		{
			get
			{
				return pobjLogicalOperators.ToArray();
			}
		}
			
		private void EnsurePreviousLogicalOperatorExists()
		{
			//Add the AND operator if an operator hasn't been called after the previous Add call
			if (pobjLogicalOperators.Count< pobjConditions.Count)
				this.AddLogicalOperator(LogicalOperator.And);
		}
			
		public void AddLogicalOperator(LogicalOperator eLogicalOperator)
		{
			if (pobjLogicalOperators.Count+ 1 > pobjConditions.Count)
				throw new Exceptions.DatabaseObjectsException("First call the Add function - this function has been called without a prior call to Add");
				
			pobjLogicalOperators.Add(eLogicalOperator);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return pobjConditions.GetEnumerator();
		}
	}
}
