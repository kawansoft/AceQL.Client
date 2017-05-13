// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-25-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="RowParser.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using AceQL.Client.Api.File;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace AceQL.Client.Api.Util
{
    /// <summary>
    /// Class RowParser.
    /// We pass a TextReader instead of a PortableFile as we want the methods to be all sync for end user.
    /// </summary>
    internal class RowParser
    {
        private const string COL_INDEX = "idx";
        private const string COL_TYPE = "typ";
        private const string COL_NAME = "nam";
        private const string COL_VALUE = "val";

        /// <summary>
        /// The trace on
        /// </summary>
        private bool traceOn = false;
        /// <summary>
        /// The values per col index
        /// </summary>
        private Dictionary<int, object> valuesPerColIndex;
        private Dictionary<int, string> typesPerColIndex;
        private Dictionary<int, string> colNamesPerColIndex;
        private Dictionary<string, int> colIndexesPerColName;

        private IFile file;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">The JSON file containing the Result Set.</param>
        /// <exception cref="System.ArgumentNullException">The file is null.</exception>
        public RowParser(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file is null!");
            }

            this.file = file;
        }

        internal Dictionary<int, string> GetTypesPerColIndex()
        {
            return typesPerColIndex;
        }

        internal Dictionary<int, object> GetValuesPerColIndex()
        {
            return valuesPerColIndex;
        }


        internal Dictionary<string, int> GetColIndexesPerColName()
        {
            return colIndexesPerColName;
        }

        internal Dictionary<int, string> GetColNamesPerColIndex()
        {
            return colNamesPerColIndex;
        }


        /// <summary>
        /// Builds the row number.
        /// </summary>
        /// <param name="rowNum">The row number.</param>
        internal async Task BuildRowNumAsync(int rowNum)
        {

            using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read).ConfigureAwait(false))
            {
                TextReader textReader = new StreamReader(stream);
                JsonTextReader reader = new JsonTextReader(textReader);

                while (reader.Read())
                {
                    if (reader.Value == null)
                    {
                        continue;
                    }

                    Treat(reader, rowNum);
                }
            }

        }

        /// <summary>
        /// Continue reading...
        /// </summary>
        /// <param name="reader">The text reader </param>
        /// <param name="rowNum">The row number.</param>
        private void Treat(JsonTextReader reader, int rowNum)
        {

            if (reader.TokenType != JsonToken.PropertyName || !reader.Value.Equals("row_" + rowNum))
            {
                return;
            }

            int colIndex = 0;
            String colName = null;
            String colType = null;

            valuesPerColIndex = new Dictionary<int, object>();
            typesPerColIndex = new Dictionary<int, string>();
            colNamesPerColIndex = new Dictionary<int, string>();
            colIndexesPerColName = new Dictionary<string, int>();

            while (reader.Read())
            {
                // We are done at end of row
                if (reader.TokenType.Equals(JsonToken.EndArray))
                {
                    return;
                }

                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_INDEX))
                {
                    if (reader.Read())
                    {
                        String colIndexStr = reader.Value.ToString();
                        colIndex = Int32.Parse(colIndexStr);

                        Trace();
                        Trace("" + colIndex);
                    }
                    else
                    {
                        return;
                    }
                }

                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_TYPE))
                {
                    if (reader.Read())
                    {
                        String colIndexStr = reader.Value.ToString();
                        colType = reader.Value.ToString();
                        Trace("" + colType);
                    }
                    else
                    {
                        return;
                    }
                }

                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_NAME))
                {
                    if (reader.Read())
                    {
                        String colIndexStr = reader.Value.ToString();
                        colName = reader.Value.ToString();
                        Trace("" + colName);
                    }
                    else
                    {
                        return;
                    }
                }

                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_VALUE))
                {
                    if (reader.Read())
                    {
                        String colValue = reader.Value.ToString();

                        if (colValue != null)
                        {
                            colValue = colValue.Trim();
                        }

                        // because it's start at 0 on C# insted of 1 in JDBC
                        colIndex--;

                        Trace("" + colValue);
                        valuesPerColIndex.Add(colIndex, colValue);
                        typesPerColIndex.Add(colIndex, colType);

                        colNamesPerColIndex.Add(colIndex, colName);
                        colIndexesPerColName.Add(colName, colIndex);

                    }
                    else
                    {
                        return;
                    }
                }


            }

        }

        /**
         * Says if trace is on
         * 
         * @return true if trace is on
         */
        /// <summary>
        /// Determines whether [is trace on].
        /// </summary>
        /// <returns><c>true</c> if [is trace on]; otherwise, <c>false</c>.</returns>
        internal bool IsTraceOn()
        {
            return traceOn;
        }

        /**
         * Sets the trace on/off
         * 
         * @param traceOn
         *            if true, trace will be on
         */
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
        /// Traces the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        private void Trace(String s)
        {
            if (traceOn)
            {
                ConsoleEmul.WriteLine(s);
            }
        }


        internal void Close()
        {
            // Delete the file in fire and forget mode

            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                file.DeleteAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception exception)
            {
                ConsoleEmul.WriteLine(exception.ToString());
            }

        }


    }
}
