// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DatabaseObjects.SQL.Serializers
{
	/// <summary>
	/// Serializes an DatabaseObjects.SQL.* object into an SQL string ready for execution.
	/// This class manages generic SQL serialisation that applies to all database systems.
	/// Methods can be overridden if a database system serializes differently.
	/// </summary>
	internal abstract class Serializer
	{
		/// <summary>
		/// Joins all of the strings and ensures that there are spaces between each item if the item not blank.
		/// </summary>
		protected class TokenSerializer
		{
			private string separator;
			private List<string> tokens = new List<string>();

			public TokenSerializer()
				: this(separator: " ")
			{
			}

			public TokenSerializer(string separator)
			{
				if (String.IsNullOrEmpty(separator))
					throw new ArgumentNullException();

				this.separator = separator;
			}

			public void Add(string token)
			{
				tokens.Add(token);
			}

			/// <summary>
			/// Joins all of the strings and ensures that there are spaces between each item if the item not blank.
			/// </summary>
			public override string ToString()
			{
				return String.Join(this.separator, tokens.Where(token => !String.IsNullOrEmpty(token)).ToArray());
			}
		}

		/// <summary>
		/// Indicates the serialization type to utilise.
		/// </summary>
		public abstract Database.ConnectionType Type { get; }

		/// <summary>
		/// Should place tags around a field name or table name to ensure it doesn't
		/// conflict with a reserved word or if it contains spaces it is not misinterpreted
		/// </summary>
		public abstract string SerializeIdentifier(string strIdentifierName);

		/// <summary>
		/// Should return the SELECT statement that when executed returns a row.
		/// If a row is returned then the table exists.
		/// </summary>
		public abstract string SerializeTableExists(SQLTableExists tableExists);

		/// <summary>
		/// Should return the SELECT statement for determining whether a view exists.
		/// If a row is returned then the view exists.
		/// </summary>
		public abstract string SerializeViewExists(SQLViewExists viewExists);

		public string SerializeInsert(SQLInsert insert)
		{
			if (String.IsNullOrEmpty(insert.TableName))
				throw new Exceptions.DatabaseObjectsException("TableName property has not been set.");
			else if (insert.Fields.Count == 0)
				throw new Exceptions.DatabaseObjectsException("Field values have not been set.");

			string fieldNames = String.Join(", ", insert.Fields.Select(field => SerializeIdentifier(field.Name)).ToArray());
			string fieldValues = String.Join(", ", insert.Fields.Select(field => SerializeValue(field.Value)).ToArray());

			var tokens = new TokenSerializer();

			tokens.Add("INSERT INTO");
			tokens.Add(SerializeIdentifier(insert.TableName));
			tokens.Add("(" + fieldNames + ")");
			tokens.Add("VALUES");
			tokens.Add("(" + fieldValues + ")");

			return tokens.ToString();
		}

		public virtual string SerializeUpdate(SQLUpdate update)
		{
			if (String.IsNullOrEmpty(update.TableName))
				throw new Exceptions.DatabaseObjectsException("TableName property has not been set.");
			else if (update.Fields.Count == 0)
				throw new Exceptions.DatabaseObjectsException("Field values have not been set.");
			else if (update.Fields.Any(field => String.IsNullOrEmpty(field.Name)))
				throw new Exceptions.DatabaseObjectsException("Field Name has not been set.");

			var fieldNameAndValues = String.Join(", ", update.Fields.Select(field => SerializeIdentifier(field.Name) + " = " + SerializeValue(field.Value)).ToArray());

			var tokens = new TokenSerializer();

			tokens.Add("UPDATE");
			tokens.Add(SerializeIdentifier(update.TableName));
			tokens.Add("SET");
			tokens.Add(fieldNameAndValues);

			if (update.Where != null && !update.Where.IsEmpty)
			{
				tokens.Add("WHERE");
				tokens.Add(SerializeConditions(update.Where));
			}

			return tokens.ToString();
		}

		public string SerializeInsertFromSelect(SQLInsertFromSelect insertFromSelect)
		{
			if (String.IsNullOrEmpty(insertFromSelect.TableName))
				throw new Exceptions.DatabaseObjectsException("TableName property has not been set.");

			var tokens = new TokenSerializer();
			tokens.Add("INSERT INTO");
			tokens.Add(SerializeIdentifier(insertFromSelect.TableName));

			if (insertFromSelect.Fields.Count > 0)
			{
				var fields = String.Join(", ", insertFromSelect.Fields.Cast<string>().Select(fieldName => SerializeIdentifier(fieldName)).ToArray());
				tokens.Add("(" + fields + ")");
			}

			tokens.Add(SerializeSelect(insertFromSelect.Source));

			return tokens.ToString();
		}

		public virtual string SerializeDelete(SQLDelete delete)
		{
			if (String.IsNullOrEmpty(delete.TableName))
				throw new Exceptions.DatabaseObjectsException("TableName property has not been set.");

			var tokens = new TokenSerializer();

			tokens.Add("DELETE FROM");
			tokens.Add(SerializeIdentifier(delete.TableName));

			if (delete.Where != null && !delete.Where.IsEmpty)
			{
				tokens.Add("WHERE");
				tokens.Add(SerializeConditions(delete.Where));
			}

			return tokens.ToString();
		}

		public virtual string SerializeSelect(SQLSelect select)
		{
			if (select.Tables.Count == 0)
				throw new Exceptions.DatabaseObjectsException("The table has not been set.");

			var tokens = new TokenSerializer();

			tokens.Add("SELECT");
			tokens.Add(SerializeBeforeSelectFields(select));
			tokens.Add(SerializeSelectFields(select.Fields));
			tokens.Add("FROM");
			tokens.Add(SerializeSelectTables(select.Tables));
			tokens.Add(SerializeAfterSelectTables(select));

			if (select.Where != null && !select.Where.IsEmpty)
			{
				tokens.Add("WHERE");
				tokens.Add(SerializeConditions(select.Where));
			}

			if (select.GroupBy != null && !select.GroupBy.IsEmpty)
			{
				tokens.Add("GROUP BY");
				tokens.Add(SerializeSelectGroupByFields(select.GroupBy));
			}

			if (select.OrderBy != null && !select.OrderBy.IsEmpty)
			{
				tokens.Add("ORDER BY");
				tokens.Add(SerializeSelectOrderByFields(select.OrderBy));
			}

			if (select.Having != null && !select.Having.IsEmpty)
			{
				tokens.Add("HAVING");
				tokens.Add(SerializeSelectHavingConditions(select.Having));
			}

			return tokens.ToString();
		}

		public virtual string SerializeDropView(SQLDropView dropView)
		{
			return "DROP VIEW " + SerializeIdentifier(dropView.ViewName);
		}

		public virtual string SerializeCreateView(SQLCreateView createView)
		{
			if (string.IsNullOrEmpty(createView.Name))
				throw new Exceptions.DatabaseObjectsException("View name has not been set");
			else if (createView.Select == null)
				throw new Exceptions.DatabaseObjectsException("Select statement has not been set");

			return "CREATE VIEW " + SerializeIdentifier(createView.Name) + " AS " + SerializeSelect(createView.Select);
		}

		public virtual string SerializeSetTransactionIsolationLevel(SQLSetTransactionIsolationLevel level)
		{
			return "SET TRANSACTION ISOLATION LEVEL " + SerializeIsolationLevel(level.Value);
		}
		
		public virtual string SerializeIsolationLevel(System.Data.IsolationLevel isolationLevel)
		{
			switch (isolationLevel)
			{
				case IsolationLevel.ReadCommitted:
					return "READ COMMITTED";
				case IsolationLevel.ReadUncommitted:
					return "READ UNCOMMITTED";
				case IsolationLevel.RepeatableRead:
					return "REPEATABLE READ";
				case IsolationLevel.Serializable:
					return "SERIALIZABLE";
				default:
					throw new NotSupportedException("Snapshots isolation level is not supported for " + isolationLevel.ToString());
			}
		}

		public virtual string SerializeRollbackTransaction(SQLRollbackTransaction rollbackTransaction)
		{
			return "ROLLBACK TRANSACTION";
		}
		
		public virtual string SerializeCommitTransaction(SQLCommitTransaction commitTransaction)
		{
			return "COMMIT TRANSACTION";
		}
		
		public virtual string SerializeBeingTransaction(SQLBeginTransaction beginTransaction)
		{
			return "BEGIN TRANSACTION";
		}
		
		public virtual string SerializeSelectHavingConditions(SQLSelectHavingConditions havingConditions)
		{
			int index = 0;
			string strSQL = string.Empty;

			foreach (var condition in havingConditions)
			{
				if (index > 0)
					strSQL += " " + SerializeLogicalOperator(havingConditions.LogicalOperators[index - 1]) + " ";

				if (condition is SQLSelectHavingCondition)
					strSQL += ((SQLSelectHavingCondition)condition).SQL(this);
				else if (condition is SQLSelectHavingConditions)
					strSQL += "(" + SerializeSelectHavingConditions((SQLSelectHavingConditions)condition) + ")";
				else
					throw new NotImplementedException(condition.GetType().FullName);

				index++;
			}
				
			return strSQL;
		}

		public virtual string SerializeSelectOrderByFields(SQLSelectOrderByFields orderByFields)
		{
			return String.Join(", ", orderByFields.Select(field => SerializeSelectOrderByField(field)).ToArray());
		}

		public virtual string SerializeSelectOrderByField(SQLSelectOrderByField orderByField)
		{
			if (String.IsNullOrEmpty(orderByField.Name))
				throw new Exceptions.DatabaseObjectsException("Order By field has not been set.");

			string strSQL = string.Empty;
			
			if (orderByField.AggregateFunction > 0)
				strSQL = SerializeAggregateFunction(orderByField.AggregateFunction) + "(";

			strSQL += SerializeFieldNameAndTablePrefix(orderByField.Table, orderByField.Name);

			if (orderByField.AggregateFunction > 0)
				strSQL += ")";

			switch (orderByField.Order)
			{
				case OrderBy.Ascending:
					break;
				case OrderBy.Descending:
					strSQL += " DESC";
					break;
			}
				
			return strSQL;
		}
			
		public virtual string SerializeSelectGroupByFields(SQLSelectGroupByFields groupByFields)
		{
			return String.Join(", ", groupByFields.Select(field => field.Expression.SQL(this)).ToArray());
		}

		public string SerializeConditions(SQLConditions conditions)
		{
			int index = 0;
			string strSQL = string.Empty;

			foreach (var condition in conditions)
			{
				if (index > 0)
					strSQL += " " + SerializeLogicalOperator(conditions.LogicalOperators[index - 1]) + " ";

				if (condition is SQLCondition)
					strSQL += SerializeCondition((SQLCondition)condition);
				else if (condition is SQLConditions)
					strSQL += "(" + SerializeConditions((SQLConditions)condition) + ")";
				else if (condition is SQLConditionInSelect)
					strSQL += SerializeConditionInSelect((SQLConditionInSelect)condition);
				else if (condition is SQLConditionSelect)
					strSQL += SerializeConditionSelect((SQLConditionSelect)condition);
				else if (condition is SQLConditionFieldCompare)
					strSQL += SerializeConditionFieldCompare((SQLConditionFieldCompare)condition);
				else if (condition is SQLConditionExpression)
					strSQL += ((SQLConditionExpression)condition).SQL(this);
				else
					throw new NotImplementedException(condition.GetType().FullName);

				index++;
			}

			return strSQL;
		}

		public virtual string SerializeConditionFieldCompare(SQLConditionFieldCompare fieldCompare)
		{
			if (String.IsNullOrEmpty(fieldCompare.FieldName1))
				throw new ArgumentException("FieldName1 not set.");
			else if (String.IsNullOrEmpty(fieldCompare.FieldName2))
				throw new ArgumentException("FieldName2 not set.");

			return SerializeFieldNameAndTablePrefix(fieldCompare.Table1, fieldCompare.FieldName1) + " " + SerializeComparisonOperator(fieldCompare.Compare) + " " + SerializeFieldNameAndTablePrefix(fieldCompare.Table2, fieldCompare.FieldName2);
		}

		public string SerializeArithmeticOperator(SQL.ArithmeticOperator @operator)
		{
			switch (@operator)
			{
				case ArithmeticOperator.Add:
					return "+";
				case ArithmeticOperator.Subtract:
					return "-";
				case ArithmeticOperator.Multiply:
					return "*";
				case ArithmeticOperator.Divide:
					return "/";
				case ArithmeticOperator.Modulus:
					return "%";
				default:
					throw new NotSupportedException(@operator.ToString());
			}
		}
		
		public virtual string SerializeArithmeticExpression(SQLArithmeticExpression arithmeticExpression)
		{
			if (arithmeticExpression.LeftExpression == null)
				throw new ArgumentNullException(arithmeticExpression.GetType().Name + ".LeftExpression");
			else if (arithmeticExpression.RightExpression == null)
				throw new ArgumentNullException(arithmeticExpression.GetType().Name + ".RightExpression");

			return "(" + arithmeticExpression.LeftExpression.SQL(this) + " " + SerializeArithmeticOperator(arithmeticExpression.Operator) + " " + arithmeticExpression.RightExpression.SQL(this) + ")";
		}
		
		public virtual string SerializeLogicalExpression(SQLLogicalExpression logicalExpression)
		{
			if (logicalExpression.LeftExpression == null)
				throw new ArgumentNullException(logicalExpression.GetType().Name + ".LeftExpression");
			else if (logicalExpression.RightExpression == null)
				throw new ArgumentNullException(logicalExpression.GetType().Name + ".RightExpression");

			return "(" + logicalExpression.LeftExpression.SQL(this) + " " + SerializeLogicalOperator(logicalExpression.Operator) + " " + logicalExpression.RightExpression.SQL(this) + ")";
		}

		public virtual string SerializeAggregateExpression(SQLAggregateExpression aggregateExpression)
		{
			return SerializeAggregateFunction(aggregateExpression.Aggregate) + SerializeFunctionExpressionArguments(aggregateExpression);
		}

		public virtual string SerializeConditionSelect(SQLConditionSelect conditionSelect)
		{
			if (conditionSelect.Select == null)
				throw new Exceptions.DatabaseObjectsException("Select is not set.");

			return "(" + SerializeSelect(conditionSelect.Select) + ") " + SerializeCondition(conditionSelect.Compare, conditionSelect.Value);
		}

		public virtual string SerializeConditionInSelect(SQLConditionInSelect conditionInSelect)
		{
			if (String.IsNullOrEmpty(conditionInSelect.FieldName))
				throw new Exceptions.DatabaseObjectsException("FieldName not set.");
			else if (conditionInSelect.Select == null)
				throw new Exceptions.DatabaseObjectsException("SelectSet not set.");
				
			string strIn;

			if (conditionInSelect.NotInSelect)
				strIn = "NOT IN";
			else
				strIn = "IN";

			return SerializeFieldNameAndTablePrefix(conditionInSelect.Table, conditionInSelect.FieldName) + " " + strIn + " (" + SerializeSelect(conditionInSelect.Select) + ")";
		}

		/// <summary>
		/// Serializes all of the tables from the SELECT statment.
		/// </summary>
		public virtual string SerializeSelectTables(SQLSelectTables selectTables)
		{
			var strSQL = String.Join(", ",
				selectTables
				.Where(table => selectTables.Joins == null || !selectTables.Joins.Exists(table))
				.Select(table => SerializeSelectTableBase(table))
				.ToArray());

			if (selectTables.Joins != null)
			{
				string strJoinsSQL = SerializeSelectTableJoins(selectTables.Joins);

				if (strJoinsSQL != string.Empty && strSQL != string.Empty)
					strSQL += " ";

				strSQL += strJoinsSQL;
			}

			return strSQL;
		}

		public virtual string SerializeAfterSelectTables(SQLSelect select)
		{
			return String.Empty;
		}

		public virtual string SerializeSelectTableFromSelect(SQLSelectTableFromSelect tableFromSelect)
		{
			return "(" + SerializeSelect(tableFromSelect.Select) + ")";
		}

		public virtual string SerializeSelectHavingCondition(SQLSelectHavingCondition havingCondition)
		{
			return havingCondition.LeftExpression.SQL(this) + " " + SerializeComparisonOperator(havingCondition.Compare) + " " + havingCondition.RightExpression.SQL(this);
		}

		public virtual string SerializeTableFieldComputed(SQLTableFieldComputed tableFieldComputed, SQLTableFields.AlterModeType alterMode)
		{
			if (alterMode == SQLTableFields.AlterModeType.Drop)
				throw new InvalidOperationException("Computed columns cannot be used for dropping fields");

			return tableFieldComputed.NameAsExpression.SQL(this) + " AS (" + tableFieldComputed.Computation.SQL(this) + ")";
		}

		public virtual string SerializeCreateTable(SQLCreateTable createTable)
		{
			if (String.IsNullOrEmpty(createTable.Name))
				throw new Exceptions.DatabaseObjectsException("TableName has not been set.");

			var tokens = new TokenSerializer();
			tokens.Add("CREATE TABLE");
			tokens.Add(SerializeIdentifier(createTable.Name));
			tokens.Add("(" + SerializeTableFields(createTable.Fields, includeColumnModifier: false) + ")");
		
			return tokens.ToString();
		}

		public virtual string SerializeDropTable(SQLDropTable dropTable)
		{
			if (String.IsNullOrEmpty(dropTable.Name))
				throw new Exceptions.DatabaseObjectsException("TableName has not been set.");

			return "DROP TABLE " + SerializeIdentifier(dropTable.Name);
		}

		public virtual string SerializeDropIndex(SQLDropIndex dropIndex)
		{
			if (String.IsNullOrEmpty(dropIndex.Name))
				throw new Exceptions.DatabaseObjectsException("IndexName has not been set.");
			else if (String.IsNullOrEmpty(dropIndex.TableName))
				throw new Exceptions.DatabaseObjectsException("TableName has not been set.");

			return "DROP INDEX " + SerializeIdentifier(dropIndex.Name) + " ON " + SerializeIdentifier(dropIndex.TableName);
		}

		public virtual string SerializeCreateIndex(SQLCreateIndex createIndex)
		{
			//Although the index name is optional with SQL Server it is not optional with MySQL or Pervasive
			if (String.IsNullOrEmpty(createIndex.Name))
				throw new Exceptions.DatabaseObjectsException("IndexName has not been set.");
			else if (String.IsNullOrEmpty(createIndex.TableName))
				throw new Exceptions.DatabaseObjectsException("TableName has not been set.");

			var tokens = new TokenSerializer();
			tokens.Add("CREATE");
			tokens.Add(SerializeCreateIndexModifier(createIndex));
			tokens.Add("INDEX");
			tokens.Add(SerializeIdentifier(createIndex.Name));
			tokens.Add("ON");
			tokens.Add(SerializeIdentifier(createIndex.TableName));
			tokens.Add("(" + SerializeIndexFields(createIndex.Fields) + ")");

			return tokens.ToString();
		}

		public virtual string SerializeCreateIndexModifier(SQLCreateIndex createIndex)
		{
			if (createIndex.IsUnique)
				return "UNIQUE";
			else
				return String.Empty;
		}

		public virtual string SerializeIndexFields(SQLIndexFields fields)
		{
			var tokens = new TokenSerializer(", ");

			foreach (SQLIndexField objField in fields)
				tokens.Add(SerializeIndexField(objField));

			return tokens.ToString();
		}

		public virtual string SerializeIndexField(SQLIndexField indexField)
		{
			var tokens = new TokenSerializer();

			tokens.Add(SerializeIdentifier(indexField.Name));
			tokens.Add(SerializeOrderBy(indexField.Order));

			return tokens.ToString();
		}

		public virtual string SerializeOrderBy(OrderBy orderBy)
		{
			switch (orderBy)
			{
				case OrderBy.None:
				case OrderBy.Ascending:
					return String.Empty;
				case OrderBy.Descending:
					return "DESC";
				default:
					throw new NotImplementedException();
			}
		}
		
		public virtual string SerializeAlterTable(SQLAlterTable alterTable)
		{
			if (String.IsNullOrEmpty(alterTable.Name))
				throw new Exceptions.DatabaseObjectsException("TableName has not been set.");

			return "ALTER TABLE " + SerializeIdentifier(alterTable.Name) + " " + SerializeTableFields(alterTable.Fields, includeColumnModifier: true);
		}

		/// <summary>
		/// </summary>
		/// <param name="includeColumnModifier">
		/// Indicates whether the ADD, MODIFY or DROP modifiers are required for each column.
		/// When utilised from SQLCreateTable this will always be false. However, for SQLAlterTable this will be true.</param>
		/// <returns></returns>
		public virtual string SerializeTableFields(SQLTableFields tableFields, bool includeColumnModifier)
		{
			var tokens = new TokenSerializer();

			//Include mode when altering a table, otherwise when creating a table the mode is not required.
			if (includeColumnModifier)
				tokens.Add(SerializeAlterTableFieldsModifier(tableFields.AlterMode));

			var fieldsTokens = new TokenSerializer(", ");

			foreach (SQLTableFieldBase field in tableFields)
			{
				var fieldToken = new TokenSerializer();

				if (includeColumnModifier)
					fieldToken.Add(SerializeAlterTableFieldModifier(tableFields.AlterMode));

				fieldToken.Add(field.SQL(this, tableFields.AlterMode));

				fieldsTokens.Add(fieldToken.ToString());
			}

			tokens.Add(fieldsTokens.ToString());

			return tokens.ToString();
		}

		/// <summary>
		/// The field modifier that is used before all of the fields modifications are listed.
		/// Typically used by SQL Server to indicate the type of operation to be performed on ALL fields.
		/// </summary>
		/// <param name="tableFields"></param>
		/// <returns></returns>
		public virtual string SerializeAlterTableFieldsModifier(SQLTableFields.AlterModeType alterMode)
		{
			return SerializeColumnAlterMode(alterMode);
		}

		/// <summary>
		/// The field modifier that is used before each field that is being modified.
		/// Typically used by MySQL the type of operation to be performed on each field.
		/// </summary>
		/// <param name="tableFields"></param>
		/// <returns></returns>
		public virtual string SerializeAlterTableFieldModifier(SQLTableFields.AlterModeType alterMode)
		{
			return String.Empty;
		}

		public virtual string SerializeColumnAlterMode(SQLTableFields.AlterModeType alterMode)
		{
			switch (alterMode)
			{
				case SQLTableFields.AlterModeType.Add:
					return "ADD";
				case SQLTableFields.AlterModeType.Modify:
					return "ALTER COLUMN";
				case SQLTableFields.AlterModeType.Drop:
					return "DROP COLUMN";
				default:
					throw new NotImplementedException();
			}
		}

		public virtual string SerializeSelectTableJoins(SQLSelectTableJoins tableJoins)
		{
			if (tableJoins.Count > 0)
				//recurse through the joins from right to left
				return SerializeSelectTableJoin(tableJoins[tableJoins.Count - 1]);
			else
				return string.Empty;
		}

		public virtual string SerializeTableFieldAsName(SQLTableField field)
		{
			return field.NameAsExpression.SQL(this);
		}

		public virtual string SerializeTableField(SQLTableField field, SQLTableFields.AlterModeType alterMode)
		{
			var tokens = new TokenSerializer();

			if (alterMode == SQLTableFields.AlterModeType.Drop)
				tokens.Add(SerializeTableFieldAsName(field));
			else
			{
				tokens.Add(SerializeTableFieldAsName(field));
				tokens.Add(SerializeTableFieldDataType(field));
				tokens.Add(SerializeTableFieldDefaultOption(field));
				tokens.Add(SerializeTableFieldNullableOption(field));
				tokens.Add(SerializeTableFieldKeyTypeOption(field));
			}

			return tokens.ToString();
		}

		public virtual string SerializeTableFieldDataType(SQLTableField field)
		{
			return SerializeDataType(field.DataType, field.Size, field.Precision, field.ScaleLength);
		}

		public virtual string SerializeTableFieldDefaultOption(SQLTableField field)
		{
			if (field.Default != null)
				return "DEFAULT " + SerializeValue(field.Default);
			else
				return String.Empty;
		}

		public virtual string SerializeTableFieldNullableOption(SQLTableField field)
		{
			if (field.AcceptsNull)
				return "NULL";
			else
				return "NOT NULL";
		}

		public virtual string SerializeTableFieldKeyTypeOption(SQLTableField field)
		{
			if (field.KeyType == KeyType.Primary)
				return "PRIMARY KEY";
			else if (field.KeyType == KeyType.Unique)
				return "UNIQUE";
			else
				return String.Empty;
		}
		
		public virtual string SerializeSelectTable(SQLSelectTable table)
		{
			if (String.IsNullOrEmpty(table.Name))
				throw new Exceptions.DatabaseObjectsException("Table has not been set.");

			string strSQL = string.Empty;

			if (!String.IsNullOrEmpty(table.DatabaseName))
				strSQL += SerializeIdentifier(table.DatabaseName) + ".";

			if (!String.IsNullOrEmpty(table.SchemaName))
				strSQL += SerializeIdentifier(table.SchemaName) + ".";

			return strSQL + SerializeIdentifier(table.Name);
		}

		public virtual string SerializeSelectTableJoin(SQLSelectTableJoin table)
		{
			string strSQL = SerializeSelectTableBase(table.LeftTable) + " " + SerializeTableJoinType(table.TheType) + " " + SerializeSelectTableBase(table.RightTable);

			var conditions = table.Where;

			if (conditions != null && !conditions.IsEmpty)
				strSQL += " ON " + SerializeSelectTableJoinConditions(conditions);
				
			//Surround the join with parentheses - MS Access won't accept it otherwise
			return "(" + strSQL + ")";
		}

		public virtual string SerializeSelectTableJoinConditions(SQLSelectTableJoinConditions joinConditions)
		{
			string strSQL = string.Empty;
			int intIndex = 0;

			foreach (object objCondition in joinConditions)
			{
				if (intIndex > 0)
					strSQL += " " + SerializeLogicalOperator(joinConditions.LogicalOperators[intIndex - 1]) + " ";

				if (objCondition is SQLSelectTableJoinConditions)
					strSQL += "(" + SerializeSelectTableJoinConditions((SQLSelectTableJoinConditions)objCondition) + ")";
				else if (objCondition is SQLSelectTableJoinCondition)
					strSQL += SerializeSelectTableJoinCondition((SQLSelectTableJoinCondition)objCondition);
				else
					throw new NotImplementedException(objCondition.GetType().FullName);

				intIndex++;
			}

			return strSQL;
		}

		public virtual string SerializeSelectTableJoinCondition(SQLSelectTableJoinCondition joinCondition)
		{
			//Account for the situation where EqualTo to NULL is appropriately translated to 'IS NULL'
			if (joinCondition.RightExpression is SQLValueExpression)
				return joinCondition.LeftExpression.SQL(this) + " " + SerializeCondition(joinCondition.Compare, ((SQLValueExpression)joinCondition.RightExpression).Value);
			else
				return joinCondition.LeftExpression.SQL(this) + " " + SerializeComparisonOperator(joinCondition.Compare) + " " + joinCondition.RightExpression.SQL(this);
		}

		public virtual string SerializeCaseExpressionCase(SQLCaseExpression.Case expressionCase)
		{
			return "WHEN " + expressionCase.WhenCondition.SQL(this) + " THEN " + expressionCase.Result.SQL(this);
		}

		public virtual string SerializeCaseExpression(SQLCaseExpression expression)
		{
			if (expression.Cases.Count() == 0)
				throw new InvalidOperationException("There are no cases for the CASE statement");

			var tokens = new TokenSerializer();

			tokens.Add("CASE");

			if (expression.InputExpression != null)
				tokens.Add(expression.InputExpression.SQL(this));

			var cases = string.Join(" ", expression.Cases.Select(objCase => SerializeCaseExpressionCase(objCase)).ToArray());
			tokens.Add(cases);

			if (expression.ElseResult != null)
			{
				tokens.Add("ELSE");
				tokens.Add(expression.ElseResult.SQL(this));
			}

			tokens.Add("END");

			return tokens.ToString();
		}

		public virtual string SerializeCastExpression(SQLCastExpression expression)
		{
			return "CAST(" + SerializeIdentifier(expression.FieldName) + " AS " + SerializeDataType(expression.ToDataType, expression.Size, expression.Precision, expression.ScaleLength) + ")";
		}

		public virtual string SerializeDataType(SQL.DataType dataType, int intSize, int intPrecision, int intScale)
		{
			throw new NotImplementedException("Data type " + dataType.ToString() + " is not implemented for " + this.GetType().Name);
		}

		public virtual string SerializeBitwiseExpression(SQLBitwiseExpression expression)
		{
			if (expression.LeftExpression == null)
				throw new ArgumentNullException(expression.GetType().Name + ".LeftExpression");
			else if (expression.RightExpression == null)
				throw new ArgumentNullException(expression.GetType().Name + ".RightExpression");

			return "(" + expression.LeftExpression.SQL(this) + " " + SerializeBitwiseOperator(expression.Operator) + " " + expression.RightExpression.SQL(this) + ")";
		}

		public virtual string SerializeFieldAggregateExpression(SQLFieldAggregateExpression expression)
		{
			if (expression.AggregateFunction == AggregateFunction.None)
				throw new InvalidOperationException("AggregateFunction unspecified for " + this.GetType().Name);

			string strFieldName = string.Empty;

			if (expression.AggregateFunction == AggregateFunction.Count)
				strFieldName = "*";
			else
				strFieldName = SerializeFieldExpression(expression);

			return SerializeAggregateFunction(expression.AggregateFunction) + "(" + strFieldName + ")";
		}

		/// <summary>
		/// Includes parentheses around arguments if required.
		/// </summary>
		/// <returns></returns>
		public virtual string SerializeFunctionExpressionArguments(SQLFunctionExpression functionExpression)
		{
			string strArguments = string.Empty;

			if (functionExpression.Arguments != null)
				strArguments = String.Join(", ", functionExpression.Arguments.Select(argument => argument.SQL(this)).ToArray());

			if ((functionExpression.IncludeParenthesesWhenArgumentsEmpty && !functionExpression.HasArguments) || functionExpression.HasArguments)
				strArguments = "(" + strArguments + ")";

			return strArguments;
		}

		/// <summary>
		/// Should return a SELECT statement that can be used to obtain the value from a auto increment / identity field
		/// after an INSERT statement.
		/// </summary>
		public virtual string SerializeAutoIncrementValue(SQLAutoIncrementValue autoIncrementValue)
		{
			return "SELECT @@IDENTITY AS " + SerializeIdentifier(autoIncrementValue.ReturnFieldName);
		}

		public virtual string SerializeSelectExpression(SQLSelectExpression selectExpression)
		{
			return "SELECT " + selectExpression.Expression.SQL(this);
		}

		public virtual string SerializeSelectField(SQLSelectField field)
		{
			string strSQL = field.Expression.SQL(this);

			if (!String.IsNullOrEmpty(field.Alias))
				strSQL += " AS " + SerializeIdentifier(field.Alias);

			return strSQL;
		}

		public virtual string SerializeStringContactExpression(SQLStringConcatExpression expression)
		{
			if (expression.LeftExpression == null)
				throw new ArgumentNullException(expression.GetType().Name + ".LeftExpression");
			else if (expression.RightExpression == null)
				throw new ArgumentNullException(expression.GetType().Name + ".RightExpression");

			return expression.LeftExpression.SQL(this) + " + " + expression.RightExpression.SQL(this);
		}

		public virtual string SerializeLengthFunctionExpression(SQLLengthFunctionExpression expression)
		{
			throw new NotImplementedException();
		}

		public virtual string SerializeLeftFunctionExpression(SQLLeftFunctionExpression expression)
		{
			return "LEFT" + SerializeFunctionExpressionArguments(expression);
		}

		public virtual string SerializeRightFunctionExpression(SQLRightFunctionExpression expression)
		{
			return "RIGHT" + SerializeFunctionExpressionArguments(expression);
		}

		public virtual string SerializeGetDateFunctionExpression(SQLGetDateFunctionExpression expression)
		{
			throw new NotImplementedException();
		}

		public virtual string SerializeFieldExpression(SQLFieldExpression fieldExpression)
		{
			if (String.IsNullOrEmpty(fieldExpression.Name))
				throw new Exceptions.DatabaseObjectsException("Field Name has not been set.");

			return SerializeFieldNameAndTablePrefix(fieldExpression.Table, fieldExpression.Name);
		}

		public virtual string SerializeBitwiseOperator(SQL.BitwiseOperator eOperator)
		{
			if (eOperator == BitwiseOperator.And)
				return "&";
			else if (eOperator == BitwiseOperator.Or)
				return "|";
			else
				throw new NotSupportedException(eOperator.ToString());
		}

		public virtual string SerializeUnion(SQLUnion union)
		{
			if (union.SelectStatements.Count() < 2)
				throw new Exceptions.DatabaseObjectsException("The select statements have not been set");

			var tokens = new TokenSerializer();
			int index = 0;

			foreach (var statement in union.SelectStatements)
			{
				if (index > 0)
					tokens.Add(SerializeUnionType(union.UnionTypes[index - 1]));

				tokens.Add("(" + SerializeSelect(statement) + ")");
				index++;
			}

			if (union.OrderBy != null && !union.OrderBy.IsEmpty)
			{
				tokens.Add("ORDER BY");
				tokens.Add(SerializeSelectOrderByFields(union.OrderBy));
			}

			return tokens.ToString();
		}

		public virtual string SerializeUnionType(SQLUnion.Type type)
		{
			switch (type)
			{
				case SQLUnion.Type.Distinct:
					return "UNION";
				case SQLUnion.Type.All:
					return "UNION ALL";
				default:
					throw new NotImplementedException(type.ToString());
			}
		}
					
		public string SerializeConditionExpression(SQLConditionExpression expression)
		{
			if (expression.LeftExpression == null)
				throw new InvalidOperationException("Left expression is not set");
			else if (expression.RightExpression == null)
				throw new InvalidOperationException("Right expression is not set");

			string strSQL = expression.LeftExpression.SQL(this) + " ";
				
			if (expression.RightExpression is SQLValueExpression)
				strSQL += SerializeCondition(expression.Compare, ((SQLValueExpression)expression.RightExpression).Value);
			else
				strSQL += SerializeComparisonOperator(expression.Compare) + " " + expression.RightExpression.SQL(this);
				
			return strSQL;
		}

		private static void EnsureCompareValuePairValid(SQL.ComparisonOperator eCompare, object objValue)
		{
			if (!(objValue is string) && (eCompare == ComparisonOperator.Like || eCompare == ComparisonOperator.NotLike))
				throw new Exceptions.DatabaseObjectsException("The LIKE operator cannot be used in conjunction with a non-string data type");
			else if (objValue is bool && !(eCompare == ComparisonOperator.EqualTo || eCompare == ComparisonOperator.NotEqualTo))
				throw new Exceptions.DatabaseObjectsException("A boolean value can only be used in conjunction with the " + ComparisonOperator.EqualTo.ToString() + " or " + ComparisonOperator.NotEqualTo.ToString() + " operators");
		}

		public virtual string SerializeCondition(SQLCondition condition)
		{
			return SerializeFieldNameAndTablePrefix(condition.Table, condition.FieldName) + " " + SerializeCondition(condition.Compare, condition.Value);
		}

		protected string SerializeCondition(ComparisonOperator eCompare, object objValue)
		{
			EnsureCompareValuePairValid(eCompare, objValue);

			string strSQL = string.Empty;

			//Return 'IS NULL' rather than '= NULL'
			if (ValueIsNull(objValue))
			{
				if (eCompare == ComparisonOperator.EqualTo)
					strSQL += "IS " + SerializeValue(objValue);
				else if (eCompare == ComparisonOperator.NotEqualTo)
					strSQL += "IS NOT " + SerializeValue(objValue);
				else
					throw new Exceptions.DatabaseObjectsException("DBNull or Nothing/null was specified as a value using the " + eCompare.ToString() + " operator");
			}
			else
				strSQL += SerializeComparisonOperator(eCompare) + " " + SerializeValue(objValue);

			return strSQL;
		}

		public virtual string SerializeLogicalOperator(SQL.LogicalOperator eLogicalOperator)
		{
			string strLogicalOperator;

			if (eLogicalOperator == LogicalOperator.And)
				strLogicalOperator = "AND";
			else if (eLogicalOperator == LogicalOperator.Or)
				strLogicalOperator = "OR";
			else
				throw new NotSupportedException(eLogicalOperator.ToString());

			return strLogicalOperator;
		}

		private static string SerializeTableJoinType(SQLSelectTableJoin.Type joinType)
		{
			switch (joinType)
			{
				case SQLSelectTableJoin.Type.Inner:
					return "INNER JOIN";
				case SQLSelectTableJoin.Type.FullOuter:
					return "FULL OUTER JOIN";
				case SQLSelectTableJoin.Type.LeftOuter:
					return "LEFT OUTER JOIN";
				case SQLSelectTableJoin.Type.RightOuter:
					return "RIGHT OUTER JOIN";
				default:
					throw new NotImplementedException(joinType.ToString());
			}
		}
		
		public virtual string SerializeSelectTableBase(SQLSelectTableBase tableBase)
		{
			string strSQL = tableBase.Source(this);

			if (!String.IsNullOrEmpty(tableBase.Alias))
				strSQL += " " + SerializeIdentifier(tableBase.Alias);
				
			return strSQL;
		}

		/// <summary>
		/// Serializes any commands that must be placed before the field listing
		/// but after the SELECT command.
		/// </summary>
		public virtual string SerializeBeforeSelectFields(SQLSelect select)
		{
			return select.Distinct ? "DISTINCT" : String.Empty;
		}

		/// <summary>
		/// Serializes the list of fields as a comma separated string.
		/// i.e. Field1, Field2, Field3.
		/// </summary>
		public virtual string SerializeSelectFields(SQLSelectFields selectFields)
		{
			if (selectFields.Count == 0)
				return "*";
			else
				return String.Join(", ", selectFields.Select(field => SerializeSelectField(field)).ToArray());
		}

		public virtual string SerializeAggregateFunction(SQL.AggregateFunction aggregate)
		{
			switch (aggregate)
			{
				case AggregateFunction.Average:
					return "AVG";
				case AggregateFunction.Count:
					return "COUNT";
				case AggregateFunction.Sum:
					return "SUM";
				case AggregateFunction.Minimum:
					return "MIN";
				case AggregateFunction.Maximum:
					return "MAX";
				case AggregateFunction.StandardDeviation:
					return "STDEV";
				case AggregateFunction.Variance:
					return "VAR";
				default:
					throw new NotSupportedException(aggregate.ToString());
			}
		}

		public virtual string SerializeComparisonOperator(SQL.ComparisonOperator compare)
		{
			switch (compare)
			{
				case ComparisonOperator.EqualTo:
					return "=";
				case ComparisonOperator.NotEqualTo:
					return "<>";
				case ComparisonOperator.LessThan:
					return "<";
				case ComparisonOperator.LessThanOrEqualTo:
					return "<=";
				case ComparisonOperator.GreaterThan:
					return ">";
				case ComparisonOperator.GreaterThanOrEqualTo:
					return ">=";
				case ComparisonOperator.Like:
					return "LIKE";
				case ComparisonOperator.NotLike:
					return "NOT LIKE";
				default:
					throw new NotSupportedException(compare.ToString());
			}
		}

		public virtual string SerializeBoolean(bool value)
		{
			return value ? "1" : "0";
		}

		public virtual string SerializeCharacter(char value)
		{
			if (value == '\'')
				return SerializeSingleQuoteCharacter();
			else if (value == '\\')
				return SerializeBackslashCharacter();
			else
				return "'" + value + "'";
		}

		public virtual string SerializeSingleQuoteCharacter()
		{
			return @"''''";
		}

		public virtual string SerializeBackslashCharacter()
		{
			return @"\";
		}

		public virtual string SerializeNull()
		{
			return "NULL";
		}

		/// <summary>
		/// Should serialize the binary data to string for writing to the database.
		/// Can use the helper function Serializer.SerializeBinaryArray().
		/// </summary>
		public virtual string SerializeByteArray(byte[] bytData)
		{
			return SerializeByteArray("0x", bytData, String.Empty);
		}

		public virtual string SerializeDateTime(DateTime dateTime)
		{
			return "\'" + SerializeDateTimeValue(dateTime) + "\'";
		}
		
		/// <summary>
		/// Serializes just the value - not any surrounding tags such as ' or #.
		/// If the time part is 0:00 then the time is not serialized.
		/// </summary>
		public virtual string SerializeDateTimeValue(DateTime dateTime)
		{
			string dateTimeString;

			// If the date hasn't been set then set to the 1899-12-30 so that
			// the date isn't set to 2001-01-01
			if (dateTime.Day == 1 && dateTime.Month == 1 && dateTime.Year == 1)
				dateTimeString = "1899-12-30";
			else
				dateTimeString = dateTime.ToString("yyyy-MM-dd");

			if (dateTime.Hour != 0 || dateTime.Minute != 0 || dateTime.Second != 0)
				dateTimeString += " " + dateTime.ToString("HH:mm:ss.fff");

			return dateTimeString;
		}

		/// <summary>
		/// Serializes a string and replaces and special characters such as ' with ''.
		/// </summary>
		public virtual string SerializeString(string stringValue)
		{
			return "'" + stringValue.Replace(@"'", @"''") + "'";
		}

		public virtual string SerializeGuid(Guid guid)
		{
			return "'" + guid.ToString("D") + "'";
		}

		public virtual string SerializeValue(object value)
		{
			if (ValueIsNull(value))
				return SerializeNull();
			else if (value is bool)
				return SerializeBoolean((bool)value);
			else if (value is char)
				return SerializeCharacter((char)value);
			else if (value is DateTime)
				return SerializeDateTime((DateTime)value);
			else if (value is string)
				return SerializeString((string)value);
			else if (value is byte[])
				return SerializeByteArray((byte[])value);
			else if (value is System.Guid)
				return SerializeGuid((Guid)value);
			else if (value is SQLExpression)
				return ((SQLExpression)value).SQL(this);
			else
				return value.ToString();
		}

		public virtual string SerializeFieldNameAndTablePrefix(SQLSelectTable objTable, string strFieldName)
		{
			string strTablePrefix = string.Empty;

			if (objTable != null)
				strTablePrefix = SerializeTablePrefix(objTable) + ".";

			return strTablePrefix + SerializeIdentifier(strFieldName);
		}

		/// <summary>
		/// Returns the table alias or if the alias is not set the table's name.
		/// </summary>
		public virtual string SerializeTablePrefix(SQLSelectTable objTable)
		{
			if (!String.IsNullOrEmpty(objTable.Alias))
				return SerializeIdentifier(objTable.Alias);
			else
				return SerializeIdentifier(objTable.Name);
		}

		protected string SerializeByteArray(string strHexPrefix, byte[] bytData, string strHexSuffix)
		{
			System.Text.StringBuilder objHexData = new System.Text.StringBuilder(strHexPrefix, System.Convert.ToInt32((bytData.Length * 2) + strHexPrefix.Length + strHexSuffix.Length));

			foreach (byte bytByte in bytData)
				objHexData.Append(string.Format("{0:X2}", bytByte));

			objHexData.Append(strHexSuffix);

			return objHexData.ToString();
		}

		private bool ValueIsNull(object objValue)
		{
			if (objValue == null)
				return true;
			else if (objValue == DBNull.Value)
				return true;
			else
				return false;
		}
	}
}
