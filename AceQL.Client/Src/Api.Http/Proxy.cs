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
    /// <see cref="Proxy"/>Class. Allow to create a proxy from an end user proxyUri.
    /// </summary>
    public class Proxy : IWebProxy
    {
        private ICredentials credentials;
        private readonly String proxyUri;

        /// <summary>
        /// Builds an  <see cref="IWebProxy"/> implementation.
        /// </summary>
        /// <param name="proxyUri">The proxy URI. Example: http://localhost:8080.</param>
        /// 
        public Proxy(String proxyUri)
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
            return new Uri(this.proxyUri);
        }

        /// <summary>
        /// Indicates that the proxy should not be used for the specified host.
        /// </summary>
        /// <param name="host">
        ///   <see cref="T:System.Uri" /> of the host whose proxy usage is to be verified.</param>
        /// <returns>true if the proxy server should not be used for <paramref name="host" />�; else false.</returns>
        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}
