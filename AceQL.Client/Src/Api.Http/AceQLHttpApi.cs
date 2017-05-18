/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2017,  KawanSoft SAS
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

using AceQL.Client.Api.File;
using AceQL.Client.Api.Util;
using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Http
{

    /// <summary>
    /// Class <see cref="AceQLHttpApi"/>. Allows to create a Connection to the remote server
    /// </summary>
    internal class AceQLHttpApi
    {
        private const string ESCAPED_SEMICOLON_WORD = "\\semicolon";
        private const string ESCAPED_SEMICOLON = "\\;";

        internal static bool DEBUG = false;

        /// <summary>
        /// The server URL
        /// </summary>
        private String server = null;

        /// <summary>
        /// The database
        /// </summary>
        private String database = null;

        /// <summary>
        /// Says if session is stateless. defaults to false.
        /// </summary>
        private bool stateless = false;

        /// <summary>
        /// The Proxy Uri, if we don't want 
        /// </summary>
        private string proxyUri = null;

        /// <summary>
        /// The credentials
        /// </summary>
        private ICredentials proxyCredentials = null;

        /// <summary>
        /// The timeout in milliseconds
        /// </summary>
        private int timeout = 0;

        /// <summary>
        /// The HTTP status code
        /// </summary>
        internal HttpStatusCode httpStatusCode;

        // Future usage
        //int connectTimeout = 0;

        /// <summary>
        /// The pretty printing
        /// </summary>
        bool prettyPrinting = false;
        /// <summary>
        /// The gzip result
        /// </summary>
        bool gzipResult = true;

        /// <summary>
        /// The trace on
        /// </summary>
        private static bool TRACE_ON = false;
        /// <summary>
        /// The URL
        /// </summary>
        private string url;

        private string connectionString;

        private ProgressIndicator progressIndicator;
        private AceQLCredential credential;
        private CancellationToken cancellationToken;
        private bool useCancellationToken = false;

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
                DecodeConnectionString();

                String username = null;
                String password = null;

                if (credential != null)
                {
                    username = credential.Username;
                    password = credential.Password;
                }

                if (server == null)
                {
                    throw new ArgumentNullException("Server keyword not found in connection string.");
                }
                if (username == null)
                {
                    throw new ArgumentNullException("Username keyword not found in connection string or AceQLCredential not set.");
                }
                if (password == null)
                {
                    throw new ArgumentNullException("Password keyword not found in connection string or AceQLCredential not set");
                }
                if (database == null)
                {
                    throw new ArgumentNullException("Database keyword not found in connection string.");
                }

                String theUrl = server + "/database/" + database + "/username/" + username + "/connect"
                    + "?password=" + password + "&stateless=" + stateless;
                ConsoleEmul.WriteLine("theUrl: " + theUrl);

                String result = await CallWithGetAsync(theUrl).ConfigureAwait(false);
                ConsoleEmul.WriteLine("result: " + result);

                ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, httpStatusCode);
                if (!resultAnalyzer.IsStatusOk())
                {
                    throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                        resultAnalyzer.GetErrorId(),
                        resultAnalyzer.GetStackTrace(),
                        httpStatusCode);
                }

                String theSessionId = resultAnalyzer.GetValue("session_id");

                this.url = server + "/session/" + theSessionId + "/";
                await TraceAsync("OpenAsync url: " + this.url).ConfigureAwait(false);

            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, httpStatusCode);
                }
            }

        }


        /// <summary>
        /// Traces this instance.
        /// </summary>
        internal static async Task TraceAsync()
        {
            await TraceAsync("").ConfigureAwait(false);
        }

        /// <summary>
        /// Traces the specified string.
        /// </summary>
        /// <param name="contents">The string to trace</param>
        internal static async Task TraceAsync(String contents)
        {
            if (TRACE_ON)
            {
                IFile file = await AceQLCommandUtil.GetTraceFileAsync().ConfigureAwait(false);
                await PortableFile.AppendAllTextAsync(file, "\r\n" + contents).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [pretty printing] is on or off.
        /// </summary>
        /// <value><c>true</c> if [pretty printing]; otherwise, <c>false</c>.</value>
        internal bool PrettyPrinting
        {
            get
            {
                return prettyPrinting;
            }

            set
            {
                prettyPrinting = value;
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
        /// Decode connection string and load elements in memory.
        /// </summary>
        private void DecodeConnectionString()
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString has not been set and is null!");
            }

            // Replace escaped "\;"
            connectionString = connectionString.Replace(ESCAPED_SEMICOLON, ESCAPED_SEMICOLON_WORD);

            String theServer = null;
            String theDatabase = null;
            String theUsername = null;
            String thePassword = null;
            bool theStateless = false;
            String theProxyUri = null;
            ICredentials theProxyCredentials = null;

            bool isNTLM = false;
            String proxyUsername = null;
            String proxyPassword = null;

            int theTimeout = 0;

            string[] lines = connectionString.Split(';');
            foreach (string line in lines)
            {
                // If some empty ;
                if (line.Trim().Length <= 2)
                {
                    continue;
                }

                string[] theLines = line.Split('=');

                if (theLines.Length != 2)
                {
                    throw new ArgumentException("connectionString token does not contain a = separator: " + line);
                }

                String property = theLines[0].Trim();
                String value = theLines[1].Trim();

                if (property.ToLower().Equals("server"))
                {
                    theServer = value;
                }
                else if (property.ToLower().Equals("database"))
                {
                    theDatabase = value;
                }
                else if (property.ToLower().Equals("username"))
                {
                    value = value.Replace("\\semicolon", ";");
                    theUsername = value;
                }
                else if (property.ToLower().Equals("password"))
                {
                    value = value.Replace("\\semicolon", ";");
                    thePassword = value;
                }
                else if (property.ToLower().Equals("stateless"))
                {
                    theStateless = Boolean.Parse(value);
                }
                else if (property.ToLower().Equals("ntlm"))
                {
                    isNTLM = Boolean.Parse(value);
                }
                else if (property.ToLower().Equals("proxyuri"))
                {

                    theProxyUri = value;
                    // Set to null a "null" string
                    if (theProxyUri.ToLower().Equals("null") || theProxyUri.Length == 0)
                    {
                        theProxyUri = null;
                    }
                    ConsoleEmul.WriteLine("theProxyUri:" + theProxyUri + ":");
                }
                else if (property.ToLower().Equals("proxyusername"))
                {
                    value = value.Replace(ESCAPED_SEMICOLON_WORD, ";");
                    proxyUsername = value;

                    // Set to null a "null" string
                    if (proxyUsername.ToLower().Equals("null") || proxyUsername.Length == 0)
                    {
                        proxyUsername = null;
                    }

                }
                else if (property.ToLower().Equals("proxypassword"))
                {
                    value = value.Replace("\\semicolon", ";");
                    proxyPassword = value;

                    // Set to null a "null" string
                    if (proxyPassword.ToLower().Equals("null") || proxyPassword.Length == 0)
                    {
                        proxyPassword = null;
                    }
                }
                else if (property.ToLower().Equals("timeout"))
                {
                    theTimeout = Int32.Parse(value);
                }
            }

            Debug("connectionString   : " + connectionString);
            Debug("theProxyUri        : " + theProxyUri);
            Debug("theProxyCredentials: " + proxyUsername + " / " + proxyPassword);

            // username & password maybe set by Credential
            if (credential != null)
            {
                theUsername = credential.Username;
                thePassword = credential.Password;
            }

            if (isNTLM)
            {
                theProxyCredentials = CredentialCache.DefaultCredentials;
            }
            else
            {
                if (proxyUsername != null && proxyPassword != null)
                {
                    theProxyCredentials = new NetworkCredential(proxyUsername, proxyPassword);
                }
            }

            Init(theServer, theDatabase, theUsername, thePassword, theStateless, theProxyUri, theProxyCredentials, theTimeout);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLConnection"/> class.
        /// </summary>
        /// <param name="server">The server URL.</param>
        /// <param name="database">The database.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="stateless">The stateless.</param>
        /// <param name="proxyUri">The Proxy Uri.</param>
        /// <param name="proxyCredentials">The credentials.</param>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="System.ArgumentNullException">
        /// server is null!
        /// or
        /// username is null!
        /// or
        /// password is null!
        /// or
        /// database is null!
        /// </exception>

        private void Init(string server, string database, string username, string password, bool stateless, string proxyUri, ICredentials proxyCredentials, int timeout)
        {
            this.server = server;
            this.database = database;

            if (username != null && password != null && credential == null)
            {
                this.credential = new AceQLCredential(username, password);
            }

            this.stateless = stateless;
            this.proxyUri = proxyUri;
            this.proxyCredentials = proxyCredentials;
            this.timeout = timeout;
        }



        /// <summary>
        /// Build an HttpClient instance with proxy settings, if necessary. Proxy used is System.Net.WebRequest.DefaultWebProxy
        /// </summary>
        /// <param name="proxyUri"></param>
        /// <param name="credentials">The credentials to use for an authenticated proxy. null if none.</param>
        /// <returns>The HtpClientHandler.</returns>
        internal static HttpClientHandler BuildHttpClientHandler(string proxyUri, ICredentials credentials)
        {
            Proxy proxy = null;
            // Used to test if have Proxy defined in IE
            String proxyUriToUse = null;

            // Test if used the default Web Proxy or the one passed in connection string:
            if (proxyUri == null)
            {
                proxyUriToUse = System.Net.WebRequest.DefaultWebProxy.GetProxy(new Uri("http://www.google.com")).ToString();
            }
            else
            {
                proxy = new Proxy(proxyUri);
                proxyUriToUse = proxy.GetProxy(new Uri("http://www.google.com")).ToString();
            }

            Debug("uriProxy: " + proxyUriToUse);

            if (credentials != null && credentials.GetType() == typeof(NetworkCredential))
            {
                Debug("credentials: " + ((NetworkCredential)credentials).UserName + "/" + ((NetworkCredential)credentials).Password);
            }

            if (proxyUriToUse.Contains("http://www.google.com"))
            {
                Debug("System.Net.WebRequest.DefaultWebProxy is default");
                HttpClientHandler handler = new HttpClientHandler();
                return handler;
            }
            else
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler()
                {
                    UseProxy = true,
                    UseDefaultCredentials = false
                };

                if (proxy == null)
                {
                    httpClientHandler.Proxy = System.Net.WebRequest.DefaultWebProxy;
                }
                else
                {
                    httpClientHandler.Proxy = proxy;
                }

                httpClientHandler.Proxy.Credentials = credentials;
                httpClientHandler.PreAuthenticate = true;
                return httpClientHandler;
            }

        }

        /// <summary>
        /// Calls the with get return stream.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Stream.</returns>
        private async Task<Stream> CallWithGetReturnStreamAsync(String url)
        {
            HttpClient httpClient = new HttpClient(BuildHttpClientHandler(proxyUri, proxyCredentials));

            if (timeout != 0)
            {
                long nanoseconds = 1000000 * timeout;
                httpClient.Timeout = new TimeSpan(nanoseconds / 100);
            }

            HttpResponseMessage response = null;

            if (!UseCancellationToken)
            {
                response = await httpClient.GetAsync(url).ConfigureAwait(false);
            }
            else
            {
                response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            }

            this.httpStatusCode = response.StatusCode;

            HttpContent content = response.Content;

            return await content.ReadAsStreamAsync().ConfigureAwait(false);

        }


        /// <summary>
        /// Executes a POST with parameters.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="parameters">The request parameters.</param>
        /// <returns>Stream.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// action is null!
        /// or
        /// postParameters is null!
        /// </exception>
        private async Task<Stream> CallWithPostAsync(String action, Dictionary<string, string> parameters)
        {

            if (action == null)
            {
                throw new ArgumentNullException("action is null!");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("postParameters is null!");
            }

            String urlWithaction = url + action;

            HttpClient httpClient = new HttpClient(BuildHttpClientHandler(proxyUri, proxyCredentials));

            if (timeout != 0)
            {
                long nanoseconds = 1000000 * timeout;
                httpClient.Timeout = new TimeSpan(nanoseconds / 100);
            }

            MultipartFormDataContent formData = new MultipartFormDataContent();

            // This is the postdata
            var postData = new List<KeyValuePair<string, string>>();

            await TraceAsync().ConfigureAwait(false);
            await TraceAsync("----------------------------------------").ConfigureAwait(false);
            await TraceAsync(url).ConfigureAwait(false);

            foreach (var param in parameters)
            {
                postData.Add(new KeyValuePair<string, string>(param.Key, param.Value));
                await TraceAsync("param: " + param.Key + "/" + param.Value);
            }
            await TraceAsync("----------------------------------------").ConfigureAwait(false);

            HttpContent content = new FormUrlEncodedContent(postData);

            HttpResponseMessage response = null;

            if (!UseCancellationToken)
            {
                response = await httpClient.PostAsync(urlWithaction, content);
            }
            else
            {
                response = await httpClient.PostAsync(urlWithaction, content, cancellationToken);
            }

            this.httpStatusCode = response.StatusCode;
            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

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

                ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, httpStatusCode);
                if (!resultAnalyzer.IsStatusOk())
                {
                    throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                        resultAnalyzer.GetErrorId(),
                        resultAnalyzer.GetStackTrace(),
                        httpStatusCode);
                }

            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, httpStatusCode);
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

                ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, httpStatusCode);
                if (!resultAnalyzer.IsStatusOk())
                {
                    throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                        resultAnalyzer.GetErrorId(),
                        resultAnalyzer.GetStackTrace(),
                        httpStatusCode);
                }

                return resultAnalyzer.GetResult();

            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, httpStatusCode);
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

            return await CallWithGetAsync(urlWithaction).ConfigureAwait(false);

        }

        /// <summary>
        /// Calls the with get.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>String.</returns>
        /// <exception cref="System.ArgumentNullException">url is null!</exception>
        private async Task<string> CallWithGetAsync(String url)
        {

            if (url == null)
            {
                throw new ArgumentNullException("url is null!");
            }

            using (Stream stream = await CallWithGetReturnStreamAsync(url).ConfigureAwait(false))
            {
                if (stream == null)
                {
                    return null;
                }

                var responseString = new StreamReader(stream).ReadToEnd();

                await TraceAsync().ConfigureAwait(false);
                await TraceAsync("----------------------------------------").ConfigureAwait(false);
                await TraceAsync(url).ConfigureAwait(false);
                await TraceAsync(responseString).ConfigureAwait(false);
                await TraceAsync("----------------------------------------").ConfigureAwait(false);

                return responseString;
            }

        }


        internal async Task<Stream> ExecuteQueryAsync(string cmdText, bool isPreparedStatement, Dictionary<string, string> statementParameters)
        {
            String action = "execute_query";

            Dictionary<string, string> parametersMap = new Dictionary<string, string>
            {
                { "sql", cmdText },
                { "prepared_statement", isPreparedStatement.ToString() },
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

            Stream input = await CallWithPostAsync(action, parametersMap).ConfigureAwait(false);
            return input;
        }


        internal async Task<int> ExecuteUpdateAsync(string sql, bool isPreparedStatement, Dictionary<string, string> statementParameters)
        {
            String action = "execute_update";

            Dictionary<string, string> parametersMap = new Dictionary<string, string>
            {
                { "sql", sql },
                { "prepared_statement", isPreparedStatement.ToString() }
            };
            if (statementParameters != null)
            {
                List<string> keyList = new List<string>(statementParameters.Keys);
                foreach (string key in keyList)
                {
                    parametersMap.Add(key, statementParameters[key]);
                }
            }

            using (Stream input = await CallWithPostAsync(action, parametersMap).ConfigureAwait(false))
            {

                String result = null;

                if (input != null)
                {
                    result = new StreamReader(input).ReadToEnd();
                }

                ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, httpStatusCode);
                if (!resultAnalyzer.IsStatusOk())
                {
                    throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                        resultAnalyzer.GetErrorId(),
                        resultAnalyzer.GetStackTrace(),
                        httpStatusCode);
                }

                int rowCount = resultAnalyzer.GetIntvalue("row_count");
                return rowCount;

            }


        }


        /// <summary>
        /// Uploads a Blob/Clob on the server.
        /// </summary>
        /// <param name="blobId">the Blob/Clob Id</param>
        /// <param name="stream">the stream of the Blo/Clob</param>
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

            response = await formUploadStream.UploadAsync(theUrl, proxyUri, proxyCredentials, timeout, blobId, stream,
                totalLength, progressIndicator, cancellationToken, useCancellationToken).ConfigureAwait(false);

            this.httpStatusCode = response.StatusCode;

            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            HttpStatusCode httpStatusCode = response.StatusCode;

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

            using (Stream input = await CallWithPostAsync(action, parametersMap).ConfigureAwait(false))
            {
                if (input != null)
                {
                    result = new StreamReader(input).ReadToEnd();
                }

            }

            ResultAnalyzer resultAnalyzer = new ResultAnalyzer(result, httpStatusCode);
            if (!resultAnalyzer.IsStatusOk())
            {
                throw new AceQLException(resultAnalyzer.GetErrorMessage(),
                    resultAnalyzer.GetErrorId(),
                    resultAnalyzer.GetStackTrace(),
                    httpStatusCode);
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
                String action = "blob_download";

                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "blob_id", blobId }
                };
                Stream input = await CallWithPostAsync(action, parameters).ConfigureAwait(false);
                return input;
            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, httpStatusCode);
                }
            }
        }

        /// <summary>
        /// Says if trace is on
        /// </summary>
        /// <returns>true if trace is on</returns>
        internal static bool IsTraceOn()
        {
            return TRACE_ON;
        }

        /// <summary>
        /// Sets the trace on/off
        /// </summary>
        /// <param name="traceOn">if true, trace will be on; else race will be off</param>
        internal static void SetTraceOn(bool traceOn)
        {
            TRACE_ON = traceOn;
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
        /// <param name="cancellationToken">ancellationToken asked by user to pass for the current public xxxAsync() call api.</param>
        internal void SetCancellationToken(CancellationToken cancellationToken)
        {
            this.useCancellationToken = true;
            this.cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Returns the sharable progress variable that will store Blob/Clob upload or download progress between 0 and 100.
        /// </summary>
        /// <returns>The sharable progress variable that will store Blob/Clob upload or download progress between 0 and 100.</returns>
        internal ProgressIndicator GetProgressIndicator()
        {
            return progressIndicator;
        }


        /// <summary>
        /// Sets the sharable progress variable that will store Blob/Clob upload or download progress between 0 and 100. Will be used by progress indicators to show the progress.
        /// </summary>
        /// <param name="progressIndicator">The sharable progress variable.</param>
        internal void SetProgressIndicator(ProgressIndicator progressIndicator)
        {
            this.progressIndicator = progressIndicator;
        }

        /// <summary>
        /// Returns the SDK current Version.
        /// </summary>
        /// <returns>the SDK current Version.</returns>
        internal String GetVersion()
        {
            return Util.Version.GetVersion();
        }

        ///// <summary>
        ///// Creates a new object that is a copy of the current instance.
        ///// </summary>
        ///// <returns>A new object that is a copy of this instance.</returns>
        //internal object Clone()
        //{
        //    return new AceQLHttpApi();
        //}


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
