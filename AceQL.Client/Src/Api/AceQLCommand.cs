/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2018,  KawanSoft SAS
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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AceQL.Client.Api.Util;
using PCLStorage;
using System.Threading;
using AceQL.Client.Src.Api;

/// <summary>
/// The AceQL.Client.Api namespace allows mobile and desktop application developers
/// to access remote SQL databases and/or SQL databases in the cloud by simply including
/// standard SQL calls in their code. The syntax is identical to the Microsoft SQL Server C# API.
/// </summary>
namespace AceQL.Client.Api
{
    /// <summary>
    /// Represents a SQL statement to execute against a remote SQL database.
    /// </summary>
    public class AceQLCommand : IDisposable
    {
        /// <summary>
        /// The instance that does all http stuff
        /// </summary>
        private AceQLHttpApi aceQLHttpApi;

        /// <summary>
        /// The text of the query.
        /// </summary>
        private string cmdText;
        /// <summary>
        /// The AceQL connection
        /// </summary>
        private AceQLConnection connection;

        /// <summary>
        /// The associated AceQLTransaction. 
        /// </summary>
        private AceQLTransaction transaction;


        /// <summary>
        /// The parameters
        /// </summary>
        private AceQLParameterCollection parameters = null;

        private bool prepare;

        private CommandType commandType = CommandType.Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLCommand"/> class.
        /// </summary>
        public AceQLCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLCommand"/> class with the text of the query.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <exception cref="System.ArgumentNullException">If cmdText is null.
        /// </exception>
        public AceQLCommand(string cmdText)
        {
            this.cmdText = cmdText ?? throw new ArgumentNullException("cmdText is null!");
            parameters = new AceQLParameterCollection(cmdText);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLCommand"/> class with the text of the query 
        /// and a <see cref="AceQLConnection"/>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="AceQLConnection"/> that represents the connection to a remote database.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If cmdText is null
        /// or
        /// connection is null.
        /// </exception>
        public AceQLCommand(string cmdText, AceQLConnection connection) : this(cmdText)
        {

            if (connection == null)
            {
                throw new ArgumentNullException("connection is null!");
            }

            connection.TestConnectionOpened();

            this.connection = connection;
            this.aceQLHttpApi = connection.aceQLHttpApi;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLCommand"/> class with the text of the query and a 
        /// <see cref="AceQLConnection"/>, and the <see cref="AceQLTransaction"/>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="AceQLConnection"/> that represents the connection to a remote database.</param>
        /// <param name="transaction">A <see cref="AceQLTransaction"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If cmdText is null or connection or transaction is null.
        /// </exception>
        public AceQLCommand(string cmdText, AceQLConnection connection, AceQLTransaction transaction) : this(cmdText, connection)
        {
            this.transaction = transaction ?? throw new ArgumentNullException("transaction is null!");
        }

        /// <summary>
        /// Sends the <see cref="AceQLCommand"/>.CommandText to the <see cref="AceQLConnection"/> and builds an <see cref="AceQLDataReader"/>.
        /// <para/>The cancellation token can be used to can be used to request that the operation be abandoned before the http request timeout.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>An <see cref="AceQLDataReader"/>object.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<AceQLDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Global var avoids to propagate cancellationToken as parameter to all methods... 
                aceQLHttpApi.SetCancellationToken(cancellationToken);
                return await ExecuteReaderAsync();
            }
            finally
            {
                aceQLHttpApi.ResetCancellationToken();
            }
        }

