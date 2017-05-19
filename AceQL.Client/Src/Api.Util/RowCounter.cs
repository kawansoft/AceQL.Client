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
﻿using AceQL.Client.Api.File;

using AceQL.Client.Api.Util;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using PCLStorage;

namespace AceQL.Client.Api.Util
{
    /// <summary>
    /// Counts rows in a JSON file.
    /// </summary>
    internal class RowCounter
    {
        private bool traceOn;
        private IFile file;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">The Result Set JSON file to count the rows for.</param>
        /// <exception cref="System.ArgumentNullException">The file is null.</exception>
        public RowCounter(IFile file)
        {
            this.file = file ?? throw new ArgumentNullException("file is null!");
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <returns>System.Int32.</returns>
        internal async Task<int> CountAsync()
        {
            Trace();
            using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read).ConfigureAwait(false))
            {
                TextReader textReader = new StreamReader(stream);
                var reader = new JsonTextReader(textReader);

                while (reader.Read())
                {
                    if (reader.Value == null)
                    {
                        continue;
                    }

                    if (reader.TokenType != JsonToken.PropertyName || !reader.Value.Equals("row_count"))
                    {
                        continue;
                    }

                    if (reader.Read())
                    {
                        String rowCountStr = reader.Value.ToString();
                        int rowCount = Int32.Parse(rowCountStr);

                        Trace();
                        Trace("rowCount: " + rowCount);
                        return rowCount;
                    }
                    else
                    {
                        return 0;
                    }

                }

            }

            return 0;

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
        /// Traces the specified string.
        /// </summary>
        /// <param name="theString">The string to trace.</param>
        private void Trace(String theString)
        {
            if (traceOn)
            {
                ConsoleEmul.WriteLine(theString);
            }
        }
    }
}
