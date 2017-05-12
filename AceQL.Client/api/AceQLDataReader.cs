// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-23-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="AceQLDataReader.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

using AceQL.Client.Api.Http;
using AceQL.Client.Api.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using PCLStorage;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Class AceQLDataReader. Provides a way of reading a forward-only stream of rows from a remote database
    /// transferred in a local file.
    /// </summary>
    public class AceQLDataReader : IDisposable
    {
        internal static bool DEBUG = false;

        /// <summary>
        /// The instance that does all http stuff
        /// </summary>
        private AceQLHttpApi aceQLHttpApi;

        private bool traceOn = false;

        private int currentRowNum = 0;
        private int rowsCount;

        RowParser rowParser = null;
        private bool isClosed = false;

        private Dictionary<int, object> valuesPerColIndex = new Dictionary<int, object>();
        private Dictionary<int, string> colTypesPerColIndex = new Dictionary<int, string>();
        private Dictionary<int, string> colNamesPerColIndex = new Dictionary<int, string>();
        private Dictionary<string, int> colIndexesPerColName = new Dictionary<string, int>();

        private AceQLConnection connection;

        /// <summary>
        /// The JSON file containing the Result Set
        /// </summary>
        private IFile file;


        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLDataReader" /> class.
        /// </summary>
        /// <param name="file">The JSON file containing the Result Set.</param>
        /// <param name="rowsCount">The number of rows in the file/result set.</param>
        /// <param name="connection">The AceQL connection.</param>
        /// <exception cref="System.ArgumentNullException">The file is null.</exception>
        internal AceQLDataReader(IFile file, int rowsCount, AceQLConnection connection)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file is null!");
            }

            if (connection == null)
            {
                throw new ArgumentNullException("connection is null!");
            }

            this.file = file;
            this.rowsCount = rowsCount;
            this.connection = connection;

            this.aceQLHttpApi = connection.aceQLHttpApi;

            rowParser = new RowParser(file);
            this.rowsCount = rowsCount;
        }


        /// <summary>
        /// Determines whether [is trace on].
        /// </summary>
        /// <returns><c>true</c> if [is trace on]; otherwise, <c>false</c>.</returns>
        internal bool IsTraceOn()
        {
            return traceOn;
        }

        /// <summary>
        /// Sets the trace on.
        /// </summary>
        /// <param name="traceOn">if set to <c>true</c> [trace on].</param>
        internal void SetTraceOn(bool traceOn)
        {
            this.traceOn = traceOn;
        }

        /// <summary>
        /// Traces this instance.
        /// </summary>
        private void Trace()
        {
            if (traceOn)
            {
                ConsoleEmul.WriteLine();
            }
        }

        /// <summary>
        /// Traces the specified string.
        /// </summary>
        /// <param name="s">The string to trace.</param>
        private void Trace(String s)
        {
            if (traceOn)
            {
                System.Diagnostics.Debug.WriteLine("");
            }
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
                int colIndex = colIndexesPerColName[name];
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
                return valuesPerColIndex.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has rows.
        /// </summary>
        /// <value><c>true</c> if this instance has rows; otherwise, <c>false</c>.</value>
        public bool HasRows
        {
            get
            {
                return rowsCount > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is closed.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed
        {
            get
            {
                return isClosed;
            }
        }

        /// <summary>
        /// Downloads the Blob and gets the stream.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>The Stream to read the downloaded Blob.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<Stream> GetStreamAsync(int ordinal)
        {
            String blobId = GetString(ordinal);

            Debug("");
            Debug("blobId  : " + blobId);

            Stream stream = await aceQLHttpApi.BlobDownloadAsync(blobId).ConfigureAwait(false);
            return stream;
        }


        /// <summary>
        /// Gets the value of the specified column as a boolean..
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public bool GetBoolean(int ordinal)
        {
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            return Boolean.Parse(valuesPerColIndex[ordinal].ToString());
        }

        ///// <summary>
        ///// Gets the byte.
        ///// </summary>
        ///// <param name="ordinal">The ordinal.</param>
        ///// <returns>System.Byte.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public byte GetByte(int ordinal)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Gets the bytes.
        ///// </summary>
        ///// <param name="ordinal">The ordinal.</param>
        ///// <param name="dataOffset">The data offset.</param>
        ///// <param name="buffer">The buffer.</param>
        ///// <param name="bufferOffset">The buffer offset.</param>
        ///// <param name="length">The length.</param>
        ///// <returns>System.Int64.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Gets the character.
        ///// </summary>
        ///// <param name="ordinal">The ordinal.</param>
        ///// <returns>System.Char.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public char GetChar(int ordinal)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Gets the chars.
        ///// </summary>
        ///// <param name="ordinal">The ordinal.</param>
        ///// <param name="dataOffset">The data offset.</param>
        ///// <param name="buffer">The buffer.</param>
        ///// <param name="bufferOffset">The buffer offset.</param>
        ///// <param name="length">The length.</param>
        ///// <returns>System.Int64.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Gets the name of the data type.
        ///// </summary>
        ///// <param name="ordinal">The ordinal.</param>
        ///// <returns>System.String.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public string GetDataTypeName(int ordinal)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Gets the value of the specified column as a DateTime.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public DateTime GetDateTime(int ordinal)
        {
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
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
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            return Decimal.Parse(valuesPerColIndex[ordinal].ToString());
        }

        /// <summary>
        /// Gets the value of the specified column as a Double.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public double GetDouble(int ordinal)
        {
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            return Double.Parse(valuesPerColIndex[ordinal].ToString());
        }

        ///// <summary>
        ///// Gets the enumerator.
        ///// </summary>
        ///// <returns>IEnumerator.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public IEnumerator GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Gets the type of the field.
        ///// </summary>
        ///// <param name="ordinal">The ordinal.</param>
        ///// <returns>Type.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public Type GetFieldType(int ordinal)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Gets the value of the specified column as a float.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public float GetFloat(int ordinal)
        {
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            return float.Parse(valuesPerColIndex[ordinal].ToString());
        }

        ///// <summary>
        ///// Gets the unique identifier. Not implemented.
        ///// </summary>
        ///// <param name="ordinal">The ordinal.</param>
        ///// <returns>Guid.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public Guid GetGuid(int ordinal)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public short GetInt16(int ordinal)
        {
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
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
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
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
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
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
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
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
                // If we don't know ==> just object, user will do the cast...
                return valuesPerColIndex[ordinal];
            }

        }

        ///// <summary>
        ///// Gets the values.
        ///// </summary>
        ///// <param name="values">The values.</param>
        ///// <returns>System.Int32.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public int GetValues(object[] values)
        //{
        //    throw new NotImplementedException();
        //}

        //   Gets a value that indicates whether the column contains non-existent or missing values.

        /// <summary>
        ///  Gets a value that indicates whether the column contains non-existent or missing values.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns> rue if column contains non-existent or missing values, else false.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public bool IsDBNull(int ordinal)
        {
            if (!valuesPerColIndex.ContainsKey(ordinal))
            {
                throw new AceQLException("No value found for ordinal: " + ordinal, 0, (Exception)null, (HttpStatusCode)200);
            }

            return (valuesPerColIndex[ordinal] == null ? true : false);
        }

        /// <summary>
        /// Advances the reader to the next record.
        /// </summary>
        /// <returns>true if there are more rows; otherwise, false.</returns>
        /// <exception cref="AceQL.Client.Api.AceQLException">If any Exception occurs.</exception>
        public async Task<bool> ReadAsync()
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine();
                ConsoleEmul.WriteLine("currentRowNum: " + currentRowNum);
                ConsoleEmul.WriteLine("rowCount     : " + rowsCount);
            }

            if (IsClosed)
            {
                throw new AceQLException("Reader is closed!", 0, (Exception)null, (HttpStatusCode)200);
            }

            if (currentRowNum == rowsCount)
            {
                return false;
            }

            try
            {
                currentRowNum++;
                await rowParser.BuildRowNumAsync(currentRowNum).ConfigureAwait(false);

                valuesPerColIndex = rowParser.GetValuesPerColIndex();
                colNamesPerColIndex = rowParser.GetColNamesPerColIndex();
                colTypesPerColIndex = rowParser.GetTypesPerColIndex();
                colIndexesPerColName = rowParser.GetColIndexesPerColName();
            }
            catch (Exception exception)
            {
                throw new AceQLException("Error when reading.", 0, exception, (HttpStatusCode)200);
            }

            return true;

        }


        /// <summary>
        /// Closes the DataReader. This is recommended in order to delete the local corresponding temporary files.
        /// </summary>
        public void Dispose()
        {
            isClosed = true;
            rowParser.Close();
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