
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
        /// Initializes a new instance of the <see cref="AceQLTransaction"/> class.
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
        /// Commits the remote database transaction. 
        /// <para/>Note that this call will put the remote connection in auto commit mode on after Commit.
        /// </summary>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task CommitAsync()
        {
            await aceQLHttpApi.CallApiNoResultAsync("set_auto_commit", "true").ConfigureAwait(false);
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// <para/>Note that this call will put the remote connection in auto commit mode on after Rollback.
        /// </summary>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task RollbackAsync()
        {
            await aceQLHttpApi.CallApiNoResultAsync("rollback", null).ConfigureAwait(false);
            await aceQLHttpApi.CallApiNoResultAsync("set_auto_commit", "true").ConfigureAwait(false);
        }

        /// <summary>
        /// Optional call, does nothing.
        /// The opened <see cref="AceQLTransaction"/> must be closed  by an <see cref="AceQLTransaction"/>.CommitAsync 
        /// or an <see cref="AceQLTransaction"/>.RollbackAsync.
        /// 
        /// <para/>Method is provided for consistency a SQL Server SqlTransaction is <see cref="IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            
        }

    }
}
