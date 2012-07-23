// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects.SQL
{
	public class SQLSelect : SQLStatement
	{
		private SQLSelectFields pobjFields = new SQLSelectFields();
		private SQLSelectTables pobjTables;
		private SQLConditions pobjConditions = new SQLConditions();
		private SQLSelectHavingConditions pobjHavingConditions = new SQLSelectHavingConditions();
		private SQLSelectOrderByFields pobjOrderByFields = new SQLSelectOrderByFields();
		private SQLSelectGroupByFields pobjGroupByFields = new SQLSelectGroupByFields();
		private bool pbDistinct = false;
		private int pintTop;
		private bool pbPerformLocking = false;
			
		public SQLSelect()
		{
		    pobjTables = new SQLSelectTables(this);
		}
			
		public SQLSelect(string strTableName)
            : this()
		{
			pobjTables.Add(strTableName);
		}
			
		public SQLSelect(string strTableName, SQLCondition objWhereCondition)
		{
			this.Tables.Add(strTableName);
			this.Where.Add(objWhereCondition);
		}
			
		public bool Distinct
		{
			get
			{
				return pbDistinct;
			}
				
			set
			{
				pbDistinct = value;
			}
		}
			
		public int Top
		{
			get
			{
				return pintTop;
			}
				
			set
			{
				if (value < 0)
					throw new ArgumentException();
					
				pintTop = value;
			}
		}
			
		/// <summary>
		/// Indicates whether the rows that are selected are locked for reading and updating.
		/// Equivalent to Serialiazable isolation level.
		/// These rows cannot be read or updated until the lock is released.
		/// Locks are released when the transaction has been committed or rolled back.
		/// </summary>
		public bool PerformLocking
		{
			get
			{
				return pbPerformLocking;
			}
				
			set
			{
				pbPerformLocking = value;
			}
		}
			
		public SQLSelectTables Tables
		{
			get
			{
				return pobjTables;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjTables = value;
				pobjTables.Parent = this;
			}
		}
			
		public SQLSelectFields Fields
		{
			get
			{
				return pobjFields;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjFields = value;
			}
		}
			
		public SQLConditions Where
		{
			get
			{
				return pobjConditions;
			}
				
			set
			{
				pobjConditions = value;
			}
		}
			
		public SQLSelectHavingConditions Having
		{
			get
			{
				return pobjHavingConditions;
			}
				
			set
			{
				pobjHavingConditions = value;
			}
		}
			
		public SQLSelectOrderByFields OrderBy
		{
			get
			{
				return pobjOrderByFields;
			}
				
			set
			{
				pobjOrderByFields = value;
			}
		}
			
		public SQLSelectGroupByFields GroupBy
		{
			get
			{
				return pobjGroupByFields;
			}
				
			set
			{
				pobjGroupByFields = value;
			}
		}
			
		public override string SQL
		{
			get
			{
				string strSQL;
					
				if (pobjTables.Count == 0)
					throw new Exceptions.DatabaseObjectsException("The table has not been set.");
					
				strSQL = "SELECT " + DistinctClause();
					
				switch (base.ConnectionType)
				{
					case Database.ConnectionType.MicrosoftAccess:
						break;
					case Database.ConnectionType.SQLServer:
						break;
					case Database.ConnectionType.SQLServerCompactEdition:
						strSQL += TopClause();
						break;
				}
					
				strSQL += pobjFields.SQL(this.ConnectionType) + " FROM " + pobjTables.SQL(this.ConnectionType);
					
				if (pbPerformLocking)
				{
					switch (this.ConnectionType)
					{
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
							strSQL += " WITH (HOLDLOCK, ROWLOCK)";
							break;
						case Database.ConnectionType.MySQL:
						case Database.ConnectionType.Pervasive:
							break;
							//Done below
						case Database.ConnectionType.HyperSQL:
						case Database.ConnectionType.MicrosoftAccess:
							throw new NotSupportedException("Locking is not supported for " + this.ConnectionType.ToString());
							break;
						default:
							throw new NotImplementedException(this.ConnectionType.ToString());
							break;
					}
				}
					
				if (pobjConditions != null && !pobjConditions.IsEmpty)
					strSQL += " WHERE " + pobjConditions.SQL(this.ConnectionType);
					
				if (pobjGroupByFields != null && !pobjGroupByFields.IsEmpty)
					strSQL += " GROUP BY " + pobjGroupByFields.SQL(this.ConnectionType);
					
				if (pobjOrderByFields != null && !pobjOrderByFields.IsEmpty)
					strSQL += " ORDER BY " + pobjOrderByFields.SQL(this.ConnectionType);
					
				if (pobjHavingConditions != null && !pobjHavingConditions.IsEmpty)
					strSQL += " HAVING " + pobjHavingConditions.SQL(this.ConnectionType);
					
				if (pbPerformLocking)
				{
					switch (this.ConnectionType)
					{
						case Database.ConnectionType.MySQL:
						case Database.ConnectionType.Pervasive:
							strSQL += " FOR UPDATE";
							break;
					}
				}
					
				if (pintTop > 0)
				{
					switch (this.ConnectionType)
					{
						case Database.ConnectionType.MySQL:
						case Database.ConnectionType.Pervasive:
							strSQL += " LIMIT " + pintTop.ToString();
							break;
					}
				}
					
				return strSQL;
			}
		}
			
		private string TopClause()
		{
			if (pintTop > 0)
			{
				if (base.ConnectionType == Database.ConnectionType.SQLServerCompactEdition)
					return "TOP(" + pintTop.ToString() + ") ";
				else
					return "TOP " + pintTop.ToString() + " ";
			}
			else
				return string.Empty;
		}
			
		private string DistinctClause()
		{
			if (pbDistinct)
				return "DISTINCT ";
			else
				return string.Empty;
		}
	}
}
