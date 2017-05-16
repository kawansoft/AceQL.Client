// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-21-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="AceQLTransaction.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

using AceQL.Client.Api.Http;
using System;
using System.Threading.Tasks;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Class <see cref="AceQLTransaction"/>. Allows to define a Transaction in order to execute remote commit or rollback.
    /// </summary>
    public class AceQLTransaction : IDisposable
    {
        /// <summary>
        /// The AceQL connection
        /// </summary>
        private AceQLConnection connection = null;

        /// <summary>
        /// The instance that does all http stuff
        /// </summary>
        private AceQLHttpApi aceQLHttpApi;

        /// <summary>
        /// The isolation level
        /// </summary>
        private IsolationLevel isolationLevel = IsolationLevel.Unspecified;


        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLTransaction" /> class.
        /// </summary>
        /// <param name="connection">The AceQL connection.</param>
        internal AceQLTransaction(AceQLConnection connection)
        {
            this.aceQLHttpApi = connection.aceQLHttpApi;
            this.connection = connection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLTransaction"/> class.
        /// </summary>
        /// <param name="connection">The AceQL connection.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        internal AceQLTransaction(AceQLConnection connection, IsolationLevel isolationLevel) : this(connection)
        {
            this.isolationLevel = isolationLevel;
            string isolationLevelStr = GetTransactionIsolationAsString(isolationLevel);
        }

        /// <summary>
        /// Gets the transaction isolation as string.
        /// </summary>
        /// <param name="transactionIsolationLevel">The transaction isolation level.</param>
        /// <returns>The transaction isolation as string.</returns>
        internal static String GetTransactionIsolationAsString(IsolationLevel transactionIsolationLevel)
        {
            if (transactionIsolationLevel == IsolationLevel.Unspecified)
            {
                return "NONE";
            }
            else if (transactionIsolationLevel == IsolationLevel.ReadCommitted)
            {
                return "READ_COMMITTED";
            }
            else if (transactionIsolationLevel == IsolationLevel.ReadUncommitted)
            {
                return "READ_UNCOMMITTED";
            }
            else if (transactionIsolationLevel == IsolationLevel.RepeatableRead)
            {
                return "REPEATABLE_READ";
            }
            else if (transactionIsolationLevel == IsolationLevel.Serializable)
            {
                return "SERIALIZABLE";
            }
            else {
                return "UNKNOWN";
            }
        }


        /// <summary>
        /// Specifies the isolation level for this transaction.
        /// </summary>
        /// <value>The isolation level.</value>
        public IsolationLevel IsolationLevel
        {
            get
            {
                return isolationLevel;
            }
        }


        /// <summary>
        /// Gets the connection to remote database.
        /// </summary>
        /// <value>the connection to remote database.</value>
        public AceQLConnection AceQLConnection
        {
            get
            {
                return connection;
            }
        }

        /// <summary>
        /// Commits the database transaction. 
        /// Note that this call will put the remote connection in auto commit mode on after commit.
        /// </summary>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task CommitAsync()
        {
            await aceQLHttpApi.CallApiNoResultAsync("set_auto_commit", "true").ConfigureAwait(false);
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// Note that this call will put the remote connection in auto commit mode on after rollback.
        /// </summary>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task RollbackAsync()
        {
            await aceQLHttpApi.CallApiNoResultAsync("rollback", null).ConfigureAwait(false);
            await aceQLHttpApi.CallApiNoResultAsync("set_auto_commit", "true").ConfigureAwait(false);
        }

        /// <summary>
        /// Disposes this instance. Call is optional as it does nothing. It is provided to ease code migration as SQL Server SqlTransaction is disposable.
        /// <see cref="AceQLTransaction"/>.CommitAsync and <see cref="AceQLTransaction"/>.RollbackAsync() reset the server auto commit mode to true.
        /// connection.
        /// </summary>
        public void Dispose()
        {
            
        }

    }
}
