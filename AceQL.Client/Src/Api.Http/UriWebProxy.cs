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
using System;
using System.Net;

namespace AceQL.Client.Api.Http
{
    /// <summary>
    /// <see cref="UriWebProxy"/>Class. Allows to create a web proxy thats interfaces an <see cref="System.Net.IWebProxy"/> from an URI.
    /// </summary>
    internal class UriWebProxy : IWebProxy
    {
        private ICredentials credentials;
        private readonly Uri proxyUri;

        /// <summary>
        /// Builds an <see cref="IWebProxy" /> concrete implementation.
        /// </summary>
        /// <param name="proxyUri">The proxy URI. Example: <c>new Uri("http://localhost:8080")</c>.</param>
        /// <exception cref="ArgumentNullException">proxyUri is null!</exception>
        public UriWebProxy(Uri proxyUri)
        {
            this.proxyUri = proxyUri ?? throw new ArgumentNullException("proxyUri is null!");
        }

        /// <summary>
        /// Credentials to send to the proxy server for authentication.
        /// </summary>
        /// <value>The credentials.</value>
        public ICredentials Credentials
        {
            get
            {
                return credentials;       
            }

            set
            {
                this.credentials = value; 
            }
        }

        /// <summary>
        /// Specifies the protocols for authentication.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <returns>System.Uri.</returns>
        public Uri GetProxy(Uri destination)
        {
            return this.proxyUri;
        }

        /// <summary>
        /// Indicates that the proxy should not be used for the specified host.
        /// </summary>
        /// <param name="host">
        ///   <see cref="T:System.Uri" /> of the host whose proxy usage is to be verified.</param>
        /// <returns><c>true</c> if the proxy server should not be used for <paramref name="host" /> ; else <c>false</c>.</returns>
        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}
