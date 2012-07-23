// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Collections.Generic;

namespace DatabaseObjects.SQL
{
	public class SQLUnion : SQLStatement
	{
		public enum Type
		{
			Distinct, //default
			All
		}
			
		private const SQLUnion.Type pceDefaultUnionType = Type.Distinct;
			
		private List<SQL.SQLSelect> pobjSelectStatements = new List<SQL.SQLSelect>();
		private ArrayList pobjUnionType = new ArrayList();
		private SQLSelectOrderByFields pobjOrderByFields = new SQLSelectOrderByFields();
			
		public SQLUnion()
		{
		}
			
		public SQLUnion(SQLSelect objSelect1, SQLSelect objSelect2) 
            : this(objSelect1, pceDefaultUnionType, objSelect2)
		{
		}
			
		public SQLUnion(SQLSelect objSelect1, SQLUnion.Type eType, SQLSelect objSelect2)
		{
			Add(objSelect1);
			AddUnionType(eType);
			Add(objSelect1);
		}
			
		public void Add(SQLSelect objSelect)
		{
			EnsurePreviousTypeExists();
			pobjSelectStatements.Add(objSelect);
		}
			
		public void AddUnionType(SQLUnion.Type eType)
		{
			if (pobjUnionType.Count + 1 > pobjSelectStatements.Count)
				throw new Exceptions.DatabaseObjectsException("First call the Add function - this function has been called without a prior call to Add");
				
			pobjUnionType.Add(eType);
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
			
		internal SQL.SQLSelect[] SelectStatements
		{
			get
			{
				return pobjSelectStatements.ToArray();
			}
		}
			
		private void EnsurePreviousTypeExists()
		{
			//Add the AND operator if an operator hasn't been called after the previous Add call
			if (pobjUnionType.Count < pobjSelectStatements.Count)
				this.AddUnionType(pceDefaultUnionType);
		}
			
		public override string SQL
		{
			get
			{
				if (pobjSelectStatements.Count == 0)
					throw new Exceptions.DatabaseObjectsException("The table has not been set.");

                string strSQL = string.Empty;
                SQLSelect objSelect;
                string strOrderBy;
	
				for (int intIndex = 0; intIndex < pobjSelectStatements.Count; intIndex++)
				{
					if (intIndex > 0)
					{
						strSQL += " UNION ";
						if (((SQLUnion.Type)pobjUnionType[intIndex - 1]) == Type.All)
							strSQL += "ALL ";
					}
						
					objSelect = pobjSelectStatements[intIndex];
					objSelect.ConnectionType = this.ConnectionType;
					strSQL += "(" + objSelect.SQL + ")";
				}
					
				if (pobjOrderByFields != null)
				{
					strOrderBy = pobjOrderByFields.SQL(this.ConnectionType);
					if (strOrderBy != string.Empty)
						strSQL += " ORDER BY " + strOrderBy;
				}
					
				return strSQL;
			}
		}
	}
}
