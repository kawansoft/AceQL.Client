// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-21-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-24-2017
// ***********************************************************************
// <copyright file="AceQLCommand.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using AceQL.Client.api.file;
using AceQL.Client.api.http;
using AceQL.Client.api.util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace AceQL.Client.api
{
    /// <summary>
    /// Represents a Transact-SQL statement or stored procedure to execute against a remote SQL database.
    /// </summary>
    public class AceQLCommand : IDisposable
    {
        /// <summary>
        /// The instance that does all http stuff
        /// </summary>
        private AceQLHttpApi aceQLHttpApi;

        /// <summary>
        /// The AceQL connection
        /// </summary>
        private AceQLConnection connection;
        /// <summary>
        /// The text of the query.
        /// </summary>
        private string cmdText;

        /// <summary>
        /// The parameters
        /// </summary>
        private AceQLParameterCollection parameters = null;


        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public AceQLParameterCollection Parameters
        {
            get
            {
                return parameters;
            }
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText
        {
            get
            {
                return cmdText;
            }
        }


        /// <summary>
        /// Gets the remote database connection.
        /// </summary>
        /// <value>The remote database connection.</value>
        protected AceQLConnection Connection
        {
            get
            {
                return connection;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLCommand"/> class with the text of the query and a AceQL.Client.api.AceQLConnection.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">The AceQL connection.</param>
        /// <exception cref="System.ArgumentNullException">
        /// cmdText is null
        /// or
        /// connection is null.
        /// </exception>
        public AceQLCommand(string cmdText, AceQLConnection connection)
        {
            if (cmdText == null)
            {
                throw new ArgumentNullException("cmdText is null!");
            }

            if (connection == null)
            {
                throw new ArgumentNullException("connection is null!");
            }

            connection.TestConnectionOpened();

            this.cmdText = cmdText;
            this.connection = connection;
            this.aceQLHttpApi = connection.aceQLHttpApi;

            parameters = new AceQLParameterCollection(cmdText);
        }


        /// <summary>
        ///  Sends the AceQL.Client.api.AceQLCommand.CommandText to the AceQL.Client.api.Connection and builds a AceQL.Client.api.AceQLDataReader.
        /// </summary>
        /// <returns>An AceQL.Client.api.AceQLDataReader object.</returns>
        /// <exception cref="AceQL.Client.api.AceQLException">If any Exception occurs.</exception>
        public async Task<AceQLDataReader> ExecuteReaderAsync()
        {
            if (Parameters.Count == 0)
            {
                return await ExecuteQueryAsStatementAsync().ConfigureAwait(false);
            }
            else
            {
                return await ExecuteQueryAsPreparedStatementAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Executes the SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="AceQL.Client.api.AceQLException">If any Exception occurs.</exception>
        public async Task<int> ExecuteNonQueryAsync()
        {
            if (Parameters.Count == 0)
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
        /// </summary>
        /// <returns>An AceQL.Client.api.AceQLDataReader object.</returns>
        /// <exception cref="AceQL.Client.api.AceQLException">If any Exception occurs.</exception>
        private async Task<AceQLDataReader> ExecuteQueryAsStatementAsync()
        {
            try
            {
                String fileName = AceQLCommandUtil.BuildResultSetFileName();

                Boolean isPreparedStatement = false;
                Dictionary<string, string> parametersMap = null;

                using (Stream input = await aceQLHttpApi.ExecuteQueryAsync(cmdText, isPreparedStatement, parametersMap).ConfigureAwait(false))
                {
                    if (aceQLHttpApi.GzipResult)
                    {
                        using (GZipStream decompressionStream = new GZipStream(input, CompressionMode.Decompress))
                        {
                            using (var stream = await PortableFile.CreateAsync(Parms.ACEQL_PCL_FOLDER, fileName).ConfigureAwait(false))
                            {
                                decompressionStream.CopyTo(stream);
                            }
                        }
                    }
                    else
                    {
                        using (var stream = await PortableFile.CreateAsync(Parms.ACEQL_PCL_FOLDER, fileName).ConfigureAwait(false))
                        {
                            input.CopyTo(stream);
                        }
                    }
                }

                StreamResultAnalyser resultAnalyser = new StreamResultAnalyser(Parms.ACEQL_PCL_FOLDER, fileName, aceQLHttpApi.httpStatusCode);
                if (! await resultAnalyser.IsStatusOkAsync().ConfigureAwait(false))
                {
                    throw new AceQLException(resultAnalyser.GetErrorMessage(),
                        resultAnalyser.GetErrorType(),
                        resultAnalyser.GetStackTrace(),
                        aceQLHttpApi.httpStatusCode);
                }

                RowCounter rowcounter = new RowCounter(Parms.ACEQL_PCL_FOLDER, fileName);
                int rowsCount = await rowcounter.CountAsync().ConfigureAwait(false);
                TextReader textReader =await PortableFile.OpenTextAsync(Parms.ACEQL_PCL_FOLDER, fileName).ConfigureAwait(false);

                AceQLDataReader aceQLDataReader = new AceQLDataReader(Parms.ACEQL_PCL_FOLDER, fileName, textReader, rowsCount, connection);
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
        /// Executes the query as prepared statement.
        /// </summary>
        /// <returns>An AceQL.Client.api.AceQLDataReader object.</returns>
        /// <exception cref="AceQL.Client.api.AceQLException">If any Exception occurs.</exception>
        private async Task<AceQLDataReader> ExecuteQueryAsPreparedStatementAsync()
        {
            try
            {
                AceQLCommandUtil aceQLCommandUtil = new AceQLCommandUtil(cmdText, Parameters);

                // Get the parameters and build the result set
                Dictionary<string, string> statementParameters = aceQLCommandUtil.GetPreparedStatementParameters();

                // Replace all @parms with ? in sql command
                cmdText = aceQLCommandUtil.ReplaceParmsWithQuestionMarks();

                String fileName = AceQLCommandUtil.BuildResultSetFileName();

                bool isPreparedStatement = true;
                using (Stream input = await aceQLHttpApi.ExecuteQueryAsync(cmdText, isPreparedStatement, statementParameters).ConfigureAwait(false))
                {
                    if (input != null)
                    {
                        if (aceQLHttpApi.GzipResult)
                        {
                            using (GZipStream decompressionStream = new GZipStream(input, CompressionMode.Decompress))
                            {
                                using (var stream = await PortableFile.CreateAsync(Parms.ACEQL_PCL_FOLDER, fileName).ConfigureAwait(false))
                                {
                                    decompressionStream.CopyTo(stream);
                                }
                            }
                        }
                        else
                        {
                            using (var stream = await PortableFile.CreateAsync(Parms.ACEQL_PCL_FOLDER, fileName).ConfigureAwait(false))
                            {
                                input.CopyTo(stream);
                            }
                        }
                        
                    }

                }

                StreamResultAnalyser resultAnalyser = new StreamResultAnalyser(Parms.ACEQL_PCL_FOLDER, fileName, aceQLHttpApi.httpStatusCode);
                if (!await resultAnalyser.IsStatusOkAsync().ConfigureAwait(false))
                {
                    throw new AceQLException(resultAnalyser.GetErrorMessage(),
                        resultAnalyser.GetErrorType(),
                        resultAnalyser.GetStackTrace(),
                        aceQLHttpApi.httpStatusCode);
                }

                RowCounter rowcounter = new RowCounter(Parms.ACEQL_PCL_FOLDER, fileName);
                int rowsCount = await rowcounter.CountAsync().ConfigureAwait(false);
                TextReader textReader = await PortableFile.OpenTextAsync(Parms.ACEQL_PCL_FOLDER, fileName).ConfigureAwait(false); ;

                AceQLDataReader aceQLDataReader = new AceQLDataReader(Parms.ACEQL_PCL_FOLDER, fileName, textReader, rowsCount, connection);
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
                    await aceQLHttpApi.BlobUploadAsync(blobIds[i], blobStreams[i], totalLength);
                }

                // Replace all @parms with ? in sql command
                cmdText = aceQLCommandUtil.ReplaceParmsWithQuestionMarks();

                Dictionary<string, string> parametersMap = new Dictionary<string, string>();
                parametersMap.Add("sql", cmdText);
                parametersMap.Add("prepared_statement", "true");

                //statementParameters.ToList().ForEach(x => parametersMap.Add(x.Key, x.Value));
                List<string> keyList = new List<string>(statementParameters.Keys);
                foreach (string key in keyList)
                {
                    parametersMap.Add(key, statementParameters[key]);
                }

                bool isPreparedStatement = true;

                int result =  await aceQLHttpApi.ExecuteUpdateAsync(cmdText, isPreparedStatement, statementParameters).ConfigureAwait(false);
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
            bool isPreparedStatement = false;
            Dictionary<string, string> statementParameters = null;
            return await aceQLHttpApi.ExecuteUpdateAsync(cmdText, isPreparedStatement, statementParameters).ConfigureAwait(false);
        }


        /// <summary>
        /// Disposes this instance. This is optional.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
