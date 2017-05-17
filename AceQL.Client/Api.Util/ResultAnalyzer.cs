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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Util
{
    /// <summary>
    /// Class <see cref="ResultAnalyzer"/>. Used to analyze a JSON response from the AceQL server.
    /// </summary>
    internal class ResultAnalyzer
    {
        /// <summary>
        /// The json result
        /// </summary>
        private string jsonResult;
        private HttpStatusCode httpStatusCode;

        /// <summary>
        /// We try to find status. If error parsing, invalidJsonStream = true
        /// </summary>
        private bool invalidJsonStream = false;

        /** Exception when parsing the JSON stream. Future usage */
        private Exception parseException = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultAnalyzer"/> class.
        /// </summary>
        /// <param name="jsonResult">The json result.</param>
        /// <param name="httpStatusCode">The http status code.</param>
        /// <exception cref="System.ArgumentNullException">jsonResult is null!</exception>
        public ResultAnalyzer(string jsonResult, HttpStatusCode httpStatusCode)
        {
            this.httpStatusCode = httpStatusCode;
            this.jsonResult = jsonResult;
        }

        /// <summary>
        /// Determines whether the SQL command correctly executed on server side.
        /// </summary>
        /// <returns><c>true</c> if [is status ok]; otherwise, <c>false</c>.</returns>
        public bool IsStatusOk()
        {
            if (jsonResult == null || jsonResult.Length == 0)
            {
                return false;
            }

            try
            {
                dynamic xj = JsonConvert.DeserializeObject(jsonResult);
                if (xj == null)
                {
                    return false;
                }

                String status = xj.status;

                if (status == null)
                {
                    return false;
                }

                if (status.Equals("OK"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                this.parseException = e;
                invalidJsonStream = true;
                return false;
            }

        }


        /// <summary>
        /// Says if the JSON Stream is invalid.
        /// </summary>
        /// <returns>true if JSN stream is invalid</returns>
        private bool IsInvalidJsonStream()
        {
            if (jsonResult == null || jsonResult.Length == 0)
            {
                return true;
            }

            if (invalidJsonStream)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the result for a a key name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string GetResult(String name)
        {
            return GetValue(name);
        }

        /// <summary>
        /// Gets the result for the key name "result"
        /// </summary>
        /// <returns></returns>
        public String GetResult()
        {
            return GetValue("result");
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">name is null!</exception>
        /// <exception cref="System.Exception">Illegal name: " + name</exception>
        public string GetValue(String name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name is null!");
            }

            if (IsInvalidJsonStream())
            {
                return null; ;
            }

            try
            {
                dynamic xj = JsonConvert.DeserializeObject(jsonResult);
                if (xj == null)
                {
                    return null;
                }

                String value = null;

                if (name.Equals("session_id"))
                {
                    value = xj.session_id;

                    //ConsoleEmul.WriteLine("xj       : " + xj);
                    //ConsoleEmul.WriteLine("session_id: " + value);
                }
                else if (name.Equals("length"))
                {
                    value = xj.length;
                }
                else if (name.Equals("result"))
                {
                    value = xj.result;
                }
                else if (name.Equals("row_count"))
                {
                    value = xj.row_count;
                }
                else
                {
                    throw new Exception("Illegal name: " + name);
                }

                return value;
            }
            catch (Exception e)
            {
                this.parseException = e;
                invalidJsonStream = true;
                return null;
            }
        }

        /// <summary>
        /// Gets the error_type.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int GetErrorId()
        {
            if (IsInvalidJsonStream())
            {
                return 0; ;
            }

            try
            {
                dynamic xj = JsonConvert.DeserializeObject(jsonResult);
                if (xj == null)
                {
                    return -1;
                }

                int errorType = xj.error_type;
                return errorType;
            }
            catch (Exception e)
            {
                this.parseException = e;
                invalidJsonStream = true;
                return 0;
            }

        }

        /// <summary>
        /// Gets the error_message.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetErrorMessage()
        {

            if (IsInvalidJsonStream())
            {
                String theErrorMessage = "Unknown error.";
                if (httpStatusCode != HttpStatusCode.OK)
                {
                    theErrorMessage = "HTTP FAILURE " + (int)httpStatusCode + " (" + httpStatusCode + ")";
                }

                return theErrorMessage;
            }

            try
            {
                dynamic xj = JsonConvert.DeserializeObject(jsonResult);
                if (xj == null)
                {
                    return null;
                }

                String errorMessage = xj.error_message;
                return errorMessage;
            }
            catch (Exception e)
            {
                this.parseException = e;
                invalidJsonStream = true;
                return null;
            }

        }

        /// <summary>
        /// Gets the remote stack_trace.
        /// </summary>
        /// <returns>String.</returns>
        public String GetStackTrace()
        {
            if (IsInvalidJsonStream())
            {
                return null;
            }

            try
            {
                dynamic xj = JsonConvert.DeserializeObject(jsonResult);
                if (xj == null)
                {
                    return null;
                }

                String stackTrace = xj.stack_trace;
                return stackTrace;
            }
            catch (Exception e)
            {
                this.parseException = e;
                invalidJsonStream = true;
                return null;
            }
        }

        /// <summary>
        /// Gets the int value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        internal int GetIntvalue(string name)
        {
            String insStr = GetValue(name);

            if (insStr == null)
            {
                return -1;
            }

            return Int32.Parse(insStr);
        }
    }
}