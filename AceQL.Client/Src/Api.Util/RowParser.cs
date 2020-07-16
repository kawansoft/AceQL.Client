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
using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Util
{
    /// <summary>
    /// Class RowParser.
    /// We pass a TextReader instead of a PortableFile as we want the methods to be all sync for end user.
    /// <seealso cref="System.IDisposable" />
    /// </summary>
    internal class RowParser : IDisposable
    {
        private readonly StreamReader streamReader;
        private readonly JsonTextReader reader;

        /// <summary>
        /// The trace on
        /// </summary>
        private bool traceOn;
        /// <summary>
        /// The values per col index
        /// </summary>
        private Dictionary<int, object> valuesPerColIndex;
        private Dictionary<int, string> typesPerColIndex;
        private Dictionary<string, int> colIndexesPerColName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="readStream">The reading stream on file.</param>
        public RowParser(Stream readStream)
        {
            streamReader = new StreamReader(readStream);
            reader = new JsonTextReader(streamReader);

            BuildTypes();
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

        private void BuildTypes()
        {
            typesPerColIndex = new Dictionary<int, string>();

            while (reader.Read())
            {
                if (reader.Value == null)
                {
                    continue;
                }

                if (reader.TokenType != JsonToken.PropertyName || !reader.Value.Equals("column_types"))
                {
                    continue;
                }

                int idx = 0;

                while (reader.Read())
                {
                    // We are done at end of row
                    if (reader.TokenType.Equals(JsonToken.EndArray))
                    {
                        return;
                    }

                    if (reader.Value != null)
                    {
                        typesPerColIndex.Add(idx++, reader.Value.ToString());
                    }
                }

            }
        }

        /// <summary>
        /// Builds the row number.
        /// </summary>
        /// <param name="rowNum">The row number.</param>
        internal void BuildRowNum(int rowNum)
        {
            // Value needed because we don't want to take columns with "row_xxx" names as row numbers
            bool firstStartArrayPassed = false;
            bool isInsideRowValuesArray = false;

            while (reader.Read())
            {
                if (reader.Value == null)
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        if (!firstStartArrayPassed)
                        {
                            firstStartArrayPassed = true;
                        }
                        else
                        {
                            isInsideRowValuesArray = true;
                        }
                    }

                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        isInsideRowValuesArray = false;
                    }

                    continue;
                }

                if (reader.TokenType != JsonToken.PropertyName || !reader.Value.Equals("row_" + rowNum) || isInsideRowValuesArray)
                {
                    continue;
                }

                int colIndex = 0;
                String colName = null;

                valuesPerColIndex = new Dictionary<int, object>();

                if (colIndexesPerColName == null)
                {
                    colIndexesPerColName = new Dictionary<string, int>();
                }

                while (reader.Read())
                {
                    // We are done at end of row
                    if (reader.TokenType.Equals(JsonToken.EndArray))
                    {
                        return;
                    }

                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        colName = reader.Value.ToString();
                        reader.Read();

                        String colValue = reader.Value.ToString();

                        if (colValue.Equals("NULL"))
                        {
                            colValue = null;
                        }

                        if (colValue != null)
                        {
                            colValue = colValue.Trim();
                        }

                        Trace("" + colValue);

                        valuesPerColIndex.Add(colIndex, colValue);

                        if (rowNum == 1)
                        {
                            colIndexesPerColName.Add(colName, colIndex);
                        }

                        // Do the increment at end to start indexes at 0
                        colIndex++;
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


        public void Dispose()
        {
            if (this.streamReader != null)
            {
                this.streamReader.Dispose();
                if (this.reader != null)
                {
                    this.reader.Close();
                }
            }

        }

    }
}
