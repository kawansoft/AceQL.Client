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
using AceQL.Client.Api.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using PCLStorage;
using System.Threading;
using System.Globalization;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Class <see cref="AceQLDataReader"/>. Provides a way of reading a forward-only stream of rows from a remote database
    /// transferred in a local file.
    /// <para/>Note that all data of the stream are already downloaded when <see cref="AceQLDataReader"/> is created.
    /// </summary>
    public class AceQLDataReader : IDisposable
    {
        private static readonly bool DEBUG;

        /// <summary>
        /// The instance that does all http stuff
        /// </summary>
        private readonly AceQLHttpApi aceQLHttpApi;

        private int currentRowNum;
        private readonly int rowsCount;

        readonly RowParser rowParser;
        private bool isClosed;

        private readonly Dictionary<int, string> colNamesPerColIndex = new Dictionary<int, string>();

        private Dictionary<int, object> valuesPerColIndex = new Dictionary<int, object>();
        private Dictionary<int, string> colTypesPerColIndex = new Dictionary<int, string>();
        private Dictionary<string, int> colIndexesPerColName = new Dictionary<string, int>();

        /// <summary>
        /// The JSON file containing the Result Set
        /// </summary>
        private readonly IFile file;

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLDataReader"/> class.
        /// </summary>
        /// <param name="file">The JSON file containing the Result Set. Passed only for delete action.</param>
        /// <param name="readStream">The reading stream on file.</param>
        /// <param name="rowsCount">The number of rows in the file/result set.</param>
        /// <param name="connection">The AceQL connection.</param>
        /// <exception cref="System.ArgumentNullException">The file is null.</exception>
        internal AceQLDataReader(IFile file, Stream readStream, int rowsCount, AceQLConnection connection)
        {
            this.file = file ?? throw new ArgumentNullException("file is null!");
            this.rowsCount = rowsCount;

            this.aceQLHttpApi = connection.aceQLHttpApi;

            rowParser = new RowParser(readStream);
            this.rowsCount = rowsCount;
        }


        /// <summary>
        /// Advances the reader to the next record. 
        /// Method is provided only for consistency: same method exists in SQLServer SqlDataReader class.
        /// <para/>
        /// It's cleaner to use <see cref="AceQLDataReader"/>.Read() because data are read from a <see cref="TextReader"/> 
        /// (all data are already downloaded when <see cref="AceQLDataReader"/> is created.)
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param> 
        /// <returns>true if there are more rows; otherwise, false.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            Task<bool> task = new Task<bool>(Read);
            task.Start();
            return await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Advances the reader to the next record.
        /// </summary>
        /// <returns>true if there are more rows; otherwise, false.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public bool Read()
        {
            TestIsClosed();

            if (DEBUG)
            {
                ConsoleEmul.WriteLine();
                ConsoleEmul.WriteLine("currentRowNum: " + currentRowNum);
                ConsoleEmul.WriteLine("rowCount     : " + rowsCount);
            }

            if (currentRowNum == rowsCount)
            {
                return false;
            }

            try
            {
                currentRowNum++;
                rowParser.BuildRowNum(currentRowNum);

                valuesPerColIndex = rowParser.GetValuesPerColIndex();
                colTypesPerColIndex = rowParser.GetTypesPerColIndex();
                colIndexesPerColName = rowParser.GetColIndexesPerColName();

                // Build first time the Dic of (colIndex, colName) from Dic (colName, colIndex) 
                if (currentRowNum == 1)
                {
                    foreach (KeyValuePair<String, int> theKeyPair in colIndexesPerColName)
                    {
                        colNamesPerColIndex.Add(theKeyPair.Value, theKeyPair.Key);
                    }
                }

            }
            catch (Exception exception)
            {
                throw new AceQLException("Error when reading.", 0, exception, (HttpStatusCode)200);
            }

            return true;

        }

        /// <summary>
        /// Gets the value for  the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The value.</returns>
        public object this[string name]
        {
            get
            {
                TestIsClosed();
                int colIndex = colIndexesPerColName[name];

                if (IsDBNull(colIndex))
                {
                    return null;
                }

                return valuesPerColIndex[colIndex];
            }
        }

        /// <summary>
        /// Gets the value for  the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>The value.</returns>
        public object this[int ordinal]
        {
            get
            {
                TestIsClosed();

                if (IsDBNull(ordinal))
                {
                    return null;
                }

                return valuesPerColIndex[ordinal];
            }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <value>The number of columns in the current row.</value>
        public int FieldCount
        {
            get
            {
                TestIsClosed();
                return valuesPerColIndex.Count;
            }
        }

        /// <summary>Gets a value that indicates whether the <see cref="AceQLDataReader"/> contains one or more rows.
        /// </summary>
        /// <value>true if the <see cref="AceQLDataReader"/> contains one or more rows; otherwise.</value>
        public bool HasRows
        {
            get
            {
                TestIsClosed();
                return rowsCount > 0;
            }
        }

        /// <summary>
        /// Retrieves a Boolean value that indicates whether the specified <see cref="AceQLDataReader"/> 
        /// instance has been closed.
        /// </summary>
        /// <value>true if the specified <see cref="AceQLDataReader"/> instance is closed; otherwise false.</value>
        public bool IsClosed
        {
            get
            {
                return isClosed;
            }
        }

        /// <summary>
        /// Downloads the Blob and gets the stream.
        /// <para/>The cancellation token can be used to can be used to request that the operation be abandoned before the http request timeout.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>The Stream to read the downloaded Blob.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<Stream> GetStreamAsync(int ordinal, CancellationToken cancellationToken)
        {
            try
            {
                // Global var avoids to propagate cancellationToken as parameter to all methods... 
                aceQLHttpApi.SetCancellationToken(cancellationToken);
                return await GetStreamAsync(ordinal).ConfigureAwait(false);
            }
            finally
            {
                aceQLHttpApi.ResetCancellationToken();
            }
        }
        /// <summary>
        /// Downloads the Blob and gets a reading <see cref="Stream"/>.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>The <see cref="Stream"/> to read the downloaded Blob.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<Stream> GetStreamAsync(int ordinal)
        {
            if (IsDBNull(ordinal))
            {
                return null;
            }

            TestIsClosed();
            String blobId = GetString(ordinal);

            Debug("");
            Debug("blobId  : " + blobId);

            Stream stream = await aceQLHttpApi.BlobDownloadAsync(blobId).ConfigureAwait(false);
            return stream;
        }


        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public bool GetBoolean(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            if (IsDBNull(ordinal))
            {
                return false;
            }

            return Boolean.Parse(valuesPerColIndex[ordinal].ToString());
        }


        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Byte.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public byte GetByte(int ordinal)
        {
            bool throwExcepion = true;
            if (throwExcepion)
            {
                throw new NotSupportedException();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="dataOffset">The data offset.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferOffset">The buffer offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            bool throwExcepion = true;
            if (throwExcepion)
            {
                throw new NotSupportedException();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Char.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public char GetChar(int ordinal)
        {
            bool throwExcepion = true;
            if (throwExcepion)
            {
                throw new NotSupportedException();
            }
            else
            {
                return ' ';
            }
        }

        /// <summary>
        /// Gets the chars.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="dataOffset">The data offset.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferOffset">The buffer offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            bool throwExcepion = true;
            if (throwExcepion)
            {
                throw new NotSupportedException();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the name of the data type.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public string GetDataTypeName(int ordinal)
        {
            bool throwExcepion = true;
            if (throwExcepion)
            {
                throw new NotSupportedException();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the value of the specified column as a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public DateTime GetDateTime(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            if (IsDBNull(ordinal))
            {
                return DateTime.MinValue;
            }

            String theDateTime = valuesPerColIndex[ordinal].ToString();
            long unixDate = Convert.ToInt64(theDateTime);
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(unixDate).ToLocalTime();
            return date;
        }

        /// <summary>
        /// Gets the value of the specified column as a Decimal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public decimal GetDecimal(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            if (IsDBNull(ordinal))
            {
                return 0;
            }

            return Decimal.Parse(valuesPerColIndex[ordinal].ToString(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the value of the specified column as a Double.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public double GetDouble(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            if (IsDBNull(ordinal))
            {
                return 0;
            }

            return Double.Parse(valuesPerColIndex[ordinal].ToString(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the value of the specified column as a float.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public float GetFloat(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            if (IsDBNull(ordinal))
            {
                return 0;
            }

            return float.Parse(valuesPerColIndex[ordinal].ToString(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public short GetInt16(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            if (IsDBNull(ordinal))
            {
                return 0;
            }

            return short.Parse(valuesPerColIndex[ordinal].ToString());
        }

        /// <summary>
        /// Gets the value of the specified column as a 32-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public int GetInt32(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            if (IsDBNull(ordinal))
            {
                return 0;
            }

            return int.Parse(valuesPerColIndex[ordinal].ToString());
        }

        /// <summary>
        /// Gets the value of the specified column as a 64-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public long GetInt64(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            if (IsDBNull(ordinal))
            {
                return 0;
            }

            return long.Parse(valuesPerColIndex[ordinal].ToString());
        }

        /// <summary>
        /// Gets the name of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The name of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public string GetName(int ordinal)
        {
            TestIsClosed();

            if (!colNamesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No name found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            return colNamesPerColIndex[ordinal];
        }

        /// <summary>
        /// Gets the column ordinal, given the name of the column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The zero-based column ordinal.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public int GetOrdinal(string name)
        {
            TestIsClosed();

            if (!colIndexesPerColName.ContainsKey(name))
            {
                throw new AceQLException("No ordinal found for name: " + name, 0, (Exception)null, (HttpStatusCode)200);
            }

            return colIndexesPerColName[name];
        }

        /// <summary>
        /// Gets the value of the specified column as a string.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public string GetString(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            if (IsDBNull(ordinal))
            {
                return null;
            }

            return valuesPerColIndex[ordinal].ToString();
        }

        /// <summary>
        /// Gets the value of the specified column in its native format.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public object GetValue(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            String colType = colTypesPerColIndex[ordinal];

            Debug("");
            Debug("ordinal: " + ordinal);
            Debug("colType: " + colType);
            Debug("value  : " + valuesPerColIndex[ordinal]);

            if (AceQLTypes.IsStringType(colType))
            {
                return valuesPerColIndex[ordinal];
            }
            else if (colType.Equals(AceQLTypes.BIT))
            {
                return GetBoolean(ordinal);
            }
            else if (AceQLTypes.IsDateTimeType(colType))
            {
                return GetDateTime(ordinal);
            }
            else if (colType.Equals(AceQLTypes.SMALLINT) || colType.Equals(AceQLTypes.TINYINT))
            {
                return GetInt16(ordinal);
            }
            else if (colType.Equals(AceQLTypes.DECIMAL) || colType.Equals(AceQLTypes.NUMERIC))
            {
                return GetDecimal(ordinal);
            }
            else if (colType.Equals(AceQLTypes.INTEGER))
            {
                return GetInt32(ordinal);
            }
            else if (colType.Equals(AceQLTypes.REAL))
            {
                return GetFloat(ordinal);
            }
            else if (colType.Equals(AceQLTypes.FLOAT) || colType.Equals(AceQLTypes.DOUBLE_PRECISION))
            {
                return GetDouble(ordinal);
            }
            else
            {
                if (IsDBNull(ordinal))
                {
                    return null;
                }

                // If we don't know ==> just object, user will do the cast...
                return valuesPerColIndex[ordinal];
            }

        }


        /// <summary>
        ///  Gets a value that indicates whether the column contains non-existent or missing values.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>true if column contains non-existent or missing values, else false.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public bool IsDBNull(int ordinal)
        {
            TestIsClosed();

            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            return (valuesPerColIndex[ordinal] == null ? true : false);
        }

        internal void TestIsClosed()
        {
            if (isClosed)
            {
                throw new AceQLException("Instance is closed and disposed.", 0, (Exception)null, HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Closes the <see cref="AceQLDataReader"/>instance. Same as call to <see cref="AceQLDataReader"/>.Dispose().
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="AceQLDataReader"/> class. 
        /// This is recommended in order to delete the local corresponding temporary files.
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
            isClosed = true;
            rowParser.Dispose();

            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                if (! DEBUG)
                {
                    file.DeleteAsync();
                }

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception exception)
            {
                ConsoleEmul.WriteLine(exception.ToString());
            }

        }


        private static void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }

    }
}
