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

using AceQL.Client.Api;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Tests
{
    /// <summary>
    /// Tests AceQL client SDK by calling all APIs.
    /// </summary>
    static class AceQLTestSessionId
    {
        public static void TheMain()
        {
            try
            {
                DoIt().Wait();

                AceQLConsole.WriteLine();
                AceQLConsole.WriteLine("Press enter to close....");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                AceQLConsole.WriteLine(exception.ToString());
                AceQLConsole.WriteLine(exception.StackTrace);
                AceQLConsole.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
        }

        static async Task DoIt()
        {

            string serverUrlLocalhost = "http://localhost:9090/aceql";
            string server = serverUrlLocalhost;
            string database = "sampledb";
            string username = "user1";
            string sessionId = "4pdh8p2t14nd6j7dxt1owyjxef";

            string connectionString = $"Server={server}; Database={database}";

            Boolean doItWithCredential = true;

            if (!doItWithCredential)
            {
                connectionString += $"; Username={username}; SessionId={sessionId}";
                AceQLConsole.WriteLine("Using connectionString with SessionId: " + connectionString);

                // Make sure connection is always closed to close and release server connection into the pool
                using (AceQLConnection connection = new AceQLConnection(connectionString))
                {
                    await AceQLTest.ExecuteExample(connection).ConfigureAwait(false);
                    await connection.CloseAsync();
                }
            }
            else
            {
                AceQLConsole.WriteLine("Using AceQLCredential with SessionId: " + sessionId);
                AceQLCredential credential = new AceQLCredential(username, sessionId);

                // Make sure connection is always closed to close and release server connection into the pool
                using (AceQLConnection connection = new AceQLConnection(connectionString))
                {
                    connection.Credential = credential;
                    await AceQLTest.ExecuteExample(connection).ConfigureAwait(false);
                    await connection.CloseAsync();
                }
            }
        }
    }
}
