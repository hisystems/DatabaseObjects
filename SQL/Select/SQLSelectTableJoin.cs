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
			
		private SQLSelectTableBase pobjLeftTable;
		private SQLSelectTableBase pobjRightTable;
		private SQLSelectTableJoinConditions pobjConditions;
		private SQLSelectTableJoin.Type peType;
			
		internal SQLSelectTableJoin()
		{
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

		internal override string Source(Serializers.Serializer serializer)
		{
			return serializer.SerializeSelectTableJoin(this);
		}
	}
}
