
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This class can be used to determine whether a table exists. This class can be 
    ''' used will all databases. If after running the SQL statement the data set is not
    ''' empty then the table exists. 
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Class SQLTableExists
        Inherits SQLStatement

        Private pstrName As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal strTableName As String)

            Me.Name = strTableName

        End Sub

        Public Property Name() As String
            Get

                Return pstrName

            End Get

            Set(ByVal Value As String)

                pstrName = Value.Trim

            End Set
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                Dim strSQL As String = String.Empty
                Dim objSelect As SQLSelect

                Select Case Me.ConnectionType
                    Case Database.ConnectionType.MicrosoftAccess
                        objSelect = New SQLSelect
                        With objSelect
                            .ConnectionType = Me.ConnectionType
                            .Tables.Add("msysobjects")
                            .Where.Add("Name", ComparisonOperator.EqualTo, Me.Name)
                            .Where.Add("Type", ComparisonOperator.EqualTo, 1)
                            strSQL = .SQL
                        End With
                    Case Database.ConnectionType.MySQL
                        strSQL = "SHOW TABLES LIKE " & SQLConvertValue(Me.Name, Me.ConnectionType)
                    Case Database.ConnectionType.SQLServer
                        objSelect = New SQLSelect
                        With objSelect
                            .ConnectionType = Me.ConnectionType
                            .Tables.Add("sysobjects")
                            .Where.Add("Name", ComparisonOperator.EqualTo, Me.Name)
                            .Where.Add("XType", ComparisonOperator.EqualTo, "U")       'U = User defined table
                            strSQL = .SQL
                        End With
                    Case Database.ConnectionType.Pervasive
                        objSelect = New SQLSelect
                        With objSelect
                            .ConnectionType = Me.ConnectionType
                            .Tables.Add("X$FILE")
                            .Where.Add("Xf$name", ComparisonOperator.EqualTo, Me.Name)
                            strSQL = .SQL
                        End With
                    Case Database.ConnectionType.HyperSQL
                        objSelect = New SQLSelect
                        With objSelect
                            .ConnectionType = Me.ConnectionType
                            .Tables.Add("SYSTEM_TABLES").SchemaName = "INFORMATION_SCHEMA"
                            .Where.Add("TABLE_SCHEM", ComparisonOperator.EqualTo, "PUBLIC")
                            .Where.Add("TABLE_NAME", ComparisonOperator.EqualTo, Me.Name)
                            strSQL = .SQL
                        End With
                    Case Database.ConnectionType.SQLServerCompactEdition
                        Throw New NotSupportedException("Use SELECT COUNT(*) FROM table to determine if a record exists")
                    Case Else
                        Throw New NotImplementedException(Me.ConnectionType.ToString)
                End Select

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
