﻿
using AceQL.Client.api;
using AceQL.Client.api.file;
using PCLStorage;
using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Examples
{
    /// <summary>
    /// Tests AceQL client SDK by calling all APIs.
    /// </summary>
    class AceQLApiConnectionTests
    {
        public static void TheMain(string[] args)
        {
            try
            {
                DoIt(args).Wait();
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

            String IN_DIRECTORY = "c:\\test\\";
            String OUT_DIRECTORY = "c:\\test\\out\\";

#pragma warning disable CS0219 // Variable is assigned but its value is never used
            String serverUrlLocalhost = "http://localhost:9090/aceql";
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            String serverUrlLocalhostTomcat = "http://localhost:8080/aceql-test/aceql";
#pragma warning restore CS0219 // Variable is assigned but its value is never used
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            String serverUrlLinux = "http://www.aceql.com:9090/aceql";
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            String server = serverUrlLocalhost;
            String database = "kawansoft_example";
            String username = "username";
            String password = "password";

            String proxyUsername = "ndepomereu2";
            String proxyPassword = null;

            PortableFileInfo portableFileInfo = new PortableFileInfo("AceQLPclFolder");
            String dir = await portableFileInfo.GetDirectoryNameAsync();
            Console.WriteLine("AceQLPclFolder: " + dir);

            if (await PortableFile.ExistsAsync("AceQLPclFolder", "assword.txt"))
            {
                proxyPassword = await PortableFile.ReadAllTextAsync("AceQLPclFolder", "password.txt");
            }

            //customer_id integer NOT NULL,
            //customer_title character(4),
            //fname character varying(32),
            //lname character varying(32) NOT NULL,
            //addressline character varying(64),
            //town character varying(32),
            //zipcode character(10) NOT NULL,
            //phone character varying(32),

            string connectionString = $"Server={server}; Database={database}; Username={username}; Password={password}";
            connectionString += $"; ProxyUsername ={ proxyUsername}; ProxyPassword ={ proxyPassword}";

            AceQLConnection connection = new AceQLConnection(connectionString);

            await connection.OpenAsync();

            Console.WriteLine("aceQLConnection.GetClientVersion(): " + connection.GetClientVersion());
            Console.WriteLine("aceQLConnection.GetServerVersion(): " + await connection.GetServerVersionAsync());
            Console.WriteLine("AceQL local folder: ");
            Console.WriteLine(await AceQLConnection.GetAceQLLocalFolderAsync());

            AceQLTransaction transaction = await connection.BeginTransactionAsync();
            await transaction.CommitAsync();
            transaction.Dispose();

            String sql = "delete from customer";
            AceQLCommand command = new AceQLCommand(sql, connection);
            await command.ExecuteNonQueryAsync();

            for (int i = 0; i < 3; i++)
            {
                sql =
                "insert into customer values (@parm1, @parm2, @parm3, @parm4, @parm5, @parm6, @parm7, @parm8)";

                command = new AceQLCommand(sql, connection);

                int customer_id = i;

                command.Parameters.AddWithValue("@parm1", customer_id);
                command.Parameters.AddWithValue("@parm2", "Sir");
                command.Parameters.AddWithValue("@parm3", "André_" + customer_id);
                command.Parameters.Add(new AceQLParameter("@parm4", "Name_" + customer_id));
                command.Parameters.AddWithValue("@parm5", customer_id + ", road 6");
                command.Parameters.AddWithValue("@parm6", "Town_" + customer_id);
                command.Parameters.AddWithValue("@parm7", customer_id + "11111");
                command.Parameters.AddNullValue("@parm8", SqlType.VARCHAR); //null value for NULL SQL insert.

                await command.ExecuteNonQueryAsync();
            }

            command.Dispose();

            sql = "select * from customer";
            command = new AceQLCommand(sql, connection);
            using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
            {
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
                        + "GetValue: " + dataReader.GetValue(i++));
                }
            }

            command.Dispose();

            Console.WriteLine("Before delete from orderlog");

            // Do next delete in a transaction because of BLOB
            transaction = await connection.BeginTransactionAsync();

            sql = "delete from orderlog";
            command = new AceQLCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
            command.Dispose();

            await transaction.CommitAsync();

            // Do next inserts in a transaction because of BLOB
            transaction = await connection.BeginTransactionAsync();

            try
            {
                for (int j = 1; j < 4; j++)
                {
                    sql =
                    "insert into orderlog values (@parm1, @parm2, @parm3, @parm4, @parm5, @parm6, @parm7, @parm8, @parm9)";

                    command = new AceQLCommand(sql, connection);

                    int customer_id = j;

                    String blobPath = IN_DIRECTORY + "username_koala.jpg";
                    Stream stream = new FileStream(blobPath, FileMode.Open, System.IO.FileAccess.Read);
                    long length = new FileInfo(blobPath).Length;

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
                    command.Parameters.AddWithValue("@parm4", (double)j * 1000);
                    command.Parameters.AddWithValue("@parm5", DateTime.Now);
                    command.Parameters.AddWithValue("@parm6", DateTime.Now);
                    command.Parameters.AddBlob("@parm7", stream, length);
                    command.Parameters.AddWithValue("@parm8", 1);
                    command.Parameters.AddWithValue("@parm9", j * 2000);

                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    ProgressHolder progressHolder = new ProgressHolder();

                    connection.SetCancellationTokenSource(cancellationTokenSource);
                    connection.SetProgress(progressHolder);
                    
                    await command.ExecuteNonQueryAsync();

                }
                command.Dispose();
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

            sql = "select * from orderlog";
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
                    String blobPath = OUT_DIRECTORY + "username_koala_" + k + ".jpg";
                    k++;

                    using (Stream stream = await dataReader.GetStreamAsync(6).ConfigureAwait(false))
                    {
                        using (var fileStream = File.Create(blobPath))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }

            await transaction.CommitAsync();
            connection.Dispose();

            Console.WriteLine();
            Console.WriteLine("Press enter to close....");
            Console.ReadLine();

        }


    }
}
