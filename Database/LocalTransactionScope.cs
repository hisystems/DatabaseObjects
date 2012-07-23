// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using DatabaseObjects;

/// <summary>
/// Represents a local (not distributed) database transaction.
/// Should be used with a 'Using' statement to ensure that the transaction is rolled back if any exception occurs (database or code related).
/// Performs like the System.Transactions.TransactionScope object, except that it only targets local database transactions
/// (i.e. BEING/COMMIT/ROLLBACK TRANSACTION commands) and does not utilise the Microsoft Distributed Transaction Coordinator.
/// </summary>
/// <example>
/// Using localTransaction = New LocalTransactionScope(database)
///
///     With collection.Add
///         .Field = "Value"
///         .Save()
///     End With
///
///     localTransaction.Complete()
/// End Using
/// </example>
namespace DatabaseObjects
{
	public class LocalTransactionScope : IDisposable
	{
		private Database.TransactionsClass transaction;
		private bool disposed;
		private bool completed = false;
		
		/// <summary>
		/// Begins a local database transaction.
		/// </summary>
		public LocalTransactionScope(Database database) 
            : this(database, IsolationLevel.Unspecified)
		{
		}
		
		/// <summary>
		/// Begins a local database transaction.
		/// </summary>
		public LocalTransactionScope(Database database, System.Data.IsolationLevel isolationLevel)
		{
			if (database == null)
				throw new ArgumentNullException();
			
			transaction = database.Transactions;
			transaction.Begin(isolationLevel);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Executes the SQL statement.
		/// Returns Nothing/null if no record was selected, otherwise the first field from the
		/// returned result.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public object ExecuteScalar(SQL.ISQLStatement sqlStatement)
		{
			return transaction.Execute(sqlStatement);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Executes the SQL statement and returns the result.
		/// The returned reader can only be used until the LocalTransactionScope is disposed
		/// (typically at the end of a using construct).
		/// </summary>
		/// --------------------------------------------------------------------------------
		public IDataReader Execute(SQL.ISQLStatement sqlStatement)
		{
			return transaction.Execute(sqlStatement);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Executes the SQL statements and returns the first result.
		/// The returned reader(s) can only be used until the LocalTransactionScope is disposed
		/// (typically at the end of a using construct).
		/// </summary>
		/// --------------------------------------------------------------------------------
		public IDataReader Execute(SQL.ISQLStatement[] sqlStatements)
		{
			return transaction.Execute(sqlStatements);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Executes the SQL statement and returns the number of rows affected.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public int ExecuteNonQuery(SQL.ISQLStatement sqlStatement)
		{
			return transaction.ExecuteNonQuery(sqlStatement);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Executes the SQL statements and returns the number of rows affected for all of the statements.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public int ExecuteNonQuery(SQL.ISQLStatement[] sqlStatements)
		{
			return transaction.ExecuteNonQuery(sqlStatements);
		}
		
		/// <summary>
		/// Indicates that the transaction should be commited.
		/// </summary>
		/// <remarks></remarks>
		public void Complete()
		{
			if (completed)
				throw new InvalidOperationException("Transaction has already been completed");
			
			transaction.Commit();
			completed = true;
		}
		
		/// <summary>
		/// Closes the connection if it has not already been closed / disposed.
		/// </summary>
		/// <remarks></remarks>
		public void Dispose()
		{
			if (!this.disposed && !completed)
			{
				transaction.Rollback();
				this.disposed = true;
			}
			
			GC.SuppressFinalize(this);
		}
	}
}
