using AceQL.Client;
using AceQL.Client.Api;
using AceQL.Client.Api.File;
using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AceQL.Client.Examples
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

        public static async Task DoIt(string[] args)
        {

            AceQLConnection connection = null;

            try
            {
                int customerId = 1;
                int itemId = 1;

                using (connection = await remoteConnectionBuilderAsync().ConfigureAwait(false))
                {
                    MyRemoteConnection myRemoteConnection = new MyRemoteConnection(
                        connection);

                    // Delete previous instances, so that user can recall class
                    Console.WriteLine("deleting customer...");
                    await myRemoteConnection.DeleteCustomerAsync(customerId).ConfigureAwait(false);

                    Console.WriteLine("deleting orderlog...");
                    await myRemoteConnection.DeleteOrderlogAsync(customerId, itemId).ConfigureAwait(false);

                    await myRemoteConnection.InsertCustomerAndOrderLogAsync(customerId, itemId).ConfigureAwait(false);
                    await myRemoteConnection.SelectCustomerAndOrderLogAsync(customerId, itemId).ConfigureAwait(false);

                    Console.WriteLine();
                    Console.WriteLine("Press enter to close....");
                    Console.ReadLine();
                }

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
        /// Creates a Connection to a remote database.
        /// </summary>
        /// <returns>The connection to the remote database</returns>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public static async Task<AceQLConnection> remoteConnectionBuilderAsync()
        {
            // Port number is the port number used to start the Web Server:
            //String server = "http://www.aceql.com:9090/aceql";
            String server = "http://localhost:9090/aceql";
            String database = "kawansoft_example";

            // (username, password) for authentication on server side.
            // No authentication will be done for our Quick Start:
            String username = "MyUsername";
            String password = "MySecret";

            string connectionString = $"Server={server}; Database={database}; "
                + $"Username={username}; Password={password}";

            AceQLConnection connection = new AceQLConnection(connectionString);

            // Opens the connection with the remote database
            await connection.OpenAsync().ConfigureAwait(false);

            return connection;
        }

        /// <summary>
        /// RemoteConnection Quick Start client example.
        /// Creates a Connection to a remote database using a proxy.
        /// </summary>
        /// <returns>The connection to the remote database</returns>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public static async Task<AceQLConnection> remoteConnectionBuilderUsingProxyAsync()
        {
            // Port number is the port number used to start the Web Server:
            String server = "http://www.aceql.com:9090/aceql";
            String database = "kawansoft_example";

            // (username, password) for authentication on server side.
            // No authentication will be done for our Quick Start:
            String username = "MyUsername";
            String password = "MySecret";

            // Proxy will be detected, pass the auth info for proxy that require authentication:
            String proxyUri = "http://localhost:8080";
            String proxyUsername = "ndepomereu2";
            String proxyPassword = null;

            if (await ExistsAsync("AceQLPclFolder", "password.txt").ConfigureAwait(false))
            {
                IFile file = await GetFileAsync("AceQLPclFolder", "password.txt").ConfigureAwait(false);
                proxyPassword = await file.ReadAllTextAsync().ConfigureAwait(false);
            }
            
            string connectionString = $"Server={server}; Database={database}; "
                + $"Username={username}; Password={password};"
                + $"ProxyUri={proxyUri};" 
                + $"ProxyUsername={proxyUsername}; ProxyPassword={proxyPassword}";

            AceQLConnection connection = new AceQLConnection(connectionString);

            // Opens the connection with the remote database
            await connection.OpenAsync().ConfigureAwait(false);

            return connection;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connection">The AceQL connection to remote database.</param>
        private MyRemoteConnection(AceQLConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Example of 2 INSERT in the same transaction.
        /// </summary>
        /// <param name="customerId">The cutomer ID.</param>
        /// <param name="itemId">the item ID.</param>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public async Task InsertCustomerAndOrderLogAsync(int customerId, int itemId)
        {
            // Create a transaction
            using (AceQLTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
            {
                string sql = "insert into customer values " + "" +
                    "(@parm1, @parm2, @parm3, @parm4, @parm5, @parm6, @parm7, @parm8)";

                using (AceQLCommand command = new AceQLCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@parm1", customerId);
                    command.Parameters.AddWithValue("@parm2", "Sir");
                    command.Parameters.AddWithValue("@parm3", "Doe");
                    command.Parameters.AddWithValue("@parm4", "John");
                    // Alternate syntax
                    command.Parameters.Add(new AceQLParameter("@parm5", "1 Madison Ave"));
                    command.Parameters.AddWithValue("@parm6", "New York");
                    command.Parameters.AddWithValue("@parm7", "NY 10010");
                    command.Parameters.AddNullValue("@parm8", SqlType.VARCHAR);

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }

                sql = "insert into orderlog values " +
                            "(@customer_id, @item_id, @description, " +
                             "@item_cost, @date_placed, @date_shipped, " +
                             "@jpeg_image, @is_delivered, @quantity)";

                using (AceQLCommand command = new AceQLCommand(sql, connection))
                {
                    try
                    {
                        Console.WriteLine("insert into orderlog...");

                        command.Parameters.AddWithValue("@customer_id", customerId);
                        command.Parameters.AddWithValue("@item_id", itemId);
                        command.Parameters.AddWithValue("@description", "Item Description");
                        command.Parameters.AddWithValue("@item_cost", 99D);
                        command.Parameters.AddWithValue("@date_placed", DateTime.Now);
                        command.Parameters.AddWithValue("@date_shipped", DateTime.Now);
                        // No blob in our Quickstart
                        command.Parameters.AddNullValue("@jpeg_image", SqlType.BLOB);
                        command.Parameters.AddWithValue("@is_delivered", 1);
                        command.Parameters.AddWithValue("@quantity", 1);

                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        await transaction.CommitAsync().ConfigureAwait(false);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        await transaction.RollbackAsync().ConfigureAwait(false);
                    }
                }
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
            String sql = "select customer_id, fname, lname from customer "
                + " where customer_id = @customer_id";

            AceQLCommand command = new AceQLCommand(sql, connection);
            command.Parameters.AddWithValue("@customer_id", customerId);

            using (AceQLDataReader dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await dataReader.ReadAsync().ConfigureAwait(false))
                {
                    int i = 0;
                    int customerId2 = dataReader.GetInt32(i++);
                    String fname = dataReader.GetString(i++);
                    String lname = dataReader.GetString(i++);

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

            using (AceQLDataReader dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await dataReader.ReadAsync().ConfigureAwait(false))
                {
                    int i = 0;
                    int customerId2 = dataReader.GetInt32(i++);
                    int itemId2 = dataReader.GetInt32(i++);

                    String description = dataReader.GetString(i++);
                    Decimal costPrice = dataReader.GetDecimal(i++);

                    DateTime datePlaced = dataReader.GetDateTime(i++).Date;
                    DateTime dateShipped = dataReader.GetDateTime(i++);

                    Stream stream = await dataReader.GetStreamAsync(i++).ConfigureAwait(false); // null stream

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
            String sql = "delete from customer where customer_id = @customer_id";

            using (AceQLCommand command = new AceQLCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@customer_id", customerId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes an existing orderlog row.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="itemId">the item ID.</param>
        public async Task DeleteOrderlogAsync(int customerId, int idemId)
        {
            String sql = "delete from orderlog where customer_id = @customer_id and item_id = @item_id";

            using (AceQLCommand command = new AceQLCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@customer_id", customerId);
                command.Parameters.AddWithValue("@item_id", idemId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets an existing file.
        /// </summary>
        /// <param name="folderName">Name of the folder, without path separators.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>An existing file instance.</returns>
        /// <exception cref="System.IO.FileNotFoundException">If the folder does not exist or the file was not found in the specified folder.</exception>
        public static async Task<IFile> GetFileAsync(String folderName, String fileName)
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
            IFolder folder = await rootFolder.GetFolderAsync(folderName).ConfigureAwait(false);
            IFile file = await folder.GetFileAsync(fileName).ConfigureAwait(false);
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
                folder = await rootFolder.GetFolderAsync(folderName).ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            IFile file = null;

            try
            {
                file = await folder.GetFileAsync(fileName).ConfigureAwait(false);
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
