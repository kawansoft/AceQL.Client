using AceQL.Client.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests.Test
{


    public class ConnectionStringBuilder
    {

        private string connectionString = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringBuilder"/> class.
        /// </summary>
        public ConnectionStringBuilder()
        {
 
        }

        /// <summary>
        /// Adds the server.
        /// </summary>
        /// <param name="server">The server.</param>
        public void AddServer(string server)
        {
            connectionString += "Server=" + server + ";";
        }

        /// <summary>
        /// Adds the database.
        /// </summary>
        /// <param name="dbname">The dbname.</param>
        public void AddDatabase(String dbname)
        {
            connectionString += "Database=" + dbname + ";";
        }

        /// <summary>
        /// Adds the credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public void AddCredentials(string username, string password)
        {
            connectionString += $"Username={username}; Password={password};";
        }

        /// <summary>
        /// Adds the authenticated proy.
        /// </summary>
         public void AddAuthenticatedProy()
        {
            MyProxyInfo myProxyInfo = new MyProxyInfo();
            string proxyUsername = myProxyInfo.ProxyUsername;
            string proxyPassword = myProxyInfo.ProxyPassword;
            connectionString += $"ProxyUri=http://localhost:8081 ; ProxyUsername={proxyUsername}; ProxyPassword={proxyPassword};";
        }


        internal void AddAuthenticatedProyCredentialCache()
        {
            MyProxyInfo myProxyInfo = new MyProxyInfo(MyProxyInfo.NEOTUNNEL_SAVE_TXT);
            String username = myProxyInfo.ProxyUsername;
            String password = myProxyInfo.ProxyPassword;
            CredentialCache.DefaultNetworkCredentials.UserName = username;
            CredentialCache.DefaultNetworkCredentials.Password = password;

            AceQLConnection.SetDefaultWebProxy(WebRequest.GetSystemWebProxy());
            connectionString += $"UseCredentialCache=True;";

        }

        /// <summary>
        /// Adds EnableDefaultSystemAuthentication=True into connection string
        /// </summary>
        public void AddEnableDefaultSystemAuthentication()
        {
            connectionString += "EnableDefaultSystemAuthentication=True;";
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>System.String.</returns>
        public String GetConnectionString()
        {
            return connectionString;
        }

    }
}
