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


using AceQL.Client.Api.Http;
using AceQL.Client.Api.Metadata;
using AceQL.Client.Api.Util;
using AceQL.Client.Src.Api.Util;
using PCLStorage;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Class <see cref="AceQLConnection"/>. Allows to create a database connection to the remote server.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class AceQLConnection : IDisposable
    {

        internal AceQLHttpApi aceQLHttpApi;
        private bool connectionOpened;

        /// <summary>
        ///  Says if connection is closed
        /// </summary>
        private bool closeAsyncDone;

        /// <summary>
        ///  Says if user has done a logout/logoff
        /// </summary>
        private bool logoutAsyncDone;

        /// <summary>
        /// The default web proxy that the end user may set
        /// </summary>
        private static IWebProxy defaultWebProxy;

        /// <summary>
        /// The request retry/
        /// </summary>
        private bool requestRetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLConnection"/> class.
        /// </summary>
        public AceQLConnection()
        {
            aceQLHttpApi = new AceQLHttpApi();
        }

        /// <summary>
        /// Adds A request header. The header will be added at each server call.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        public void AddRequestHeader(string name, string value)
        {
            aceQLHttpApi.AddRequestHeader(name, value);
        }

        /// <summary>
        /// Resets the request headers. The previously added headers with <see cref="AddRequestHeader"/> will be suppressed.
        /// </summary>
        public void ResetRequestHeaders()
        {
            aceQLHttpApi.ResetRequestHeaders();
        }

        internal AceQLHttpApi GetAceQLHttpApi()
        {
            return aceQLHttpApi;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLConnection"/> class  when given a string that contains the connection string.
        /// </summary>
        /// <param name="connectionString">The connection used to open the remote database.</param>
        /// <exception cref="System.ArgumentNullException">If connectionString is null.</exception>
        public AceQLConnection(String connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString is null!");
            }

            aceQLHttpApi = new AceQLHttpApi(connectionString);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLConnection"/> class  when given a string that contains the connection string 
        /// and an <see cref="AceQLCredential"/> object that contains the username and password.
        /// </summary>
        /// <param name="connectionString">A connection string that does not use any of the following connection string keywords: Username
        /// or Password.</param>
        /// <param name="credential"><see cref="AceQLCredential"/> object. </param>
        /// <exception cref="System.ArgumentNullException">If connectionString is null or <see cref="AceQLCredential"/> is null.</exception>
        /// <exception cref="System.ArgumentException">connectionString token does not contain a = separator: " + line</exception>
        public AceQLConnection(string connectionString, AceQLCredential credential)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString is null!");
            }

            if (credential == null)
            {
                throw new ArgumentNullException("credential is null!");
            }

            aceQLHttpApi = new AceQLHttpApi(connectionString, credential);
        }

        /// <summary>
        /// Gets the path to the local AceQL folder where SQL queries results are stored.
        /// </summary>
        /// <returns>The path to the local AceQL folder.</returns>
        public static async Task<string> GetAceQLLocalFolderAsync()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(Parms.ACEQL_PCL_FOLDER, CreationCollisionOption.OpenIfExists);
            return folder.Path;
        }


        /// <summary>
        /// Creates a RemoteDatabaseMetaData in order to query remote database metadata info.
        /// </summary>
        /// <returns>RemoteDatabaseMetaData.</returns>
        public RemoteDatabaseMetaData GetRemoteDatabaseMetaData()
        {
            return new RemoteDatabaseMetaData(this);
        }

        /// <summary>
        /// Opens a connection with the remote database.
        /// </summary>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public async Task OpenAsync()
        {
            this.connectionOpened = true;
            await aceQLHttpApi.OpenAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Opens a connection with the remote database.
        /// <para/>The cancellation token can be used to can be used to request that the operation be abandoned before the http request timeout.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Global var avoids to propagate cancellationToken as parameter to all methods... 
                aceQLHttpApi.SetCancellationToken(cancellationToken);
                await OpenAsync().ConfigureAwait(false);
            }
            finally
            {
                aceQLHttpApi.ResetCancellationToken();
            }
        }

        /// <summary>
        /// Initializes a <see cref="AceQLTransaction"/>object. This will put the remote connection in auto commit mode off.
        /// <para/>The cancellation token can be used to can be used to request that the operation be abandoned before the http request timeout.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A new <see cref="AceQLTransaction"/> object.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<AceQLTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Global var avoids to propagate cancellationToken as parameter to all methods... 
                aceQLHttpApi.SetCancellationToken(cancellationToken);
                return await BeginTransactionAsync().ConfigureAwait(false);
            }
            finally
            {
                aceQLHttpApi.ResetCancellationToken();
            }
        }

        /// <summary>
        /// Initializes a <see cref="AceQLTransaction"/>object. This will put the remote connection in auto commit mode off.
        /// </summary>
        /// <returns>A new <see cref="AceQLTransaction"/> object.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<AceQLTransaction> BeginTransactionAsync()
        {
            TestConnectionOpened();
            await aceQLHttpApi.CallApiNoResultAsync("set_auto_commit", "false").ConfigureAwait(false);
            AceQLTransaction aceQLTransaction = new AceQLTransaction(this);
            return aceQLTransaction;
        }

        internal void TestConnectionOpened()
        {
            if (!connectionOpened)
            {
                throw new AceQLException("AceQlConnection has not been opened.", 0, (Exception)null, HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="AceQLTransaction"/>object with the specified isolation level.
        /// This will put the remote connection in auto commit mode off.
        /// <para/>The cancellation token can be used to can be used to request that the operation be abandoned before the http request timeout.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A new <see cref="AceQLTransaction"/> object.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<AceQLTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            try
            {
                // Global var avoids to propagate cancellationToken as parameter to all methods... 
                aceQLHttpApi.SetCancellationToken(cancellationToken);
                return await BeginTransactionAsync(isolationLevel).ConfigureAwait(false);
            }
            finally
            {
                aceQLHttpApi.ResetCancellationToken();
            }
        }
        /// <summary>
        /// Initializes a new <see cref="AceQLTransaction"/>object with the specified isolation level.
        /// This will put the remote connection in auto commit mode off.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>A new <see cref="AceQLTransaction"/> object.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<AceQLTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            TestConnectionOpened();
            await aceQLHttpApi.CallApiNoResultAsync("set_auto_commit", "false").ConfigureAwait(false);
            string isolationLevelStr = AceQLTransaction.GetTransactionIsolationAsString(isolationLevel);
            await aceQLHttpApi.CallApiNoResultAsync("set_transaction_isolation_level", isolationLevelStr).ConfigureAwait(false);
            AceQLTransaction aceQLTransaction = new AceQLTransaction(this, isolationLevel);
            return aceQLTransaction;
        }

        /// <summary>
        /// Closes the connection to the remote database. 
        /// This is the preferred method of closing any open connection.
        /// </summary>
        public async Task CloseAsync()
        {
            if (closeAsyncDone)
            {
                return;
            }

            await aceQLHttpApi.CallApiNoResultAsync("close", null).ConfigureAwait(false);

            if (aceQLHttpApi.httpManager != null)
            {
                aceQLHttpApi.httpManager.Dispose();
            }

            closeAsyncDone = true;
        }

        /// <summary>
        /// Closes the session to the remote AceQL server and Close on server all the connections to the database. 
        /// This is the preferred method of closing any open connection.
        /// </summary>
        public async Task LogoutAsync()
        {
            if (logoutAsyncDone)
            {
                return;
            }

            UserLoginStore loginStore = new UserLoginStore(this.aceQLHttpApi.GetServer(), this.aceQLHttpApi.GetUsername(), this.aceQLHttpApi.GetDatabase());
            loginStore.Remove();
            await aceQLHttpApi.CallApiNoResultAsync("logout", null).ConfigureAwait(false);

            if (aceQLHttpApi.httpManager != null)
            {
                aceQLHttpApi.httpManager.Dispose();
            }

            logoutAsyncDone = true;
        }

        /// <summary>
        /// If connection has not been closed by an explicit call to <see cref="AceQLConnection"/>CloseAsync:
        /// calls silently <see cref="AceQLConnection"/>CloseAsync without reporting any Exception.
        /// <para/>
        /// As the call is not awaited, execution of the 
        /// current method continues before the call to is <see cref="AceQLConnection"/>CloseAsync completed.
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            try
            {
                CloseAsync();
            }
            catch (Exception)
            {
                // Do nothing
            }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }

        /// <summary>
        /// Returns the progress indicator variable that will store Blob/Clob upload progress between 0 and 100.
        /// </summary>
        /// <returns>The progress indicator variable that will store Blob/Clob upload progress between 0 and 100.</returns>
        public AceQLProgressIndicator GetProgressIndicator()
        {
            return aceQLHttpApi.GetProgressIndicator();
        }

        /// <summary>
        /// Sets the progress indicator variable that will store Blob/Clob upload progress between 0 and 100.
        /// </summary>
        /// <param name="progressIndicator">The progress indicator variable that will store Blob/Clob upload progress between 0 and 100.</param>
        public void SetProgressIndicator(AceQLProgressIndicator progressIndicator)
        {
            aceQLHttpApi.SetProgressIndicator(progressIndicator);
        }

        /// <summary>
        /// Returns the AceQL Client SDK current Version.
        /// </summary>
        /// <returns>the AceQL SDK current Version.</returns>
        public static String GetClientVersion()
        {
            return AceQL.Client.Api.Util.Version.GetVersion();
        }

        /// <summary>
        /// Returns the remote AceQL HTTP Server Version.
        /// <para/>The cancellation token can be used to can be used to request that the operation be abandoned before the http request timeout.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>the remote  AceQL Server Version.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<string> GetServerVersionAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Global var avoids to propagate cancellationToken as parameter to all methods... 
                aceQLHttpApi.SetCancellationToken(cancellationToken);
                return await GetServerVersionAsync().ConfigureAwait(false);
            }
            finally
            {
                aceQLHttpApi.ResetCancellationToken();
            }
        }

        /// <summary>
        /// Returns the remote AceQL HTTP Server Version.
        /// </summary>
        /// <returns>the remote  AceQL Server Version.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<string> GetServerVersionAsync()
        {
            TestConnectionOpened();
            String serverVersion = await aceQLHttpApi.CallApiWithResultAsync("get_version", null).ConfigureAwait(false);
            return serverVersion;
        }

        /// <summary>
        /// Gets the default web proxy set. Returns null if none was previously set by <see cref="AceQLConnection.SetDefaultWebProxy"/>.
        /// </summary>
        /// <returns>The default web proxy set. Returns null if none was previously set by <see cref="AceQLConnection.SetDefaultWebProxy"/>.</returns>
        public static IWebProxy GetDefaultWebProxy()
        {
            return defaultWebProxy;
        }

        /// <summary>
        /// Sets the default web proxy to use. Allows to force to use on some environments (Windows, ...)  <c>System.Net.WebRequest.GetSystemProxy()</c> instead of <see cref="System.Net.WebRequest.DefaultWebProxy"/>.
        /// </summary>
        /// <param name="webProxy">The default web proxy that will be used.</param>
        public static void SetDefaultWebProxy(IWebProxy webProxy)
        {
            defaultWebProxy = webProxy;
        }

        /// <summary>
        /// Gets or sets the connection string used to connect to the remote database.
        /// </summary>
        /// <value>The connection string used to connect to the remote database.</value>
        public string ConnectionString
        {
            get
            {
                return aceQLHttpApi.ConnectionString;
            }

            set
            {
                aceQLHttpApi.ConnectionString = value;
            }
        }

        /// <summary>
        /// Gets the current database in use.
        /// </summary>
        /// <value>The current database in use.</value>
        public string Database
        {
            get
            {
                return aceQLHttpApi.Database;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether SQL result sets are returned compressed with the GZIP file format
        /// before download. Defaults to true.
        /// </summary>
        /// <value>True if SQL result sets are returned compressed with the GZIP file format
        /// before download.</value>
        public bool GzipResult
        {
            get
            {
                return aceQLHttpApi.GzipResult;
            }

            set
            {
                aceQLHttpApi.GzipResult = value;
            }

        }

        /// <summary>
        /// Gets the time to wait in milliseconds while trying to establish a connection before terminating the attempt and generating an error.
        /// If value is 0, <see cref="System.Net.Http.HttpClient"/> default will value be used.
        /// </summary>
        public int ConnectionTimeout
        {
            get
            {
                return aceQLHttpApi.Timeout;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="AceQLCredential"/> object for this connection. 
        /// </summary>
        /// <value>The <see cref="AceQLCredential"/> object for this connection.</value>
        public AceQLCredential Credential
        {
            get
            {
                return aceQLHttpApi.Credential;
            }

            set
            {
                aceQLHttpApi.Credential = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a failed request will be retried. The retry will be done once, without compression. Defaults to false.
        /// </summary>
        /// <value><c>true</c> if a failed request will be retried without compression.; otherwise, <c>false</c>.</value>
        public bool RequestRetry { get => requestRetry; set => requestRetry = value; }
    }

}
