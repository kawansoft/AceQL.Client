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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api.Util
{

    /// <summary>
    /// Stores the session Id per serverUrl/username/database triplet in order to
    /// get new AceQL Connection with /get_connection without new login action.
    /// </summary>
    class UserLoginStore
    {
        private static Dictionary<string, string> loggedUsers = new Dictionary<string, string>();

        private readonly string serverUrl;
        private readonly string username;
        private readonly string database;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginStore"/> class.
        /// </summary>
        /// <param name="serverUrl">The AceQL server URL.</param>
        /// <param name="username">the client username.</param>
        /// <param name="database">The database to which users wants to connect</param>
        /// <exception cref="ArgumentNullException">
        /// serverUrl is null!
        /// or
        /// username is null!
        /// or
        /// database is null!
        /// </exception>
        public UserLoginStore(String serverUrl, String username, String database)
        {
            this.serverUrl = serverUrl ?? throw new ArgumentNullException("serverUrl is null!");
            this.username = username ?? throw new ArgumentNullException("username is null!");
            this.database = database ?? throw new ArgumentNullException("database is null!");
        }


        /// <summary>
        /// Says if user is already logged (ie. it exist a session_if for (serverUrl, username, database) triplet.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is already logged]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAlreadyLogged()
        {
            String key = BuildKey();
            return loggedUsers.ContainsKey(key);
        }

        /// <summary>
        /// Returns the session If of logged user with (serverUrl, username, database) triplet.
        /// </summary>
        /// <returns>the stored session Id for the (serverUrl, username, database) triplet.</returns>
        public String GetSessionId()
        {
            String key = BuildKey();
            String sessionId = loggedUsers[key];
            return sessionId;
        }

        /// <summary>
        /// Stores the session Id of a logged user with (serverUrl, username, database) triplet.
        /// </summary>
        /// <param name="sessionId">The session Id of a logged user.</param>
        public void SetSessionId(String sessionId)
        {
            String key = BuildKey();
            loggedUsers[key] = sessionId;
        }

        /// <summary>
        /// Removes (serverUrl, username, database) triplet. This is to be called at /logout API.
        /// </summary>
        public void Remove()
        {
            String key = BuildKey();
            loggedUsers.Remove(key);
        }

        /// <summary>
        /// Builds the key.
        /// </summary>
        /// <returns>The built key</returns>
        private String BuildKey()
        {
            return serverUrl + "/" + username + "/" + database;
        }

    }
}
