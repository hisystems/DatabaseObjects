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
	public class SQLSelectHavingCondition : SQLExpression
	{
		private SQLExpression pobjLeftHandExpression;
		private ComparisonOperator peCompare;
		private SQLExpression pobjRightHandExpression;
			
		public SQLSelectHavingCondition(SQLExpression objLeftHandExpression, ComparisonOperator eCompare, SQLExpression objRightHandExpression)
		{
			if (objLeftHandExpression == null)
				throw new ArgumentNullException();
			else if (objRightHandExpression == null)
				throw new ArgumentNullException();
				
			pobjLeftHandExpression = objLeftHandExpression;
			peCompare = eCompare;
			pobjRightHandExpression = objRightHandExpression;
		}

		internal ComparisonOperator Compare
		{
			get
			{
				return peCompare;
			}
		}

		internal SQLExpression LeftExpression
		{
			get
			{
				return pobjLeftHandExpression;
			}
		}

		internal SQLExpression RightExpression
		{
			get
			{
				return pobjRightHandExpression;
			}
		}

		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeSelectHavingCondition(this);
		}
	}
}
