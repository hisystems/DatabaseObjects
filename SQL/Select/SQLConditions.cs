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
using DatabaseObjects.SQL.Serializers;

namespace DatabaseObjects.SQL
{
	public class SQLConditions : SQLExpression, IEnumerable
	{
		private ArrayList pobjSQLConditions = new ArrayList();
		private List<LogicalOperator> pobjLogicalOperators = new List<LogicalOperator>();
			
		public SQLConditions()
		{
		}
			
		public SQLCondition Add(string strFieldName, ComparisonOperator eCompare, object objValue)
		{
			return Add(strFieldName, eCompare, objValue, null);
		}
			
		public SQLCondition Add(string strFieldName, ComparisonOperator eCompare, object objValue, SQLSelectTable objTable)
		{
			if (String.IsNullOrEmpty(strFieldName))
				throw new ArgumentNullException("Fieldname is null");
				
			EnsurePreviousLogicalOperatorExists();
				
			var condition = new SQLCondition();
				
			condition.Table = objTable;
			condition.FieldName = strFieldName;
			condition.Compare = eCompare;
			condition.Value = objValue;
				
			pobjSQLConditions.Add(condition);
				
			return condition;
		}
			
		public void Add(SQLCondition objCondition)
		{
			this.Add(objCondition.FieldName, objCondition.Compare, objCondition.Value, objCondition.Table);
		}
			
		public void Add(SQLConditions objConditions)
		{
			if (objConditions.IsEmpty)
				throw new ArgumentException("SQLConditions does not contain any conditions.");
				
			EnsurePreviousLogicalOperatorExists();
			pobjSQLConditions.Add(objConditions);
		}
			
		public SQLConditionExpression Add(SQLExpression objLeftExpression, ComparisonOperator eCompare, SQLExpression objRightExpression)
		{
			SQLConditionExpression objCondition = new SQLConditionExpression(objLeftExpression, eCompare, objRightExpression);
				
			this.Add(objCondition);
				
			return objCondition;
		}
			
		public void Add(SQLConditionExpression objCondition)
		{
			if (objCondition == null)
				throw new ArgumentNullException();
				
			EnsurePreviousLogicalOperatorExists();
			pobjSQLConditions.Add(objCondition);
		}
			
		public SQLCondition Add()
		{
			return Add("", ComparisonOperator.EqualTo, null, null);
		}
			
		public SQLConditionInSelect AddInSelect()
		{
			return AddInSelect(string.Empty, null, null);
		}
			
		public SQLConditionInSelect AddInSelect(string strFieldName, SQLSelect objSelect)
		{
			return AddInSelect(strFieldName, objSelect, null);
		}
			
		public SQLConditionInSelect AddInSelect(string strFieldName, SQLSelect objSelect, SQLSelectTable objTable)
		{
			EnsurePreviousLogicalOperatorExists();
				
			var condition = new SQLConditionInSelect();
			condition.Table = objTable;
			condition.FieldName = strFieldName;
			condition.Select = objSelect;
				
			pobjSQLConditions.Add(condition);
				
			return condition;
		}
			
		public SQLConditionInSelect AddNotInSelect()
		{
			return AddNotInSelect(string.Empty, null, null);
		}
			
		public SQLConditionInSelect AddNotInSelect(string strFieldName, SQLSelect objSelect)
		{
			return AddNotInSelect(strFieldName, objSelect, null);
		}
			
		public SQLConditionInSelect AddNotInSelect(string strFieldName, SQLSelect objSelect, SQLSelectTable objTable)
		{
			var condition = AddInSelect(strFieldName, objSelect, objTable);
			condition.NotInSelect = true;
				
			return condition;
		}
			
		public SQLConditionSelect AddSelect()
		{
			return AddSelect(null, ComparisonOperator.EqualTo, null);
		}
			
		public SQLConditionSelect AddSelect(SQLSelect objSelect, ComparisonOperator eCompare, object objValue)
		{
			EnsurePreviousLogicalOperatorExists();
				
			var condition = new SQLConditionSelect();
			condition.Select = objSelect;
			condition.Compare = eCompare;
			condition.Value = objValue;
				
			pobjSQLConditions.Add(condition);
				
			return condition;
		}
			
		public SQLConditionFieldCompare AddFieldCompare()
		{
			return AddFieldCompare(null, "", ComparisonOperator.EqualTo, null, "");
		}
			
		public SQLConditionFieldCompare AddFieldCompare(string strFieldName1, ComparisonOperator eCompare, SQLSelectTable objTable2, string strFieldName2)
		{
			return AddFieldCompare(null, strFieldName1, eCompare, objTable2, strFieldName2);
		}
			
		public SQLConditionFieldCompare AddFieldCompare(SQLSelectTable objTable1, string strFieldName1, ComparisonOperator eCompare, SQLSelectTable objTable2, string strFieldName2)
		{
			EnsurePreviousLogicalOperatorExists();
				
			var condition = new SQLConditionFieldCompare();
            condition.Table1 = objTable1;
            condition.FieldName1 = strFieldName1;
            condition.Compare = eCompare;
            condition.Table2 = objTable2;
            condition.FieldName2 = strFieldName2;
				
			pobjSQLConditions.Add(condition);
				
			return condition;
		}
			
		public bool IsEmpty
		{
			get
			{
				return pobjSQLConditions.Count == 0;
			}
		}
			
		private void EnsurePreviousLogicalOperatorExists()
		{
			//Add the AND operator if an operator hasn't been called after the previous Add call
			if (pobjLogicalOperators.Count < pobjSQLConditions.Count)
				this.AddLogicalOperator(LogicalOperator.And);
		}
			
		public void AddLogicalOperator(LogicalOperator eLogicalOperator)
		{
			if (pobjLogicalOperators.Count + 1 > pobjSQLConditions.Count)
				throw new Exceptions.DatabaseObjectsException("First call the Add function - this function has been called without a prior call to Add");
				
			pobjLogicalOperators.Add(eLogicalOperator);
		}
			
		public void Delete(ref SQLCondition objCondition)
		{
			int intConditionIndex = pobjSQLConditions.IndexOf(objCondition);
				
			if (!pobjSQLConditions.Contains(objCondition))
				throw new IndexOutOfRangeException();
				
			if (intConditionIndex > 0)
				pobjLogicalOperators.Remove(pobjLogicalOperators[intConditionIndex - 1]);
			else if (intConditionIndex == 0)
				pobjLogicalOperators.Remove(pobjLogicalOperators[0]);
				
			pobjSQLConditions.Remove(objCondition);
			objCondition = null;
		}

		internal LogicalOperator[] LogicalOperators
		{
			get
			{
				return this.pobjLogicalOperators.ToArray();
			}
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return pobjSQLConditions.GetEnumerator();
		}

		internal override string SQL(Serializer serializer)
		{
			return serializer.SerializeConditions(this);
		}
	}
}
