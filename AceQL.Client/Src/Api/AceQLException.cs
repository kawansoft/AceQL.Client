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

using System;
using System.Net;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Class <see cref="AceQLException"/>. All client and server side exceptions raised are wrapped in an <see cref="AceQLException"/>.
    /// </summary>
    public class AceQLException : Exception
    {
        /// <summary>
        /// The HTTP status code.
        /// </summary>
        private readonly HttpStatusCode httpStatusCode;

        /// <summary>
        /// The reason.
        /// </summary>
        private readonly string reason;
        /// <summary>
        /// The error type.
        /// </summary>
        private readonly int errorType;
        /// <summary>
        /// The remote Java stack trace.
        /// </summary>
        private readonly string remoteStackTrace;
        /// <summary>
        /// The exception.
        /// </summary>
        private readonly Exception exceptionCause;

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string Reason
        {
            get
            {
                return reason;
            }
        }

        /// <summary>
        /// Gets the error type:
        /// <list type="table">
        /// <item><description>0 for local <see cref="AceQLException"/>.</description></item>
        /// <item><description>1 for JDBC Driver Exception on the server.</description></item>
        /// <item><description>2 for AceQL Java Exception on the server.</description></item>
        /// <item><description>3 for AceQL Security Exception on the server.</description></item>
        /// <item><description>4 for AceQL failure.</description></item>
        /// </list>
        /// </summary>
        /// <value>The error type.</value>
        public int ErrorType
        {
            get
            {
                return errorType;
            }
        }

        /// <summary>
        /// Gets the remote Java stack trace.
        /// </summary>
        /// <value>The remote Java stack trace.</value>
        public string RemoteStackTrace
        {
            get
            {
                return remoteStackTrace;
            }

        }

        /// <summary>
        /// Gets the <see cref="Exception"/> that is the cause.
        /// </summary>
        /// <value>The <see cref="Exception"/>.</value>
        public Exception ExceptionCause
        {
            get
            {
                return exceptionCause;
            }
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <value>The HTTP status code.</value>
        public HttpStatusCode HttpStatusCode
        {
            get
            {
                return httpStatusCode;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLException"/> class.
        /// Corresponds to a client side failure/exception thrown.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="errorType">The error type.</param>
        /// <param name="exceptionCause">The <see cref="Exception"/> cause.</param>
        /// <param name="httpStatusCode">The http status code.</param>
        public AceQLException(string reason, int errorType, Exception exceptionCause, HttpStatusCode httpStatusCode)
        {
            this.reason = reason;
            this.errorType = errorType;
            this.exceptionCause = exceptionCause;
            this.httpStatusCode = httpStatusCode;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLException"/> class.
        /// Corresponds to a Java Exception raised on server side.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="errorType">The error type.</param>
        /// <param name="remoteStackTrace">The remote Java stack trace.</param>
        /// <param name="httpStatusCode">The http status code.</param>
        public AceQLException(string reason, int errorType, string remoteStackTrace, HttpStatusCode httpStatusCode)
        {
            this.reason = reason;
            this.errorType = errorType;
            this.remoteStackTrace = remoteStackTrace;
            this.httpStatusCode = httpStatusCode;
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            String returnStr = "errorType: " + errorType;
            returnStr += " / reason: " + reason;
            returnStr += " / httpStatusCode: " + httpStatusCode;

            if (exceptionCause != null)
            {
                returnStr += " / exception: " + exceptionCause;
            }

            if (remoteStackTrace != null)
            {
                returnStr += " / remoteStackTrace: " + remoteStackTrace;
            }

            return returnStr;
        }

    }
}
