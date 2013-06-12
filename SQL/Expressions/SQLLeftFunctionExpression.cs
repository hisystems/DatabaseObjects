// _________________________________________________________________________
//
//  (c) Hi-Integrity Systems 2010. All rights reserved.
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
	public class SQLLeftFunctionExpression : SQLFunctionExpression
	{
		public SQLLeftFunctionExpression(string strFieldName, int intLength) 
            : this(new SQLFieldExpression(strFieldName), intLength)
		{
		}
			
		public SQLLeftFunctionExpression(SQLExpression objExpression, int intLength) 
            : base(objExpression, new SQLValueExpression(intLength))
		{
			if (intLength < 0)
				throw new ArgumentException("Length: " + intLength.ToString());
		}

		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeLeftFunctionExpression(this);
		}
	}
}
