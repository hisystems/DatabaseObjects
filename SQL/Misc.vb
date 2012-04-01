
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Module Misc

        Public Function SQLConvertIdentifierName( _
            ByVal strIdentifierName As String, _
            ByVal eConnectionType As Database.ConnectionType) As String

            'This function places tags around a field name or table name to ensure it doesn't
            'conflict with a reserved word or if it contains spaces it is not misinterpreted 

            Select Case eConnectionType
                Case Database.ConnectionType.MicrosoftAccess, _
                     Database.ConnectionType.SQLServer, _
                     Database.ConnectionType.SQLServerCompactEdition
                    Return "[" & strIdentifierName.Trim & "]"
                Case Database.ConnectionType.MySQL
                    Return "`" & strIdentifierName.Trim & "`"
                Case Database.ConnectionType.Pervasive
                    Return """" & strIdentifierName.Trim & """"
                Case Database.ConnectionType.HyperSQL
                    Return """" & strIdentifierName.Trim & """"
                Case Else
                    Throw New NotImplementedException(eConnectionType.ToString)
            End Select

        End Function

        Public Function SQLConvertValue( _
            ByVal objValue As Object, _
            ByVal eConnectionType As Database.ConnectionType) As String

            If SQLValueIsNull(objValue) Then

                Return "NULL"

            ElseIf TypeOf objValue Is Boolean Then

                Dim bBoolean As Boolean = DirectCast(objValue, Boolean)

                If bBoolean Then
                    Return "1"
                Else
                    Return "0"
                End If

            ElseIf TypeOf objValue Is Char Then

                Dim chChar As Char = DirectCast(objValue, Char)

                If chChar.Equals("'") Then
                    Select Case eConnectionType
                        Case Database.ConnectionType.MicrosoftAccess, _
                             Database.ConnectionType.SQLServer, _
                             Database.ConnectionType.SQLServerCompactEdition, _
                             Database.ConnectionType.Pervasive, _
                             Database.ConnectionType.HyperSQL
                            Return "''''"
                        Case Database.ConnectionType.MySQL
                            Return "'\''"
                        Case Else
                            Throw New NotImplementedException(eConnectionType.ToString)
                    End Select
                ElseIf chChar.Equals("\") Then
                    Select Case eConnectionType
                        Case Database.ConnectionType.MicrosoftAccess, _
                             Database.ConnectionType.SQLServer, _
                             Database.ConnectionType.SQLServerCompactEdition, _
                             Database.ConnectionType.Pervasive, _
                             Database.ConnectionType.HyperSQL
                            Return "\"
                        Case Database.ConnectionType.MySQL
                            Return "\\"
                        Case Else
                            Throw New NotImplementedException(eConnectionType.ToString)
                    End Select
                Else
                    Return "'" & chChar & "'"
                End If

            ElseIf TypeOf objValue Is DateTime Then

                Dim dtDate As Date = DirectCast(objValue, DateTime)
                Dim strDate As String

                'If the date hasn't been set then set to the 1899-12-30 so that
                'the date isn't set to 2001-01-01
                If dtDate.Day = 1 And dtDate.Month = 1 And dtDate.Year = 1 Then
                    strDate = "1899-12-30"
                Else
                    strDate = dtDate.Year & "-" & dtDate.Month & "-" & dtDate.Day
                End If

                If dtDate.Hour <> 0 Or dtDate.Minute <> 0 Or dtDate.Second <> 0 Then
                    strDate &= " " & dtDate.Hour & ":" & dtDate.Minute & ":" & dtDate.Second & "." & dtDate.Millisecond
                End If

                Select Case eConnectionType
                    Case Database.ConnectionType.MicrosoftAccess
                        Return "#" & strDate & "#"
                    Case Database.ConnectionType.MySQL, _
                         Database.ConnectionType.SQLServer, _
                         Database.ConnectionType.SQLServerCompactEdition, _
                         Database.ConnectionType.Pervasive, _
                         Database.ConnectionType.HyperSQL
                        Return "'" & strDate & "'"
                    Case Else
                        Throw New NotImplementedException(eConnectionType.ToString)
                End Select

            ElseIf TypeOf objValue Is String Then

                Select Case eConnectionType
                    Case Database.ConnectionType.MicrosoftAccess, _
                         Database.ConnectionType.SQLServer, _
                         Database.ConnectionType.SQLServerCompactEdition, _
                         Database.ConnectionType.Pervasive, _
                         Database.ConnectionType.HyperSQL
                        Return "'" & DirectCast(objValue, String).Replace("'"c, "''") & "'"
                    Case Database.ConnectionType.MySQL
                        'Replace the \ escape character with \\ and replace single quotes with two single quotes
                        Return "'" & DirectCast(objValue, String).Replace("\"c, "\\").Replace("'"c, "\'") & "'"
                    Case Else
                        Throw New NotImplementedException(eConnectionType.ToString)
                End Select

            ElseIf TypeOf objValue Is Byte() Then

                Select Case eConnectionType
                    Case Database.ConnectionType.MicrosoftAccess, _
                         Database.ConnectionType.SQLServer, _
                         Database.ConnectionType.SQLServerCompactEdition, _
                         Database.ConnectionType.Pervasive, _
                         Database.ConnectionType.MySQL
                        Return SQLConvertBinaryArray("0x", CType(objValue, Byte()), String.Empty)
                    Case Database.ConnectionType.HyperSQL
                        Return SQLConvertBinaryArray("x'", CType(objValue, Byte()), "'")
                    Case Else
                        Throw New NotImplementedException(eConnectionType.ToString)
                End Select

            ElseIf TypeOf objValue Is System.Guid Then

                Return "'" & DirectCast(objValue, System.Guid).ToString("D") & "'"

            Else

                Return CType(objValue, String)

            End If

        End Function

        Private Function SQLConvertBinaryArray(ByVal strHexPrefix As String, ByVal bytData As Byte(), ByVal strHexSuffix As String) As String

            Dim objHexData As New System.Text.StringBuilder(strHexPrefix, (bytData.Length * 2) + strHexPrefix.Length + strHexSuffix.Length)

            For Each bytByte As Byte In bytData
                objHexData.Append(String.Format("{0:X2}", bytByte))
            Next

            objHexData.Append(strHexSuffix)

            Return objHexData.ToString

        End Function

        Public Function SQLValueIsNull(ByVal objValue As Object) As Boolean

            If objValue Is Nothing Then
                Return True
            ElseIf objValue Is DBNull.Value Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Function SQLConvertAggregate(ByVal eAggregate As SQL.AggregateFunction) As String

            Dim strAggregate As String

            Select Case eAggregate
                Case SQL.AggregateFunction.Average
                    strAggregate = "AVG"
                Case AggregateFunction.Count
                    strAggregate = "COUNT"
                Case AggregateFunction.Maximum
                    strAggregate = "MAX"
                Case AggregateFunction.Minimum
                    strAggregate = "MIN"
                Case AggregateFunction.StandardDeviation
                    strAggregate = "STDEV"
                Case AggregateFunction.Sum
                    strAggregate = "SUM"
                Case AggregateFunction.Variance
                    strAggregate = "VAR"
                Case Else
                    Throw New NotSupportedException
            End Select

            Return strAggregate

        End Function

        Public Function SQLConvertCompare(ByVal eCompare As SQL.ComparisonOperator) As String

            Dim strCompare As String

            Select Case eCompare
                Case ComparisonOperator.EqualTo
                    strCompare = "="
                Case ComparisonOperator.GreaterThan
                    strCompare = ">"
                Case ComparisonOperator.GreaterThanOrEqualTo
                    strCompare = ">="
                Case ComparisonOperator.LessThan
                    strCompare = "<"
                Case ComparisonOperator.LessThanOrEqualTo
                    strCompare = "<="
                Case ComparisonOperator.Like
                    strCompare = "LIKE"
                Case ComparisonOperator.NotLike
                    strCompare = "NOT LIKE"
                Case ComparisonOperator.NotEqualTo
                    strCompare = "<>"
                Case Else
                    Throw New NotSupportedException
            End Select

            Return strCompare

        End Function

        Public Function SQLConvertLogicalOperator(ByVal eLogicalOperator As SQL.LogicalOperator) As String

            Dim strLogicalOperator As String

            Select Case eLogicalOperator
                Case LogicalOperator.And
                    strLogicalOperator = "AND"
                Case LogicalOperator.Or
                    strLogicalOperator = "OR"
                Case Else
                    Throw New NotSupportedException
            End Select

            Return strLogicalOperator

        End Function

        Public Function SQLFieldNameAndTablePrefix( _
            ByVal objTable As SQLSelectTable, _
            ByVal strFieldName As String, _
            ByVal eConnectionType As Database.ConnectionType) As String

            Dim strTablePrefix As String = String.Empty

            If objTable IsNot Nothing Then
                strTablePrefix = SQLTablePrefix(objTable, eConnectionType) & "."
            End If

            Return strTablePrefix & SQLConvertIdentifierName(strFieldName, eConnectionType)

        End Function

        ''' <summary>
        ''' Returns the table alias or if the alias is not set the table's name.
        ''' </summary>
        Public Function SQLTablePrefix( _
            ByVal objTable As SQLSelectTable, _
            ByVal eConnectionType As Database.ConnectionType) As String

            If objTable.Alias <> String.Empty Then
                Return SQLConvertIdentifierName(objTable.Alias, eConnectionType)
            Else
                Return SQLConvertIdentifierName(objTable.Name, eConnectionType)
            End If

        End Function

        Public Function SQLConditionValue(ByVal objValue As Object) As Object

            If TypeOf objValue Is SQLFieldValue Then
                Dim objSQLFieldValue As SQLFieldValue = DirectCast(objValue, SQLFieldValue)
                Return objSQLFieldValue.Value
            Else
                Return objValue
            End If

        End Function

        Public Sub CompareValuePairAssertValid( _
            ByVal eCompare As SQL.ComparisonOperator, _
            ByVal objValue As Object)

            If Not TypeOf objValue Is String And (eCompare = ComparisonOperator.Like Or eCompare = ComparisonOperator.NotLike) Then
                Throw New Exceptions.DatabaseObjectsException("The LIKE operator cannot be used in conjunction with a non-string data type")
            ElseIf TypeOf objValue Is Boolean And Not (eCompare = ComparisonOperator.EqualTo Or eCompare = ComparisonOperator.NotEqualTo) Then
                Throw New Exceptions.DatabaseObjectsException("A boolean value can only be used in conjunction with the " & ComparisonOperator.EqualTo.ToString & " or " & ComparisonOperator.NotEqualTo.ToString & " operators")
            End If

        End Sub

        Public Sub SQLConvertBooleanValue( _
            ByRef objValue As Object, _
            ByRef eCompare As SQL.ComparisonOperator)

            'If a boolean variable set to true then use the NOT
            'operator and compare it to 0. ie. if the condition is 'field = true' then
            'SQL code should be 'field <> 0'
            '-1 is true in MSAccess and 1 is true in SQLServer.

            If TypeOf objValue Is Boolean Then
                If DirectCast(objValue, Boolean) = True Then
                    If eCompare = ComparisonOperator.EqualTo Then
                        eCompare = ComparisonOperator.NotEqualTo
                    Else
                        eCompare = ComparisonOperator.EqualTo
                    End If
                    objValue = False
                End If
            End If

        End Sub

        Public Function SQLConvertCondition( _
            ByVal eCompare As ComparisonOperator, _
            ByVal objValue As Object, _
            ByVal eConnectionType As Database.ConnectionType) As String

            Dim strSQL As String = String.Empty

            SQLConvertBooleanValue(objValue, eCompare)

            'Return 'IS NULL' rather than '= NULL'
            If SQLValueIsNull(objValue) Then
                If eCompare = ComparisonOperator.EqualTo Then
                    strSQL &= "IS " & SQLConvertValue(objValue, eConnectionType)
                ElseIf eCompare = ComparisonOperator.NotEqualTo Then
                    strSQL &= "IS NOT " & SQLConvertValue(objValue, eConnectionType)
                Else
                    Throw New Exceptions.DatabaseObjectsException("DBNull or Nothing/null specified as an SQLCondition value using the " & eCompare.ToString & " operator")
                End If
            Else
                strSQL &= _
                    SQLConvertCompare(eCompare) & " " & _
                    SQLConvertValue(objValue, eConnectionType)
            End If

            Return strSQL

        End Function

        Public Function SQLConvertDataTypeString( _
            ByVal eConnection As Database.ConnectionType, _
            ByVal eDataType As DataType, _
            ByVal intSize As Integer, _
            ByVal intPrecision As Integer, _
            ByVal intScale As Integer) As String

            Dim strDataType As String = String.Empty

            Select Case eDataType
                Case DataType.TinyInteger
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "BYTE"
                        Case Database.ConnectionType.MySQL
                            strDataType = "TINYINT UNSIGNED"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "TINYINT"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "TINYINT"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "TINYINT"
                    End Select

                Case DataType.SmallInteger
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "SMALLINT"
                        Case Database.ConnectionType.MySQL
                            strDataType = "SMALLINT"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "SMALLINT"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "SMALLINT"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "SMALLINT"
                    End Select

                Case DataType.Integer
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "INTEGER"
                        Case Database.ConnectionType.MySQL
                            strDataType = "INT"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "INTEGER"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "INTEGER"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "INT"
                    End Select

                Case DataType.BigInteger
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "NUMERIC(19,0)"
                        Case Database.ConnectionType.MySQL
                            strDataType = "BIGINT"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "BIGINT"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "BIGINT"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "BIGINT"
                    End Select

                Case DataType.Character
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "TEXT(" & intSize & ")"
                        Case Database.ConnectionType.MySQL
                            strDataType = "CHAR(" & intSize & ")"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "CHAR(" & intSize & ")"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "CHAR(" & intSize & ")"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "CHAR(" & intSize & ")"
                    End Select

                Case DataType.UnicodeCharacter
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            'Unicode is only supported in Microsoft Access 2000+
                            strDataType = "TEXT(" & intSize & ")"
                        Case Database.ConnectionType.MySQL
                            strDataType = "NCHAR(" & intSize & ")"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "NCHAR(" & intSize & ")"
                        Case Database.ConnectionType.Pervasive
                            'Unable to verify this is correct.
                            strDataType = "CHAR(" & intSize & ")"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "CHAR(" & intSize & ")"
                    End Select

                Case DataType.VariableCharacter
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "TEXT(" & intSize & ")"
                        Case Database.ConnectionType.MySQL
                            strDataType = "VARCHAR(" & intSize & ")"
                        Case Database.ConnectionType.SQLServer
                            strDataType = "VARCHAR(" & intSize & ")"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "VARCHAR(" & intSize & ")"
                        Case Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "VARCHAR(" & intSize & ")"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "VARCHAR(" & intSize & ")"
                    End Select

                Case DataType.UnicodeVariableCharacter
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            'Unicode is only supported in Microsoft Access 2000+
                            strDataType = "TEXT(" & intSize & ")"
                        Case Database.ConnectionType.MySQL
                            strDataType = "NVARCHAR(" & intSize & ")"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "NVARCHAR(" & intSize & ")"
                        Case Database.ConnectionType.Pervasive
                            'Unable to verify this is correct.
                            strDataType = "VARCHAR(" & intSize & ")"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "VARCHAR(" & intSize & ")"
                    End Select

                Case DataType.Decimal
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "NUMERIC(" & intPrecision & "," & intScale & ")"
                        Case Database.ConnectionType.MySQL
                            strDataType = "DECIMAL(" & intPrecision & "," & intScale & ")"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "NUMERIC(" & intPrecision & "," & intScale & ")"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "NUMERIC(" & intPrecision & "," & intScale & ")"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "NUMBER(" & intPrecision & "," & intScale & ")"
                    End Select

                Case DataType.Real
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "REAL"
                        Case Database.ConnectionType.MySQL
                            strDataType = "FLOAT"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "REAL"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "REAL"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "DOUBLE"
                    End Select

                Case DataType.Float
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "FLOAT"
                        Case Database.ConnectionType.MySQL
                            strDataType = "DOUBLE"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "FLOAT"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "FLOAT"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "DOUBLE"
                    End Select

                Case DataType.SmallMoney
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "NUMERIC(10,4)"
                        Case Database.ConnectionType.MySQL
                            strDataType = "DECIMAL(10,4)"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "SMALLMONEY"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "DECIMAL(10,4)"
                        Case Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "MONEY"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "NUMBER(10,4)"
                    End Select

                Case DataType.Money
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "NUMERIC(19,4)"
                        Case Database.ConnectionType.MySQL
                            strDataType = "DECIMAL(19,4)"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "MONEY"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "DECIMAL(19,4)"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "NUMBER(19,4)"
                    End Select

                Case DataType.Boolean
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "YESNO"
                        Case Database.ConnectionType.MySQL
                            strDataType = "BOOLEAN"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "BIT"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "BIT"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "BOOLEAN"
                    End Select

                Case DataType.SmallDateTime
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "DATETIME"
                        Case Database.ConnectionType.MySQL
                            strDataType = "DATETIME"
                        Case Database.ConnectionType.SQLServer
                            strDataType = "SMALLDATETIME"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "DATETIME"
                        Case Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "DATETIME"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "DATE"
                    End Select

                Case DataType.DateTime
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "DATETIME"
                        Case Database.ConnectionType.MySQL
                            strDataType = "DATETIME"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "DATETIME"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "DATETIME"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "DATE"
                    End Select

                Case DataType.TimeStamp
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            Throw New NotSupportedException("TIMESTAMP")
                        Case Database.ConnectionType.MySQL
                            strDataType = "TIMESTAMP"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "TIMESTAMP"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "TIMESTAMP"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "TIMESTAMP"
                    End Select

                Case DataType.Text
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "MEMO"
                        Case Database.ConnectionType.MySQL
                            strDataType = "LONGTEXT"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "TEXT"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "LONGVARCHAR"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "CLOB(2G)"
                    End Select

                Case DataType.UnicodeText
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            'Unicode is only supported in Microsoft Access 2000+
                            strDataType = "MEMO"
                        Case Database.ConnectionType.MySQL
                            strDataType = "LONGTEXT CHARACTER SET UTF8"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "NTEXT"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "LONGVARCHAR"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "CLOB(2G)"
                    End Select

                Case DataType.Binary
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "OLEOBJECT"
                        Case Database.ConnectionType.MySQL
                            strDataType = "BLOB"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "BINARY"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "BINARY"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "BINARY(8000)"
                    End Select

                Case DataType.VariableBinary
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "OLEOBJECT"
                        Case Database.ConnectionType.MySQL
                            strDataType = "BLOB"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "VARBINARY"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "LONGVARBINARY"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "VARBINARY(8000)"
                    End Select

                Case DataType.Image
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "IMAGE"
                        Case Database.ConnectionType.MySQL
                            strDataType = "LONGBLOB"
                        Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "IMAGE"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "LONGVARBINARY"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "BLOB(2G)"
                    End Select

                Case DataType.UniqueIdentifier
                    Select Case eConnection
                        Case Database.ConnectionType.MicrosoftAccess
                            strDataType = "UNIQUEIDENTIFIER"
                        Case Database.ConnectionType.MySQL
                            strDataType = "UNIQUEIDENTIFIER"
                        Case Database.ConnectionType.Pervasive
                            strDataType = "UNIQUEIDENTIFIER"
                        Case Database.ConnectionType.SQLServer
                            strDataType = "UNIQUEIDENTIFIER"
                        Case Database.ConnectionType.SQLServerCompactEdition
                            strDataType = "UNIQUEIDENTIFIER"
                        Case Database.ConnectionType.HyperSQL
                            strDataType = "UNIQUEIDENTIFIER"
                    End Select

                Case Else
                    Throw New NotImplementedException("Data type " & eDataType.ToString)
            End Select

            If strDataType = String.Empty Then
                Throw New NotImplementedException("Data type " & eDataType.ToString & " is not implemented for connection type " & eConnection.ToString)
            End If

            Return strDataType

        End Function

        Public Sub DataTypeEnsureIsDecimal(ByVal eDataType As DataType)

            If eDataType <> DataType.Decimal Then
                Throw New Exceptions.MethodLockedException("First set Type to " & DataType.Decimal.ToString)
            End If

        End Sub

        Public Sub DataTypeEnsureIsCharacter(ByVal eDataType As SQL.DataType)

            If Not DataTypeIsCharacter(eDataType) Then
                Throw New Exceptions.MethodLockedException("Data type is not character based")
            End If

        End Sub

        Public Sub DataTypeEnsureIsInteger(ByVal eDataType As SQL.DataType)

            If Not DataTypeIsInteger(eDataType) Then
                Throw New Exceptions.MethodLockedException
            End If

        End Sub

        Public Function DataTypeIsInteger(ByVal eDataType As SQL.DataType) As Boolean

            Select Case eDataType
                Case DataType.BigInteger, DataType.Integer, DataType.SmallInteger, DataType.TinyInteger
                    Return True
                Case Else
                    Return False
            End Select

        End Function

        Public Function DataTypeIsCharacter(ByVal eDataType As DataType) As Boolean

            Select Case eDataType
                Case _
                    DataType.Character, DataType.UnicodeCharacter, _
                    DataType.VariableCharacter, DataType.UnicodeVariableCharacter
                    Return True
                Case Else
                    Return False
            End Select

        End Function

        ''' <summary>
        ''' All keys are returned in lower case.
        ''' </summary>
        ''' <exception cref="FormatException">If the connection string is in an invalid format.</exception>
        Friend Function GetDictionaryFromConnectionString(ByVal strConnectionString As String) As System.Collections.Generic.IDictionary(Of String, String)

            Dim objDictionary As New System.Collections.Generic.Dictionary(Of String, String)
            Dim strPropertyValueArray() As String

            For Each strPropertyValue As String In strConnectionString.Split(";"c)
                If Not String.IsNullOrEmpty(strPropertyValue) Then
                    strPropertyValueArray = strPropertyValue.Split("="c)
                    If strPropertyValueArray.Length = 2 Then
                        objDictionary.Add(strPropertyValueArray(0).Trim.ToLower, strPropertyValueArray(1).Trim)
                    Else
                        Throw New FormatException("Invalid key property definition for '" & strPropertyValue & "' from '" & strConnectionString & "'")
                    End If
                End If
            Next

            Return objDictionary

        End Function

    End Module

End Namespace
