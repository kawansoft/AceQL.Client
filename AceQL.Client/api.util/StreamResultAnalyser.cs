// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-23-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="StreamResultAnalyser.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using AceQL.Client.api.file;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AceQL.Client.api.util
{
    /// <summary>
    /// Class StreamResultAnalyser. Allows to analyze the result of a downloaded result of a SQL query stored in a local PC file.
    /// </summary>
    internal class StreamResultAnalyser
    {
        /// <summary>
        /// The JSON file
        /// </summary>
        private string fileName;
        /// <summary>
        /// The error identifier
        /// </summary>
        private string errorType;
        /// <summary>
        /// The error message
        /// </summary>
        private string errorMessage;
        /// <summary>
        /// The stack trace
        /// </summary>
        private string stackTrace;

        private HttpStatusCode httpStatusCode;
        private string folderName;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamResultAnalyser"/> class.
        /// </summary>
        /// <param name="folderName">The folder name.</param>
        /// <param name="fileName">The JSON file containing the remote SQL query result.</param>
        /// <param name="httpStatusCode">The http status code.</param>
        internal StreamResultAnalyser(string folderName, string fileName, HttpStatusCode httpStatusCode)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            this.folderName = folderName;
            this.fileName = fileName;
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Determines whether the SQL correctly executed on server side.
        /// </summary>
        /// <returns><c>true</c> if [is status ok]; otherwise, <c>false</c>.</returns>
        internal async Task<bool> IsStatusOkAsync()
        {
            if (! await PortableFile.ExistsAsync(folderName, fileName).ConfigureAwait(false))
            {
                this.errorType = "0";

                errorMessage = "Unknown error.";
                if (httpStatusCode != HttpStatusCode.OK)
                {
                    errorMessage = "HTTP FAILURE " + (int)httpStatusCode + " (" + httpStatusCode + ")";
                }
                return false;
            }

            using (TextReader textReader = await PortableFile.OpenTextAsync(folderName, fileName).ConfigureAwait(false))
            {
                var reader = new JsonTextReader(textReader);

                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("status"))
                        {
                            if (reader.Read())
                            {
                                if (reader.Value.Equals("OK"))
                                {
                                    return true;
                                }
                                else
                                {
                                    ParseErrorKeywords(reader);
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                }

            }

            return false;
        }

        /// <summary>
        /// Parses the error keywords.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ParseErrorKeywords(JsonTextReader reader)
        {
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("error_type"))
                    {
                        if (reader.Read())
                        {
                            this.errorType = reader.Value.ToString();
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("error_message"))
                    {
                        if (reader.Read())
                        {
                            this.errorMessage = reader.Value.ToString();
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("stack_trace"))
                    {
                        if (reader.Read())
                        {
                            this.stackTrace = (string)reader.Value;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <returns>The error message</returns>
        internal string GetErrorMessage()
        {
            return this.errorMessage;
        }

        /// <summary>
        /// Gets the error type.
        /// </summary>
        /// <returns>The error type.</returns>
        internal int GetErrorType()
        {
            return Int32.Parse(this.errorType);
        }

        /// <summary>
        /// Gets the remote stack trace.
        /// </summary>
        /// <returns>The remote stack strace.</returns>
        internal string GetStackTrace()
        {
            return this.stackTrace;
        }
    }
}