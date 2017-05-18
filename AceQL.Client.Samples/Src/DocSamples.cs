using AceQL.Client.Api;
using System;
using System.Collections.Generic;
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
            AceQLConnection connection = await connectionBuilderAsyncWithCredential();
            DocSamples docSamples = new DocSamples(connection);

            Console.WriteLine("Insert customer...");
            await docSamples.InsertCustomer();
        }



        /// <summary>
        /// RemoteConnection Quick Start client example.
        /// Creates a Connection to a remote database and open it.
        /// </summary>
        /// <returns>The connection to the remote database</returns>
        /// <exception cref="AceQLException">If any Exception occurs.</exception>
        public static async Task<AceQLConnection> connectionBuilderAsync()
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
        public static async Task<AceQLConnection> connectionBuilderAsyncWithCredential()
        {
    // Port number is the port number used to start the Web Server:
    String server = "http://localhost:9090/aceql";
    String database = "kawansoft_example";

    string connectionString = $"Server={server}; Database={database}";

    String username = "username";
    char[] password = GetFromUserInput();

    AceQLConnection connection = new AceQLConnection(connectionString);
    connection.Credential = new AceQLCredential(username, password);

    // Opens the connection with the remote database
    await connection.OpenAsync();

    return connection;
        }

        public static async Task useConnection()
        {
    string connectionString = null;
    AceQLConnection connection1 = null;

    try
    {
        connection1 = new AceQLConnection(connectionString);
        await connection1.OpenAsync();
        /// SQL stuff...
    }
    finally
    {
        await connection1.CloseAsync();
    }

    using (AceQLConnection connection = new AceQLConnection(connectionString))
    {
        // ... do stuff
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
    string sql = "insert into customer values " + "" +
            "(@parm1, @parm2, @parm3, @parm4, @parm5, @parm6, @parm7, @parm8)";

    AceQLCommand command = new AceQLCommand(sql, connection);
    command.Prepare(); // Optional

    command.Parameters.AddWithValue("@parm1", 1);
    command.Parameters.AddWithValue("@parm2", "Sir");
    command.Parameters.AddWithValue("@parm3", "Doe");
    command.Parameters.AddWithValue("@parm4", "John");
    // Alternate syntax
    command.Parameters.Add(new AceQLParameter("@parm5", "1 Madison Ave"));
    command.Parameters.AddWithValue("@parm6", "New York");
    command.Parameters.AddWithValue("@parm7", "NY 10010");
    command.Parameters.AddWithValue("@parm8", "+1 (212) 586-7000");

    // We don't know the phone number
    command.Parameters.AddWithNullValue("@parm8", SqlType.VARCHAR);

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
            using (AceQLTransaction transaction = await connection.BeginTransactionAsync())
            {
                string sql = "insert into customer values " + "" +
                    "(@parm1, @parm2, @parm3, @parm4, @parm5, @parm6, @parm7, @parm8)";

                try
                {
                    AceQLCommand command = new AceQLCommand(sql, connection);

                    command.Parameters.AddWithValue("@parm1", customerId);
                    command.Parameters.AddWithValue("@parm2", "Sir");
                    command.Parameters.AddWithValue("@parm3", "Doe");
                    command.Parameters.AddWithValue("@parm4", "John");
                    // Alternate syntax
                    command.Parameters.Add(new AceQLParameter("@parm5", "1 Madison Ave"));
                    command.Parameters.AddWithValue("@parm6", "New York");
                    command.Parameters.AddWithValue("@parm7", "NY 10010");
                    command.Parameters.AddWithNullValue("@parm8", SqlType.VARCHAR);

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
                    // No blob in our Quickstart
                    command.Parameters.AddWithNullValue("@jpeg_image", SqlType.BLOB);
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
        }

    }
}
