' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This class can be used to determine whether a view exists. This class can be 
    ''' used will all databases. If after running the SQL statement the data set is not
    ''' empty then the view exists. 
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Class SQLViewExists
        Inherits SQLStatement

        Private pstrViewName As String

        Public Sub New(ByVal strViewName As String)

            If String.IsNullOrEmpty(strViewName) Then
                Throw New ArgumentNullException
            End If

            pstrViewName = strViewName

        End Sub

        Public Overrides ReadOnly Property SQL() As String
            Get

                Dim strSQL As String = String.Empty
                Dim objSelect As SQLSelect

                Select Case Me.ConnectionType
                    Case Database.ConnectionType.MicrosoftAccess
                        Throw New NotSupportedException
                    Case Database.ConnectionType.MySQL
                        objSelect = New SQLSelect
                        With objSelect
                            .ConnectionType = Me.ConnectionType
                            .Tables.Add("Tables").SchemaName = "INFORMATION_SCHEMA"
                            .Where.Add("Table_Type", ComparisonOperator.EqualTo, "View")
                            .Where.Add("TABLE_NAME", ComparisonOperator.Like, pstrViewName)
                            strSQL = .SQL
                        End With
                    Case Database.ConnectionType.SQLServer
                        objSelect = New SQLSelect
                        With objSelect
                            .ConnectionType = Me.ConnectionType
                            .Tables.Add("sysobjects")
                            .Where.Add("Name", ComparisonOperator.EqualTo, pstrViewName)
                            .Where.Add("XType", ComparisonOperator.EqualTo, "V")       'V = User defined view
                            strSQL = .SQL
                        End With
                    Case Database.ConnectionType.Pervasive
                        Throw New NotSupportedException
                    Case Database.ConnectionType.HyperSQL
                        objSelect = New SQLSelect
                        With objSelect
                            .ConnectionType = Me.ConnectionType
                            .Tables.Add("VIEWS").SchemaName = "INFORMATION_SCHEMA"
                            .Where.Add("TABLE_SCHEMA", ComparisonOperator.EqualTo, "PUBLIC")
                            .Where.Add("TABLE_NAME", ComparisonOperator.EqualTo, pstrViewName)
                            strSQL = .SQL
                        End With
                    Case Database.ConnectionType.SQLServerCompactEdition
                        Throw New NotSupportedException
                    Case Else
                        Throw New NotImplementedException(Me.ConnectionType.ToString)
                End Select

                Return strSQL

            End Get
        End Property

    End Class

End Namespace
