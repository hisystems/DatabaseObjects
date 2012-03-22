
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Imports System.Linq

Namespace SQL

    Public Class SQLTableFields
        Implements System.Collections.Generic.IEnumerable(Of SQLTableFieldBase)

        Friend Enum AlterModeType

            ''' <summary>
            ''' Columns are being added or created
            ''' </summary>
            ''' <remarks></remarks>
            Add

            ''' <summary>
            ''' Columns are being modified.
            ''' </summary>
            ''' <remarks></remarks>
            Modify

            ''' <summary>
            ''' Columns are being dropped
            ''' </summary>
            ''' <remarks></remarks>
            Drop
        End Enum

        Private Const pcintAlterModeUninitialized As Integer = -1

        Friend AlterMode As AlterModeType = CType(pcintAlterModeUninitialized, AlterModeType)
        Private pobjFields As New Collections.Generic.List(Of SQLTableFieldBase)

        Public Sub New()

        End Sub

        Public Function Add() As SQLTableField

            Return Add("", DataType.VariableCharacter, 255)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eDataType As SQL.DataType) As SQLTableField

            Return Me.Add(strFieldName, eDataType, 0)

        End Function

        Public Function Add( _
            ByVal strFieldName As String, _
            ByVal eDataType As SQL.DataType, _
            ByVal intSize As Integer) As SQLTableField

            EnsureAlterModeValid(AlterModeType.Add)

            Dim objField As SQLTableField = New SQLTableField

            With objField
                .Name = strFieldName
                .DataType = eDataType
                If intSize > 0 Then
                    .Size = intSize
                End If
                If DataTypeIsCharacter(eDataType) And intSize = 0 Then
                    Throw New ArgumentException("Size not specified for character based field " & strFieldName)
                End If
            End With

            pobjFields.Add(objField)

            Return objField

        End Function

        Public Function AddComputed( _
            ByVal strFieldName As String, _
            ByVal objComputation As SQLExpression) As SQLTableFieldComputed

            EnsureAlterModeValid(AlterModeType.Add)

            Dim objField As New SQLTableFieldComputed(strFieldName, objComputation)

            pobjFields.Add(objField)

            Return objField

        End Function

        Public Sub Add(ByVal objField As SQLTableField)

            If objField Is Nothing Then
                Throw New ArgumentNullException
            End If

            EnsureAlterModeValid(AlterModeType.Add)

            pobjFields.Add(objField)

        End Sub

        Default Public ReadOnly Property Item(ByVal strFieldName As String) As SQLTableField
            Get

                EnsureAlterModeValid(AlterModeType.Modify)

                Dim intIndex As Integer

                strFieldName = strFieldName.Trim
                intIndex = FieldNameIndex(strFieldName)

                If intIndex = -1 Then
                    Item = New SQLTableField
                    Item.Name = strFieldName
                    pobjFields.Add(Item)
                Else
                    Item = DirectCast(pobjFields(intIndex), SQLTableField)
                End If

            End Get
        End Property

        Public Sub Drop(ByVal strFieldName As String)

            EnsureAlterModeValid(AlterModeType.Drop)

            Dim objField As SQLTableField = New SQLTableField

            objField.Name = strFieldName.Trim

            If FieldNameIndex(strFieldName) = -1 Then
                pobjFields.Add(objField)
            Else
                Throw New ArgumentException("Field '" & strFieldName & "' already exists")
            End If

        End Sub

        Private Function FieldNameIndex( _
            ByVal strFieldName As String) As Integer

            strFieldName = strFieldName.Trim
            FieldNameIndex = -1

            For intIndex As Integer = 0 To pobjFields.Count - 1
                If String.Compare(DirectCast(pobjFields(intIndex), SQLTableField).Name, strFieldName, True) = 0 Then
                    Return intIndex
                End If
            Next

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="eConnectionType"></param>
        ''' <param name="bIncludeColumnModifier">
        ''' Indicates whether the ADD, MODIFY or DROP modifiers are required for each column.
        ''' When utilised from SQLCreateTable this will always be false. However, for SQLAlterTable this will be true.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function SQL(ByVal eConnectionType As Database.ConnectionType, ByVal bIncludeColumnModifier As Boolean) As String

            Const cstrSeperator As String = ", "

            Dim strSQL As String = String.Empty
            Dim strColumnModifier As String = String.Empty

            'Include mode when altering a table, otherwise when creating a table the mode is not required.
            If bIncludeColumnModifier Then
                strColumnModifier = GetAlterModeColumnModifier(AlterMode, eConnectionType)

                'This case statement is related to the if statement below with the mode space char being added for MySQL
                If eConnectionType <> Database.ConnectionType.MySQL Then
                    strSQL = strColumnModifier & " "
                End If
            End If

            For Each objField As SQLTableFieldBase In pobjFields
                If bIncludeColumnModifier Then
                    If eConnectionType = Database.ConnectionType.MySQL Then
                        strSQL &= strColumnModifier & " "
                    End If
                End If

                strSQL &= objField.SQL(eConnectionType, bOnlyFieldName:=(AlterMode = AlterModeType.Drop)) & cstrSeperator
            Next

            Return strSQL.Substring(0, strSQL.Length - cstrSeperator.Length)        'remove the last comma and space

        End Function

        Private Shared Function GetAlterModeColumnModifier(ByVal eAlterMode As AlterModeType, ByVal eConnectionType As Database.ConnectionType) As String

            Select Case eAlterMode
                Case AlterModeType.Add
                    Return "ADD"
                Case AlterModeType.Modify
                    Select Case eConnectionType
                        Case Database.ConnectionType.MySQL, _
                             Database.ConnectionType.Pervasive
                            Return "MODIFY COLUMN"
                        Case Database.ConnectionType.MicrosoftAccess, _
                             Database.ConnectionType.SQLServer, _
                             Database.ConnectionType.SQLServerCompactEdition, _
                             Database.ConnectionType.HyperSQL
                            Return "ALTER COLUMN"
                        Case Else
                            Throw New NotImplementedException(eConnectionType.ToString)
                    End Select
                Case AlterModeType.Drop
                    Return "DROP COLUMN"
                Case Else
                    Throw New NotImplementedException
            End Select

        End Function

        Private Sub EnsureAlterModeValid(ByVal eAlterMode As SQLTableFields.AlterModeType)

            'if the alter mode hasn't been set then any of the modes are valid
            If AlterMode = pcintAlterModeUninitialized Then
                AlterMode = eAlterMode
            Else
                If eAlterMode <> AlterMode Then
                    Throw New Exceptions.DatabaseObjectsException("Cannot mix " & AlterModeDescription(AlterMode) & " fields and " & AlterModeDescription(eAlterMode) & " fields into one SQL statement")
                End If
            End If

        End Sub

        Private Function AlterModeDescription(ByVal eAlterMode As SQLTableFields.AlterModeType) As String

            Select Case eAlterMode
                Case AlterModeType.Add
                    Return "adding"
                Case AlterModeType.Modify
                    Return "altering"
                Case AlterModeType.Drop
                    Return "dropping"
                Case Else
                    Throw New NotSupportedException
            End Select

        End Function

        Private Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of SQLTableFieldBase) Implements System.Collections.Generic.IEnumerable(Of SQLTableFieldBase).GetEnumerator

            Return pobjFields.GetEnumerator

        End Function

        Private Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

            Return pobjFields.GetEnumerator

        End Function

    End Class


    Public MustInherit Class SQLTableFieldBase

        Protected Friend MustOverride Function SQL(ByVal eConnectionType As Database.ConnectionType, ByVal bOnlyFieldName As Boolean) As String

        Private pobjNameAsExpression As New SQLFieldExpression

        Public Property Name() As String
            Get

                Return pobjNameAsExpression.Name

            End Get

            Set(ByVal Value As String)

                If String.IsNullOrEmpty(Value) Then
                    Throw New ArgumentNullException
                End If

                pobjNameAsExpression.Name = Value.Trim

            End Set
        End Property

        Protected ReadOnly Property NameAsExpression As SQLExpression
            Get

                Return pobjNameAsExpression

            End Get
        End Property

    End Class

    Public Class SQLTableFieldComputed
        Inherits SQLTableFieldBase

        Private pobjComputation As SQLExpression

        Public Sub New(ByVal strFieldName As String, ByVal objComputation As SQLExpression)

            MyBase.Name = strFieldName

            If objComputation Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjComputation = objComputation

        End Sub

        Protected Friend Overrides Function SQL(eConnectionType As Database.ConnectionType, bOnlyFieldName As Boolean) As String

            If bOnlyFieldName Then
                Throw New InvalidOperationException("Computed columns cannot be used for dropping fields")
            End If

            Return MyBase.NameAsExpression.SQL(eConnectionType) & " AS (" & pobjComputation.SQL(eConnectionType) & ")"

        End Function

    End Class

    ''' <summary>
    ''' Represents a new database field to be created or added to a database table.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SQLTableField
        Inherits SQLTableFieldBase

        Private peType As SQL.DataType = DataType.VariableCharacter
        Private pintSize As Integer = 1
        Private pintScale As Integer = 0
        Private pintPrecision As Integer = 18
        Private pbAutoIncrements As Boolean
        Private pbAcceptsNull As Boolean = True
        Private pobjDefault As Object
        Private peKeyType As SQL.KeyType = KeyType.None

        Public Sub New()

        End Sub

        Public Property DataType() As SQL.DataType
            Get

                Return peType

            End Get

            Set(ByVal Value As SQL.DataType)

                If pbAutoIncrements AndAlso Not DataTypeIsInteger(Value) Then
                    Throw New InvalidOperationException("Data type " & Value.ToString & " cannot be used as an autoincrement field, as it is not an integer field")
                End If

                peType = Value

            End Set
        End Property

        Public Property KeyType() As SQL.KeyType
            Get

                Return peKeyType

            End Get

            Set(ByVal Value As SQL.KeyType)

                peKeyType = Value

                If Value = KeyType.Primary Then
                    pbAcceptsNull = False
                End If

            End Set
        End Property

        Public Property Size() As Integer
            Set(ByVal Value As Integer)

                Misc.DataTypeEnsureIsCharacter(peType)

                If Value <= 1 Then
                    Throw New ArgumentException
                End If

                pintSize = Value

            End Set

            Get

                Misc.DataTypeEnsureIsCharacter(peType)
                Return pintSize

            End Get
        End Property

        ''' <summary>
        ''' Sets or returns the scale of the decimal number.
        ''' This is the location within the number where the decimal is placed.
        ''' The default is 0. 
        ''' Throws an exception if the data type is not SQl.DataType.Decimal.
        ''' </summary>
        Public Property ScaleLength() As Integer
            Get

                DataTypeEnsureIsDecimal(peType)
                Return pintScale

            End Get

            Set(ByVal Value As Integer)

                DataTypeEnsureIsDecimal(peType)

                If Value <= 0 Then
                    Throw New ArgumentException
                End If

                pintScale = Value

            End Set
        End Property

        ''' <summary>
        ''' Sets or returns the precision of the decimal number.
        ''' This is the number of number characters that are stored.
        ''' The default is 18 precision and 0 scale.
        ''' Throws an exception if the data type is not SQl.DataType.Decimal.
        ''' </summary>
        Public Property Precision() As Integer
            Get

                DataTypeEnsureIsDecimal(peType)
                Return pintPrecision

            End Get

            Set(ByVal Value As Integer)

                DataTypeEnsureIsDecimal(peType)

                If Value <= 0 Then
                    Throw New ArgumentException
                End If

                pintPrecision = Value

            End Set
        End Property

        Public Property AutoIncrements() As Boolean
            Get

                DataTypeEnsureIsInteger(peType)
                AutoIncrements = pbAutoIncrements

            End Get

            Set(ByVal Value As Boolean)

                DataTypeEnsureIsInteger(peType)
                pbAutoIncrements = Value
                pbAcceptsNull = Not Value

            End Set
        End Property

        Public Property AcceptsNull() As Boolean
            Get

                Return pbAcceptsNull

            End Get

            Set(ByVal Value As Boolean)

                pbAcceptsNull = Value

            End Set
        End Property

        Public Property [Default]() As Object
            Get

                Return pobjDefault

            End Get

            Set(ByVal Value As Object)

                If peType = DataType.UniqueIdentifier Then
                    Throw New InvalidOperationException("Cannot set the default value when the data type is of type DatabaseObjects.SQL.DataType.UniqueIdentifier")
                End If

                pobjDefault = Value

            End Set
        End Property

        Protected Friend Overrides Function SQL(eConnectionType As Database.ConnectionType, bOnlyFieldName As Boolean) As String

            If bOnlyFieldName Then
                Return MyBase.NameAsExpression.SQL(eConnectionType)
            Else
                Return SQLForColumnAddOrModify(eConnectionType)
            End If

        End Function

        Private Function SQLForColumnAddOrModify(eConnectionType As Database.ConnectionType) As String

            Dim strDataType As String = String.Empty
            Dim strColumnOptions As String
            Dim strSQL As String = String.Empty

            'For Pervasive do not specify NULL/NOT NULL and the data type for an IDENTITY field
            Dim bSpecifyNullStatus As Boolean = Not (pbAutoIncrements And (eConnectionType = Database.ConnectionType.Pervasive))
            Dim bSpecifyDataType As Boolean = Not (pbAutoIncrements And (eConnectionType = Database.ConnectionType.Pervasive))

            If bSpecifyDataType Then
                strDataType = Misc.SQLConvertDataTypeString(eConnectionType, peType, pintSize, pintPrecision, pintScale)
            End If

            strColumnOptions = ColumnOptions(eConnectionType, bSpecifyNullStatus)
            strSQL = MyBase.NameAsExpression.SQL(eConnectionType) & " " & strDataType & strColumnOptions

            Return strSQL

        End Function

        Private Function ColumnOptions( _
            ByVal eConnection As Database.ConnectionType, _
            ByVal bSpecifyNullStatus As Boolean) As String

            Dim objOptions As ArrayList = New ArrayList
            Dim strOptions As String = String.Empty

            'In version 2.13+ the IDENTITY constraint has been reordered
            'to before the contraints (i.e. DEFAULT, NULL etc.)
            If Misc.DataTypeIsInteger(peType) And pbAutoIncrements Then
                Select Case eConnection
                    Case Database.ConnectionType.MicrosoftAccess
                        objOptions.Add("IDENTITY")
                    Case Database.ConnectionType.MySQL
                        objOptions.Add("AUTO_INCREMENT")
                        'must be set to a key 
                        If peKeyType = KeyType.None Then
                            peKeyType = KeyType.Unique
                        End If
                    Case Database.ConnectionType.SQLServer, Database.ConnectionType.SQLServerCompactEdition
                        objOptions.Add("IDENTITY")
                    Case Database.ConnectionType.Pervasive
                        objOptions.Add("IDENTITY")
                    Case Database.ConnectionType.HyperSQL
                        objOptions.Add("GENERATED BY DEFAULT AS IDENTITY(START WITH 1 INCREMENT BY 1)")
                    Case Else
                        Throw New NotImplementedException(eConnection.ToString & "; Auto Increment")
                End Select
            End If

            If Not pobjDefault Is Nothing Then
                objOptions.Add("DEFAULT " & SQLConvertValue(pobjDefault, eConnection))
            End If

            'NULL status MUST be after the default value for Pervasive 
            'In version 2.13+ the NULL/NOT NULL constraint has been reordered
            'to after the DEFAULT constraint. Hopefully, this won't cause any problems. 
            'According to the T-SQL docs it should not be a problem.
            If bSpecifyNullStatus Then
                If pbAcceptsNull Then
                    objOptions.Add("NULL")
                Else
                    objOptions.Add("NOT NULL")
                End If
            End If

            Select Case peKeyType
                Case KeyType.Primary
                    objOptions.Add("PRIMARY KEY")
                Case KeyType.Unique
                    objOptions.Add("UNIQUE")
            End Select

            For Each objOption As Object In objOptions
                strOptions &= " " & DirectCast(objOption, String)
            Next

            ColumnOptions = strOptions

        End Function

    End Class

End Namespace
