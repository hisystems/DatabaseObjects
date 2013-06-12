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
	public enum LogicalOperator
	{
		@And,
		@Or
	}

	public class SQLLogicalExpression : SQLExpression
	{
		private SQLExpression left;
		private LogicalOperator @operator;
		private SQLExpression right;
			
		public SQLLogicalExpression()
		{
		}
			
		public SQLLogicalExpression(SQLExpression leftExpression, LogicalOperator @operator, SQLExpression rightExpression)
		{
			this.LeftExpression = leftExpression;
			this.@operator = @operator;
			this.RightExpression = rightExpression;
		}
			
		public SQLExpression LeftExpression
		{
			get
			{
				return this.left;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				this.left = value;
			}
		}
			
		public SQLExpression RightExpression
		{
			get
			{
				return this.right;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				this.right = value;
			}
		}
			
		public LogicalOperator Operator
		{
			get
			{
				return this.@operator;
			}
				
			set
			{
				this.@operator = value;
			}
		}
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeLogicalExpression(this);
		}
	}
}
