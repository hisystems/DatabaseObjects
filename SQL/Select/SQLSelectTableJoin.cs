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
	public class SQLSelectTableJoin : SQLSelectTableBase
	{
		public enum Type
		{
			Inner,
			FullOuter,
			LeftOuter,
			RightOuter
		}
			
		private SQLSelectTableJoins pobjParent;
		private SQLSelectTableBase pobjLeftTable;
		private SQLSelectTableBase pobjRightTable;
		private SQLSelectTableJoinConditions pobjConditions;
		private SQLSelectTableJoin.Type peType;
			
		internal SQLSelectTableJoin(SQLSelectTableJoins objParent)
		{
			pobjParent = objParent;
			pobjConditions = new SQLSelectTableJoinConditions(this);
		}
			
		public SQLSelectTableJoin.Type TheType
		{
			get
			{
				return peType;
			}
				
			set
			{
				peType = value;
			}
		}
			
		public SQLSelectTableBase LeftTable
		{
			get
			{
                return pobjLeftTable;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjLeftTable = value;
			}
		}
			
		public SQLSelectTableBase RightTable
		{
			get
			{
				return pobjRightTable;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjRightTable = value;
			}
		}
			
		public SQLSelectTableJoinConditions Where
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
			
		protected override string Source(Database.ConnectionType eConnectionType)
		{
			string strSQL = pobjLeftTable.SQL(eConnectionType) + " " + GetJoinString(this.TheType) + " " + pobjRightTable.SQL(eConnectionType);
				
			if (pobjConditions != null && !pobjConditions.IsEmpty)
				strSQL += " ON " + pobjConditions.SQL(eConnectionType);
				
			//Surround the join with parentheses - MS Access won't accept it otherwise
			return "(" + strSQL + ")";
		}
			
		private static string GetJoinString(Type eJoinType)
		{
			if (eJoinType == SQLSelectTableJoin.Type.Inner)
				return "INNER JOIN";
			else if (eJoinType == SQLSelectTableJoin.Type.FullOuter)
				return "FULL OUTER JOIN";
			else if (eJoinType == SQLSelectTableJoin.Type.LeftOuter)
				return "LEFT OUTER JOIN";
			else if (eJoinType == SQLSelectTableJoin.Type.RightOuter)
				return "RIGHT OUTER JOIN";
			else
				throw new NotImplementedException(eJoinType.ToString());
		}
	}
}
