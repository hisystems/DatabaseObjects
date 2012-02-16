
' ___________________________________________________
'
'  © Hi-Integrity Systems 2008. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    ''' <summary>
    ''' Allows data to be inserted using records from another table.
    ''' </summary>
    '''
    ''' <example>
    ''' Dim objCopy As New DatabaseObjects.SQL.SQLInsertFromSelect
    ''' objCopy.Fields.Add("DestinationField1")
    ''' objCopy.Fields.Add("DestinationField1")
    ''' objCopy.TableName = "DestinationTable"
    ''' objCopy.Source.Fields.Add("SourceField1")
    ''' objCopy.Source.Fields.Add("SourceField1")
    ''' objCopy.Source.Tables.Add("SourceTable")
    ''' objCopy.Source.Where.Add("Field1", DatabaseObjects.SQL.ComparisonOperator.EqualTo, "ABC")
    '''
    ''' INSERT INTO DestinationTable
    ''' SELECT * FROM SourceTable
    ''' WHERE SomeField = Condition
    ''' or
    ''' INSERT INTO DestinationTable (IntoField1, IntoField2)
    ''' SELECT (FromField1, FromField2) FROM SourceTable
    ''' WHERE SomeField = Condition
    '''
    ''' </example>
    ''' <remarks></remarks>
    Public Class SQLInsertFromSelect
        Inherits SQLStatement

        Public Class FieldsClass
            Inherits Collections.Specialized.StringCollection

        End Class

        Private pstrInsertIntoTableName As String = String.Empty
        Private pobjInsertIntoFields As New FieldsClass
        Private pobjSourceSelect As SQLSelect = New SQLSelect

        Public Sub New()

        End Sub

        ''' <summary>
        ''' Sets/Returns the fields that will be inserted into the database.
        ''' The order of the fields must match with the order of the fields from
        ''' the source SELECT statement.
        ''' </summary>
        Public Property Fields() As FieldsClass
            Get

                Return pobjInsertIntoFields

            End Get

            Set(ByVal value As FieldsClass)

                If value Is Nothing Then
                    Throw New NullReferenceException
                End If

                pobjInsertIntoFields = value

            End Set
        End Property

        ''' <summary>
        ''' The table to insert into.
        ''' </summary>
        Public Property TableName() As String
            Get

                Return pstrInsertIntoTableName

            End Get

            Set(ByVal Value As String)

                pstrInsertIntoTableName = Value.Trim

            End Set
        End Property

        ''' <summary>
        ''' The SELECT statement where the data is to be copied from.
        ''' The order of the fields must match the order of fields
        ''' specified in the Fields property.
        ''' </summary>
        Public Property Source() As SQLSelect
            Get

                Return pobjSourceSelect

            End Get

            Set(ByVal Value As SQLSelect)

                If Value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjSourceSelect = Value

            End Set
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                Dim strFields As String = String.Empty

                If TableName = String.Empty Then
                    Throw New Exceptions.DatabaseObjectsException("TableName property has not been set.")
                End If

                If pobjInsertIntoFields.Count > 0 Then
                    For intIndex As Integer = 0 To pobjInsertIntoFields.Count - 1
                        strFields &= SQLConvertIdentifierName(pobjInsertIntoFields(intIndex), Me.ConnectionType)
                        If intIndex <> pobjInsertIntoFields.Count - 1 Then
                            strFields &= ", "
                        End If
                    Next
                    strFields = "(" & strFields & ") "
                End If

                pobjSourceSelect.ConnectionType = MyBase.ConnectionType

                Return _
                    "INSERT INTO " & SQLConvertIdentifierName(Me.TableName, Me.ConnectionType) & " " & _
                    strFields & pobjSourceSelect.SQL

            End Get
        End Property

    End Class

End Namespace
