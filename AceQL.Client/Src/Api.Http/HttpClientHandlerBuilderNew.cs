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
    static internal class HttpClientHandlerBuilderNew
    {
        internal readonly static bool DEBUG = true;

        internal static readonly String SECRET_URL = "http://secret.aceql.com";

        /// <summary>
        /// Builds an HttpClientHandler instance with proxy settings, if necessary. IWebProxy used is System.Net.WebRequest.DefaultWebProxy
        /// </summary>
        /// <param name="proxyUri">The URI of the web proxy to use.</param>
        /// <param name="credentials">The credentials to use for an authenticated proxy. null if none.</param>
        /// <param name="enableDefaultSystemAuthentication">if True ==> call HttpClientHandler.UseDefaultCredentials = true</param>
        /// <returns>The HtpClientHandler.</returns>
        internal static HttpClientHandler Build(string proxyUri, ICredentials credentials, bool enableDefaultSystemAuthentication)
        {
            Debug("httpClientHandler.UseDefaultCredentials: "  + enableDefaultSystemAuthentication);

            IWebProxy webProxy = null;
            HttpClientHandlerCreator httpClientHandlerCreator = null;

            if (proxyUri == null)
            {
                // Detect the System.Net.WebRequest.DefaultWebProxy or WebRequest.GetSystemWebProxy() in use. 
                // We will get null if no Default/System proxy is configured.
                // We use the webproxy credentials if they are set / not null:
                webProxy = DefaultWebProxyCreator.GetWebProxy();
               
                if (webProxy == null || webProxy.Credentials == null)
                {
                    Debug("webProxy or webProxy.Credentials is NULL!");
                    httpClientHandlerCreator = new HttpClientHandlerCreator(webProxy, credentials, enableDefaultSystemAuthentication);
                }
                else
                {
                    // Use the Credentials of the Web Proxy if webProxy.Credentials  is not null
                    httpClientHandlerCreator = new HttpClientHandlerCreator(webProxy, webProxy.Credentials, enableDefaultSystemAuthentication);
                }

                
            }
            else
            {
                Uri uri = new Uri(proxyUri);
                webProxy = new UriWebProxy(uri);

                // Creates the HttpClientHandler, with or without an associated IWebProxy
                httpClientHandlerCreator = new HttpClientHandlerCreator(webProxy, credentials, enableDefaultSystemAuthentication);
            }

            HttpClientHandler httpClientHandler = httpClientHandlerCreator.GetHttpClientHandler();
            return httpClientHandler;


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
