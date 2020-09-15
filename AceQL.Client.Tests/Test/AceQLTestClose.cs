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
using AceQL.Client.Api.Http;
using AceQL.Client.Tests.Test;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Tests
{
    /// <summary>
    /// Tests AceQL client SDK by calling all APIs.
    /// </summary>
    public static class AceQLTestClose
    {
        public static void TheMain(string[] args)
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
 
            var netCoreVer = System.Environment.Version; // 3.0.0
            AceQLConsole.WriteLine(netCoreVer + "");

            string connectionString = ConnectionStringCurrent.Build();

            using (AceQLConnection connection = new AceQLConnection(connectionString))
            {
                await ExecuteExample(connection).ConfigureAwait(false);
                //NOT Neccessary: await connection.CloseAsync(); 
            }
        }

        /// <summary>
        /// Executes our example using an <see cref="AceQLConnection"/> 
        /// </summary>
        /// <param name="connection"></param>
        public static async Task ExecuteExample(AceQLConnection connection)
        {

            await connection.OpenAsync();

            AceQLConsole.WriteLine("host: " + connection.ConnectionString);
            AceQLConsole.WriteLine("aceQLConnection.GetClientVersion(): " + AceQLConnection.GetClientVersion());
            AceQLConsole.WriteLine("aceQLConnection.GetServerVersion(): " + await connection.GetServerVersionAsync());
            AceQLConsole.WriteLine("AceQL local folder: ");
            AceQLConsole.WriteLine(await AceQLConnection.GetAceQLLocalFolderAsync());

            int maxSelect = 1;
            for (int j = 0; j < maxSelect; j++)
            {
                string sql = "select * from customer where customer_id > @parm1 and lname = @parm2"; ;
                AceQLCommand command = new AceQLCommand(sql, connection);

                command.Parameters.AddWithValue("@parm2", "Name_5");
                command.Parameters.AddWithValue("@parm1", 1);

                // Our dataReader must be disposed to delete underlying downloaded files
                using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    //await dataReader.ReadAsync(new CancellationTokenSource().Token)
                    while (dataReader.Read())
                    {
                        AceQLConsole.WriteLine();
                        AceQLConsole.WriteLine("" + DateTime.Now);
                        int i = 0;
                        AceQLConsole.WriteLine("GetValue: " + dataReader.GetValue(i++) + "\n"
                            + "GetValue: " + dataReader.GetValue(i++) + "\n"
                            + "GetValue: " + dataReader.GetValue(i++) + "\n"
                            + "GetValue: " + dataReader.GetValue(i++) + "\n"
                            + "GetValue: " + dataReader.GetValue(i++) + "\n"
                            + "GetValue: " + dataReader.GetValue(i++) + "\n"
                            + "GetValue: " + dataReader.GetValue(i++) + "\n"
                            + "GetValue: " + dataReader.GetValue(i));
                    }
                }
            }

        }

    }
}
