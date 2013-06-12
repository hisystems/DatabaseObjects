// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
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
	public enum BitwiseOperator
	{
		@And,
		@Or
	}
		
	public class SQLBitwiseExpression : SQLExpression
	{
		private SQLExpression pobjLeft;
		private BitwiseOperator peOperator;
		private SQLExpression pobjRight;
			
		public SQLBitwiseExpression()
		{
		}
			
		public SQLBitwiseExpression(string strLeftFieldName, BitwiseOperator eOperator, object objRightValue) 
            : this(new SQLFieldExpression(strLeftFieldName), eOperator, new SQLValueExpression(objRightValue))
		{
		}
			
		public SQLBitwiseExpression(string strLeftFieldName, BitwiseOperator eOperator, SQLExpression objRightExpression) 
            : this(new SQLFieldExpression(strLeftFieldName), eOperator, objRightExpression)
		{
		}
			
		public SQLBitwiseExpression(SQLExpression objLeftExpression, BitwiseOperator eOperator, SQLExpression objRightExpression)
		{
			pobjLeft = objLeftExpression;
			peOperator = eOperator;
			pobjRight = objRightExpression;
		}
			
		public SQLExpression LeftExpression
		{
			get
			{
				return pobjLeft;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjLeft = value;
			}
		}
			
		public SQLExpression RightExpression
		{
			get
			{
				return pobjRight;
			}
				
			set
			{
				if (value == null)
					throw new ArgumentNullException();
					
				pobjRight = value;
			}
		}
			
		public BitwiseOperator Operator
		{
			get
			{
				return peOperator;
			}
				
			set
			{
				peOperator = value;
			}
		}
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeBitwiseExpression(this);
		}
	}
}
