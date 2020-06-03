using AceQL.Client.Api.Http;
using AceQL.Client.Api.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api.Http
{
    /// <summary>
    /// Class HttpClientHandlerBuilder. Allows to build an HttpClientHandler.
    /// </summary>
    internal class HttpClientHandlerBuilder
    {
        internal static bool DEBUG = false;

        /// <summary>
        /// Build an HttpClientHandler instance with proxy settings, if necessary. Proxy used is System.Net.WebRequest.DefaultWebProxy
        /// </summary>
        /// <param name="proxyUri"></param>
        /// <param name="credentials">The credentials to use for an authenticated proxy. null if none.</param>
        /// <param name="enableDefaultSystemAuthentication">if True ==> call HttpClientHandler.UseDefaultCredentials = true</param>
        /// <returns>The HtpClientHandler.</returns>
        internal static HttpClientHandler Build(string proxyUri, ICredentials credentials, bool enableDefaultSystemAuthentication)
        {
            Debug("httpClientHandler.UseDefaultCredentials: "  + enableDefaultSystemAuthentication);

            Proxy proxy = null;
            // Used to test if have Proxy defined in IE
            String proxyUriToUse = null;

            // Test if used the default Web Proxy or the one passed in connection string:
            if (proxyUri == null)
            {
                proxyUriToUse = System.Net.WebRequest.DefaultWebProxy.GetProxy(new Uri("http://www.google.com")).ToString();
            }
            else
            {
                proxy = new Proxy(proxyUri);
                proxyUriToUse = proxy.GetProxy(new Uri("http://www.google.com")).ToString();
            }

            Debug("uriProxy: " + proxyUriToUse);

            if (credentials != null && credentials.GetType() == typeof(NetworkCredential))
            {
                Debug("credentials: " + ((NetworkCredential)credentials).UserName + "/" + ((NetworkCredential)credentials).Password);
            }

            if (proxyUriToUse.Contains("http://www.google.com"))
            {
                Debug("System.Net.WebRequest.DefaultWebProxy is default");
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                if (enableDefaultSystemAuthentication)
                {
                    httpClientHandler.UseDefaultCredentials = true;
                }
                return httpClientHandler;
            }
            else
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler()
                {
                    UseProxy = true,
                    UseDefaultCredentials = false
                };

                if (proxy == null)
                {
                    httpClientHandler.Proxy = System.Net.WebRequest.DefaultWebProxy;
                }
                else
                {
                    httpClientHandler.Proxy = proxy;
                }

                httpClientHandler.Proxy.Credentials = credentials;
                httpClientHandler.PreAuthenticate = true;
                if (enableDefaultSystemAuthentication)
                {
                    httpClientHandler.UseDefaultCredentials = true;
                }
                return httpClientHandler;
            }
        }

        internal static void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }
    }
}
