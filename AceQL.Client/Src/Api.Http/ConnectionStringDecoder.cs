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
        internal static readonly bool DEBUG;

        private const string ESCAPED_SEMICOLON_WORD = "\\semicolon";
        private const string ESCAPED_SEMICOLON = "\\;";

        private string server;
        private string database;
        private string username;
        private char[] password;
        private string sessionId;
        private string proxyUri;
        private ICredentials proxyCredentials;
        private int timeout;
        private bool enableDefaultSystemAuthentication;
        private bool enableTrace;
        private bool useCredentialCache;

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

            string[] lines = connectionString.Split(';');

            if (lines.Length < 2)
            {
                throw new ArgumentException("connectionString does not contain a ; separator: " + connectionString);
            }

            foreach (string line in lines)
            {
                // Do not trea empty lines
                if (line.Trim().Length <= 2)
                {
                    continue;
                }

                if (!line.Contains("="))
                {
                    throw new ArgumentException("connectionString element token does not contain a = separator: " + line);
                }

                String property = line.Trim().Substring(0, line.Trim().IndexOf("=", StringComparison.CurrentCulture));
                String value = line.Trim().Substring(line.Trim().IndexOf("=", StringComparison.CurrentCulture) + 1);

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

                if (property.ToLowerInvariant().Equals("server"))
                {
                    server = value;
                }
                else if (property.ToLowerInvariant().Equals("database"))
                {
                    database = value;
                }
                else if (property.ToLowerInvariant().Equals("username"))
                {
                    value = value.Replace("\\semicolon", ";");
                    username = value;
                }
                else if (property.ToLowerInvariant().Equals("password"))
                {
                    value = value.Replace("\\semicolon", ";");
                    password = value.ToCharArray();
                }
                else if (property.ToLowerInvariant().Equals("ntlm"))
                {
                    isNTLM = Boolean.Parse(value);
                }
                else if (property.ToLowerInvariant().Equals("usecredentialcache"))
                {
                    useCredentialCache = Boolean.Parse(value);
                }
                else if (property.ToLowerInvariant().Equals("proxyuri"))
                {
                    proxyUri = value;
                    // Set to null a "null" string
                    if (proxyUri.ToLowerInvariant().Equals("null") || proxyUri.Length == 0)
                    {
                        proxyUri = null;
                    }
                    ConsoleEmul.WriteLine("proxyUri:" + proxyUri + ":");
                }
                else if (property.ToLowerInvariant().Equals("proxyusername"))
                {
                    value = value.Replace(ESCAPED_SEMICOLON_WORD, ";");
                    proxyUsername = value;

                    // Set to null a "null" string
                    if (proxyUsername.ToLowerInvariant().Equals("null") || proxyUsername.Length == 0)
                    {
                        proxyUsername = null;
                    }

                }
                else if (property.ToLowerInvariant().Equals("proxypassword"))
                {
                    value = value.Replace("\\semicolon", ";");
                    proxyPassword = value;

                    // Set to null a "null" string
                    if (proxyPassword.ToLowerInvariant().Equals("null") || proxyPassword.Length == 0)
                    {
                        proxyPassword = null;
                    }
                }
                else if (property.ToLowerInvariant().Equals("sessionid"))
                {
                    sessionId = value;
                }
                else if (property.ToLowerInvariant().Equals("enabledefaultsystemauthentication"))
                {
                    enableDefaultSystemAuthentication = Boolean.Parse(value);
                }
                else if (property.ToLowerInvariant().Equals("timeout"))
                {
                    timeout = Int32.Parse(value);
                    if (timeout < 0)
                    {
                        throw new ArgumentException("timeout cannot be < 0");
                    }

                }
                else if (property.ToLowerInvariant().Equals("enabletrace"))
                {
                    enableTrace = Boolean.Parse(value);
                }
                else if (property.ToLowerInvariant().Equals("maxretryforhttpstatus407"))
                {
                    int maxRetryForHttpStatus407 = Int32.Parse(value);

                    if (maxRetryForHttpStatus407 < 0)
                    {
                        throw new ArgumentException("maxRetryForHttpStatus407 cannot be < 0");
                    }

                    HttpRetryManager.ProxyAuthenticationCallLimit = maxRetryForHttpStatus407;
                }
            }

            Debug("connectionString   : " + connectionString);
            Debug("theProxyUri        : " + proxyUri);
            Debug("theProxyCredentials: " + proxyUsername + " / " + proxyPassword);
            Debug("isNTLM             : " + isNTLM + "");
            Debug("useCredentialCache : " + useCredentialCache + "");

            if (isNTLM)
            {
                proxyCredentials = CredentialCache.DefaultCredentials;
            }

            if (useCredentialCache)
            {
                Debug("useCredentialCache: " + CredentialCache.DefaultNetworkCredentials.UserName + " / xxxxxxxx");
                NetworkCredential networkCredential = (NetworkCredential)CredentialCache.DefaultCredentials;
                Debug("networkCredential.UserName: " + networkCredential.UserName);
                proxyCredentials = new NetworkCredential(networkCredential.UserName, networkCredential.Password);
            }
            else
            {
                if (proxyUsername != null && proxyPassword != null)
                {
                    proxyCredentials = new NetworkCredential(proxyUsername, proxyPassword);
                }
            }

            CheckBasicConnectionStringValues();

        }

        private void CheckBasicConnectionStringValues()
        {
            if (server == null)
            {
                throw new ArgumentException("\"Server\" mandatory property not found in connection string.");
            }

            if (database == null)
            {
                throw new ArgumentException("\"Database\" mandatory property not found in connection string.");
            }
        }

        public string Server { get => server; }
        public string Database { get => database; }
        public string Username { get => username; }
        public char[] Password { get => password; }
        public string SessionId { get => sessionId; }
        public string ProxyUri { get => proxyUri; }
        public ICredentials ProxyCredentials { get => proxyCredentials; }
        public int Timeout { get => timeout; }

        public bool EnableDefaultSystemAuthentication { get => enableDefaultSystemAuthentication; }

        public bool EnableTrace { get => enableTrace; }
        public bool UseCredentialCache { get => useCredentialCache; }

        internal static void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }
    }
}
