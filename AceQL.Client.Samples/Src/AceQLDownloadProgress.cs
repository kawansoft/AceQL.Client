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
using AceQL.Client.Api.File;
using PCLStorage;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Samples
{
    /// <summary>
    /// Tests AceQL client SDK by calling all APIs.
    /// </summary>
    class AceQLDownloadProgress
    {
        private const string ACEQL_PCL_FOLDER = "AceQLPclFolder";

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

#pragma warning disable CS0219 // Variable is assigned but its value is never used
            string serverUrlLocalhost = "http://localhost:9090/aceql";
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            string serverUrlLocalhostTomcat = "http://localhost:8080/aceql-test/aceql";
#pragma warning restore CS0219 // Variable is assigned but its value is never used
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            string serverUrlLinux = "https://www.aceql.com:9443/aceql";
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            string server = serverUrlLinux;
            string database = "sampledb";
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
            //Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string IN_DIRECTORY = "c:\\test\\";
            string OUT_DIRECTORY = "c:\\test\\out\\";

            await connection.OpenAsync();

            Console.WriteLine("aceQLConnection.GetClientVersion(): " + connection.GetClientVersion());
            Console.WriteLine("aceQLConnection.GetServerVersion(): " + await connection.GetServerVersionAsync());
            Console.WriteLine("AceQL local folder: ");
            Console.WriteLine(await AceQLConnection.GetAceQLLocalFolderAsync());

            AceQLTransaction transaction = await connection.BeginTransactionAsync();
            await transaction.CommitAsync();
            transaction.Dispose();

            Console.WriteLine("Before delete from orderlog");

            // Do next delete in a transaction because of BLOB
            transaction = await connection.BeginTransactionAsync();

            string sql = "delete from orderlog";
            AceQLCommand command = new AceQLCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
            command.Dispose();

            await transaction.CommitAsync();

            // Do next inserts in a transaction because of BLOB
            transaction = await connection.BeginTransactionAsync();

            Console.WriteLine("Before insert into orderlog");
            try
            {
                sql =
                "insert into orderlog values (@parm1, @parm2, @parm3, @parm4, @parm5, @parm6, @parm7, @parm8, @parm9)";

                command = new AceQLCommand(sql, connection);

                int customer_id = 1;

                string blobPath = IN_DIRECTORY + "username_koala.jpg";
                Stream stream = new FileStream(blobPath, FileMode.Open, System.IO.FileAccess.Read);

                //customer_id integer NOT NULL,
                //item_id integer NOT NULL,
                //description character varying(64) NOT NULL,
                //cost_price numeric,
                //date_placed date NOT NULL,
                //date_shipped timestamp without time zone,
                //jpeg_image oid,
                //is_delivered numeric,
                //quantity integer NOT NULL,

                command.Parameters.AddWithValue("@parm1", customer_id);
                command.Parameters.AddWithValue("@parm2", customer_id);
                command.Parameters.AddWithValue("@parm3", "Description_" + customer_id);

                command.Parameters.Add(new AceQLParameter("@parm4", new AceQLNullValue(AceQLNullType.DECIMAL))); //null value for NULL SQL insert.

                command.Parameters.AddWithValue("@parm5", DateTime.Now);
                command.Parameters.AddWithValue("@parm6", DateTime.Now);
                // Adds the Blob. (Stream will be closed by AceQLCommand)
                command.Parameters.AddWithValue("@parm7", stream);
                command.Parameters.AddWithValue("@parm8", 1);
                command.Parameters.AddWithValue("@parm9", 1* 2000);

                await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync();
                throw exception;
            }

            Console.WriteLine("Before select *  from orderlog");

            // Do next selects in a transaction because of BLOB
            transaction = await connection.BeginTransactionAsync();

            sql = "select * from orderlog where cutomer_id = 1 and order_id = 1";
            command = new AceQLCommand(sql, connection);

            using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
            {
                int k = 0;
                while (dataReader.Read())
                {
                    Console.WriteLine();
                    int i = 0;
                    Console.WriteLine("GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++));

                    // Download Blobs
                    string blobPath = OUT_DIRECTORY + "username_koala_" + k + ".jpg";
                    k++;

                    using (Stream stream = await dataReader.GetStreamAsync(6))
                    {
                        using (var fileStream = File.Create(blobPath))
                        {
                            //stream.CopyTo(fileStream);
                            CopyStream(stream, fileStream);
                        }


                    }
                }
            }

            await transaction.CommitAsync();
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

    }
}
