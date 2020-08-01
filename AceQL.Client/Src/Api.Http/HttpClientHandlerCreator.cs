using AceQL.Client.Api.Http;
using System;
using System.Net;
using System.Net.Http;

namespace AceQL.Client.Src.Api.Http
{
    /// <summary>
    /// Class HttpClientHandlerCreator. HttpClientHandler creation to be used with HttpClient.
    /// </summary>
    internal class HttpClientHandlerCreator
    {
        private readonly IWebProxy webProxy;
        private readonly ICredentials credentials;
        private readonly bool enableDefaultSystemAuthentication;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientHandlerCreator"/> class.
        /// </summary>
        /// <param name="webProxy">The proxy to use. If null, there is no proxy to use.</param>
        /// <param name="credentials">The credentials. Can be null.</param>
        /// <param name="enableDefaultSystemAuthentication">if set to <c>true</c> [enable default system authentication].</param>
        public HttpClientHandlerCreator(IWebProxy webProxy, ICredentials credentials, bool enableDefaultSystemAuthentication)
        {
            this.webProxy = webProxy; // Can be null.
            this.credentials = credentials; // Can be null.
            this.enableDefaultSystemAuthentication = enableDefaultSystemAuthentication;
        }

        /// <summary>
        /// Creates the HttpClientHandler instance with or without a web proxy.
        /// </summary>
        /// <returns>HttpClientHandler.</returns>
        public HttpClientHandler GetHttpClientHandler()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();

            // No proxy.
            if (webProxy == null)
            {
                if (enableDefaultSystemAuthentication)
                {
                    httpClientHandler.UseDefaultCredentials = true;
                }
                return httpClientHandler;
            }
            else
            {
                // We have a web proxy set, set accordingly the HttpClientHandler
                httpClientHandler = new HttpClientHandler
                {
                    UseProxy = true,
                    UseDefaultCredentials = false
                };

                httpClientHandler.Proxy = webProxy;

                httpClientHandler.Proxy.Credentials = credentials;
                httpClientHandler.PreAuthenticate = true;
                if (enableDefaultSystemAuthentication)
                {
                    httpClientHandler.UseDefaultCredentials = true;
                }
                return httpClientHandler;

            }

        }
    }
}