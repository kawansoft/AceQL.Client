using AceQL.Client.Api.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api.Http
{
    /// <summary>
    /// Class ConnectionStringDecoder. Allows to decode the elemeents of the connection string.
    /// </summary>
    internal class ConnectionStringDecoder
    {
        internal static bool DEBUG = false;
        private const string ESCAPED_SEMICOLON_WORD = "\\semicolon";
        private const string ESCAPED_SEMICOLON = "\\;";

        private string server = null;
        private string database = null;
        private string username = null;
        private char[] password = null;
        private string sessionId = null;
        private string proxyUri = null;
        private ICredentials proxyCredentials = null;
        private int timeout = 0;
        private bool enableDefaultSystemAuthentication = false;
        private bool enableTrace;


        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringDecoder"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">connectionString has not been set and is null!</exception>
        public ConnectionStringDecoder()
        {

        }

        /// <summary>
        /// Decodes the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentException">connectionString does not contain a ; separator: " + connectionString</exception>
        /// <exception cref="ArgumentException">connectionString element token does not contain a = separator: " + line</exception>
        /// <exception cref="ArgumentException">Can not find left of = in connectionString element token. Please correct connection string: " + line</exception>
        /// <exception cref="ArgumentException">Can not find right of = in connectionString element token. Please correct connection string: " + line</exception>
        public void Decode(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString has not been set and is null!");
            }

            // Replace escaped "\;"
            connectionString = connectionString.Replace(ESCAPED_SEMICOLON, ESCAPED_SEMICOLON_WORD);

            bool isNTLM = false;
            String proxyUsername = null;
            String proxyPassword = null;

            int theTimeout = 0;

            string[] lines = connectionString.Split(';');

            if (lines.Length < 2)
            {
                throw new ArgumentException("connectionString does not contain a ; separator: " + connectionString);
            }

            foreach (string line in lines)
            {
                // If some empty ;
                if (line.Trim().Length <= 2)
                {
                    continue;
                }

                if (!line.Contains("="))
                {
                    throw new ArgumentException("connectionString element token does not contain a = separator: " + line);
                }

                String property = line.Trim().Substring(0, line.Trim().IndexOf("="));
                String value = line.Trim().Substring(line.Trim().IndexOf("=") + 1);

                // Security check
                if (property == null)
                {
                    throw new ArgumentException("Can not find left of = in connectionString element token. Please correct connection string: " + line);
                }
                if (value == null)
                {
                    throw new ArgumentException("Can not find right of = in connectionString element token. Please correct connection string: " + line);
                }

                property = property.Trim();
                value = value.Trim();

                ConsoleEmul.WriteLine("property: " + property + " (value: " + value + ")");

                if (property.ToLower().Equals("server"))
                {
                    server = value;
                }
                else if (property.ToLower().Equals("database"))
                {
                    database = value;
                }
                else if (property.ToLower().Equals("username"))
                {
                    value = value.Replace("\\semicolon", ";");
                    username = value;
                }
                else if (property.ToLower().Equals("password"))
                {
                    value = value.Replace("\\semicolon", ";");
                    password = value.ToCharArray();
                }
                else if (property.ToLower().Equals("ntlm"))
                {
                    isNTLM = Boolean.Parse(value);
                }
                else if (property.ToLower().Equals("proxyuri"))
                {

                    proxyUri = value;
                    // Set to null a "null" string
                    if (proxyUri.ToLower().Equals("null") || proxyUri.Length == 0)
                    {
                        proxyUri = null;
                    }
                    ConsoleEmul.WriteLine("proxyUri:" + proxyUri + ":");
                }
                else if (property.ToLower().Equals("proxyusername"))
                {
                    value = value.Replace(ESCAPED_SEMICOLON_WORD, ";");
                    proxyUsername = value;

                    // Set to null a "null" string
                    if (proxyUsername.ToLower().Equals("null") || proxyUsername.Length == 0)
                    {
                        proxyUsername = null;
                    }

                }
                else if (property.ToLower().Equals("proxypassword"))
                {
                    value = value.Replace("\\semicolon", ";");
                    proxyPassword = value;

                    // Set to null a "null" string
                    if (proxyPassword.ToLower().Equals("null") || proxyPassword.Length == 0)
                    {
                        proxyPassword = null;
                    }
                }
                else if (property.ToLower().Equals("sessionid"))
                {
                    sessionId = value;
                }
                else if (property.ToLower().Equals("enabledefaultsystemauthentication"))
                {
                    enableDefaultSystemAuthentication = Boolean.Parse(value);
                }
                else if (property.ToLower().Equals("timeout"))
                {
                    theTimeout = Int32.Parse(value);
                }
                else if (property.ToLower().Equals("enableTrace"))
                {
                    enableTrace = Boolean.Parse(value);
                }
            }

            Debug("connectionString   : " + connectionString);
            Debug("theProxyUri        : " + proxyUri);
            Debug("theProxyCredentials: " + proxyUsername + " / " + proxyPassword);

            if (isNTLM)
            {
                proxyCredentials = CredentialCache.DefaultCredentials;
            }
            else
            {
                if (proxyUsername != null && proxyPassword != null)
                {
                    proxyCredentials = new NetworkCredential(proxyUsername, proxyPassword);
                }
            }

        }


        public string Server { get => server;  }
        public string Database { get => database; }
        public string Username { get => username; }
        public char[] Password { get => password; }
        public string SessionId { get => sessionId; }
        public string ProxyUri { get => proxyUri; }
        public ICredentials ProxyCredentials { get => proxyCredentials;  }
        public int Timeout { get => timeout;  }

        public bool EnableDefaultSystemAuthentication { get => enableDefaultSystemAuthentication; }

        public bool EnableTrace { get => enableTrace; }

        internal static void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }
    }
}
