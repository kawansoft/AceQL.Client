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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Samples.Src
{
    public class DocSamples
    {
        private AceQLConnection connection;

        public static void TheMain(string[] args)
        {
            try
            {
                DoIt(args).Wait();
                //DoIt(args).GetAwaiter().GetResult();

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
            Console.WriteLine("Building connection with credential...");
            AceQLConnection connection = await ConnectionBuilderAsyncWithCredential();
            DocSamples docSamples = new DocSamples(connection);

            await docSamples.DeleteCustomers();

            Console.WriteLine("Insert customer...");
            await docSamples.InsertCustomer();

            Console.WriteLine("display customer...");
            await docSamples.SelectCustomer();

            await docSamples.DeleteCustomers();
            await docSamples.DeleteOrderlogs();

            await docSamples.InsertCustomerAndOrderLogAsync(1, 1);

            await docSamples.DeleteOrderlogs();

            Console.WriteLine("Insert BLOB...");
            await docSamples.InsertBlob(1, 1);

            Console.WriteLine("Insert BLOB...");
            await docSamples.SelectBlob(1, 1);
        }

        private async Task SelectCustomer()
        {
            String sql = "select * from customer";
            AceQLCommand command = new AceQLCommand(sql, connection);

            // Our dataReader should be disposed to delete underlying downloaded files
            using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
            {
                // Read is synchronous  because all data to read are already downloaded
                // when AceQLDataReader instance is created. Read accesses a StreamReader.
                while (dataReader.Read())
                {
                    //customer_id integer     not null,
                    Console.WriteLine();
                    int i = 0;
                    Console.WriteLine("customer_id   : " + dataReader.GetValue(i++));
                    Console.WriteLine("customer_title: " + dataReader.GetValue(i++));
                    Console.WriteLine("fname         : " + dataReader.GetValue(i++));
                    Console.WriteLine("lname         : " + dataReader.GetValue(i++));
                    Console.WriteLine("addressline   : " + dataReader.GetValue(i++));
                    Console.WriteLine("town          : " + dataReader.GetValue(i++));
                    Console.WriteLine("zipcode       : " + dataReader.GetValue(i++));
                    Console.WriteLine("phone         : " + dataReader.GetValue(i++));

                    Console.WriteLine("Is phone NULL? : " + dataReader.IsDBNull(7));
                }
            }
        }

        private async Task DeleteCustomers()
        {
            String sql = "delete from customer";

            AceQLCommand command = new AceQLCommand(sql, connection);

            command.Prepare();

            await command.ExecuteNonQueryAsync();
        }

        private async Task DeleteOrderlogs()
        {
            String sql = "delete from orderlog";

            AceQLCommand command = new AceQLCommand(sql, connection);

            command.Prepare();

            await command.ExecuteNonQueryAsync();
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
            String server = "http://localhost:9090/aceql";
            String database = "kawansoft_example";

            String username = "MyUsername";
            String password = "MySecret";

            string connectionString = $"Server={server}; Database={database}; "
                + $"Username={username}; Password={password}";

            AceQLConnection connection = new AceQLConnection(connectionString);

            // Opens the connection with the remote database
            await connection.OpenAsync();

            return connection;
        }

        /// <summary>
        /// RemoteConnection Quick Start client example.
        /// Creates a Connection to a remote database and open it.
        /// </summary>
        /// <returns>The connection to the remote database</returns>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public static async Task<AceQLConnection> ConnectionBuilderAsyncWithCredential()
        {
            // Port number is the port number used to start the Web Server:
            String server = "http://localhost:9090/aceql";
            String database = "kawansoft_example";

            string connectionString = $"Server={server}; Database={database}";

            String username = "username";
            char[] password = GetFromUserInput();

            AceQLConnection connection = new AceQLConnection(connectionString)
            {
                Credential = new AceQLCredential(username, password)
            };

            // Opens the connection with the remote database
            await connection.OpenAsync();

            return connection;
        }

        public static async Task UseConnection()
        {
            string connectionString = null;
            AceQLConnection connection = null;

            try
            {
                connection = new AceQLConnection(connectionString);
                await connection.OpenAsync();
                // SQL stuff...
            }
            finally
            {
                await connection.CloseAsync();
            }


        }

        private static void UseConnection2()
        {
            string connectionString = null;
            using (AceQLConnection connection = new AceQLConnection(connectionString))
            {
                // SQL stuff...
            }
        }

        private static char[] GetFromUserInput()
        {
            return "password".ToArray();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connection">The AceQL connection to remote database.</param>
        public DocSamples(AceQLConnection connection)
        {
            this.connection = connection;
        }

        //command.Parameters.AddWithNullValue("@parm8", SqlType.VARCHAR);
        // 
        private async Task InsertCustomer()
        {
            //customer_id integer     not null,
            //customer_title  char(4)         null,
            //fname varchar(32)     null,
            //lname varchar(32) not null,
            //addressline varchar(64) not null,
            //town varchar(32) not null,
            //zipcode         char(10)    not null,
            //phone varchar(32)     null,

            string sql = "insert into customer values " + "" +
                    "(@customer_id, @customer_title, @fname, " +
                    "@lname, @addressline, @town, @zipcode, @phone)";

            AceQLCommand command = new AceQLCommand(sql, connection);
            command.Prepare(); // Optional

            command.Parameters.AddWithValue("@customer_id", 1);
            command.Parameters.AddWithValue("@customer_title", "Sir");
            command.Parameters.AddWithValue("@fname", "Doe");
            command.Parameters.AddWithValue("@lname", "John");
            // Alternate syntax
            command.Parameters.Add(new AceQLParameter("@addressline", "1 Madison Ave"));
            command.Parameters.AddWithValue("@town", "New York");
            command.Parameters.AddWithValue("@zipcode", "NY 10010");
            //command.Parameters.AddWithValue("@phone", "+1 (212) 586-71XX");

            // We don't know the phone number
            command.Parameters.Add(new AceQLParameter("@phone", AceQLNullType.VARCHAR));

            int rows = await command.ExecuteNonQueryAsync();

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

            string sql = "insert into customer values " +
                        "(@customer_id, @customer_title, @fname, " +
                    "@lname, @addressline, @town, @zipcode, @phone)";

            try
            {
                AceQLCommand command = new AceQLCommand(sql, connection)
                {
                    Transaction = transaction // Not required, will do nothing.
                };

                command.Parameters.AddWithValue("@customer_id", customerId);
                command.Parameters.AddWithValue("@customer_title", "Sir");
                command.Parameters.AddWithValue("@fname", "Doe");
                command.Parameters.AddWithValue("@lname", "John");
                command.Parameters.AddWithValue("@addressline", "1 Madison Ave");
                command.Parameters.AddWithValue("@town", "New York");
                command.Parameters.AddWithValue("@zipcode", "NY 10010");
                command.Parameters.Add(new AceQLParameter("@phone", AceQLNullType.VARCHAR));

                await command.ExecuteNonQueryAsync();

                sql = "insert into orderlog values " +
                            "(@customer_id, @item_id, @description, " +
                                "@item_cost, @date_placed, @date_shipped, " +
                                "@jpeg_image, @is_delivered, @quantity)";

                command = new AceQLCommand(sql, connection);

                Console.WriteLine("insert into orderlog...");

                command.Parameters.AddWithValue("@customer_id", customerId);
                command.Parameters.AddWithValue("@item_id", itemId);
                command.Parameters.AddWithValue("@description", "Item Description");
                command.Parameters.AddWithValue("@item_cost", 99D);
                command.Parameters.AddWithValue("@date_placed", DateTime.Now);
                command.Parameters.AddWithValue("@date_shipped", DateTime.Now);
                // No blob for now
                command.Parameters.Add(new AceQLParameter("@jpeg_image", AceQLNullType.BLOB));
                command.Parameters.AddWithValue("@is_delivered", 1);
                command.Parameters.AddWithValue("@quantity", 1);

                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                // Transaction must always be terminated by a CommitAsync() or RollbackAsync()
                await transaction.RollbackAsync();
                throw e;
            }

        }

        /// <summary>
        /// Example of an INSERT of a BLOB
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="itemId">the item ID.</param>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public async Task InsertBlob(int customerId, int itemId)
        {

            // Create a transaction because some database engines require autocommit off
            AceQLTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                String sql = "insert into orderlog values " +
                        "(@customer_id, @item_id, @description, " +
                        "@item_cost, @date_placed, @date_shipped, " +
                            "@jpeg_image, @is_delivered, @quantity)";

                AceQLCommand command = new AceQLCommand(sql, connection);

                string userPath =
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                String blobPath = userPath + "\\koala.jpg";
                Stream stream = new FileStream(blobPath, FileMode.Open, FileAccess.Read);

                Console.WriteLine("blobPath: " + blobPath);
                Console.WriteLine("insert into orderlog...");

                command.Parameters.AddWithValue("@customer_id", customerId);
                command.Parameters.AddWithValue("@item_id", itemId);
                command.Parameters.AddWithValue("@description", "Item Description");
                command.Parameters.AddWithValue("@item_cost", 99D);
                command.Parameters.AddWithValue("@date_placed", DateTime.Now);
                command.Parameters.AddWithValue("@date_shipped", DateTime.Now);
                command.Parameters.AddWithValue("@jpeg_image", stream);
                command.Parameters.AddWithValue("@is_delivered", 1);
                command.Parameters.AddWithValue("@quantity", 1);

                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                // Transaction must always be terminated by a CommitAsync() or RollbackAsync()
                await transaction.RollbackAsync();
                throw e;
            }

        }

        /// <summary>
        /// Example of an SELECT of a BLOB
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="itemId">the item ID.</param>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public async Task SelectBlob(int customerId, int itemId)
        {
    // Create a transaction because some database engines require autocommit off
    AceQLTransaction transaction = await connection.BeginTransactionAsync();

    try
    {
        String sql = "select customer_id, item_id, jpeg_image from orderlog" +
            " where customer_id =  @customer_id and item_id = @item_id";

        AceQLCommand command = new AceQLCommand(sql, connection);
        command.Parameters.AddWithValue("@customer_id", customerId);
        command.Parameters.AddWithValue("@item_id", itemId);

        using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
        {
            while (dataReader.Read())
            {
                int i = 0;
                Console.WriteLine("customer_id   : " + dataReader.GetValue(i++));
                Console.WriteLine("item_id: " + dataReader.GetValue(i++));

                string userPath =
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                String blobPath = userPath + "\\koala_download.jpg";

                Console.WriteLine("Creating file from server BLOB in: " + blobPath);

                // Download Blob
                using (Stream stream = await dataReader.GetStreamAsync(i++))
                {
                    using (var fileStream = File.Create(blobPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }

            await transaction.CommitAsync();
        }
    }
    catch (Exception e)
    {
        // Transaction must always be terminated by a CommitAsync() or RollbackAsync()
        await transaction.RollbackAsync();
        throw e;
    }

        }
    }
}
