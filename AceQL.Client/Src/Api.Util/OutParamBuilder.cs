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
    /// Class OutParamBuilder.
    /// Parses the result out Json file and extracts the callable statement / stored procedure out parameters dictionnaries.
    /// <seealso cref="System.IDisposable" />
    /// </summary>
    internal class OutParamBuilder : IDisposable
    {

        private StreamReader streamReader;
        JsonTextReader reader;

        /// <summary>
        /// The trace on
        /// </summary>
        private bool traceOn;

        /// <summary>
        /// The values per col index
        /// </summary>
        private readonly Dictionary<int, string> valuesPerParamIndex = new Dictionary<int, string>();
        private readonly Dictionary<string, string> valuesPerParamName = new Dictionary<string, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="readStream">The reading stream on file.</param>
        public OutParamBuilder(Stream readStream)
        {
            streamReader = new StreamReader(readStream);
            reader = new JsonTextReader(streamReader);

            BuildOutParametersDicts();
        }


        /// <summary>
        /// Buils the out parameter dictionnaries reading Json in stream.s
        /// </summary>
        private void BuildOutParametersDicts()
        {

            // Necessary because a SQL columns could have the name "parameters_out_per_index" or "parameters_out_per_name"
            // We know that we are reading the good end of file
            // We are not any more in a array
            bool isInsideArray = false;

            while (reader.Read())
            {

                if (reader.TokenType != JsonToken.PropertyName || !reader.Value.Equals("parameters_out_per_index") || isInsideArray)
                {
                    continue;
                }

                String typeParameter = "per_index";

                string paramName = null;
                string paramValue = null;

                while (reader.Read())
                {
                    if (reader.Value != null && reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("parameters_out_per_name"))
                    {
                        typeParameter = "per_name";
                    }

                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        paramName = reader.Value.ToString();
                        paramName = paramName.Trim();
                        Trace("property: " + paramName + ":");
                    }
                    else if (reader.TokenType == JsonToken.String)
                    {

                        paramValue = reader.Value.ToString();
                        Trace("paramIndex: " + paramName + ":");
                        Trace("value     : " + paramValue + ":");
                        if (paramValue.Equals("NULL"))
                        {
                            paramValue = null;
                        }

                        if (typeParameter.Equals("per_index"))
                        {
                            int paramIndexInt = Int32.Parse(paramName);
                            valuesPerParamIndex.Add(paramIndexInt, paramValue);
                        }
                        else
                        {
                            valuesPerParamName.Add(paramName, paramValue);
                        }

                    }
                }

                // We are 2 or 3 lines near end of Json file, go to end for file without tests...

            }
        }

        /// <summary>
        /// Retuns the dict of parameter indexes/values.
        /// </summary>
        /// <returns>The dict of parameter indexes/values</returns>
        public Dictionary<int, string> GetvaluesPerParamIndex()
        {
            return valuesPerParamIndex;
        }

        /// <summary>
        /// Retuns the dict of parameter name/values.
        /// </summary>
        /// <returns>The dict of parameter name/values.</returns>
        public Dictionary<string, string> GetvaluesPerParamName()
        {
            return valuesPerParamName;
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
        internal void Trace()
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
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="OutParamBuilder"/> class. 
        /// This is recommended in order to delete the local corresponding temporary files.
        /// </summary>
        public void Dispose()
        {
            if (this.streamReader != null)
            {
                this.streamReader.Dispose();
                if (this.reader != null)
                {
                    this.reader.Close();
                }

                this.streamReader = null;
                this.reader = null;

            }

        }

    }
}
