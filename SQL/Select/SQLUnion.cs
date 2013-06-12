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
using System.Linq;

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
			Add(objSelect2);
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

		internal SQLUnion.Type[] UnionTypes
		{
			get
			{
				return pobjUnionType.Cast<SQLUnion.Type>().ToArray();
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
				return base.Serializer.SerializeUnion(this);
			}
		}
	}
}
