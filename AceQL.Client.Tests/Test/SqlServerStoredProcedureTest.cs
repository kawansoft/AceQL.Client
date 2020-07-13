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

using AceQL.Client;
using AceQL.Client.Api;
using AceQL.Client.Api.File;
using AceQL.Client.Src.Api;
using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AceQL.Client.Tests
{
    /// <summary>
    /// This example: 
    /// 1) Inserts a Customer and an Order on a remote database. 
    /// 2) Displays the inserted raws on the console with two SELECT executed on the remote database.
    /// </summary>
    public class SqlServerStoredProcedureTest
    {
        /// <summary>
        /// The connection to the remote database
        /// </summary>
        AceQLConnection connection = null;

        public static void TheMain(string[] args)
        {
            DoIt(args).Wait();
        }

        /// <summary>
        /// Does it.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static async Task DoIt(string[] args)
        {

            try
            {

                // Make sure connection is always closed in order to close and release
                // server connection into the pool
                using (AceQLConnection connection = await ConnectionBuilderAsync())
                {
                    SqlServerStoredProcedureTest myRemoteConnection = new SqlServerStoredProcedureTest(
                        connection);
                    AceQLConsole.WriteLine("Connection created....");

                    await myRemoteConnection.CallStoredProcedure();
                    await connection.CloseAsync();
                }

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


        /// <summary>
        /// RemoteConnection Quick Start client example.
        /// Creates a Connection to a remote database and open it.
        /// </summary>
        /// <returns>The connection to the remote database</returns>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public static async Task<AceQLConnection> ConnectionBuilderAsync()
        {
            // Port number is the port number used to start the Web Server:
            string server = "http://localhost:9090/aceql";
            string database = "sampledb";

            // (username, password) for authentication on server side.
            // No authentication will be done for our Quick Start:
            // (username, password) for authentication on server side.
            // No authentication will be done for our Quick Start:
            string username = "MyUsername";
            string password = "MySecret";

            string connectionString = $"Server={server}; Database={database}; "
                + $"Username={username}; Password={password};";

            AceQLConnection connection = new AceQLConnection(connectionString);

            // Opens the connection with the remote database.
            // On the server side, a JDBC connection is extracted from the connection 
            // pool created by the server at startup. The connection will remain ours 
            // during the session.
            await connection.OpenAsync();

            return connection;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connection">The AceQL connection to remote database.</param>
        public SqlServerStoredProcedureTest(AceQLConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Example of MS SQL Server Stored Procedure.
        /// </summary>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public async Task CallStoredProcedure()
        {
            string sql = "{call ProcedureName(@parm1, @parm2, @parm3)}";

            AceQLCommand command = new AceQLCommand(sql, connection);
            command.CommandType = CommandType.StoredProcedure;

            AceQLParameter aceQLParameter1 = new AceQLParameter("@parm1", 0);

            AceQLParameter aceQLParameter2 = new AceQLParameter("@parm2", 2)
            {
                Direction = ParameterDirection.InputOutput
            };

            AceQLParameter aceQLParameter3 = new AceQLParameter("@parm3")
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(aceQLParameter1);
            command.Parameters.Add(aceQLParameter2);
            command.Parameters.Add(aceQLParameter3);

            AceQLConsole.WriteLine(sql);
            AceQLConsole.WriteLine("BEFORE execute @parm2: " + aceQLParameter2.ParameterName + " / " + aceQLParameter2.Value + " (" + aceQLParameter2.Value.GetType() + ")");
            AceQLConsole.WriteLine("BEFORE execute @parm3: " + aceQLParameter3.ParameterName + " / " + aceQLParameter3.Value);
            AceQLConsole.WriteLine();

            // Our dataReader must be disposed to delete underlying downloaded files
            using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
            {
                //await dataReader.ReadAsync(new CancellationTokenSource().Token)
                while (dataReader.Read())
                {
                    int i = 2;
                    AceQLConsole.WriteLine("GetValue: " + dataReader.GetValue(i++));
                }
            }
            AceQLConsole.WriteLine();
            AceQLConsole.WriteLine("AFTER execute @parm2: " + aceQLParameter2.ParameterName + " / " + aceQLParameter2.Value + " (" + aceQLParameter2.Value.GetType() + ")");
            AceQLConsole.WriteLine("AFTER execute @parm3: " + aceQLParameter3.ParameterName + " / " + aceQLParameter3.Value);

        }
     
    }
}
