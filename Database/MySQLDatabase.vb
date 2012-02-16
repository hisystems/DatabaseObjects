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

    ''' <summary>
    ''' Connects to a MySQL database using the ODBC driver.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal strDataSource As String, ByVal strDatabaseName As String, ByVal strUserName As String, ByVal strPassword As String)

        MyBase.New("Driver={MySQL}; Server=" & strDataSource & "; Database=" & strDatabaseName & "; UID=" & strUserName & "; PWD=" & strPassword & ";", ConnectionType.MySQL)

        If String.IsNullOrEmpty(strDataSource) Then
            Throw New ArgumentNullException("DataSource")
        ElseIf String.IsNullOrEmpty(strDatabaseName) Then
            Throw New ArgumentNullException("DatabaseName")
        ElseIf String.IsNullOrEmpty(strUserName) Then
            Throw New ArgumentNullException("UserName")
        End If

    End Sub

End Class
