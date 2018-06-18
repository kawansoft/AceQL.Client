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
using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AceQL.Client.Samples
{
    /// <summary>
    /// This example: 
    /// 1) Inserts a Customer and an Order on a remote database. 
    /// 2) Displays the inserted raws on the console with two SELECT executed on the remote database.
    /// </summary>
    class MyRemoteConnection
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
                int customerId = 1;
                int itemId = 1;

                AceQLConnection.SetTraceOn(true);

                // Make sure connection is always closed in order to close and release
                // server connection into the pool
                using (AceQLConnection connection = await ConnectionBuilderAsync())
                {
                    MyRemoteConnection myRemoteConnection = new MyRemoteConnection(
                        connection);

                    // Delete previous instances, so that user can recall 

                    Console.WriteLine("deleting customer...");
                    await myRemoteConnection.DeleteCustomerAsync(customerId);

                    Console.WriteLine("deleting orderlog...");
                    await myRemoteConnection.DeleteOrderlogAsync(customerId, itemId);

                    await myRemoteConnection.InsertCustomerAndOrderLogAsync(customerId, itemId);
                    await myRemoteConnection.SelectCustomerAndOrderLogAsync(customerId, itemId);

                    await connection.CloseAsync();
                    Console.WriteLine("The end...");
                }

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


        /// <summary>
        /// RemoteConnection Quick Start client example.
        /// Creates a Connection to a remote database and open it.
        /// </summary>
        /// <returns>The connection to the remote database</returns>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public static async Task<AceQLConnection> ConnectionBuilderAsync()
        {
            // Port number is the port number used to start the Web Server:
            string server = "https://www.aceql.com:9443/aceql";
            string database = "kawansoft_example";

            string connectionString = $"Server={server}; Database={database}";

            // (username, password) for authentication on server side.
            // No authentication will be done for our Quick Start:
            string username = "MyUsername";
            char[] password = { 'M', 'y', 'S', 'e', 'c', 'r', 'e', 't' };

            AceQLConnection connection = new AceQLConnection(connectionString)
            {
                Credential = new AceQLCredential(username, password)
            };

            // Opens the connection with the remote database.
            // On the server side, a JDBC connection is extracted from the connection 
            // pool created by the server at startup. The connection will remain ours 
            // during the session.
            await connection.OpenAsync();

            return connection;
        }


        /// <summary>
        /// RemoteConnection Quick Start client example.
        /// Creates a Connection to a remote database using a proxy.
        /// </summary>
        /// <returns>The connection to the remote database</returns>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public static async Task<AceQLConnection> RemoteConnectionBuilderUsingProxyAsync()
        {
            // Port number is the port number used to start the Web Server:
            string server = "http://www.aceql.com:9090/aceql";
            string database = "kawansoft_example";

            // (username, password) for authentication on server side.
            // No authentication will be done for our Quick Start:
            string username = "MyUsername";
            string password = "MySecret";

            // Proxy will be detected, pass the auth info for proxy that require authentication:
            string proxyUri = "http://localhost:8080";
            string proxyUsername = "ndepomereu2";
            string proxyPassword = null;

            if (await ExistsAsync("AceQLPclFolder", "password.txt"))
            {
                IFile file = await GetFileAsync("AceQLPclFolder", "password.txt");
                proxyPassword = await file.ReadAllTextAsync();
            }

            string connectionString = $"Server={server}; Database={database}; "
                + $"Username={username}; Password={password};"
                + $"ProxyUri={proxyUri};"
                + $"ProxyUsername={proxyUsername}; ProxyPassword={proxyPassword}";

            AceQLConnection connection = new AceQLConnection(connectionString);

            return connection;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connection">The AceQL connection to remote database.</param>
        public MyRemoteConnection(AceQLConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Example of 2 INSERT in the same transaction.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="itemId">the item ID.</param>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public async Task InsertCustomerAndOrderLogAsync(int customerId, int itemId)
        {
            // Create a transaction
            AceQLTransaction transaction = await connection.BeginTransactionAsync();

            string sql = "insert into customer values " + "" +
                "(@parm1, @parm2, @parm3, @parm4, @parm5, @parm6, @parm7, @parm8)";

            AceQLCommand command = new AceQLCommand(sql, connection);
            try
            {
                command.Parameters.AddWithValue("@parm1", customerId);
                command.Parameters.AddWithValue("@parm2", "Sir");
                command.Parameters.AddWithValue("@parm3", "Doe");
                command.Parameters.AddWithValue("@parm4", "John");
                // Alternate syntax
                command.Parameters.Add(new AceQLParameter("@parm5", "1 Madison Ave"));
                command.Parameters.AddWithValue("@parm6", "New York");
                command.Parameters.AddWithValue("@parm7", "NY 10010");
                command.Parameters.Add(new AceQLParameter("@parm8", new AceQLNullValue(AceQLNullType.VARCHAR)));

                await command.ExecuteNonQueryAsync();

                sql = "insert into orderlog values " +
                            "(@customer_id, @item_id, @description, " +
                                "@item_cost, @date_placed, @date_shipped, " +
                                "@jpeg_image, @is_delivered, @quantity)";

                command = new AceQLCommand(sql, connection);

                command.Parameters.AddWithValue("@customer_id", customerId);
                command.Parameters.AddWithValue("@item_id", itemId);
                command.Parameters.AddWithValue("@description", "Item Description");
                command.Parameters.AddWithValue("@item_cost", 99D);
                command.Parameters.AddWithValue("@date_placed", DateTime.Now);
                command.Parameters.AddWithValue("@date_shipped", DateTime.Now);
                // No blob in our Quick start
                command.Parameters.Add(new AceQLParameter("@jpeg_image",
                    AceQLNullType.BLOB));
                command.Parameters.AddWithValue("@is_delivered", 1);
                command.Parameters.AddWithValue("@quantity", 1);

                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw e;
            }
        }

        /// <summary>
        /// Example of 2 SELECT.
        /// </summary>
        /// <param name="customerId">The cutomer ID.</param>
        /// <param name="itemId">the item ID.</param>
        private async Task SelectCustomerAndOrderLogAsync(int customerId, int itemId)
        {
            // Display the created Customer:
            string sql = "select customer_id, fname, lname from customer "
                + " where customer_id = @customer_id";

            AceQLCommand command = new AceQLCommand(sql, connection);
            command.Parameters.AddWithValue("@customer_id", customerId);

            using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
            {
                while (dataReader.Read())
                {
                    int i = 0;
                    int customerId2 = dataReader.GetInt32(i++);
                    string fname = dataReader.GetString(i++);
                    string lname = dataReader.GetString(i++);

                    Console.WriteLine();
                    Console.WriteLine("customer_id : " + customerId2);
                    Console.WriteLine("fname       : " + fname);
                    Console.WriteLine("lname       : " + lname);
                }
            }

            sql = "select * from orderlog where customer_id = @customer_id and item_id = @item_id ";

            command = new AceQLCommand(sql, connection);
            command.Parameters.AddWithValue("@customer_id", customerId);
            command.Parameters.AddWithValue("@item_id", customerId);

            using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
            {
                while (dataReader.Read())
                {
                    int i = 0;
                    int customerId2 = dataReader.GetInt32(i++);
                    int itemId2 = dataReader.GetInt32(i++);

                    string description = dataReader.GetString(i++);
                    Decimal costPrice = dataReader.GetDecimal(i++);

                    DateTime datePlaced = dataReader.GetDateTime(i++).Date;
                    DateTime dateShipped = dataReader.GetDateTime(i++);

                    Stream stream = await dataReader.GetStreamAsync(i++); // null stream

                    bool is_delivered = dataReader.GetInt32(i++) == 1 ? true : false;
                    int quantity = dataReader.GetInt32(i++);

                    Console.WriteLine("customer_id : " + customerId2);
                    Console.WriteLine("item_id     : " + itemId2);
                    Console.WriteLine("description : " + description);
                    Console.WriteLine("cost_price  : " + costPrice);
                    Console.WriteLine("date_placed : " + datePlaced.Date);
                    Console.WriteLine("date_shipped: " + dateShipped);
                    Console.WriteLine("is_delivered: " + is_delivered);
                    Console.WriteLine("quantity    : " + quantity);
                }
            }


        }

        /// <summary>
        /// Deletes an existing customer row.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        public async Task DeleteCustomerAsync(int customerId)
        {
            string sql = "delete from customer where customer_id = @customer_id";

            using (AceQLCommand command = new AceQLCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@customer_id", customerId);
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Deletes an existing orderlog row.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="itemId">the item ID.</param>
        public async Task DeleteOrderlogAsync(int customerId, int idemId)
        {
            string sql = "delete from orderlog where customer_id = @customer_id and item_id = @item_id";

            using (AceQLCommand command = new AceQLCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@customer_id", customerId);
                command.Parameters.AddWithValue("@item_id", idemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Gets an existing file.
        /// </summary>
        /// <param name="folderName">Name of the folder, without path separators.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>An existing file instance.</returns>
        /// <exception cref="System.IO.FileNotFoundException">If the folder does not exist or the file was not found in the specified folder.</exception>
        public static async Task<IFile> GetFileAsync(string folderName, string fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.GetFolderAsync(folderName);
            IFile file = await folder.GetFileAsync(fileName);
            return file;
        }

        /// <summary>
        /// Says if a file exists in a folder.
        /// </summary>
        /// <param name="folderName">Name of the folder, without path separators.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>If the folder and the file exist, else false.</returns>
        /// <exception cref="System.ArgumentNullException">The file name or folder name is null.</exception>
        public static async Task<bool> ExistsAsync(string folderName, string fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFolder folder = null;
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                folder = await rootFolder.GetFolderAsync(folderName);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            IFile file = null;

            try
            {
                file = await folder.GetFileAsync(fileName);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            if (file == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
