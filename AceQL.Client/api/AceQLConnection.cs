// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-21-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="AceQLConnection.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

using AceQL.Client.Api.File;
using AceQL.Client.Api.Http;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AceQL.Client.Api.Util;
using PCLStorage;
using System.Security;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Class <see cref="AceQLConnection"/>. Allows to create a database connection to the remote server.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class AceQLConnection : IDisposable
    {
        internal static bool DEBUG = false;

        internal AceQLHttpApi aceQLHttpApi = null;
        private bool connectionOpened = false;

        /// <summary>
        ///  Says if connection is closed
        /// </summary>
        private bool closeAsyncDone;

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLConnection"/> class.
        /// </summary>
        public AceQLConnection()
        {
            aceQLHttpApi = new AceQLHttpApi();
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
            IFolder folder = await rootFolder.CreateFolderAsync(Parms.ACEQL_PCL_FOLDER,
                CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
            return folder.Path;
        }


        /// <summary>
        /// Traces this instance.
        /// </summary>
        internal static async Task TraceAsync()
        {
            await AceQLHttpApi.TraceAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Traces the specified string.
        /// </summary>
        /// <param name="s">The string to trace</param>
        internal static async Task TraceAsync(String s)
        {
            await AceQLHttpApi.TraceAsync(s).ConfigureAwait(false);
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
        /// The cancellation token can be used to request that the operation be abandoned before the http request timeout.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Global var avoids to propagate cancellationToken as parameter to all methods... 
                aceQLHttpApi.SetCancellationToken(cancellationToken);
                await OpenAsync();
            }
            finally
            {
                aceQLHttpApi.ResetCancellationToken();
            }
        }

        /// <summary>
        /// Initializes a <see cref="AceQLTransaction"/>object. This will put the remote connection in auto commit mode off.
        /// The cancellation token can be used to request that the operation be abandoned before the http request timeout.
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
                return await BeginTransactionAsync();
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
        /// The cancellation token can be used to request that the operation be abandoned before the http request timeout.
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
                return await BeginTransactionAsync(isolationLevel);
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
        /// Closes the connection to the remote database and closes the http session. 
        /// This is the preferred method of closing any open connection.
        /// </summary>
        public async Task CloseAsync()
        {
            if (closeAsyncDone)
            {
                return;
            }

            await aceQLHttpApi.CallApiNoResultAsync("disconnect", null).ConfigureAwait(false);
            closeAsyncDone = true;
        }

        /// <summary>
        /// If connection has not been closed by an explicit call to <see cref="AceQLConnection"/>CloseAsync:
        /// calls silently <see cref="AceQLConnection"/>CloseAsync without reporting any Exception.
        /// As the call is not awaited, execution of the 
        /// current method continues before the call to is <see cref="AceQLConnection"/>CloseAsync completed.
        /// </summary>
        public void Dispose()
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
        /// Says if trace is on. If on, a trace is done on the file "trace.txt" in the path of value AceQL.Client.Api.GetAceQLLocalFolderAsync().
        /// </summary>
        /// <returns>true if trace is on, else false.</returns>
        public static bool IsTraceOn()
        {
            return AceQLHttpApi.IsTraceOn();
        }

        /// <summary>
        /// Sets the trace on/off. If on, a trace is done on the file "trace.txt" in the path of value AceQL.Client.Api.GetAceQLLocalFolderAsync().
        /// </summary>
        /// <param name="traceOn">If true, trace will be on; else trace will be off.</param>
        public static void SetTraceOn(bool traceOn)
        {
            AceQLHttpApi.SetTraceOn(traceOn);
        }


        /// <summary>
        /// Returns the progress indicator variable that will store Blob/Clob upload progress between 0 and 100.
        /// </summary>
        /// <returns>The progress indicator variable that will store Blob/Clob upload progress between 0 and 100.</returns>
        public ProgressIndicator GetProgressIndicator()
        {
            return aceQLHttpApi.GetProgressIndicator();
        }

        /// <summary>
        /// Sets the progress indicator variable that will store Blob/Clob upload progress between 0 and 100. Will be used by progress bars to show the progress.
        /// </summary>
        /// <param name="progressIndicator">The progress indicator variable that will store Blob/Clob upload progress between 0 and 100.</param>
        public void SetProgressIndicator(ProgressIndicator progressIndicator)
        {
            aceQLHttpApi.SetProgressIndicator(progressIndicator);
        }

        /// <summary>
        /// Returns the AceQL Client SDK current Version.
        /// </summary>
        /// <returns>the AceQL SDK current Version.</returns>
        public String GetClientVersion()
        {
            return AceQL.Client.Api.Util.Version.GetVersion();
        }

        /// <summary>
        /// Returns the remote AceQL HTTP Server Version.
        /// The cancellation token can be used to request that the operation be abandoned before the http request timeout.
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
                return await GetServerVersionAsync();
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
        /// Gets a value indicating whether [pretty printing] is on or off. Defaults to false.
        /// </summary>
        /// <value><c>true</c> if [pretty printing]; otherwise, <c>false</c>.</value>
        public bool PrettyPrinting
        {
            get
            {
                return aceQLHttpApi.PrettyPrinting;
            }

            set
            {
                aceQLHttpApi.PrettyPrinting = value;
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

        ///// <summary>
        ///// Creates a new object that is a copy of the current instance.
        ///// </summary>
        ///// <returns>A new object that is a copy of this instance.</returns>
        //public object Clone()
        //{
        //    return new AceQLConnection(ConnectionString);
        //}

        private void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }

    }

}
