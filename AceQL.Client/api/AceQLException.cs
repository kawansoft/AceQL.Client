// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-21-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="AceQLException.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

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
        private HttpStatusCode httpStatusCode;

        /// <summary>
        /// The reason.
        /// </summary>
        private string reason;
        /// <summary>
        /// The error type.
        /// </summary>
        private int errorType;
        /// <summary>
        /// The remote Java stack trace.
        /// </summary>
        private string remoteStackTrace;
        /// <summary>
        /// The exception.
        /// </summary>
        private Exception exceptionCause;

        /// <summary>
        /// Gets the reason.
        /// </summary>
        /// <value>The reason.</value>
        public string Reason
        {
            get
            {
                return reason;
            }
        }

        /// <summary>
        /// Gets the error type.
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
        /// Corresponds to an Java Exception raised on server side.
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
                returnStr += " / exception: " + exceptionCause.ToString();
            }

            if (remoteStackTrace != null)
            {
                returnStr += " / remoteStackTrace: " + remoteStackTrace;
            }

            return returnStr;
        }

    }
}
