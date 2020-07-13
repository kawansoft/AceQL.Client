/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2020,  KawanSoft SAS
 * (http://www.kawansoft.com). All rights reserved.                                
 *                                                                               
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */
﻿

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
        private readonly AceQLConnection connection;

        /// <summary>
        /// The instance that does all http stuff
        /// </summary>
        private readonly AceQLHttpApi aceQLHttpApi;

        /// <summary>
        /// The isolation level
        /// </summary>
        private readonly IsolationLevel isolationLevel = IsolationLevel.Unspecified;


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
        /// The opened <see cref="AceQLTransaction"/> must be closed  by an <see cref="AceQLTransaction.CommitAsync"/> 
        /// or an <see cref="AceQLTransaction.RollbackAsync"/>
        /// 
        /// <para/>Method is provided for consistency as a DbTransaction (as a SQL Server SqlTransaction) is <see cref="IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="v"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool v)
        {

        }
    }
}
