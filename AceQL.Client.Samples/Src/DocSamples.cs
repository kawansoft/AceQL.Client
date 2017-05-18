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
            AceQLConnection connection = await ConnectionBuilderAsyncWithCredential();
            DocSamples docSamples = new DocSamples(connection);

            await docSamples.DeleteCustomers();

            Console.WriteLine("Insert customer...");
            await docSamples.InsertCustomer();


        }

        private async Task DeleteCustomers()
        {
            String sql = "delete from customer";

            AceQLCommand command = new AceQLCommand()
            {
                CommandText = sql,
                Connection = connection
            };
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

        private static async Task UseConnection2()
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

            string sql = "insert into customer values " + "" +
                "(@parm1, @parm2, @parm3, @parm4, @parm5, @parm6, @parm7, @parm8)";

            try
            {
                AceQLCommand command = new AceQLCommand(sql, connection);
                command.Transaction = transaction; // Not required, will do nothing.

                command.Parameters.AddWithValue("@parm1", customerId);
                command.Parameters.AddWithValue("@parm2", "Sir");
                command.Parameters.AddWithValue("@parm3", "Doe");
                command.Parameters.AddWithValue("@parm4", "John");
                // Alternate syntax
                command.Parameters.Add(new AceQLParameter("@parm5", "1 Madison Ave"));
                command.Parameters.AddWithValue("@parm6", "New York");
                command.Parameters.AddWithValue("@parm7", "NY 10010");
                command.Parameters.Add(new AceQLParameter("@parm8", AceQLNullType.VARCHAR)); //null value for NULL SQL insert.

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
                // No blob in our Quick start
                command.Parameters.Add(new AceQLParameter("@jpeg_image", AceQLNullType.BLOB));
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
