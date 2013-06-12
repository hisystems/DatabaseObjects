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

namespace DatabaseObjects.Constraints
{
	public class StringMaxLengthConstraint : IConstraint<string>
	{
		private int pintMaxLength;
			
		public StringMaxLengthConstraint(int intMaxLength)
		{
			if (intMaxLength < 1)
				throw new ArgumentException("Maxlength cannot be less than 1 " + intMaxLength.ToString());
				
			pintMaxLength = intMaxLength;
		}
			
		public int MaxLength
		{
			get
			{
				return pintMaxLength;
			}
		}
			
		public bool ValueSatisfiesConstraint(string value)
		{
			return (value != null && value.Length <= pintMaxLength) || value == null;
		}
			
		public override string ToString()
		{
			return "string length is less than or equal to " + pintMaxLength.ToString() + " characters";
		}
	}
}
