// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
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
				DataTypeExtensions.EnsureIsCharacter(peCastAsType);

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
				DataTypeExtensions.EnsureIsDecimal(peCastAsType);

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
				DataTypeExtensions.EnsureIsDecimal(peCastAsType);
			
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
