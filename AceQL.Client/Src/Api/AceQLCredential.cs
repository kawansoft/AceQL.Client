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
﻿using System;

namespace AceQL.Client.Api
{
    /// <summary>
    /// <see cref="AceQLCredential"/> provides a more secure way than using a connection string to specify the username, password or Session ID for a login attempt.
    /// <para/>Note that this version does not encrypt the password. This could and should be done in a future version.
    /// </summary>
    public sealed  class AceQLCredential
    {
        private readonly string username;
        private readonly char[] password;
        private readonly string sessionId;

        /// <summary>
        /// Creates an object of type <see cref="AceQLCredential"/>.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="System.ArgumentNullException">If username or password is null. </exception>
        public AceQLCredential(string username, char[] password)
        {
            this.username = username ?? throw new ArgumentNullException("username is null!");
            this.password = password ?? throw new ArgumentNullException("password is null!");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLCredential"/> class. This constructor allows authentication without a password using a 
        /// session_id value returned by AceQL login API on server side.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="sessionId">The session_id value returned by AceQL login API on server side.</param>
        public AceQLCredential(string username, string sessionId)
        {
            this.username = username ?? throw new ArgumentNullException("username is null!");
            this.sessionId = sessionId ?? throw new ArgumentNullException("sessionId is null!");
        }

        /// <summary>
        /// Returns the username component of the <see cref="AceQLCredential" /> object.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get => username; }

        /// <summary>
        /// Returns the password component of the <see cref="AceQLCredential" /> object.
        /// </summary>
        /// <value>The password.</value>
        public char[] Password { get => password; }

        /// <summary>
        /// Gets the Session ID created by AceQL on server side.
        /// </summary>
        /// <value>The Session ID created by AceQL on server side.</value>
        public string SessionId { get => sessionId;}
    }
}
