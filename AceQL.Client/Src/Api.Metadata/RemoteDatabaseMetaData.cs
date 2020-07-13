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
using AceQL.Client.Api.Http;
using AceQL.Client.Api.Metadata.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Metadata
{
    /// <summary>
    /// Class RemoteDatabaseMetaData. Allows to retrieve metadata info of the remote SQL database:
    /// <ul><li>Schema description.</li><li>Databases object wrappers: Tables, Columns, Indexes, etc.</li>
    /// </ul>
    /// </summary>
    public class RemoteDatabaseMetaData
    {
        /// <summary>
        /// The Http instance that does all Http stuff
        /// </summary>
        private readonly AceQLHttpApi aceQLHttpApi;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aceQLConnection">the Connection to the remote database.</param>
        /// <exception cref="ArgumentNullException">aceQLConnection is null!</exception>
        internal RemoteDatabaseMetaData(AceQLConnection aceQLConnection)
        {
            if (aceQLConnection == null)
            {
                throw new ArgumentNullException("aceQLConnection is null!");
            }
            this.aceQLHttpApi = aceQLConnection.GetAceQLHttpApi();
        }
        
        /// <summary>
        /// Downloads into a stream the schema of the remote database using HTML format.
        /// </summary>
        /// <returns>Task&lt;Stream&gt;.</returns>
        public async Task<Stream> DbSchemaDownloadAsync()
        {
            return await DbSchemaDownloadAsync(null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads into a stream the schema of the remote database, using the specified format.
        /// </summary>
        /// <param name="format">The format. "html" or "text". Defaults to "html" if null.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        public async Task<Stream> DbSchemaDownloadAsync(String format)
        {
            return await DbSchemaDownloadAsync(format, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads into a stream the schema of the remote database.
        /// </summary>
        /// <param name="format">The format. "html" or "text". Defaults to "html" if null.</param>
        /// <param name="tableName">the table name, without dot separator. Defaults to all tables if null.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        /// <exception cref="System.ArgumentException">Invalid format value. Must be \"html\" or \"text\". is: " + format</exception>
        public async Task<Stream> DbSchemaDownloadAsync(String format, String tableName)
        {
            if (format == null)
            {
                format = "html";
            }

            if (!format.Equals("html") && !format.Equals("text"))
            {
                throw new System.ArgumentException("Invalid format value. Must be \"html\" or \"text\". is: " + format);
            }

            return await aceQLHttpApi.DbSchemaDownloadAsync(format, tableName);
        }

        /// <summary>
        /// Gets the JDBC  meta data of the remote database.
        /// </summary>
        /// <returns>Task&lt;JdbcDatabaseMetaDataDto&gt;.</returns>
        public async Task<JdbcDatabaseMetaData> GetJdbcDatabaseMetaDataAsync()
        {
            JdbcDatabaseMetaDataDto jdbcDatabaseMetaDataDto = await aceQLHttpApi.GetDbMetadataAsync();
            return jdbcDatabaseMetaDataDto.JdbcDatabaseMetaData;
        }

        /// <summary>
        /// Returns the table names of the remote database.
        /// </summary>
        /// <returns>Task&lt;List&lt;TableName&gt;&gt;.</returns>
        public async Task<List<string>> GetTableNamesAsync()
        {
            TableNamesDto tableNamesDto = await aceQLHttpApi.GetTableNamesAsync(null);
            return tableNamesDto.TableNames;
        }

        /// <summary>
        /// Returns the table names of the remote database.
        /// </summary>
        /// <param name="tableType">tableType the table type. Can be null. Possible values: "table", "view", etc. Defaults to all types if null passed.</param>
        /// <returns>Task&lt;List&lt;TableName&gt;&gt;.</returns>
        public async Task<List<string>> GetTableNamesAsync(string tableType)
        {
            TableNamesDto tableNamesDto = await aceQLHttpApi.GetTableNamesAsync(tableType);
            return tableNamesDto.TableNames;
        }

        /// <summary>
        /// Returns a table object with all it's elements from the remote database.
        /// </summary>
        /// <param name="name">The name of the table without dot separator.</param>
        /// <returns>Task&lt;Table&gt;.</returns>
        /// <exception cref="ArgumentNullException">name is null!</exception>
        public async Task<Table> GetTableAsync(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name is null!");
            }
            TableDto tableDto = await aceQLHttpApi.GetTableAsync(name);
            Table table = tableDto.Table;
            return table;
        }
    }
}
