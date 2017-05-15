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
using AceQL.Client.Api.File;
using AceQL.Client.Api.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AceQL.Client.Api.Util;
using PCLStorage;
using System.Threading;

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
            if (cmdText == null)
            {
                throw new ArgumentNullException("cmdText is null!");
            }

            this.cmdText = cmdText;
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
        /// <see cref="AceQLConnection"/>, and the System.Data.SqlClient.SqlTransaction.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="AceQLConnection"/> that represents the connection to a remote database.</param>
        /// <param name="transaction">A <see cref="AceQLTransaction"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If cmdText is null or connection or  is null.
        /// </exception>
        public AceQLCommand(string cmdText, AceQLConnection connection, AceQLTransaction transaction) : this(cmdText, connection)
        {

            if (transaction == null)
            {
                throw new ArgumentNullException("transaction is null!");
            }

            this.transaction = transaction;
        }

        /// <summary>
        ///  Sends the <see cref="AceQLCommand"/>.CommandText to the <see cref="AceQLConnection"/> and builds an <see cref="AceQLDataReader"/>.
        /// The cancellation token can be used to request that the operation be abandoned.
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
        /// The cancellation token can be used to request that the operation be abandoned.
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

            // Statement wit parameters are always prepared statement
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
        /// The cancellation token can be used to request that the operation be abandoned.
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

                Boolean isPreparedStatement = false;
                Dictionary<string, string> parametersMap = null;

                using (Stream input = await aceQLHttpApi.ExecuteQueryAsync(cmdText, isPreparedStatement, parametersMap).ConfigureAwait(false))
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

                StreamResultAnalyser resultAnalyser = new StreamResultAnalyser(file, aceQLHttpApi.httpStatusCode);
                if (!await resultAnalyser.IsStatusOkAsync().ConfigureAwait(false))
                {
                    throw new AceQLException(resultAnalyser.GetErrorMessage(),
                        resultAnalyser.GetErrorType(),
                        resultAnalyser.GetStackTrace(),
                        aceQLHttpApi.httpStatusCode);
                }

                RowCounter rowcounter = new RowCounter(file);
                int rowsCount = await rowcounter.CountAsync().ConfigureAwait(false);

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

                bool isPreparedStatement = true;
                using (Stream input = await aceQLHttpApi.ExecuteQueryAsync(cmdText, isPreparedStatement, statementParameters).ConfigureAwait(false))
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

                StreamResultAnalyser resultAnalyser = new StreamResultAnalyser(file, aceQLHttpApi.httpStatusCode);
                if (!await resultAnalyser.IsStatusOkAsync().ConfigureAwait(false))
                {
                    throw new AceQLException(resultAnalyser.GetErrorMessage(),
                        resultAnalyser.GetErrorType(),
                        resultAnalyser.GetStackTrace(),
                        aceQLHttpApi.httpStatusCode);
                }

                RowCounter rowcounter = new RowCounter(file);
                int rowsCount = await rowcounter.CountAsync().ConfigureAwait(false);

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

                int result = await aceQLHttpApi.ExecuteUpdateAsync(cmdText, isPreparedStatement, statementParameters).ConfigureAwait(false);
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
                if (value == null)
                {
                    throw new ArgumentNullException("cmdText is null!");
                }

                this.cmdText = value;
                parameters = new AceQLParameterCollection(cmdText);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SqlConnection"/> used by this instance of <see cref="AceQLCommand"/>.
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
                if (value == null)
                {
                    throw new ArgumentNullException("connection is null!");
                }

                this.connection = value;
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
                if (value == null)
                {
                    throw new ArgumentNullException("transaction is null!");
                }

                this.transaction = value;
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
        /// Disposes this instance. This call is optional and does nothing because all resources are released after 
        /// each other AceQL.Client.Api.AceQLCommand method call. Class implements IDisposable to ease code migration.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
