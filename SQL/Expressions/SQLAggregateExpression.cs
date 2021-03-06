// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
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
	public class SQLAggregateExpression : SQLFunctionExpression
	{
		private AggregateFunction aggregate;

		public SQLAggregateExpression(AggregateFunction aggregate, string fieldName) 
            : this(aggregate, new SQLFieldExpression(fieldName))
		{
		}
			
		public SQLAggregateExpression(AggregateFunction aggregate, SQLExpression expression) 
            : base(expression)
		{
			this.aggregate = aggregate;
		}

		public AggregateFunction Aggregate
		{
			get
			{
				return aggregate;
			}
		}

		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeAggregateExpression(this);
		}
	}
}
