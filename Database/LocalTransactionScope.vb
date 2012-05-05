
' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Imports DatabaseObjects
Imports System.Data

''' <summary>
''' Represents a local (not distributed) database transaction. 
''' Should be used with a 'Using' statement to ensure that the transaction is rolled back if any exception occurs (database or code related).
''' Performs like the System.Transactions.TransactionScope object, except that it only targets local database transactions
''' (i.e. BEING/COMMIT/ROLLBACK TRANSACTION commands) and does not utilise the Microsoft Distributed Transaction Coordinator.
''' </summary>
''' <example>
''' Using localTransaction = New LocalTransactionScope(database)
'''
'''     With collection.Add
'''         .Field = "Value"
'''         .Save()
'''     End With
'''
'''     localTransaction.Complete()
''' End Using
''' </example>
Public Class LocalTransactionScope
    Implements IDisposable

    Private _transaction As Database.TransactionsClass
    Private _disposed As Boolean
    Private _completed As Boolean = False

    ''' <summary>
    ''' Begins a local database transaction.
    ''' </summary>
    Public Sub New(database As Database)

        Me.New(database, IsolationLevel.Unspecified)

    End Sub

    ''' <summary>
    ''' Begins a local database transaction.
    ''' </summary>
    Public Sub New(database As Database, isolationLevel As Data.IsolationLevel)

        If database Is Nothing Then
            Throw New ArgumentNullException
        End If

        _transaction = database.Transactions

        _transaction.Begin(isolationLevel)

    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statement. 
    ''' Returns Nothing/null if no record was selected, otherwise the first field from the
    ''' returned result.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function ExecuteScalar(ByVal sqlStatement As SQL.ISQLStatement) As Object

        Return _transaction.Execute(sqlStatement)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statement and returns the result.
    ''' The returned reader can only be used until the LocalTransactionScope is disposed 
    ''' (typically at the end of a using construct).
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function Execute(ByVal sqlStatement As SQL.ISQLStatement) As IDataReader

        Return _transaction.Execute(sqlStatement)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statements and returns the first result.
    ''' The returned reader(s) can only be used until the LocalTransactionScope is disposed 
    ''' (typically at the end of a using construct).
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function Execute(ByVal sqlStatements As SQL.ISQLStatement()) As IDataReader

        Return _transaction.Execute(sqlStatements)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statement and returns the number of rows affected.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function ExecuteNonQuery(ByVal sqlStatement As SQL.ISQLStatement) As Integer

        Return _transaction.ExecuteNonQuery(sqlStatement)

    End Function

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Executes the SQL statements and returns the number of rows affected for all of the statements.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Public Function ExecuteNonQuery(ByVal sqlStatements As SQL.ISQLStatement()) As Integer

        Return _transaction.ExecuteNonQuery(sqlStatements)

    End Function

    ''' <summary>
    ''' Indicates that the transaction should be commited.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Complete()

        If _completed Then
            Throw New InvalidOperationException("Transaction has already been completed")
        End If

        _transaction.Commit()
        _completed = True

    End Sub

    ''' <summary>
    ''' Closes the connection if it has not already been closed / disposed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Dispose() Implements IDisposable.Dispose

        If Not Me._disposed AndAlso Not _completed Then
            _transaction.Rollback()
            Me._disposed = True
        End If

        GC.SuppressFinalize(Me)

    End Sub

End Class