        /// <summary>
        ///  Sends the <see cref="AceQLCommand"/>.CommandText to the <see cref="AceQLConnection"/> and builds an <see cref="AceQLDataReader"/>.
        /// </summary>
        /// <returns>An <see cref="AceQLDataReader"/>object.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<AceQLDataReader> ExecuteReaderAsync()
        {
            if (cmdText == null)
            {
                throw new ArgumentNullException("cmdText is null!");
            }

            if (connection == null)
            {
                throw new ArgumentNullException("connection is null!");
            }

            // Statement wit parameters are always prepared statement
            if (Parameters.Count == 0 && !prepare)
            {
                return await ExecuteQueryAsStatementAsync().ConfigureAwait(false);
            }
            else
            {
                return await ExecuteQueryAsPreparedStatementAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates a prepared version of the command. Optional call.
        /// Note that the remote statement will always be a prepared statement if
        /// the command contains parameters.
        /// </summary>
        public void Prepare()
        {
            this.prepare = true;
        }

        /// <summary>
        /// Executes the SQL statement against the connection and returns the number of rows affected.
        /// <para/>The cancellation token can be used to can be used to request that the operation be abandoned before the http request timeout.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Global var avoids to propagate cancellationToken as parameter to all methods... 
                aceQLHttpApi.SetCancellationToken(cancellationToken);
                return await ExecuteNonQueryAsync();
            }
            finally
            {
                aceQLHttpApi.ResetCancellationToken();
            }
        }
        /// <summary>
        /// Executes the SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<int> ExecuteNonQueryAsync()
        {
            if (cmdText == null)
            {
                throw new ArgumentNullException("cmdText is null!");
            }

            if (connection == null)
            {
                throw new ArgumentNullException("connection is null!");
            }

            // Statement with parameters are always prepared statement
            if (Parameters.Count == 0 && !prepare)
            {
                return await ExecuteUpdateAsStatementAsync().ConfigureAwait(false);
            }
            else
            {
                return await ExecuteUpdateAsPreparedStatementAsync().ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Executes the query as statement.
        /// <para/>The cancellation token can be used to can be used to request that the operation be abandoned before the http request timeout.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>An <see cref="AceQLDataReader"/>object.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        private async Task<AceQLDataReader> ExecuteQueryAsStatementAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Global var avoids to propagate cancellationToken as parameter to all methods... 
                aceQLHttpApi.SetCancellationToken(cancellationToken);
                return await ExecuteQueryAsStatementAsync();
            }
            finally
            {
                aceQLHttpApi.ResetCancellationToken();
            }
        }

