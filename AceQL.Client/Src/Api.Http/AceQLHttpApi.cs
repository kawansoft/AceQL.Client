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


using AceQL.Client.Api.File;
using AceQL.Client.Api.Metadata;
using AceQL.Client.Api.Metadata.Dto;
using AceQL.Client.Api.Util;
using AceQL.Client.Src.Api.Http;
using AceQL.Client.Src.Api.Util;
using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Http
{

    /// <summary>
    /// Class <see cref="AceQLHttpApi"/>. Allows to create a Connection to the remote server
    /// </summary>
    internal class AceQLHttpApi
    {

        internal static readonly bool DEBUG;

        /// <summary>
        /// The server URL
        /// </summary>
        private String server ;

        private string username;
        private string sessionId ;

        /// <summary>
        /// The database
        /// </summary>
        private String database ;
        private char[] password ;

        /// <summary>
        /// The Proxy Uri, if we don't want 
        /// </summary>
        private string proxyUri ;

        /// <summary>
        /// The credentials
        /// </summary>
        private ICredentials proxyCredentials ;

        /// <summary>
        /// The timeout in milliseconds
        /// </summary>
        private int timeout;
        private bool enableDefaultSystemAuthentication;

        /// <summary>
        /// The pretty printing
        /// </summary>
        const bool prettyPrinting = true;
        /// <summary>
        /// The gzip result
        /// </summary>
        bool gzipResult = true;


        /// <summary>
        /// The URL
        /// </summary>
        private string url;

        private string connectionString;

        private AceQLProgressIndicator progressIndicator;
        private AceQLCredential credential ;
        private CancellationToken cancellationToken;
        private bool useCancellationToken;

        internal SimpleTracer simpleTracer = new SimpleTracer();

        // The HttpManager that contains the HtttClient to use
        private HttpManager httpManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLHttpApi"/> class.
        /// </summary>
        internal AceQLHttpApi()
        {
           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLHttpApi"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.
        /// </param>"
        /// <exception cref="System.ArgumentException">connectionString token does not contain a = separator: " + line</exception>
        internal AceQLHttpApi(String connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString is null!");
            }

            this.connectionString = connectionString;
        }

        internal AceQLHttpApi(string connectionString, AceQLCredential credential) : this(connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString is null!");
            }

            this.credential = credential ?? throw new ArgumentNullException("credential is null!");
        }


        /// <summary>
        /// Opens this instance.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if a required parameter extracted from connection string is missing.
        /// </exception>
        /// <exception cref="AceQLException"> if any other Exception occurs.
        /// </exception>
        internal async Task OpenAsync()
        {
            try
            {
                ConnectionStringDecoder connectionStringDecoder = new ConnectionStringDecoder();
                connectionStringDecoder.Decode(connectionString);
                this.server = connectionStringDecoder.Server;
                this.database = connectionStringDecoder.Database;
                this.username = connectionStringDecoder.Username;
                this.password = connectionStringDecoder.Password;
                this.sessionId = connectionStringDecoder.SessionId;
                this.proxyUri = connectionStringDecoder.ProxyUri;
                this.proxyCredentials = connectionStringDecoder.ProxyCredentials;
                this.timeout = connectionStringDecoder.Timeout;
                this.enableDefaultSystemAuthentication = connectionStringDecoder.EnableDefaultSystemAuthentication;

                if (connectionStringDecoder.EnableTrace)
                {
                    simpleTracer.SetTraceOn(true);
                }

                await simpleTracer.TraceAsync("connectionString: " + connectionString).ConfigureAwait(false);
                await simpleTracer.TraceAsync("DecodeConnectionString() done!").ConfigureAwait(false); ;
                
                if (credential != null)
                {
                    username = credential.Username;

                    if (credential.Password != null)
                    {
                        password = credential.Password;
                    }

                    if (credential.SessionId != null)
                    {
                        sessionId = credential.SessionId;
                    }
                }

                if (server == null)
                {
                    throw new ArgumentNullException("Server keyword not found in connection string.");
                }

                if (password == null && sessionId == null)
                {
                    throw new ArgumentNullException("Password keyword or SessionId keyword not found in connection string or AceQLCredential not set");
                }

                if (database == null)
                {
                    throw new ArgumentNullException("Database keyword not found in connection string.");
                }

                this.username = username ?? throw new ArgumentNullException("Username keyword not found in connection string or AceQLCredential not set.");

                // Create the HttpManager instance
                this.httpManager = new HttpManager(proxyUri, proxyCredentials, timeout, enableDefaultSystemAuthentication);
                this.httpManager.SetSimpleTracer(simpleTracer);

                UserLoginStore userLoginStore = new UserLoginStore(server, username,
                    database);

                if (sessionId != null)
                {
                    userLoginStore.SetSessionId(sessionId);
                }

                if (userLoginStore.IsAlreadyLogged())
                {
                    await simpleTracer.TraceAsync("Get a new connection with get_connection").ConfigureAwait(false);
                    sessionId = userLoginStore.GetSessionId();

                    String theUrl = server + "/session/" + sessionId + "/get_connection";
                    String result = await httpManager.CallWithGetAsync(theUrl).ConfigureAwait(false);

                    await simpleTracer.TraceAsync("result: " + result).ConfigureAwait(false);

                    ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result,
                        HttpStatusCode);

                    if (!resultAnalyzer.IsStatusOk())
                    {
                        throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                            resultAnalyzer.GetErrorId(),
                            resultAnalyzer.GetStackTrace(),
                            HttpStatusCode);
                    }

                    String connectionId = resultAnalyzer.GetValue("connection_id");
                    await simpleTracer.TraceAsync("Ok. New Connection created: " + connectionId).ConfigureAwait(false);

                    this.url = server + "/session/" + sessionId + "/connection/"
                        + connectionId + "/";
                }
                else
                {
                    String theUrl = server + "/database/" + database + "/username/" + username + "/login";
                    ConsoleEmul.WriteLine("theUrl: " + theUrl);

                    Dictionary<string, string> parametersMap = new Dictionary<string, string>
                    {
                        { "password", new String(password) },
                        { "client_version", VersionValues.VERSION}
                    };

                    await simpleTracer.TraceAsync("Before CallWithPostAsyncReturnString: " + theUrl);

                    String result = await httpManager.CallWithPostAsyncReturnString(new Uri(theUrl), parametersMap).ConfigureAwait(false);

                    ConsoleEmul.WriteLine("result: " + result);
                    await simpleTracer.TraceAsync("result: " + result).ConfigureAwait(false);

                    ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, HttpStatusCode);
                    if (!resultAnalyzer.IsStatusOk())
                    {
                        throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                            resultAnalyzer.GetErrorId(),
                            resultAnalyzer.GetStackTrace(),
                            HttpStatusCode);
                    }

                    String theSessionId = resultAnalyzer.GetValue("session_id");
                    String theConnectionId = resultAnalyzer.GetValue("connection_id");

                    this.url = server + "/session/" + theSessionId + "/connection/" + theConnectionId + "/";
                    userLoginStore.SetSessionId(theSessionId);
                    await simpleTracer.TraceAsync("OpenAsync url: " + this.url).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                await simpleTracer.TraceAsync(exception.ToString()).ConfigureAwait(false);

                if (exception.GetType() == typeof(AceQLException))
                {
                    throw;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, HttpStatusCode);
                }
            }

        }

        /// <summary>
        /// Gets a value indicating whether [gzip result] is on or off.
        /// </summary>
        /// <value><c>true</c> if [gzip result]; otherwise, <c>false</c>.</value>
        internal bool GzipResult
        {
            get
            {
                return gzipResult;
            }

            set
            {
                gzipResult = value;
            }
        }

        internal string Database
        {
            get
            {
                return database;
            }
        }

        /// <summary>
        /// The timeout in milliseconds
        /// </summary>
        internal int Timeout { get => timeout; }


        public AceQLCredential Credential
        {
            get
            {
                return credential;
            }

            set
            {
                credential = value;
            }
        }

        public string ConnectionString { get => connectionString; set => connectionString = value; }

        /// <summary>
        /// Says it use has passed a CancellationToken
        /// </summary>
        public bool UseCancellationToken { get => useCancellationToken; }

        /// <summary>
        /// Gets the HTTP status code of hte latsexecuted HTTP call
        /// </summary>
        /// <value>The HTTP status code.</value>
        public HttpStatusCode HttpStatusCode { get => httpManager.HttpStatusCode; }

        internal string GetDatabase()
        {
            return database;
        }

        internal string GetUsername()
        {
            return username;
        }

        internal string GetServer()
        {
            return server;
        }


        /// <summary>
        /// Calls the API no result.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandOption">The command option.</param>
        /// <exception cref="System.ArgumentNullException">commandName is null!</exception>
        /// <exception cref="AceQLException">
        /// HTTP_FAILURE" + " " + httpStatusDescription - 0
        /// or
        /// or
        /// 0
        /// </exception>
        internal async Task CallApiNoResultAsync(String commandName, String commandOption)
        {
            try
            {
                if (commandName == null)
                {
                    throw new ArgumentNullException("commandName is null!");
                }

                String result = await CallWithGetAsync(commandName, commandOption).ConfigureAwait(false);

                ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, httpManager.HttpStatusCode);
                if (!resultAnalyzer.IsStatusOk())
                {
                    throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                        resultAnalyzer.GetErrorId(),
                        resultAnalyzer.GetStackTrace(),
                        httpManager.HttpStatusCode);
                }

            }
            catch (Exception exception)
            {
                await simpleTracer.TraceAsync(exception.ToString()).ConfigureAwait(false);

                if (exception.GetType() == typeof(AceQLException))
                {
                    throw;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, httpManager.HttpStatusCode);
                }
            }
        }

        /// <summary>
        /// Calls the API with result.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandOption">The command option.</param>
        /// <exception cref="System.ArgumentNullException">commandName is null!</exception>
        /// <exception cref="AceQLException">
        /// HTTP_FAILURE" + " " + httpStatusDescription - 0
        /// or
        /// or
        /// 0
        /// </exception>
        internal async Task<string> CallApiWithResultAsync(String commandName, String commandOption)
        {
            try
            {
                if (commandName == null)
                {
                    throw new ArgumentNullException("commandName is null!");
                }

                String result = await CallWithGetAsync(commandName, commandOption).ConfigureAwait(false);

                ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, httpManager.HttpStatusCode);
                if (!resultAnalyzer.IsStatusOk())
                {
                    throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                        resultAnalyzer.GetErrorId(),
                        resultAnalyzer.GetStackTrace(),
                        httpManager.HttpStatusCode);
                }

                return resultAnalyzer.GetResult();

            }
            catch (Exception exception)
            {
                await simpleTracer.TraceAsync(exception.ToString()).ConfigureAwait(false);

                if (exception.GetType() == typeof(AceQLException))
                {
                    throw;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, httpManager.HttpStatusCode);
                }
            }
        }


        /// <summary>
        /// Calls the with get.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionParameter">The action parameter.</param>
        /// <returns>String.</returns>
        private async Task<string> CallWithGetAsync(String action, String actionParameter)
        {
            String urlWithaction = this.url + action;

            if (actionParameter != null && actionParameter.Length != 0)
            {
                urlWithaction += "/" + actionParameter;
            }

            return await httpManager.CallWithGetAsync(urlWithaction).ConfigureAwait(false);
        }

        internal async Task<Stream> ExecuteQueryAsync(string cmdText, AceQLParameterCollection Parameters, bool isStoredProcedure, bool isPreparedStatement, Dictionary<string, string> statementParameters)
        {
            String action = "execute_query";

            Dictionary<string, string> parametersMap = new Dictionary<string, string>
            {
                { "sql", cmdText },
                { "prepared_statement", isPreparedStatement.ToString() },
                { "stored_procedure", isStoredProcedure.ToString() },
                { "column_types", "true" }, // Force column_types, mandatory for C# AceQLDataReader
                { "gzip_result", gzipResult.ToString() },
                { "pretty_printing", prettyPrinting.ToString() }
            };

            if (statementParameters != null)
            {
                List<string> keyList = new List<string>(statementParameters.Keys);
                foreach (string key in keyList)
                {
                    parametersMap.Add(key, statementParameters[key]);
                }
            }

            Uri urlWithaction = new Uri(url + action);
            Stream input = await httpManager.CallWithPostAsync(urlWithaction, parametersMap).ConfigureAwait(false);
            return input;
        }

        internal async Task<int> ExecuteUpdateAsync(string sql, AceQLParameterCollection Parameters, bool isStoredProcedure, bool isPreparedStatement, Dictionary<string, string> statementParameters)
        {
            String action = "execute_update";

            // Call raw execute if non query/select stored procedure. (Dirty!! To be corrected.)
            if (isStoredProcedure)
            {
                action = "execute";
            }

            Dictionary<string, string> parametersMap = new Dictionary<string, string>
            {
                { "sql", sql },
                { "prepared_statement", isPreparedStatement.ToString() },
                { "stored_procedure", isStoredProcedure.ToString() }
            };

            if (statementParameters != null)
            {
                List<string> keyList = new List<string>(statementParameters.Keys);
                foreach (string key in keyList)
                {
                    parametersMap.Add(key, statementParameters[key]);
                }
            }

            Uri urlWithaction = new Uri(url + action);

            await simpleTracer.TraceAsync("url: " + url + action);

            foreach (KeyValuePair<String, String> p in parametersMap)
            {
                await simpleTracer.TraceAsync("parm: " + p.Key + " / " + p.Value);
            }

            string result = await CallWithPostAsyncReturnString(urlWithaction, parametersMap).ConfigureAwait(false);

            Debug("result: " + result);

            ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, httpManager.HttpStatusCode);
            if (!resultAnalyzer.IsStatusOk())
            {
                throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                    resultAnalyzer.GetErrorId(),
                    resultAnalyzer.GetStackTrace(),
                    httpManager.HttpStatusCode);
            }

            int rowCount = resultAnalyzer.GetIntvalue("row_count");

            if (isStoredProcedure)
            {
                UpdateOutParametersValues(result, Parameters);
            }

            return rowCount;

        }

        /// <summary>
        /// Update the OUT parameters values
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parameters"></param>
        private static void UpdateOutParametersValues(string result, AceQLParameterCollection parameters)
        {
            //1) Build outParametersDict Dict of (paramerer names, values)

            dynamic xj = JsonConvert.DeserializeObject(result);
            dynamic xjParametersOutPername = xj.parameters_out_per_name;

            if (xjParametersOutPername == null)
            {
                return;
            }

            String dictStr = xjParametersOutPername.ToString();
            Dictionary<string, string> outParametersDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(dictStr);

            //2) Scan  foreach AceQLParameterCollection parameters and modify value if parameter name is in outParametersDict
            foreach (AceQLParameter parameter in parameters)
            {
                string parameterName = parameter.ParameterName;

                if (outParametersDict.ContainsKey(parameterName))
                {
                    parameter.Value = outParametersDict[parameterName];
                }
            }
        }

        /// <summary>
        /// Executes a POST with parameters and returns a Srring
        /// </summary>
        /// <param name="theUrl">The Url.</param>
        /// <param name="parametersMap">The request parameters.</param>
        /// <returns>Stream.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// action is null!
        /// or
        /// postParameters is null!
        /// </exception>
        private async Task<string> CallWithPostAsyncReturnString(Uri theUrl, Dictionary<string, string> parametersMap)
        {
            String result = null;

            using (Stream input = await httpManager.CallWithPostAsync(theUrl, parametersMap).ConfigureAwait(false))
            {
                if (input != null)
                {
                    result = new StreamReader(input).ReadToEnd();
                }
            }

            return result;
        }


        /// <summary>
        /// Uploads a Blob/Clob on the server.
        /// </summary>
        /// <param name="blobId">the Blob/Clob Id</param>
        /// <param name="stream">the stream of the Blob/Clob</param>
        /// <param name="totalLength">the total length of all BLOBs to upload</param>
        /// <returns>The result as JSON format.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// blobId is null!
        /// or
        /// file is null!
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist: " + file</exception>
        /// <exception cref="AceQLException">HTTP_FAILURE" + " " + httpStatusDescription - 0</exception>
        internal async Task<String> BlobUploadAsync(String blobId, Stream stream, long totalLength)
        {

            if (blobId == null)
            {
                throw new ArgumentNullException("blobId is null!");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream is null!");
            }

            String theUrl = url + "blob_upload";

            FormUploadStream formUploadStream = new FormUploadStream();
            HttpResponseMessage response = null;

            response = await formUploadStream.UploadAsync(theUrl, proxyUri, proxyCredentials, timeout, enableDefaultSystemAuthentication,  blobId, stream,
                totalLength, progressIndicator, cancellationToken, useCancellationToken).ConfigureAwait(false);

            httpManager.HttpStatusCode = response.StatusCode;

            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            String result = null;
            if (streamResult != null)
            {
                result = new StreamReader(streamResult).ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// Returns the server Blob/Clob length.
        /// </summary>
        /// <param name="blobId">the Blob/Clob Id.</param>
        /// <returns>the server Blob/Clob length.</returns>
        internal async Task<long> GetBlobLengthAsync(String blobId)
        {
            if (blobId == null)
            {
                throw new ArgumentNullException("blobId is null!");
            }

            String action = "get_blob_length";

            Dictionary<string, string> parametersMap = new Dictionary<string, string>
            {
                { "blob_id", blobId }
            };
            String result = null;

            Uri urlWithaction = new Uri(url + action);
            using (Stream input = await httpManager.CallWithPostAsync(urlWithaction, parametersMap).ConfigureAwait(false))
            {

                if (input != null)
                {
                    result = new StreamReader(input).ReadToEnd();
                }
            }

            ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, HttpStatusCode);
            if (!resultAnalyzer.IsStatusOk())
            {
                throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                    resultAnalyzer.GetErrorId(),
                    resultAnalyzer.GetStackTrace(),
                    HttpStatusCode);
            }

            String lengthStr = resultAnalyzer.GetValue("length");
            long length = Convert.ToInt64(lengthStr);
            return length;
        }


        /// <summary>
        /// Downloads a Blob/Clob from the server.
        /// </summary>
        /// <param name="blobId">the Blob/Clob Id</param>
        ///
        /// <returns>the Blob input stream</returns>
        internal async Task<Stream> BlobDownloadAsync(String blobId)
        {
            if (blobId == null)
            {
                throw new ArgumentNullException("blobId is null!");
            }

            try
            {
                String theUrl = this.url + "/blob_download?blob_id=" + blobId;
                Stream input = await httpManager.CallWithGetReturnStreamAsync(theUrl);
                return input;
            }
            catch (Exception exception)
            {
                await simpleTracer.TraceAsync(exception.ToString()).ConfigureAwait(false);

                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, HttpStatusCode);
                }
            }
        }

        /// <summary>
        /// Databases the schema download.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        /// <exception cref="ArgumentNullException">format is null!</exception>
        /// <exception cref="AceQLException">0</exception>
        /// 
        internal async Task<Stream> DbSchemaDownloadAsync(String format, String tableName)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format is null!");
            }

            try
            {
                String theUrl = this.url + "/metadata_query/db_schema_download?format=" + format;

                if (tableName != null)
                {
                    tableName = tableName.ToLowerInvariant();
                    theUrl += "&table_name=" + tableName;
                }

                Stream input = await httpManager.CallWithGetReturnStreamAsync(theUrl);
                return input;
            }
            catch (Exception exception)
            {
                await simpleTracer.TraceAsync(exception.ToString()).ConfigureAwait(false);

                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, HttpStatusCode);
                }
            }
        }

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <returns>Task&lt;System.String&gt;.</returns>
        /// <exception cref="AceQLException">
        /// 0
        /// </exception>
        internal async Task<JdbcDatabaseMetaDataDto> GetDbMetadataAsync()
        {
            try
            {
                String commandName = "metadata_query/get_db_metadata";
                String result = await CallWithGetAsync(commandName, null).ConfigureAwait(false);

                ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, HttpStatusCode);
                if (!resultAnalyzer.IsStatusOk())
                {
                    throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                        resultAnalyzer.GetErrorId(),
                        resultAnalyzer.GetStackTrace(),
                        HttpStatusCode);
                }

                JdbcDatabaseMetaDataDto jdbcDatabaseMetaDataDto = JsonConvert.DeserializeObject<JdbcDatabaseMetaDataDto>(result); 
                return jdbcDatabaseMetaDataDto;
            }
            catch (Exception exception)
            {
                await simpleTracer.TraceAsync(exception.ToString()).ConfigureAwait(false);

                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, HttpStatusCode);
                }
            }
        }

        /// <summary>
        /// Gets the table names.
        /// </summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>Task&lt;TableNamesDto&gt;.</returns>
        /// <exception cref="AceQLException">
        /// 0
        /// </exception>
        internal async Task<TableNamesDto> GetTableNamesAsync(String tableType)
        {
            try
            {
                String action = "metadata_query/get_table_names";

                Dictionary<string, string> parametersMap = new Dictionary<string, string>();
                if (tableType != null)
                {
                    parametersMap.Add("table_type", tableType);
                }

                String result = null;

                Uri urlWithaction = new Uri(url + action);
                using (Stream input = await httpManager.CallWithPostAsync(urlWithaction, parametersMap).ConfigureAwait(false))
                {
                    if (input != null)
                    {
                        result = new StreamReader(input).ReadToEnd();
                    }
                }

                ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, HttpStatusCode);
                if (!resultAnalyzer.IsStatusOk())
                {
                    throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                        resultAnalyzer.GetErrorId(),
                        resultAnalyzer.GetStackTrace(),
                        HttpStatusCode);
                }

                TableNamesDto tableNamesDto = JsonConvert.DeserializeObject<TableNamesDto>(result);
                return tableNamesDto;
            }
            catch (Exception exception)
            {
                await simpleTracer.TraceAsync(exception.ToString()).ConfigureAwait(false);

                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, HttpStatusCode);
                }
            }
        }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Task&lt;TableDto&gt;.</returns>
        /// <exception cref="AceQLException">
        /// 0
        /// </exception>
        internal async Task<TableDto> GetTableAsync(String tableName)
        {
            try
            {
                String action = "metadata_query/get_table";

                Dictionary<string, string> parametersMap = new Dictionary<string, string>();
                parametersMap.Add("table_name", tableName);

                String result = null;

                Uri urlWithaction = new Uri(url + action);
                using (Stream input = await httpManager.CallWithPostAsync(urlWithaction, parametersMap).ConfigureAwait(false))
                {
                    if (input != null)
                    {
                        result = new StreamReader(input).ReadToEnd();
                    }
                }

                ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, HttpStatusCode);
                if (!resultAnalyzer.IsStatusOk())
                {
                    throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                        resultAnalyzer.GetErrorId(),
                        resultAnalyzer.GetStackTrace(),
                        HttpStatusCode);
                }

                TableDto tableDto = JsonConvert.DeserializeObject<TableDto>(result);
                return tableDto;
            }
            catch (Exception exception)
            {
                await simpleTracer.TraceAsync(exception.ToString()).ConfigureAwait(false);

                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, HttpStatusCode);
                }
            }
        }

        /// <summary>
        /// To be call at end of each of each public aysnc(CancellationToken) calls to reset to false the usage of a CancellationToken with http calls
        /// and some reader calls.
        /// </summary>
        internal void ResetCancellationToken()
        {
            this.useCancellationToken = false;
        }

        /// <summary>
        /// Sets the CancellationToken asked by user to pass for the current public xxxAsync() call api.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken asked by user to pass for the current public xxxAsync() call api.</param>
        internal void SetCancellationToken(CancellationToken cancellationToken)
        {
            this.useCancellationToken = true;
            this.cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Returns the progress indicator variable that will store Blob/Clob upload or download progress between 0 and 100.
        /// </summary>
        /// <returns>The progress indicator variable that will store Blob/Clob upload or download progress between 0 and 100.</returns>
        internal AceQLProgressIndicator GetProgressIndicator()
        {
            return progressIndicator;
        }


        /// <summary>
        /// Sets the progress indicator variable that will store Blob/Clob upload or download progress between 0 and 100. Will be used by progress indicators to show the progress.
        /// </summary>
        /// <param name="progressIndicator">The progress variable.</param>
        internal void SetProgressIndicator(AceQLProgressIndicator progressIndicator)
        {
            this.progressIndicator = progressIndicator;
        }

        /// <summary>
        /// Returns the SDK current Version.
        /// </summary>
        /// <returns>the SDK current Version.</returns>
        internal static String GetVersion()
        {
            return Util.Version.GetVersion();
        }


        /// <summary>
        /// Closes the connection to the remote database and closes the http session.
        /// </summary>
        public async Task CloseAsync()
        {
            await CallApiNoResultAsync("disconnect", null).ConfigureAwait(false);
        }

        internal static void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }
    }
}
