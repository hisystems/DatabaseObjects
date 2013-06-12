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
	public class SQLCastExpression : SQLExpression
	{
		private string pstrFieldName;
		private DataType peCastAsType;
		private int pintPrecision;
		private int pintScale;
		private int pintSize;
			
		public SQLCastExpression(string strFieldName, DataType eCastAsType)
		{
			if (String.IsNullOrEmpty(strFieldName))
				throw new ArgumentNullException();
				
			pstrFieldName = strFieldName;
			peCastAsType = eCastAsType;
		}
			
		public SQLCastExpression(string strFieldName, DataType eCastAsType, int intSize) 
            : this(strFieldName, eCastAsType)
		{
			this.Size = intSize;
		}

		public DataType ToDataType
		{
			get
			{
				return peCastAsType;
			}
		}

		public string FieldName
		{
			get
			{
				return pstrFieldName;
			}
		}
			
		/// <summary>
		/// The size of the character data type.
		/// </summary>
		public int Size
		{
			set
			{
				DataTypeExtensions.EnsureIsCharacter(peCastAsType);
					
				if (value <= 1)
					throw new ArgumentException();
					
				pintSize = value;
			}
				
			get
			{
				return pintSize;
			}
		}
			
		/// <summary>
		/// Sets or returns the scale of the decimal number.
		/// This is the location within the number where the decimal is placed.
		/// The default is 0.
		/// Throws an exception if the data type is not SQL.DataType.Decimal.
		/// </summary>
		public int ScaleLength
		{
			get
			{
				return pintScale;
			}
				
			set
			{
				DataTypeExtensions.EnsureIsDecimal(peCastAsType);
					
				if (value <= 0)
					throw new ArgumentException();
					
				pintScale = value;
			}
		}
			
		/// <summary>
		/// Sets or returns the precision of the decimal number.
		/// This is the number of number characters that are stored.
		/// The default is 18 precision and 0 scale.
		/// Throws an exception if the data type is not SQL.DataType.Decimal.
		/// </summary>
		public int Precision
		{
			get
			{
                return pintPrecision;
			}
				
			set
			{
				DataTypeExtensions.EnsureIsDecimal(peCastAsType);
					
				if (value <= 0)
					throw new ArgumentException();
					
				pintPrecision = value;
			}
		}
			
		internal override string SQL(Serializers.Serializer serializer)
		{
			return serializer.SerializeCastExpression(this);
		}
	}
}
