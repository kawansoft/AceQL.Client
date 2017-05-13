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
        private AceQLIsolationLevel isolationLevel = AceQLIsolationLevel.Unspecified;


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
        internal AceQLTransaction(AceQLConnection connection, AceQLIsolationLevel isolationLevel) : this(connection)
        {
            this.isolationLevel = isolationLevel;
            string isolationLevelStr = GetTransactionIsolationAsString(isolationLevel);
        }

        /// <summary>
        /// Gets the transaction isolation as string.
        /// </summary>
        /// <param name="transactionIsolationLevel">The transaction isolation level.</param>
        /// <returns>String.</returns>
        internal static String GetTransactionIsolationAsString(AceQLIsolationLevel transactionIsolationLevel)
        {
            if (transactionIsolationLevel == AceQLIsolationLevel.Unspecified)
            {
                return "NONE";
            }
            else if (transactionIsolationLevel == AceQLIsolationLevel.ReadCommitted)
            {
                return "READ_COMMITTED";
            }
            else if (transactionIsolationLevel == AceQLIsolationLevel.ReadUncommitted)
            {
                return "READ_UNCOMMITTED";
            }
            else if (transactionIsolationLevel == AceQLIsolationLevel.RepeatableRead)
            {
                return "REPEATABLE_READ";
            }
            else if (transactionIsolationLevel == AceQLIsolationLevel.Serializable)
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
        public AceQLIsolationLevel IsolationLevel
        {
            get
            {
                return isolationLevel;
            }
        }


        /// <summary>
        /// Gets the AceQL connection.
        /// </summary>
        /// <value>The AceQL connection.</value>
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
        /// Disposes this instance. This code does nothing and is optional because calls to CommitAsync()/RollbackAsync() reset the server
        /// connection. Class implements IDisposable to ease code migration.
        /// </summary>
        public void Dispose()
        {
            
        }

    }
}
