
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Public Class MicrosoftSQLServerDatabase
    Inherits Database

    Private pstrDataSource As String
    Private pstrDatabase As String

    ''' <summary>
    ''' Connects to a Microsoft SQL Server database.
    ''' Uses SSPI security to connect to the database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal strDataSource As String, ByVal strDatabaseName As String)

        MyBase.New(ConnectionStringRoot(strDataSource, strDatabaseName) & "Integrated Security=SSPI;", ConnectionType.SQLServer)

        EnsureDatabaseDetailsValid(strDataSource, strDatabaseName)

        pstrDatabase = strDatabaseName
        pstrDataSource = strDataSource

    End Sub

    ''' <summary>
    ''' Connects to a Microsoft SQL Server database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal strDataSource As String, ByVal strDatabaseName As String, ByVal strUserName As String, ByVal strPassword As String)

        MyBase.New(ConnectionStringRoot(strDataSource, strDatabaseName) & "UID=" & strUserName & ";pwd=" & strPassword & ";", ConnectionType.SQLServer)

        EnsureDatabaseDetailsValid(strDataSource, strDatabaseName)

        If String.IsNullOrEmpty(strUserName) Then
            Throw New ArgumentNullException("UserName")
        End If

        pstrDatabase = strDatabaseName
        pstrDataSource = strDataSource

    End Sub

    Public ReadOnly Property Source() As String
        Get

            Return pstrDataSource

        End Get
    End Property

    Public ReadOnly Property Name() As String
        Get

            Return pstrDatabase

        End Get
    End Property

    ''' <summary>
    ''' If parsing fails then the FormatException message will contain useful information as to the cause.
    ''' </summary>
    ''' <exception cref="FormatException">If the connection string is in an invalid format.</exception>
    Public Shared Function Parse(ByVal strConnectionString As String) As MicrosoftSQLServerDatabase

        Dim objDictionary As Collections.Generic.IDictionary(Of String, String) = _
            SQL.Misc.GetDictionaryFromConnectionString(strConnectionString)

        If objDictionary.ContainsKey("integrated security") Then
            Return New MicrosoftSQLServerDatabase(GetDataSource(objDictionary), GetDatabase(objDictionary))
        Else
            Return New MicrosoftSQLServerDatabase(GetDataSource(objDictionary), GetDatabase(objDictionary), GetUserID(objDictionary), GetPassword(objDictionary))
        End If

    End Function

    Private Shared Function GetDataSource(ByVal objConnectionDictionary As Collections.Generic.IDictionary(Of String, String)) As String

        If objConnectionDictionary.ContainsKey("data source") Then
            Return objConnectionDictionary("data source")
        Else
            Throw New FormatException("Could not find 'Data Source=' definition")
        End If

    End Function

    Private Shared Function GetDatabase(ByVal objConnectionDictionary As Collections.Generic.IDictionary(Of String, String)) As String

        If objConnectionDictionary.ContainsKey("database") Then
            Return objConnectionDictionary("database")
        ElseIf objConnectionDictionary.ContainsKey("initial catalog") Then
            Return objConnectionDictionary("initial catalog")
        Else
            Throw New FormatException("Could not find 'Database=' or 'Initial Catalog=' definition")
        End If

    End Function

    Private Shared Function GetUserID(ByVal objConnectionDictionary As Collections.Generic.IDictionary(Of String, String)) As String

        If objConnectionDictionary.ContainsKey("uid") Then
            Return objConnectionDictionary("uid")
        ElseIf objConnectionDictionary.ContainsKey("user id") Then
            Return objConnectionDictionary("user id")
        Else
            Throw New FormatException("Could not find 'UID=' or 'User ID=' definition")
        End If

    End Function

    Private Shared Function GetPassword(ByVal objConnectionDictionary As Collections.Generic.IDictionary(Of String, String)) As String

        If objConnectionDictionary.ContainsKey("pwd") Then
            Return objConnectionDictionary("pwd")
        ElseIf objConnectionDictionary.ContainsKey("password") Then
            Return objConnectionDictionary("password")
        Else
            Throw New FormatException("Could not find 'Password=' or 'Pwd=' definition")
        End If

    End Function

    Private Sub EnsureDatabaseDetailsValid(ByVal strDataSource As String, ByVal strDatabaseName As String)

        If String.IsNullOrEmpty(strDataSource) Then
            Throw New ArgumentNullException("DataSource")
        ElseIf String.IsNullOrEmpty(strDatabaseName) Then
            Throw New ArgumentNullException("DatabaseName")
        End If

    End Sub

    Private Shared Function ConnectionStringRoot(ByVal strDataSource As String, ByVal strDatabaseName As String) As String

        Return "Provider=SQLOLEDB;Data Source=" & strDataSource & ";" & "Database=" & strDatabaseName & ";"

    End Function

End Class

