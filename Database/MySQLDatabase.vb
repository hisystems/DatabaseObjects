' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Public Class MySQLDatabase
    Inherits Database

    'Flags from http://dev.mysql.com/doc/refman/5.0/en/connector-odbc-configuration-connection-parameters.html
    Private Const FLAG_MULTI_STATEMENTS As Integer = 67108864
    Private Const FLAG_FOUND_ROWS As Integer = 2
    Private Const Options As Integer = FLAG_MULTI_STATEMENTS Or FLAG_FOUND_ROWS

    ''' <summary>
    ''' Connects to a MySQL database using the ODBC driver.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal strDataSource As String, ByVal strDatabaseName As String, ByVal strUserName As String, ByVal strPassword As String)

        MyBase.New("Driver={MySQL ODBC 5.1 Driver}; Server=" & strDataSource & "; Database=" & strDatabaseName & "; UID=" & strUserName & "; PWD=" & strPassword & ";Option=" & Options, ConnectionType.MySQL)

        If String.IsNullOrEmpty(strDataSource) Then
            Throw New ArgumentNullException("DataSource")
        ElseIf String.IsNullOrEmpty(strDatabaseName) Then
            Throw New ArgumentNullException("DatabaseName")
        ElseIf String.IsNullOrEmpty(strUserName) Then
            Throw New ArgumentNullException("UserName")
        End If

    End Sub

End Class
