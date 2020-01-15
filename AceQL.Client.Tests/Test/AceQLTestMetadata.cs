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
using AceQL.Client.Api.Metadata;
using AceQL.Client.Api.Metadata.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Tests
{
    /// <summary>
    /// Tests AceQL client SDK by calling all APIs.
    /// </summary>
    class AceQLTestMetadata
    {
        private const string ACEQL_PCL_FOLDER = "AceQLPclFolder";

        public static void TheMain(string[] args)
        {
            try
            {
                Console.WriteLine("AceQLTestMetadata Begin...");
                DoIt(args).Wait();

                Console.WriteLine();
                Console.WriteLine("Press enter to close....");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }

        }


        static async Task DoIt(string[] args)
        {

#pragma warning disable CS0219 // Variable is assigned but its value is never used
            string serverUrlLocalhost = "http://localhost:9090/aceql";
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            string serverUrlLocalhostTomcat = "http://localhost:8080/aceql-test/aceql";
#pragma warning restore CS0219 // Variable is assigned but its value is never used
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            string serverUrlLinuxNoSSL = "http://www.aceql.com:9090/aceql";
            string serverUrlLinux = "https://www.aceql.com:9443/aceql";
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            string server = serverUrlLocalhost;
            string database = "kawansoft_example";
            string username = "username";
            string password = "password";

            //customer_id integer NOT NULL,
            //customer_title character(4),
            //fname character varying(32),
            //lname character varying(32) NOT NULL,
            //addressline character varying(64),
            //town character varying(32),
            //zipcode character(10) NOT NULL,
            //phone character varying(32),

            string connectionString = $"Server={server}; Database={database}; ";
            //connectionString += $"Username={username}; Password={password}";

            AceQLCredential credential = new AceQLCredential(username, password.ToCharArray());

            // Make sure connection is always closed to close and release server connection into the pool
            using (AceQLConnection connection = new AceQLConnection(connectionString))
            {
                connection.Credential = credential;
                await ExecuteExample(connection).ConfigureAwait(false);
                await connection.CloseAsync();
            }

        }

        /// <summary>
        /// Executes our example using an <see cref="AceQLConnection"/> 
        /// </summary>
        /// <param name="connection"></param>
        private static async Task ExecuteExample(AceQLConnection connection)
        {
            await connection.OpenAsync();

            Console.WriteLine("host: " + connection.ConnectionString);
            Console.WriteLine("aceQLConnection.GetClientVersion(): " + connection.GetClientVersion());
            Console.WriteLine("aceQLConnection.GetServerVersion(): " + await connection.GetServerVersionAsync());
            Console.WriteLine("AceQL local folder: ");
            Console.WriteLine(await AceQLConnection.GetAceQLLocalFolderAsync());

            RemoteDatabaseMetaData remoteDatabaseMetaData = new RemoteDatabaseMetaData(connection);

            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string schemaFilePath = userPath + "\\remote_database_schema.html";
            Console.WriteLine("schemaFilePath: " + schemaFilePath);

            // Download Blob
            using (Stream stream = await remoteDatabaseMetaData.DbSchemaDownloadAsync("html", null))
            {
                using (var fileStream = File.Create(schemaFilePath))
                {
                    stream.CopyTo(fileStream);
                }
            }

            System.Diagnostics.Process.Start(schemaFilePath);

            Console.WriteLine("Creating schema done.");

            JdbcDatabaseMetaData jdbcDatabaseMetaData = await remoteDatabaseMetaData.GetJdbcDatabaseMetaDataAsync();
            Console.WriteLine("JdbcDatabaseMetaData: " + jdbcDatabaseMetaData.ToString().Substring(1, 200));
            Console.WriteLine();

            List<String> tableNames = await remoteDatabaseMetaData.GetTableNamesAsync();

            foreach (String tableName in tableNames)
            {
                Console.WriteLine("tableName: " + tableName);
            }
            Console.WriteLine();

            String name = "orderlog";
            Table table = await remoteDatabaseMetaData.GetTableAsync(name);

            Console.WriteLine("table name: " + table.TableName);
            Console.WriteLine("table keys: ");
            List<PrimaryKey> primakeys = table.PrimaryKeys;
            foreach (PrimaryKey primaryKey in primakeys)
            {
                Console.WriteLine("==> primaryKey: " + primaryKey);
            }
            Console.WriteLine();

            Console.WriteLine("Full table: " + table);

            Console.WriteLine();
            Console.WriteLine("Done.");

        }

    }
}
