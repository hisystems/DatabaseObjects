// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//	    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// _________________________________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects
{
	/// <summary>
	/// Facilitates automatically opening and closing a connection.
	/// Implements IDisposable so that the construct can be used with a Using construct.
	/// </summary>
	public class ConnectionScope : IDisposable
	{
		private Database.ConnectionController connection;
		private bool disposed;
		
		/// <summary>
		/// Ensures that a new connection is opened.
		/// If a connection is already opened then the already open connection is utilised.
		/// </summary>
		public ConnectionScope(Database database)
			: this(GetConnection(database))
		{
		}
		
        /// <summary>
        /// Ensures that a new connection is opened.
        /// If a connection is already opened then the already open connection is utilised.
        /// </summary>
        internal ConnectionScope(Database.ConnectionController connection)
        {
			if (connection == null)
				throw new ArgumentNullException();

			this.connection = connection;

            #pragma warning disable 0618
			connection.Start();
            #pragma warning restore 0618
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
			return connection.ExecuteScalar(sqlStatement);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Executes the SQL statement and returns the result.
		/// The returned reader can only be used until the ConnectionScope is disposed
		/// (typically at the end of a using construct).
		/// </summary>
		/// --------------------------------------------------------------------------------
		public IDataReader Execute(SQL.ISQLStatement sqlStatement)
		{
			return connection.Execute(sqlStatement);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Executes the SQL statements and returns the first result.
		/// The returned reader(s) can only be used until the ConnectionScope is disposed
		/// (typically at the end of a using construct).
		/// </summary>
		/// --------------------------------------------------------------------------------
		public IDataReader Execute(SQL.ISQLStatement[] sqlStatements)
		{
			return connection.Execute(sqlStatements);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Executes the SQL statement and returns the number of rows affected.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public int ExecuteNonQuery(SQL.ISQLStatement sqlStatement)
		{
			return connection.ExecuteNonQuery(sqlStatement);
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Executes the SQL statements and returns the number of rows affected for all of the statements.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public int ExecuteNonQuery(SQL.ISQLStatement[] sqlStatements)
		{
			return connection.ExecuteNonQuery(sqlStatements);
		}
		
		/// <summary>
		/// Closes the connection if it has not already been closed / disposed.
		/// </summary>
		/// <remarks></remarks>
		public void Dispose()
		{
			if (!this.disposed)
			{
                #pragma warning disable 0618
				connection.Finished();
                #pragma warning restore 0618
				this.disposed = true;
			}
			
			GC.SuppressFinalize(this);
		}

        /// <summary>
        /// Called by constructor.
        /// </summary>
        private static Database.ConnectionController GetConnection(Database database)
        {
			if (database == null)
				throw new ArgumentNullException();

			return database.Connection;
		}
	}
}
