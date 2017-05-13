﻿// ***********************************************************************
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

        private string connectionString;
        private AceQLCredential credential;

        internal AceQLHttpApi aceQLHttpApi = null;
        private bool connectionOpened = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLConnection"/> class  when given a string that contains the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string. 
        /// Minimum content is: "Server = http://www.acme.com/aceql; Database = myDataBase; Username = myUsername; Password = myPassword"
        /// You may specify if session is stateless with Stateless = true. If not specified, session is stateful.
        /// Example:"Server = http://www.acme.com/aceql; Database = myDataBase; Username = myUsername; Password = myPassword; Stateless = true"
        /// 
        /// You may alos specify using NTLM with NTLM=true or specify the username and password of an authenticated proxy with ProxyUsernme and 
        /// ProxyPassword. Examples:
        /// 1) NLTM: "Server = http://www.acme.com/aceql; Database = myDataBase; Username = myUsername; Password = myPassword; NTLM=true"
        /// 2) Authenticated proxy : "Server = http://www.acme.com/aceql; Database = myDataBase; Username = myUsername; Password = myPassword; ProxyUsername=user1; ProxyPassword=pass1"
        /// if ProxyUri is specified, the value will be used instead of System.Net.WebRequest.DefaultWebProxy. Example:
        /// "Server = http://www.acme.com/aceql; Database = myDataBase; Username = myUsername; Password = myPassword; ProxyUri=http://localhost:8080 ProxyUsername=user1; ProxyPassword=pass1"
        /// Read/Write http timeout may be specified with Timeout in milliseconds:
        /// Server = http://www.acme.com/aceql; Database = myDataBase; Username = myUsername; Password = myPassword; Timeout=300000"
        /// If timeout is not specified or equals 0, <see cref="System.Net.Http.HttpClient"/> default value will be used.
        /// ";" char is supported in password, must be escaped: Password = my\;Password;
        /// </param>
        /// <exception cref="System.ArgumentNullException">If connectionString is null.</exception>
        /// <exception cref="System.ArgumentException">connectionString token does not contain a = separator: " + line</exception>
        public AceQLConnection(String connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString is null!");
            }

            aceQLHttpApi = new AceQLHttpApi(connectionString);
            this.connectionString = connectionString;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLConnection"/> class  when given a string that contains the connection string 
        /// and an <see cref="AceQLCredential"/> object that contains the username and password.
        /// </summary>
        /// <param name="connectionString">The connection string. 
        /// Minimum content is: "Server = http://www.acme.com/aceql; Database = myDataBase". 
        /// <see cref="AceQLConnection"/>.AceQLConnection(String connectionString) for all possible values.
        /// </param>
        /// <param name="credential"><see cref="AceQLCredential"/> object. </param>
        /// <exception cref="System.ArgumentNullException">If connectionString is null or <see cref="AceQLCredential"/> is null. </exception>
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
            this.connectionString = connectionString;
            this.credential = credential;
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
        /// Gets the connection string used to connect to the remote database.
        /// </summary>
        /// <value>The connection string used to connect to the remote database.</value>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
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
                if (aceQLHttpApi != null)
                {
                    return aceQLHttpApi.Database;
                }
                else
                {
                    return null;
                }
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
        /// before download. Defauts to true.
        /// </summary>
        /// <value>true if SQL result sets are returned compressed with the GZIP file format
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
        /// Gets the <see cref="AceQLCredential"/> object for this connection. 
        /// </summary>
        /// <value>The the <see cref="AceQLCredential"/> object for this connection.</value>
        public AceQLCredential Credential { get => credential;}

        /// <summary>
        /// Closes the connection to the remote database by calling <see cref="AceQLConnection"/>.Dispose().
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Closes the connection to the remote database.
        /// This is highly recommended in default stateful mode: it will call in async mode the "disconnect" HTTP API and release the remote Connection into the pool.
        /// </summary>
        public void Dispose()
        {
            TestConnectionOpened();
            aceQLHttpApi.Dispose();
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
        /// Returns the CancellationTokenSource used to cancel a BLOB upload.
        /// </summary>
        /// <returns>The CancellationTokenSource that will be used for BLOB upload.</returns>
        internal CancellationTokenSource GetCancellationTokenSource()
        {
            return aceQLHttpApi.GetCancellationTokenSource();
        }

        /// <summary>
        /// Sets the CancellationTokenSource that will be used for BLOB upload.
        /// </summary>
        /// <param name="cancellationTokenSource">The CancellationTokenSource that will be used for BLOB upload.</param>
        public void SetCancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            aceQLHttpApi.SetCancellationTokenSource(cancellationTokenSource);
        }

        /// <summary>
        /// Returns the sharable progress variable that will store Blob/Clob upload progress between 0 and 100.
        /// </summary>
        /// <returns>The sharable progress variable that will store Blob/Clob upload progress between 0 and 100.</returns>
        public ProgressIndicator GetProgressIndicator()
        {
            return aceQLHttpApi.GetProgressIndicator();
        }

        /// <summary>
        /// Sets the sharable progress variable that will store Blob/Clob upload progress between 0 and 100. Will be used by progress bars to show the progress.
        /// </summary>
        /// <param name="progressIndicator">The sharable progress variable that will store Blob/Clob upload progress between 0 and 100.</param>
        public void SetProgressIndicator(ProgressIndicator progressIndicator)
        {
            aceQLHttpApi.SetProgressIndicator(progressIndicator);
        }


        /// <summary>
        /// Returns the AceQL SDK current Version.
        /// </summary>
        /// <returns>the AceQL SDK current Version.</returns>
        public String GetClientVersion()
        {
            return AceQL.Client.Api.Util.Version.GetVersion();
        }

        /// <summary>
        /// Returns the remote AceQL Server Version.
        /// </summary>
        /// <returns>the remote  AceQL Server Version.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<string> GetServerVersionAsync()
        {
            TestConnectionOpened();
            String serverVersion = await aceQLHttpApi.CallApiAsync("get_version", null).ConfigureAwait(false);
            return serverVersion;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new AceQLConnection(connectionString);
        }

        private void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }

    }

}
