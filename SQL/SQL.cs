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
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// Provides a set of uniform and portable data types that can be used between
	/// Microsoft Access, SQLServer and MySQL. Please refer to the DataTypeString
	/// function in the Source\SQL\SQLTable.vb file for the actual SQL equivalents for
	/// each database.
	/// </summary>
	/// --------------------------------------------------------------------------------
	public enum DataType
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Integer data from 0 through 255
		/// </summary>
		/// --------------------------------------------------------------------------------
		TinyInteger,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Integer data from 2^15 (-32,768) through 2^15 - 1 (32,767)
		/// </summary>
		/// --------------------------------------------------------------------------------
		SmallInteger,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Integer (whole number) data from -2^31 (-2,147,483,648) through 2^31 - 1 (2,147,483,647)
		/// </summary>
		/// --------------------------------------------------------------------------------
		@Integer,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Integer (whole number) data from -2^63 (-9223372036854775808) through 2^63-1 (9223372036854775807)
		/// </summary>
		/// --------------------------------------------------------------------------------
		BigInteger,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// SQLServer limitation: Fixed-length non-Unicode character data with a maximum length of 8,000 characters
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		Character,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// SQLServer limitation: Fixed-length Unicode data with a maximum length of 4,000 characters
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		UnicodeCharacter,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// SQLServer limitation: Variable-length non-Unicode data with a maximum of 8,000 characters
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		VariableCharacter,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// SQLServer limitation: Variable-length Unicode data with a maximum length of 4,000 characters
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		UnicodeVariableCharacter,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Fixed precision and scale numeric data.
		/// Use the SQLTableField.Precision and SQLTableField.Scale properties.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		@Decimal,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Floating precision number data from -3.40E + 38 through 3.40E + 38. Equivalent
		/// to VB.NET Single data type. The MySQL equivalent used is FLOAT.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		Real,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Floating precision number data from -1.79E + 308 through 1.79E + 308. Equivalent
		/// to .NET Double data type. The MySQL equivalent used is DOUBLE.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		Float,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Monetary data values from -214,748.3648 through +214,748.3647, with accuracy to a
		/// ten-thousandth of a monetary unit. The MySQL equivalent used is DECIMAL(10,4).
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		SmallMoney,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Monetary data values from -2^63 (-922,337,203,685,477.5808) through 2^63 - 1
		/// (+922,337,203,685,477.5807), with accuracy to a ten-thousandth of a monetary unit.
		/// The MySQL equivalent used is DECIMAL(19,4).
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		Money,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Integer data with either a 1 or 0 value
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		@Boolean,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// SQL Server: Date and time data from January 1, 1900, through June 6, 2079, with
		/// an accuracy of one minute. This data type for Access and MySQL is equivalent to
		/// DATETIME.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		SmallDateTime,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Date and time data from January 1, 1753, through December 31, 9999, with an
		/// accuracy of three-hundredths of a second, or 3.33 milliseconds. This is a SQLServer
		/// limitation. MySQL supports '1000-01-01 00:00:00' to '9999-12-31 23:59:59'. To
		/// provide portability, accuracy to only 1 second can be assumed.
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		DateTime,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		TimeStamp,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Variable-length non-Unicode data with a maximum length of 2^31 - 1 (2,147,483,647) characters
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		Text,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Variable-length Unicode data with a maximum length of 2^30 - 1 (1,073,741,823) characters
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		UnicodeText,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// SQLServer limitation: Fixed-length binary data with a maximum length of 8,000 bytes
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		Binary,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// SQLServer limitation: Variable-length binary data with a maximum length of 8,000 bytes
		/// </summary>
		/// --------------------------------------------------------------------------------
		///
		VariableBinary,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Variable-length binary data with a maximum length of 2^31 - 1 (2,147,483,647) bytes
		/// </summary>
		/// --------------------------------------------------------------------------------
		Image,
			
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// A globally unique identifier that is guaranteed to always be unique.
		/// </summary>
		/// --------------------------------------------------------------------------------
		UniqueIdentifier
	}
		
	public enum KeyType
	{
		None,
		Primary,
		Unique
	}
		
	public enum AggregateFunction
	{
		None = 0,
		Average = 1,
		Count,
		Sum,
		Minimum,
		Maximum,
		StandardDeviation,
		Variance
	}
		
    /// <summary>
    /// </summary>
    /// <remarks>
    /// Ascending and Descending are 0 and -1 so that the Not operator will
    /// negate the ordering from ascending to descending and visa versa.
    /// i.e. 'Not Ascending' is equivalent to 'Descending'
    /// </remarks>
	public enum OrderBy
	{
		None = 1,
		Ascending = 0,
		Descending = ~Ascending
	}
		
	public enum ComparisonOperator
	{
		EqualTo,
		NotEqualTo,
		LessThan,
		LessThanOrEqualTo,
		GreaterThan,
		GreaterThanOrEqualTo,
		@Like,
		NotLike
	}
		
	public enum FieldValueAutoAssignmentType
	{
		/// <summary>
		/// Indicates that the field is not automatically assigned by the database,
		/// it is assigned a value from the values specified in the INSERT statement.
		/// </summary>
		/// <remarks></remarks>
		None,
			
		/// <summary>
		/// Indicates that the field is an IDENTITY field and is automatically
		/// incremented by the database on table INSERTS.
		/// Using this flag for the DistinctFieldAttribute is the same as setting the
		/// bAutoIncrements field to true.
		/// </summary>
		/// <remarks></remarks>
		AutoIncrement,
			
		/// <summary>
		/// Indicates that the field is a UNIQUEIDENTIFIER and should be automatically
		/// assigned a new GUID using the System.Guid class.
		/// It does NOT use a database function (such as NEWID()) to assign the field
		/// a new unique identifier as it is difficult to extract the value from the
		/// database after insertion.
		/// </summary>
		/// <remarks></remarks>
		NewUniqueIdentifier
	}

	internal static class DataTypeExtensions
	{
		public static void EnsureIsDecimal(DataType eDataType)
		{
			if (eDataType != DataType.Decimal)
				throw new Exceptions.MethodLockedException("First set Type to " + DataType.Decimal.ToString());
		}

		public static void EnsureIsCharacter(SQL.DataType eDataType)
		{
			if (!IsCharacter(eDataType))
				throw new Exceptions.MethodLockedException("Data type is not character based");
		}

		public static void EnsureIsInteger(SQL.DataType eDataType)
		{
			if (!IsInteger(eDataType))
				throw new Exceptions.MethodLockedException();
		}

		public static bool IsInteger(SQL.DataType eDataType)
		{
			switch (eDataType)
			{
				case DataType.BigInteger:
				case DataType.Integer:
				case DataType.SmallInteger:
				case DataType.TinyInteger:
					return true;
				default:
					return false;
			}
		}

		public static bool IsCharacter(DataType eDataType)
		{
			switch (eDataType)
			{
				case DataType.Character:
				case DataType.UnicodeCharacter:
				case DataType.VariableCharacter:
				case DataType.UnicodeVariableCharacter:
					return true;
				default:
					return false;
			}
		}
	}
}
