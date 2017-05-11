// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-21-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="ResultAnalyser.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.api.util
{
    /// <summary>
    /// Class ResultAnalyser. Used to analyze a JSON response from the AceQL server.
    /// </summary>
    internal class ResultAnalyser
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
        /// Initializes a new instance of the <see cref="ResultAnalyser"/> class.
        /// </summary>
        /// <param name="jsonResult">The json result.</param>
        /// <param name="httpStatusCode">The http status code.</param>
        /// <exception cref="System.ArgumentNullException">jsonResult is null!</exception>
        public ResultAnalyser(string jsonResult, HttpStatusCode httpStatusCode)
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
        /// Gets the result.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string GetResult(String name)
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
            String insStr =  GetValue(name);

            if (insStr == null)
            {
                return -1;
            }

            return Int32.Parse(insStr);
        }
    }
}
