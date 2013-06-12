// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2008. All rights reserved.
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

namespace DatabaseObjects.SQL
{
	/// <summary>
	/// Allows data to be inserted using records from another table.
	/// </summary>
	///
	/// <example>
	/// Dim objCopy As New DatabaseObjects.SQL.SQLInsertFromSelect
	/// objCopy.Fields.Add("DestinationField1")
	/// objCopy.Fields.Add("DestinationField1")
	/// objCopy.TableName = "DestinationTable"
	/// objCopy.Source.Fields.Add("SourceField1")
	/// objCopy.Source.Fields.Add("SourceField1")
	/// objCopy.Source.Tables.Add("SourceTable")
	/// objCopy.Source.Where.Add("Field1", DatabaseObjects.SQL.ComparisonOperator.EqualTo, "ABC")
	///
	/// INSERT INTO DestinationTable
	/// SELECT * FROM SourceTable
	/// WHERE SomeField = Condition
	/// or
	/// INSERT INTO DestinationTable (IntoField1, IntoField2)
	/// SELECT (FromField1, FromField2) FROM SourceTable
	/// WHERE SomeField = Condition
	///
	/// </example>
	/// <remarks></remarks>
	public class SQLInsertFromSelect : SQLStatement
	{
		public class FieldsClass : System.Collections.Specialized.StringCollection
		{
		}
			
		private string pstrInsertIntoTableName = string.Empty;
		private FieldsClass pobjInsertIntoFields = new FieldsClass();
		private SQLSelect pobjSourceSelect = new SQLSelect();
			
		public SQLInsertFromSelect()
		{
		}
			
		/// <summary>
		/// Sets/Returns the fields that will be inserted into the database.
		/// The order of the fields must match with the order of the fields from
		/// the source SELECT statement.
		/// </summary>
		public FieldsClass Fields
		{
			get
			{
				return pobjInsertIntoFields;
			}
				
			set
			{
				if (value == null)
					throw new NullReferenceException();
					
				pobjInsertIntoFields = value;
			}
		}
			
		/// <summary>
		/// The table to insert into.
		/// </summary>
		public string TableName
		{
			get
			{
				return pstrInsertIntoTableName;
			}
				
			set
			{
				pstrInsertIntoTableName = value;
			}
		}
			
		/// <summary>
		/// The SELECT statement where the data is to be copied from.
		/// The order of the fields must match the order of fields
		/// specified in the Fields property.
		/// </summary>
		public SQLSelect Source
		{
			get
			{
				return pobjSourceSelect;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjSourceSelect = value;
			}
		}

		public override string SQL
		{
			get 
			{
				return base.Serializer.SerializeInsertFromSelect(this);
			}
		}
	}
}
