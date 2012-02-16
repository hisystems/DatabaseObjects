
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Public Class MicrosoftSQLCompactEditionDatabase
    Inherits Database

    ''' <summary>
    ''' Connects to an SQL Compact Edition 3.5 database.
    ''' Assumes the database has a blank password.
    ''' </summary>
    ''' <param name="strDatabaseFilePath">The full or relative path to the SQL compact edition database. i.e. Data\database.sdf or C:\Data\database.sdf</param>
    Public Sub New(ByVal strDatabaseFilePath As String)

        Me.New(strDatabaseFilePath, String.Empty)

    End Sub

    ''' <summary>
    ''' Connects to an SQL Compact Edition 3.5 database.
    ''' </summary>
    ''' <param name="strDatabaseFilePath">The full or relative path to the SQL compact edition database. i.e. Data\database.sdf or C:\Data\database.sdf</param>
    Public Sub New(ByVal strDatabaseFilePath As String, ByVal strPassword As String)

        MyBase.New("Provider=Microsoft.SQLSERVER.CE.OLEDB.3.5;Data Source=" & strDatabaseFilePath & ";SSCE:Database Password='" & strPassword & "';OLE DB Services=-3;", ConnectionType.SQLServerCompactEdition)

        If String.IsNullOrEmpty(strDatabaseFilePath) Then
            Throw New ArgumentNullException("Database File Path")
        End If

    End Sub

End Class
