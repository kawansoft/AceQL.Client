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
using Newtonsoft.Json;
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
    /// </summary>
    internal class RowParser
    {
        private const string COL_INDEX = "idx";
        private const string COL_TYPE = "typ";
        private const string COL_NAME = "nam";
        private const string COL_VALUE = "val";

        private StreamReader streamReader;
        JsonTextReader reader;

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


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="readStream">The reading stream on file.</param>
        public RowParser(Stream readStream)
        {
            streamReader = new StreamReader(readStream);
            reader = new JsonTextReader(streamReader);
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
        internal void BuildRowNum(int rowNum)
        {
            while (reader.Read())
            {
                if (reader.Value == null)
                {
                    continue;
                }

                if (reader.TokenType != JsonToken.PropertyName || !reader.Value.Equals("row_" + rowNum))
                {
                    continue;
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
                        if (!reader.Read())
                        {
                            return;
                        }
                        String colIndexStr = reader.Value.ToString();
                        colIndex = Int32.Parse(colIndexStr);

                        Trace();
                        Trace("" + colIndex);

                    }

                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_TYPE))
                    {
                        if (!reader.Read())
                        {
                            return;
                        }
                        String colIndexStr = reader.Value.ToString();
                        colType = reader.Value.ToString();
                        Trace("" + colType);

                    }

                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_NAME))
                    {
                        if (!reader.Read())
                        {
                            return;
                        }
                        String colIndexStr = reader.Value.ToString();
                        colName = reader.Value.ToString();
                        Trace("" + colName);

                    }

                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_VALUE))
                    {
                        if (!reader.Read())
                        {
                            return;
                        }

                        String colValue = reader.Value.ToString();

                        if (colValue.Equals("NULL"))
                        {
                            colValue = null;
                        }

                        if (colValue != null)
                        {
                            colValue = colValue.Trim();
                        }

                        // because it's start at 0 on C# instead of 1 in JDBC
                        colIndex--;

                        Trace("" + colValue);
                        valuesPerColIndex.Add(colIndex, colValue);
                        typesPerColIndex.Add(colIndex, colType);

                        colNamesPerColIndex.Add(colIndex, colName);
                        colIndexesPerColName.Add(colName, colIndex);

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
            if (this.streamReader != null)
            {
                this.streamReader.Dispose();
            }

        }

    }
}
