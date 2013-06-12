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
	public class SQLFieldAggregateExpression : SQLFieldExpression
	{
		private AggregateFunction peAggregateFunction = AggregateFunction.None;
			
		public SQLFieldAggregateExpression()
		{
		}
			
		public SQLFieldAggregateExpression(string strFieldName) 
            : base(strFieldName)
		{
		}
			
		public SQLFieldAggregateExpression(SQLSelectTable objTable, string strFieldName) 
            : base(objTable, strFieldName)
		{
		}
			
		public SQLFieldAggregateExpression(SQLSelectTable objTable, string strFieldName, AggregateFunction eAggregate) 
            : base(objTable, strFieldName)
		{
			this.AggregateFunction = eAggregate;
		}
			
		public SQL.AggregateFunction AggregateFunction
		{
			get
			{
				return peAggregateFunction;
			}
				
			set
			{
				peAggregateFunction = value;
			}
		}
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeFieldAggregateExpression(this);
		}
	}
}
