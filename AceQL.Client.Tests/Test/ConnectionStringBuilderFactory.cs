using AceQL.Client.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests.Test
{
    /// <summary>
    /// Class ConnectionStringBuilderFactory. Creates default connection strings.
    /// </summary>
    public static class ConnectionStringBuilderFactory
    {
        /// <summary>
        /// The authenticated proxy off
        /// </summary>
        public static readonly int AUTHENTICATED_PROXY_OFF = 0;
        /// <summary>
        /// The no authenticated proxy with credentials
        /// </summary>
        public static readonly int AUTHENTICATED_PROXY_WITH_PASSED_CREDENTIALS = 1;
        /// <summary>
        /// The no authenticated proxy credential cache
        /// </summary>
        public static readonly int AUTHENTICATED_PROXY_CREDENTIAL_CACHE = 2;

        public static readonly string serverUrlLocalhost = "http://localhost:9090/aceql";
        public static readonly string serverUrlLocalhostTomcat = "http://localhost:8080/aceql-test/aceql";
        public static readonly string serverUrlLinuxNoSSL = "http://www.runsafester.net:8081/aceql";
        public static readonly string serverUrlLinux = "https://www.aceql.com:9443/aceql";

        public static readonly string usernameLdap = "cn=read-only-admin,dc=example,dc=com";
        public static readonly string passwordLdap = "password";

        /// <summary>
        /// Creates the default local connection to localhost:9090 with sampledb and  with user1 and password1 password
        /// </summary>
        /// </summary>
        /// <returns>connection string.</returns>
        public static String CreateDefaultLocal()
        {
            string database = "sampledb";
            string username = "user1";
            string password = "password1";
            return Create(serverUrlLocalhost, database, username, password, AUTHENTICATED_PROXY_OFF);
        }

        /// <summary>
        /// Creates the default connection with sampledb and with a LDAP authentication.
        /// Other value: username = "CN=L. Eagle,O=Sue\\2C Grabbit and Runn,C=GB";
        /// </summary>
        /// <returns>connection string.</returns>
        public static String CreateDefaultLocalLdapAuth()
        {
            string database = "sampledb";
            string username = usernameLdap;
            string password = passwordLdap;
            return Create(serverUrlLocalhost, database, username, password, AUTHENTICATED_PROXY_OFF);
        }

        /// <summary>
        /// Creates the default remote connection to http://www.runsafester.net:8081/aceql with sampledb and with user1 and password1 password
        /// </summary>
        /// <param name="useDefaultAuthenticatedProxy">if set to <c>true</c> [use default authenticated proxy].</param>
        /// <returns>connection string.</returns>
        public static String CreateDefaultRemote(int typeAuthenticatedProxy)
        {
            string database = "sampledb";
            string username = "user1";
            string password = "password1";
            return Create(serverUrlLinuxNoSSL, database, username, password, typeAuthenticatedProxy);
        }

        /// <summary>
        /// Creates the default connection string  to http://www.runsafester.net:8081/aceql with sampledb and with user1 and password1 password
        /// </summary>
        /// <param name="useDefaultAuthenticatedProxy">if set to <c>true</c> [use default authenticated proxy].</param>
        /// <returns>connection string.</returns>
        public static String CreateDefaultRemoteLdapAuth(int typeAuthenticatedProxy)
        {
            string database = "sampledb";
            string username = usernameLdap;
            string password = passwordLdap;
            return Create(serverUrlLinuxNoSSL, database, username, password, typeAuthenticatedProxy);
        }

        /// <summary>
        /// Creates the connection string  with the passed parameters
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="database">The database.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="useAuthenticatedProxy">if set to <c>true</c> [use authenticated proxy].</param>
        /// <returns>AceQLConnection.</returns>
        public static String Create(string server, string database, string username, string password, int typeAuthenticatedProxy)
        {
            Boolean enableDefaultSystemAuthentication = false;

            ConnectionStringBuilder connectionStringBuilder = new ConnectionStringBuilder();
            connectionStringBuilder.AddServer(server);
            connectionStringBuilder.AddDatabase(database);
            connectionStringBuilder.AddCredentials(username, password);

            if (enableDefaultSystemAuthentication)
            {
                connectionStringBuilder.AddEnableDefaultSystemAuthentication();
            }

            if (typeAuthenticatedProxy == AUTHENTICATED_PROXY_WITH_PASSED_CREDENTIALS)
            {
                connectionStringBuilder.AddAuthenticatedProy();
            }
            else if (typeAuthenticatedProxy == AUTHENTICATED_PROXY_CREDENTIAL_CACHE)
            {
                connectionStringBuilder.AddAuthenticatedProyCredentialCache();
            }

            string connectionString = connectionStringBuilder.GetConnectionString();
            return connectionString;
        }
    }
}
