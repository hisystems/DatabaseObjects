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
	public class SQLSelectTableJoinConditions : IEnumerable
	{
		private List<LogicalOperator> pobjLogicalOperators = new List<LogicalOperator>();
		private ArrayList pobjConditions = new ArrayList();
		private SQLSelectTableJoin pobjParent;
			
		public SQLSelectTableJoinConditions()
		{
		}
			
		internal SQLSelectTableJoinConditions(SQLSelectTableJoin objParent)
		{
			pobjParent = objParent;
		}
			
		public SQLSelectTableJoinCondition Add()
		{
			return Add("", ComparisonOperator.EqualTo, "");
		}
			
		public void Add(SQLSelectTableJoinConditions conditions)
		{
			AddLogicalOperatorIfRequired();
			pobjConditions.Add(conditions);
		}
			
		[Obsolete("Instead use other overloaded operator Add(SQLExpression, ComparisonOperator, SQLExpression). For example: Add(New SQLFieldExpression(New SQLSelectTable(\"Table1\"), \"Field1\"), ComparisonOperator.EqualTo, New SQLFieldExpression(New SQLSelectTable(\"Table2\"), \"Field2\")")]
        public SQLSelectTableJoinCondition Add(string strLeftTableFieldName, ComparisonOperator eCompare, string strRightTableFieldName)
		{
			if (pobjParent == null)
				throw new InvalidOperationException("Use overloaded Add(SQLExpression, ComparisonOperator, SQLExpression) instead");

			//The Add function is here basically for backward compatibility when the conditions could only accept field names and the left and right tables from the parent join were used as table aliases.
			//Now that that SQLSelectTableBase is being used (which can represent a table or joined tables) we need to check that parent left and right tables are only SQLSelectTable objects.
			if (!(pobjParent.LeftTable is SQLSelectTable))
				throw new ArgumentException("The left table is expected to be an SQLSelectTable so that the left table alias can be utilised for the strLeftTableFieldName argument. Use the Add(SQLExpression, ComparisonOperator, SQLExpression) overload instead.");
			else if (!(pobjParent.RightTable is SQLSelectTable))
				throw new ArgumentException("The right table is expected to be an SQLSelectTable so that the right table alias can be utilised for the strRightTableFieldName argument. Use the Add(SQLExpression, ComparisonOperator, SQLExpression) overload instead.");

            SQLSelectTableJoinCondition joinCondition;
				
			EnsureComparisonOperatorValid(eCompare);
			AddLogicalOperatorIfRequired();

            joinCondition = new SQLSelectTableJoinCondition();
            joinCondition.LeftExpression = new SQLFieldExpression((SQLSelectTable)pobjParent.LeftTable, strLeftTableFieldName);
            joinCondition.Compare = eCompare;
            joinCondition.RightExpression = new SQLFieldExpression((SQLSelectTable)pobjParent.RightTable, strRightTableFieldName);

            pobjConditions.Add(joinCondition);

            return joinCondition;
		}
			
		public SQLSelectTableJoinCondition Add(SQLExpression objLeftExpression, ComparisonOperator eCompare, SQLExpression objRightExpression)
		{
			EnsureComparisonOperatorValid(eCompare);
			AddLogicalOperatorIfRequired();

            var joinCondition = new SQLSelectTableJoinCondition();

            joinCondition.LeftExpression = objLeftExpression;
            joinCondition.Compare = eCompare;
            joinCondition.RightExpression = objRightExpression;

            pobjConditions.Add(joinCondition);

            return joinCondition;
		}
			
		private void AddLogicalOperatorIfRequired()
		{
			//Add the AND operator if an operator hasn't been called after the previous Add call
			if (pobjLogicalOperators.Count< pobjConditions.Count)
				this.AddLogicalOperator(LogicalOperator.And);
		}
			
		private void EnsureComparisonOperatorValid(ComparisonOperator eCompare)
		{
			if (eCompare == ComparisonOperator.Like || eCompare == ComparisonOperator.NotLike)
				throw new Exceptions.DatabaseObjectsException("LIKE operator is not supported for table joins.");
		}
			
		public void AddLogicalOperator(LogicalOperator eLogicalOperator)
		{
			if (pobjLogicalOperators.Count + 1 > pobjConditions.Count)
				throw new Exceptions.DatabaseObjectsException("First call the Add function - this function has been called without a prior call to Add");
				
			pobjLogicalOperators.Add(eLogicalOperator);
		}
			
		public bool IsEmpty
		{
			get
			{
				return pobjConditions.Count == 0;
			}
		}
			
		public int Count
		{
			get
			{
				return pobjConditions.Count;
			}
		}

		internal LogicalOperator[] LogicalOperators
		{
			get
			{
				return pobjLogicalOperators.ToArray();
			}
		}
			
		public void Delete(ref SQLSelectTableJoinConditions objConditions)
		{
			if (!pobjConditions.Contains(objConditions))
				throw new IndexOutOfRangeException();
				
			pobjConditions.Remove(objConditions);
			objConditions = null;
		}
			
		public void Delete(ref SQLSelectTableJoinCondition objOrderByField)
		{
			if (!pobjConditions.Contains(objOrderByField))
				throw new IndexOutOfRangeException();
				
			pobjConditions.Remove(objOrderByField);
			objOrderByField = null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return pobjConditions.GetEnumerator();
		}
	}
}
