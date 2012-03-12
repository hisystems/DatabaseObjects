
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Imports DatabaseObjects
Imports System.Data

''' <summary>
''' Facilitates automatically opening and closing a connection.
''' Implements IDisposable so that the construct can be used with a Using construct.
''' </summary>
Public Class ConnectionScope
    Implements IDisposable

    Private _connection As Database.ConnectionController
    Private _disposed As Boolean

    ''' <summary>
    ''' Ensures that a new connection is opened.
    ''' If defined within a TransactionScope then the transaction is utilised
    ''' and object effectively does nothing.
    ''' </summary>
    ''' <param name="database"></param>
    ''' <remarks></remarks>
    Public Sub New(database As Database)

        If database Is Nothing Then
            Throw New ArgumentNullException
        End If

        _connection = database.Connection

        _connection.Start()

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statement. 
    ''' Returns Nothing/null if no record was selected, otherwise the first field from the
    ''' returned result.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function ExecuteScalar(ByVal sqlStatement As SQL.ISQLStatement) As Object

        Return _connection.ExecuteScalar(sqlStatement)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statement and returns the result.
    ''' The returned reader can only be used until the ConnectionScope is disposed 
    ''' (typically at the end of a using construct).
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function Execute(ByVal sqlStatement As SQL.ISQLStatement) As IDataReader

        Return _connection.Execute(sqlStatement)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statements and returns the first result.
    ''' The returned reader(s) can only be used until the ConnectionScope is disposed 
    ''' (typically at the end of a using construct).
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function Execute(ByVal sqlStatements As SQL.ISQLStatement()) As IDataReader

        Return _connection.Execute(sqlStatements)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statement and returns the number of rows affected.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function ExecuteNonQuery(ByVal sqlStatement As SQL.ISQLStatement) As Integer

        Return _connection.ExecuteNonQuery(sqlStatement)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statements and returns the number of rows affected for all of the statements.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function ExecuteNonQuery(ByVal sqlStatements As SQL.ISQLStatement()) As Integer

        Return _connection.ExecuteNonQuery(sqlStatements)

    End Function

    ''' <summary>
    ''' Closes the connection if it has not already been closed / disposed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Dispose() Implements IDisposable.Dispose

        If Not Me._disposed Then
            _connection.Finished()
        End If

        GC.SuppressFinalize(Me)

    End Sub

End Class