        /// <summary>
        /// Executes the query as statement.
        /// </summary>
        /// <returns>An <see cref="AceQLDataReader"/>object.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        private async Task<AceQLDataReader> ExecuteQueryAsStatementAsync()
        {
            try
            {
                IFile file = await GetUniqueResultSetFileAsync().ConfigureAwait(false);

                bool isStoredProcedure = (commandType == CommandType.StoredProcedure ? true : false);
                Boolean isPreparedStatement = false;
                Dictionary<string, string> parametersMap = null;

                using (Stream input = await aceQLHttpApi.ExecuteQueryAsync(cmdText, Parameters, isStoredProcedure, isPreparedStatement, parametersMap).ConfigureAwait(false))
                {
                    if (aceQLHttpApi.GzipResult)
                    {
                        using (GZipStream decompressionStream = new GZipStream(input, CompressionMode.Decompress))
                        {
                            using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false))
                            {
                                decompressionStream.CopyTo(stream);
                            }
                        }
                    }
                    else
                    {
                        using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false))
                        {
                            input.CopyTo(stream);
                        }
                    }
                }

                StreamResultAnalyzer streamResultAnalyzer = new StreamResultAnalyzer(file, aceQLHttpApi.httpStatusCode);
                if (!await streamResultAnalyzer.IsStatusOkAsync().ConfigureAwait(false))
                {
                    throw new AceQLException(streamResultAnalyzer.GetErrorMessage(),
                        streamResultAnalyzer.GetErrorType(),
                        streamResultAnalyzer.GetStackTrace(),
                        aceQLHttpApi.httpStatusCode);
                }

                int rowsCount = 0;

                using (Stream readStreamCout = await file.OpenAsync(FileAccess.Read).ConfigureAwait(false))
                {
                    RowCounter rowCounter = new RowCounter(readStreamCout);
                    rowsCount = rowCounter.Count();
                }

                if (isStoredProcedure)
                {
                    using (Stream readStreamOutParms = await file.OpenAsync(FileAccess.Read).ConfigureAwait(false))
                    {
                        UpdateOutParametersValues(readStreamOutParms, Parameters);
                    }
                }

                Stream readStream = await file.OpenAsync(FileAccess.Read).ConfigureAwait(false);

                AceQLDataReader aceQLDataReader = new AceQLDataReader(file, readStream, rowsCount, connection);
                return aceQLDataReader;

            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, aceQLHttpApi.httpStatusCode);
                }
            }
        }

        private void UpdateOutParametersValues(Stream stream, AceQLParameterCollection parameters)
        {
            //1) Build outParametersDict Dict of (paramerer names, values)
            //Dictionary<string, string> outParametersDict = new Dictionary<string, string>();
            OutParamBuilder outParamBuilder = new OutParamBuilder(stream);

            Dictionary<string, string> outParametersDict = outParamBuilder.GetvaluesPerParamName();

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
        /// Generates a unique File on the system for the downloaded result set content.
        /// </summary>
        /// <returns>A unique File on the system.</returns>
        private static async Task<IFile> GetUniqueResultSetFileAsync()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(Parms.ACEQL_PCL_FOLDER,
                CreationCollisionOption.OpenIfExists).ConfigureAwait(false);

            String fileName = Guid.NewGuid().ToString() + "-result-set.txt";
            IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);

            return file;
        }

        /// <summary>
        /// Executes the query as prepared statement.
        /// </summary>
        /// <returns>An <see cref="AceQLDataReader"/> object.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        private async Task<AceQLDataReader> ExecuteQueryAsPreparedStatementAsync()
        {
            try
            {
                AceQLCommandUtil aceQLCommandUtil = new AceQLCommandUtil(cmdText, Parameters);

                // Get the parameters and build the result set
                Dictionary<string, string> statementParameters = aceQLCommandUtil.GetPreparedStatementParameters();

                // Replace all @parms with ? in sql command
                cmdText = aceQLCommandUtil.ReplaceParmsWithQuestionMarks();

                IFile file = await GetUniqueResultSetFileAsync().ConfigureAwait(false);

                bool isStoredProcedure = (commandType == CommandType.StoredProcedure ? true : false);
                bool isPreparedStatement = true;
                using (Stream input = await aceQLHttpApi.ExecuteQueryAsync(cmdText, Parameters, isStoredProcedure, isPreparedStatement, statementParameters).ConfigureAwait(false))
                {
                    if (input != null)
                    {
                        if (aceQLHttpApi.GzipResult)
                        {
                            using (GZipStream decompressionStream = new GZipStream(input, CompressionMode.Decompress))
                            {
                                using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false))
                                {
                                    decompressionStream.CopyTo(stream);
                                }
                            }
                        }
                        else
                        {
                            using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false))
                            {
                                input.CopyTo(stream);
                            }
                        }

                    }

                }

                StreamResultAnalyzer streamResultAnalyzer = new StreamResultAnalyzer(file, aceQLHttpApi.httpStatusCode);
                if (!await streamResultAnalyzer.IsStatusOkAsync().ConfigureAwait(false))
                {
                    throw new AceQLException(streamResultAnalyzer.GetErrorMessage(),
                        streamResultAnalyzer.GetErrorType(),
                        streamResultAnalyzer.GetStackTrace(),
                        aceQLHttpApi.httpStatusCode);
                }

                int rowsCount = 0;

                using (Stream readStreamCout = await file.OpenAsync(FileAccess.Read).ConfigureAwait(false))
                {
                    RowCounter rowCounter = new RowCounter(readStreamCout);
                    rowsCount = rowCounter.Count();
                }

                if (isStoredProcedure)
                {
                    using (Stream readStreamOutParms = await file.OpenAsync(FileAccess.Read).ConfigureAwait(false))
                    {
                        UpdateOutParametersValues(readStreamOutParms, Parameters);
                    }
                }

                Stream readStream = await file.OpenAsync(FileAccess.Read).ConfigureAwait(false);

                AceQLDataReader aceQLDataReader = new AceQLDataReader(file, readStream, rowsCount, connection);
                return aceQLDataReader;

            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, aceQLHttpApi.httpStatusCode);
                }
            }
        }


        /// <summary>
        /// Executes the update as prepared statement.
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <exception cref="AceQLException">
        /// </exception>
        private async Task<int> ExecuteUpdateAsPreparedStatementAsync()
        {
            try
            {
                AceQLCommandUtil aceQLCommandUtil = new AceQLCommandUtil(cmdText, Parameters);

                // Get the parameters and build the result set
                Dictionary<string, string> statementParameters = aceQLCommandUtil.GetPreparedStatementParameters();

                // Uploads Blobs
                List<string> blobIds = aceQLCommandUtil.BlobIds;
                List<Stream> blobStreams = aceQLCommandUtil.BlobStreams;
                List<long> blobLengths = aceQLCommandUtil.BlobLengths;

                long totalLength = 0;
                for (int i = 0; i < blobIds.Count; i++)
                {
                    totalLength += blobLengths[i];
                }

                for (int i = 0; i < blobIds.Count; i++)
                {
                    await aceQLHttpApi.BlobUploadAsync(blobIds[i], blobStreams[i], totalLength).ConfigureAwait(false);
                }

                // Replace all @parms with ? in sql command
                cmdText = aceQLCommandUtil.ReplaceParmsWithQuestionMarks();

                Dictionary<string, string> parametersMap = new Dictionary<string, string>
                {
                    { "sql", cmdText },
                    { "prepared_statement", "true" }
                };

                //statementParameters.ToList().ForEach(x => parametersMap.Add(x.Key, x.Value));
                List<string> keyList = new List<string>(statementParameters.Keys);
                foreach (string key in keyList)
                {
                    parametersMap.Add(key, statementParameters[key]);
                }

                bool isStoredProcedure = (commandType == CommandType.StoredProcedure ? true : false);
                bool isPreparedStatement = true;

                int result = await aceQLHttpApi.ExecuteUpdateAsync(cmdText, Parameters, isStoredProcedure, isPreparedStatement, statementParameters).ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(AceQLException))
                {
                    throw exception;
                }
                else
                {
                    throw new AceQLException(exception.Message, 0, exception, aceQLHttpApi.httpStatusCode);
                }

            }
        }


        /// <summary>
        /// Executes the update as statement.
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <exception cref="AceQLException">
        /// </exception>
        private async Task<int> ExecuteUpdateAsStatementAsync()
        {
            bool isStoredProcedure = (commandType == CommandType.StoredProcedure ? true : false);
            bool isPreparedStatement = false;
            Dictionary<string, string> statementParameters = null;
            return await aceQLHttpApi.ExecuteUpdateAsync(cmdText, Parameters, isStoredProcedure, isPreparedStatement, statementParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets ot set SQL statement to execute against a remote SQL database.
        /// </summary>
        /// <value>The SQL statement to execute against a remote SQL database.</value>
        public string CommandText
        {
            get
            {
                return cmdText;
            }

            set
            {
                this.cmdText = value ?? throw new ArgumentNullException("cmdText is null!");
                parameters = new AceQLParameterCollection(cmdText);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="AceQLConnection"/> used by this instance of <see cref="AceQLCommand"/>.
        /// </summary>
        /// <value>The remote database connection.</value>
        public AceQLConnection Connection
        {
            get
            {
                return connection;
            }

            set
            {
                this.connection = value ?? throw new ArgumentNullException("connection is null!");
                this.connection.TestConnectionOpened();
                this.aceQLHttpApi = connection.aceQLHttpApi;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="AceQLTransaction"/> used by this instance of <see cref="AceQLCommand"/>.
        /// </summary>
        /// <value>The <see cref="AceQLTransaction"/>.</value>
        public AceQLTransaction Transaction
        {
            get
            {
                return transaction;
            }

            set
            {
                this.transaction = value ?? throw new ArgumentNullException("transaction is null!");
            }
        
        }

        /// <summary>
        /// Gets the <see cref="AceQLParameterCollection"/>.
        /// </summary>
        /// <value>The <see cref="AceQLParameterCollection"/>.</value>
        public AceQLParameterCollection Parameters
        {
            get
            {
                return parameters;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how the CommandText property is to be interpreted.
        /// </summary>
        /// <value>The <see cref="CommandType"/>.</value>
        public CommandType CommandType { get => commandType; set => commandType = value; }

        /// <summary>
        /// Disposes this instance. This call is optional and does nothing because all resources are released after 
        /// each other <see cref="AceQLCommand"/> method call. Class implements <see cref="IDisposable"/> to ease code migration.
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
