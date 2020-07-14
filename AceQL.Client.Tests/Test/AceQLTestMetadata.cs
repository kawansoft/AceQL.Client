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
using AceQL.Client.Tests.Test;
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
    static class AceQLTestMetadata
    {

        public static void TheMain()
        {
            try
            {
                AceQLConsole.WriteLine("AceQLTestMetadata Begin...");
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
            string connectionString = ConnectionStringCurrent.Build();

            // Make sure connection is always closed to close and release server connection into the pool
            using (AceQLConnection connection = new AceQLConnection(connectionString))
            {
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

            AceQLConsole.WriteLine("host: " + connection.ConnectionString);
            AceQLConsole.WriteLine("aceQLConnection.GetClientVersion(): " + AceQLConnection.GetClientVersion());
            AceQLConsole.WriteLine("aceQLConnection.GetServerVersion(): " + await connection.GetServerVersionAsync());
            AceQLConsole.WriteLine("AceQL local folder: ");
            AceQLConsole.WriteLine(await AceQLConnection.GetAceQLLocalFolderAsync());

            RemoteDatabaseMetaData remoteDatabaseMetaData = connection.GetRemoteDatabaseMetaData();

            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string schemaFilePath = userPath + "\\db_schema.out.html";

            // Download Schema in HTML format:
            using (Stream stream = await remoteDatabaseMetaData.DbSchemaDownloadAsync())
            {
                using (var fileStream = File.Create(schemaFilePath))
                {
                    stream.CopyTo(fileStream);
                }
            }

            System.Diagnostics.Process.Start(schemaFilePath);
            AceQLConsole.WriteLine("Creating schema done.");

            JdbcDatabaseMetaData jdbcDatabaseMetaData = await remoteDatabaseMetaData.GetJdbcDatabaseMetaDataAsync();
            AceQLConsole.WriteLine("Major Version: " + jdbcDatabaseMetaData.GetJDBCMajorVersion);
            AceQLConsole.WriteLine("Minor Version: " + jdbcDatabaseMetaData.GetJDBCMinorVersion);
            AceQLConsole.WriteLine("IsReadOnly   : " + jdbcDatabaseMetaData.IsReadOnly);

            AceQLConsole.WriteLine("JdbcDatabaseMetaData: " + jdbcDatabaseMetaData.ToString().Substring(1, 200));
            AceQLConsole.WriteLine();

            AceQLConsole.WriteLine("Get the table names:");
            List<String> tableNames = await remoteDatabaseMetaData.GetTableNamesAsync();

            AceQLConsole.WriteLine("Print the column details of each table:");
            foreach (String tableName in tableNames)
            {
                Table table = await remoteDatabaseMetaData.GetTableAsync(tableName);

                AceQLConsole.WriteLine("Columns:");
                foreach(Column column in table.Columns)
                {
                    AceQLConsole.WriteLine(column.ToString());
                }
            }

            AceQLConsole.WriteLine();

            String name = "orderlog";
            Table tableOrderlog = await remoteDatabaseMetaData.GetTableAsync(name);

            AceQLConsole.WriteLine("table name: " + tableOrderlog.TableName);
            AceQLConsole.WriteLine("table keys: ");
            List<PrimaryKey> primakeys = tableOrderlog.PrimaryKeys;
            foreach (PrimaryKey primaryKey in primakeys)
            {
                AceQLConsole.WriteLine("==> primaryKey: " + primaryKey);
            }
            AceQLConsole.WriteLine();

            AceQLConsole.WriteLine("Full table: " + tableOrderlog);

            AceQLConsole.WriteLine();
            AceQLConsole.WriteLine("Done.");

        }

    }
}
