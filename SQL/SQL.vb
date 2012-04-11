
' ___________________________________________________
'
'  � Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Provides a set of uniform and portable data types that can be used between 
    ''' Microsoft Access, SQLServer and MySQL. Please refer to the DataTypeString 
    ''' function in the Source\SQL\SQLTable.vb file for the actual SQL equivalents for 
    ''' each database.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Enum DataType

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Integer data from 0 through 255
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        TinyInteger

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Integer data from 2^15 (-32,768) through 2^15 - 1 (32,767)
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        SmallInteger

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Integer (whole number) data from -2^31 (-2,147,483,648) through 2^31 - 1 (2,147,483,647)
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        [Integer]

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Integer (whole number) data from -2^63 (-9223372036854775808) through 2^63-1 (9223372036854775807)
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        BigInteger

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' SQLServer limitation: Fixed-length non-Unicode character data with a maximum length of 8,000 characters
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        Character

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' SQLServer limitation: Fixed-length Unicode data with a maximum length of 4,000 characters
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        UnicodeCharacter

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' SQLServer limitation: Variable-length non-Unicode data with a maximum of 8,000 characters
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        VariableCharacter

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' SQLServer limitation: Variable-length Unicode data with a maximum length of 4,000 characters
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        UnicodeVariableCharacter

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Fixed precision and scale numeric data.
        ''' Use the SQLTableField.Precision and SQLTableField.Scale properties.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        [Decimal]

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Floating precision number data from -3.40E + 38 through 3.40E + 38. Equivalent
        ''' to VB.NET Single data type. The MySQL equivalent used is FLOAT.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        Real

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Floating precision number data from -1.79E + 308 through 1.79E + 308. Equivalent
        ''' to .NET Double data type. The MySQL equivalent used is DOUBLE.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        Float

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Monetary data values from -214,748.3648 through +214,748.3647, with accuracy to a 
        ''' ten-thousandth of a monetary unit. The MySQL equivalent used is DECIMAL(10,4). 
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        SmallMoney

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Monetary data values from -2^63 (-922,337,203,685,477.5808) through 2^63 - 1 
        ''' (+922,337,203,685,477.5807), with accuracy to a ten-thousandth of a monetary unit.
        ''' The MySQL equivalent used is DECIMAL(19,4). 
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        Money

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Integer data with either a 1 or 0 value
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        [Boolean]

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' SQL Server: Date and time data from January 1, 1900, through June 6, 2079, with 
        ''' an accuracy of one minute. This data type for Access and MySQL is equivalent to 
        ''' DATETIME.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        SmallDateTime

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Date and time data from January 1, 1753, through December 31, 9999, with an 
        ''' accuracy of three-hundredths of a second, or 3.33 milliseconds. This is a SQLServer
        ''' limitation. MySQL supports '1000-01-01 00:00:00' to '9999-12-31 23:59:59'. To 
        ''' provide portability, accuracy to only 1 second can be assumed.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        DateTime

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        TimeStamp

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Variable-length non-Unicode data with a maximum length of 2^31 - 1 (2,147,483,647) characters
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        Text

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Variable-length Unicode data with a maximum length of 2^30 - 1 (1,073,741,823) characters
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        UnicodeText

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' SQLServer limitation: Fixed-length binary data with a maximum length of 8,000 bytes
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        Binary

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' SQLServer limitation: Variable-length binary data with a maximum length of 8,000 bytes
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        ''' 
        VariableBinary

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' Variable-length binary data with a maximum length of 2^31 - 1 (2,147,483,647) bytes
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Image

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' A globally unique identifier that is guaranteed to always be unique.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        UniqueIdentifier

    End Enum

    Public Enum KeyType
        None
        Primary
        Unique
    End Enum

    Public Enum AggregateFunction
        None = 0
        Average = 1
        Count
        Sum
        Minimum
        Maximum
        StandardDeviation
        Variance
    End Enum

    'Ascending and Descending are 0 and -1 so that the Not operator will 
    'negate the ordering from ascending to descending and visa versa. 
    'i.e. 'Not Ascending' is equivalent to 'Descending'
    Public Enum OrderBy
        None = 1
        Ascending = 0
        Descending = Not Ascending  ' -1
    End Enum

    Public Enum ComparisonOperator
        EqualTo
        NotEqualTo
        LessThan
        LessThanOrEqualTo
        GreaterThan
        GreaterThanOrEqualTo
        [Like]
        NotLike
    End Enum

    Public Enum LogicalOperator
        [And]
        [Or]
    End Enum

    Public Enum FieldValueAutoAssignmentType

        ''' <summary>
        ''' Indicates that the field is not automatically assigned by the database,
        ''' it is assigned a value from the values specified in the INSERT statement.
        ''' </summary>
        ''' <remarks></remarks>
        None

        ''' <summary>
        ''' Indicates that the field is an IDENTITY field and is automatically
        ''' incremented by the database on table INSERTS.
        ''' Using this flag for the DistinctFieldAttribute is the same as setting the
        ''' bAutoIncrements field to true.
        ''' </summary>
        ''' <remarks></remarks>
        AutoIncrement

        ''' <summary>
        ''' Indicates that the field is a UNIQUEIDENTIFIER and should be automatically
        ''' assigned a new GUID using the System.Guid class.
        ''' It does NOT use a database function (such as NEWID()) to assign the field
        ''' a new unique identifier as it is difficult to extract the value from the 
        ''' database after insertion.
        ''' </summary>
        ''' <remarks></remarks>
        NewUniqueIdentifier

    End Enum

End Namespace
