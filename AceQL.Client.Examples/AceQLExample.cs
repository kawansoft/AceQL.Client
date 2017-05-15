
using AceQL.Client.Api;
using AceQL.Client.Api.File;
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
    class AceQLExample
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

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(ACEQL_PCL_FOLDER,
                CreationCollisionOption.OpenIfExists);
            Console.WriteLine("AceQLPclFolder: " + folder.Path);

            if (await ExistsAsync(ACEQL_PCL_FOLDER, "password.txt"))
            {
                IFile file = await GetFileAsync("AceQLPclFolder", "password.txt");
                proxyPassword = await file.ReadAllTextAsync();
            }

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
            connectionString += $"; ProxyUsername ={proxyUsername}; ProxyPassword ={ proxyPassword}";

            AceQLConnection.SetTraceOn(true);

            // Make sure connection is always closed to close and release server connection into the pool
            using (AceQLConnection connection = new AceQLConnection(connectionString))
            {
                connection.Credential = new AceQLCredential(username, password);
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
            String IN_DIRECTORY = "c:\\test\\";
            String OUT_DIRECTORY = "c:\\test\\out\\";

            await connection.OpenAsync();

            Console.WriteLine("aceQLConnection.GetClientVersion(): " + connection.GetClientVersion());
            Console.WriteLine("aceQLConnection.GetServerVersion(): " + await connection.GetServerVersionAsync());
            Console.WriteLine("AceQL local folder: ");
            Console.WriteLine(await AceQLConnection.GetAceQLLocalFolderAsync());

            AceQLTransaction transaction = await connection.BeginTransactionAsync();
            await transaction.CommitAsync();
            transaction.Dispose();

            String sql = "delete from customer";

            AceQLCommand command = new AceQLCommand();
            command.CommandText = sql;
            command.Connection = connection;

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

            // Our dataReader must be disposed to delete underlying dowloaded files
            using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
            {

                while (await dataReader.ReadAsync(new CancellationTokenSource().Token))
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

                    ProgressIndicator progressIndicator = new ProgressIndicator();
                    connection.SetProgressIndicator(progressIndicator);

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

                    using (Stream stream = await dataReader.GetStreamAsync(6))
                    {
                        using (var fileStream = File.Create(blobPath))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }

            await transaction.CommitAsync();
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
