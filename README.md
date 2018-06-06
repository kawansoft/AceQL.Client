# AceQL HTTP 2.0 - C# Client SDK

<img src="https://www.aceql.com/favicon.png" alt="AceQ HTTP Icon"/>

   * [Fundamentals](#fundamentals)
      * [Technical operating environment – Portable Class Library](#technical-operating-environment--portable-class-library)
      * [License](#license)
      * [AceQL Server side compatibility](#aceql-server-side-compatibility)
      * [AceQL C# Client SDK installation](#aceql-c-client-sdk-installation)
      * [Data transport](#data-transport)
         * [Transport format](#transport-format)
         * [Content streaming and memory management](#content-streaming-and-memory-management)
      * [Best practices for fast response time](#best-practices-for-fast-response-time)
   * [Implementation Info](#implementation-info)
      * [The AceQL SDK classes and methods](#the-aceql-sdk-classes-and-methods)
         * [Asynchronous implementation](#asynchronous-implementation)
      * [Data types](#data-types)
   * [Using the AceQL C# Client SDK](#using-the-aceql-c-client-sdk)
      * [The connection string](#the-connection-string)
         * [Using NTLM](#using-ntlm)
         * [Using a Web Proxy](#using-a-web-proxy)
      * [Handling Exceptions](#handling-exceptions)
         * [The error type](#the-error-type)
         * [Most common AceQL server messages](#most-common-aceql-server-messages)
         * [HTTP Status Codes](#http-status-codes)
      * [AceQLConnection: Connection Creation &amp; Close](#aceqlconnection-connection-creation--close)
      * [AceQLCommand: executing SQL statements](#aceqlcommand-executing-sql-statements)
         * [Inserting NULL values](#inserting-null-values)
      * [AceQLDataReader: getting queries result](#aceqldatareader-getting-queries-result)
         * [Reading NULL values](#reading-null-values)
      * [AceQLTransaction](#aceqltransaction)
         * [Precisions on transactions](#precisions-on-transactions)
      * [BLOB management](#blob-management)
         * [BLOB creation](#blob-creation)
         * [BLOB reading](#blob-reading)
      * [Managing BLOB upload progress](#managing-blob-upload-progress)

# Fundamentals 

This document describes how to use the AceQL C# Client SDK and gives some details about how it operates with the AceQL Server side.

The SDK allows you to wrap the [AceQL HTTP APIs](https://github.com/kawansoft/aceql-http/blob/master/aceql-http-1.0-user-guide-api.md) and eliminate the tedious work of handling communication errors and parsing JSON results.

C# application developers can access remote SQL databases and/or SQL databases in the cloud by simply including standard C# SQL calls in their code, just like they would do for an SQL Server database. There is zero learning curve and usage is straight forward.

The AceQL Server operation is described in [AceQL HTTP Server Installation and Configuration Guide](https://github.com/kawansoft/aceql-http/blob/master/aceql-http-1.0-user-guide-server.md), whose content is sometimes referred to in this User Guide. 

On the remote side, like the AceQL Server access to the SQL database using Java JDBC, we will sometimes use the JDBC terminology (ResultSet, etc.) in this document. Nevertheless, knowledge of Java or JDBC is *not* a requirement.

## Technical operating environment – Portable Class Library

The AceQL C# Client SDK is entirely written in C# and is packaged as a Portable Class Library, which works on Windows Desktop and on Android, iOS & macOS with Xamarin.

The targets of the library are:

- NetFramework 4.5
- ASP.NETCore 1.0
- Windows8
- Xamarin.Android
- Xamarin.iOS/ Xamarin.iOS Classic
- Xamarin.Mac

## License

The SDK is licensed with the liberal [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0) license.

## AceQL Server side compatibility

This 2.0 SDK version is compatible with AceQL HTTP v2.0 server version. It does not support previous version AceQL HTTP v1.0.

## AceQL C# Client SDK installation

Install the [NuGet Package](https://www.nuget.org/packages/AceQL.Client).

## Data transport

### Transport format

AceQL transfers the least possible amount of meta-information:

- Request parameters are transported in UTF-8 format
- JSON format is used for data and class transport (using [Newtonsoft.Json](https://www.newtonsoft.com/json) NuGet package).

### Content streaming and memory management

All requests are streamed:

- Output requests (from the client side)   are streamed directly from the socket to the server to avoid buffering any content body
- Input responses (for the client side)  are streamed directly from the socket to the server to efficiently read  the response body.

Large content (query results, Blobs/Clobs, etc.) is transferred using files. It is never loaded in memory. Streaming techniques are always used to read and write this content.

## Best practices for fast response time

Every HTTP exchange between the client and server side is time-consuming, because the client side  waits for the server's response.

Try to avoid coding SQL calls inside loops, as this can reduce execution speed. Each SQL call will send an http request and wait for the response from the server.

Note that AceQL is optimized as much as possible:

- A SELECT call returning a huge data volume will not consume memory on the server or client side:  AceQL uses input stream and output stream I/O for data  transfer.
- Server JDBC `ResultSet` retrieval is as fast as possible:
  - The `ResultSet` creation is done once on the server by the `executeQuery` order.
  - The rows are all dumped at once on the servlet output stream by the server
  - The client side gets the `ResultSet` content as a file.
  - All data reading commands are executed locally on the client side with forward-only reading

# Implementation Info

## The AceQL SDK classes and methods

Using the SDK classes is as intuitive and straight forward as possible.

Most SDK class names are the equivalent of Microsoft SQL Server `System.Data.SqlClient` namespace: they share the same suffix name for the classes, and the same method names.

Here is the correspondence table for the common classes:

| AceQL Client  <br />AceQL.Client.Api namespace | SQL Server Client  <br />System.Data.SqlClient namespace |
| ---------------------------------------- | :--------------------------------------- |
| AceQLCommand                             | SqlCommand                               |
| AceQLConnection                          | SqlConnection                            |
| AceQLCredential                          | SqlCredential                            |
| AceQLDataReader                          | SqlDataReader                            |
| AceQLParameter                           | SqlParameter                             |
| AceQLParameterCollection                 | SqlParameterCollection                   |
| AceQLTransaction                         | SqlTransaction                           |
| IsolationLevel                           | IsolationLevel                           |

N.B: Because the AceQL SDK is a Portable Class Library, the `System.Data` namespace is not available. Thus, the AceQL SDK does not implement the `System.Data.Db*` classes available on Windows Desktop.

The AceQL SDK exposes 2 specific public classes and 1 enumeration:

| Name                   | Role                                     |
| ---------------------- | ---------------------------------------- |
| AceQLException         | Generic Exception implementation  for error reporting.  <br />See [Handling Exceptions](#handling-exceptions). |
| AceQLNullType          | Enumeration that allows you to define the type of NULL  values for database updates.  See [Inserting NULL values](#inserting-null-values). |
| AceQLProgressIndicator | Allows you to  retrieve Blob upload progress as a percentage. See [Managing BLOB upload progress](#managing-blob-upload-progress). |

### Asynchronous implementation 

All SQL calls methods are asynchronous only.

The reason for this is that the AceQL SDK uses the Microsoft `System.Net.Http.HttpClient` class for all HTTP operations, which is entirely asynchronous.

All calls are cancelable before the defined `HttpClient` timeout, as all Async methods offer an overloaded version with a `System.Threading.CancellationToken` parameter.

Example in `AceQLCommand`:

```c#
// Versions of ExecuteNonQueryAsync
public async Task<int> ExecuteNonQueryAsync();
public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken);
```

Note that cancellation action impacts only the methods calls of the `System.Net.Http.HttClient` class. Cancellation action has no impact on remote SQL database state.  

## Data types

The main server side JDBC data types for columns are supported: 

Boolean, Blob/Clob, Integer, Short, Double, Float, BigDecimal, Long, String, Date, Time, Timestamp, URL and Array.

Note that the AceQL SDK does not allow you to specify data types to use; data types are implicitly chosen with the `AceQLParameter` values.

# Using the AceQL C# Client SDK

## The connection string

The standard connection string is:

```c#
"Server= https://www.acme.com:9443/aceql; Database = myDataBase; Username =myUsername; Password = myPassword"
```

Where:

- The Server value is the AceQL Server servlet path and includes the port if necessary
- The Username and Password are used for authentication by the remote AceQL Server
- The Database value is the name of the remote database to use for the session. See [AceQL HTTP Server Installation and Configuration Guide](https://github.com/kawansoft/aceql-http/blob/master/aceql-http-1.0-user-guide-server.md) for more information.

### Using NTLM

You can specify using NTLM with `NTLM=true` 

```c#
"Server= https://www.acme.com:9443/aceql; Database = myDataBase; Username =myUsername; Password = myPassword; NTLM=true"
```

### Using a Web Proxy

`System.Net.WebRequest.DefaultWebProxy` value is used by default.

You can specify the credentials of a proxy that requires authentication with `ProxyUsernme` and `ProxyPassword`:

```c#
"Server= https://www.acme.com:9443/aceql; Database = myDataBase; Username= myUsername; Password = myPassword; ProxyUsername=MyProxyUser; ProxyPassword=MyProxyPassword"
```

If `ProxyUri` is specified, the value will be used instead of the default `System.Net.WebRequest.DefaultWebProxy` value:

```c#
"Server= https://www.acme.com:9443/aceql; Database = myDataBase; Username =myUsername; Password = myPassword; ProxyUri=http://localhost:8080 ProxyUsername=proxyuser1;ProxyPassword= proxyuser1"
```

Read/Write http timeout may be specified with `Timeout` in milliseconds:

```c#
"Server = https://www.acme.com:9443/aceql;Database = myDataBase; Username = myUsername; Password = myPassword;Timeout=300000"
```

If Timeout is not specified or equals 0, Microsoft  [HttpClient.Timeout](https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.timeout) default value will be used.

The semicolon";" character is supported in password, but must be escaped: 

```
Password = my\;Password; 
```

## Handling Exceptions

Except for `ArgumentNullException`, Exceptions thrown are always an instance of `AceQLException`.

The `AceQLException` contains 5 pieces of information: 

| Info             | Description                              |
| ---------------- | ---------------------------------------- |
| Reason           | The error message. Retrieved with `Reason` property. |
| Error Type       | See below for description. Retrieved with `ErrorType` property. |
| Exception        | The original Exception that is the cause, if any.<br />Retrieved with  `ExceptionCause` property. |
| Http Status Code | See below for description. Retrieved with `HttpStatusCode` property. |
| Server Exception | The Java Exception Stack Trace thrown on server side,  if any. <br />Retrieved with `RemoteStackTrace` property. |

### The error type

The error type allows you to get the type of error, and where the error occurred. It is retrieved with the `AceQLException.ErrorType` property.

| Error Type  Value | Description                              |
| ----------------- | :--------------------------------------- |
| 0                 | The error occurred locally on the client side.  See `HttpStatusCode` property for more info.  Typical cases: no Internet connection, proxy  authentication required. |
| 1                 | The error is due to a JDBC Exception.  It was raised by the remote JDBC Driver and is rerouted  by AceQL as is.  The JDBC error message is accessible via `Reason` property. Typical case: an error in the SQL statement.  Examples: wrong table or column name. |
| 2                 | The error was raised by the AceQL Server.  This means that the AceQL Server expected a value or  parameter that was not sent by the client side.  Typical cases: misspelling in URL parameter, missing  required request parameters,  JDBC  Connection expiration, etc. The detailed error message is accessible via `Reason` property.  See below for most common AceQL Server error  messages. |
| 3                 | The AceQL Server forbade the execution of the SQL  statement for a security reason.  For security reasons, `Reason` property gives access to voluntarily vague  details. |
| 4                 | The AceQL Server is on failure and raised an  unexpected Java Exception.  The stack trace is included and accessible via `RemoteStackTrace` property. |

### Most common AceQL server messages

| AceQL Sever  Error Messages   (AceQLException.ErrorType  = 2) |
| ------------------------------------------------------------ |
| AceQL main  servlet not found in path                        |
| An error occurred  during Blob download                      |
| An error occurred  during Blob upload                        |
| Blob directory  defined in `DatabaseConfigurator.getBlobDirectory()` does not exist |
| Connection is  invalidated (probably expired)                |
| Database does not  exist                                     |
| Invalid blob_id.  Cannot be used to create a file            |
| Invalid blob_id. No Blob corresponding to blob_id            |
| Invalid  session_id                                          |
| Invalid username  or password                                |
| No action found  in request                                  |
| Unable to get a  Connection                                  |
| Unknown SQL  action or not supported by software             |

### HTTP Status Codes

The Http StatusCode is accessible with the `AceQLException.HttpStatusCode` property.

The HTTP StatusCode is 200 (OK) on successful completion calls.

When an error occurs: 

If `errortype` is 0, the HTTP Status Code is returned by the client side and may take all possible values in a malformed HTTP call.

If `errortype` is > 0, the HTTP Status Code can take one the following values returned by the server side:

| HTTP  Status Code            | Description                              |
| ---------------------------- | ---------------------------------------- |
| 400  (BAD REQUEST)           | Missing element in URL path  <br />Missing request parameters   <br />All JDBC errors raised by the remote JDBC  Driver |
| 401  (UNAUTHORIZED)          | Invalid username or password in connect.<br />Invalid session_id. <br />The AceQL Server forbade the execution of  the SQL statement for security reasons . |
| 404 (NOT_FOUND)              | BLOB directory does not exist on server. <br /> BLOB file not found on server. |
| 500  (INTERNAL_SERVER_ERROR) | The AceQL Server is on failure and raised  an unexpected Java Exception. |

## AceQLConnection: Connection Creation & Close

Connection to a remote database is done with `AceQLConnection` class:

```C#
// Port number is the port number used to start the Web Server:
string server = "https://www.acme.com:9443/aceql";
string database = "kawansoft_example";

string username = "MyUsername";
string password = "MySecret";

string connectionString = $"Server={server}; Database={database}; "
    + $"Username={username}; Password={password}";

AceQLConnection connection = new AceQLConnection(connectionString);

// Opens the connection with the remote database
await connection.OpenAsync();
```
It’s possible to string the credential information out of the connection and use a more secure `AceQLCredential`: 

```C#
// Port number is the port number used to start the Web Server:
string server = "https://www.acme.com:9443/aceql";
string database = "kawansoft_example";

string connectionString = $"Server={server}; Database={database}";

string username = "username";
char[] password = GetFromUserInput();

AceQLConnection connection = new AceQLConnection(connectionString)
{
    Credential = new AceQLCredential(username, password)
};

// Opens the connection with the remote database
await connection.OpenAsync();
```
Connection should always be closed in order to close and release the remote server JDBC connection into the pool .

The preferred way is to explicitly call `CloseAsync`:

```C#
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
```
As `AceQLConnection` is disposable, an alternate way is to dispose the instance. It will call  `CloseAsync`,but as `System.IDisposable.Dipose()` is synchronous, call will *not* be awaited:

```C#
using (AceQLConnection connection = new AceQLConnection(connectionString))
{
    // SQL stuff...
}
```
Note that it’s possible to combine both in your code:  `CloseAsync()` may safely be called more than once.

## AceQLCommand: executing SQL statements

Let’s do an INSERT in the CUSTOMER table:

```C#
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
command.Parameters.AddWithValue("@phone", "+1 (212) 586-71XX");

int rows = await command.ExecuteNonQueryAsync();
```
Notes:

It is unnecessary to dispose an `AceQLCommand`. `AceQLCommand.Dispose` calls do nothing and `AceQLCommand` is Disposable for ease of existing code migration.

`Prepare` call is optional: statement will always be prepared on the server side if a statement has parameters. This is the way JDBC works.

### Inserting NULL values

NULL values are handled in a specific way, because the remote server must know the type of the NULL value.

We use the `AceQLNullType` enum to pass the type to the ``AceQLParameter`:

```c#
// We don't know the phone number
command.Parameters.Add(new AceQLParameter("@phone", AceQLNullType.VARCHAR));
```
## AceQLDataReader: getting queries result

Let’s do a query on a remote database:

```c#
string sql = "select * from customer";
AceQLCommand command = new AceQLCommand(sql, connection);

// Our dataReader should be disposed to delete underlying downloaded files
using (AceQLDataReader dataReader = await command.ExecuteReaderAsync())
{
    // Read is synchronous because all data to read are already downloaded
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
    }
}
```
### Reading NULL values

Use `AceQLDataReader.IsDBNull` if necessary to check if a value is NULL before accessing it:

```c#
    Console.WriteLine("Is phone NULL? : " + dataReader.IsDBNull(7));
```

For string columns (CHAR, VARCHAR, etc.), note that when `AceQLDataReader.IsDBNull` returns true, there is no way to differentiate if the remote column is really NULL or if the column’s value is the “NULL” string. This is a limitation in this AceQL SDK version.

## AceQLTransaction

The AceQL SDK supports SQL transactions:

```c#
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
```

### Precisions on transactions

It is unnecessary to assign an `AceQLTransaction` to an `AceQLCommand`. (Because server side JDBCs do not handle this type of behavior). Calls are accepted to ease existing code migration.

`AceQLTransaction` must be terminated by either a `RollbackAsync` or `CommitAsync` call. This is necessary to reset the server connection to `aucommit true`.

`AceQLTransaction.Dispose` calls do nothing and `AceQLTransaction`  is Disposable for ease of existing code migration.

## BLOB management

The AceQL SDK supports BLOB creation and reading. 

Methods are implemented using streaming techniques to keep low memory consumption.

CLOB are not supported in this version.

### BLOB creation

BLOB creation is supported by entering a `System.IO.Stream` to the `AceQLParameterCollection`:

```c#
// Create a transaction because some database engines require autocommit off
AceQLTransaction transaction = await connection.BeginTransactionAsync();

try
{
    string sql = "insert into orderlog values " +
            "(@customer_id, @item_id, @description, " +
            "@item_cost, @date_placed, @date_shipped, " +
                "@jpeg_image, @is_delivered, @quantity)";

    AceQLCommand command = new AceQLCommand(sql, connection);

    string userPath = 
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    string blobPath = userPath + "\\koala.jpg";
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

    Console.WriteLine("command.ExecuteNonQueryAsync()...");

    await command.ExecuteNonQueryAsync();
    await transaction.CommitAsync();
}
catch (Exception e)
{
    // Transaction must always be terminated by a CommitAsync() or RollbackAsync()
    await transaction.RollbackAsync();
    throw e;
}
```
### BLOB reading 

BLOB reading is supported through `AceQLDataReader.GetStreamAsync` call:

```c#
// Create a transaction because some database engines require autocommit off
AceQLTransaction transaction = await connection.BeginTransactionAsync();

try
{
    string sql = "select customer_id, item_id, jpeg_image from orderlog" +
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
            string blobPath = userPath + "\\koala_download.jpg";

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
```
## Managing BLOB upload progress

You may want to give your users a progress bar when uploading Blob(s).

The `AceQLProgressIndicator.Percent` property allows you to get the current percent of upload. Value will be incremented automatically during upload. 

To activate the update mechanism:

1/ Pass the long Blob length along the Stream value when setting the Blob parameter:

```c#
  command.Parameters.AddWithValue("@jpeg_image", stream, length);
```

2/ Create your `AceQLProgressIndicator` instance and enter it to the `AceQLConnection` instance just before `AceQLCommand.ExecuteNonQueryAsync`:

```c#
  AceQLProgressIndicator progressIndicator = new AceQLProgressIndicator();
  connection.SetProgressIndicator(progressIndicator);
```
You then can read `ProgressIndicator.Percent` property in your watching thread.

------


