using AceQL.Client.Api;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api.Http
{

    /// <summary>
    /// Class DefaultWebProxyCreator. Allows to get the <see cref="System.Net.WebRequest.DefaultWebProxy"/> or <c>System.Net.WebRequest.GetSystemProxy()</c> proxy value to use. 
    /// </summary>
    internal static class DefaultWebProxyCreator
    {

        /// <summary>
        /// Gets the  the Default Or System proxy in use. Will return null if no Default/System proxy is in use.
        /// </summary>
        /// <returns>System.Net.IWebProxy.</returns>
        public static IWebProxy GetWebProxy()
        {
            IWebProxy webProxy = null;

            // See if end user has forced to use a System.Net.WebRequest.GetSystemWebProxy()
            if (AceQLConnection.GetDefaultWebProxy() != null)
            {
                webProxy = AceQLConnection.GetDefaultWebProxy();
            }
            else
            {
                webProxy = System.Net.WebRequest.DefaultWebProxy;
            }
          
            // Test the secret URL, if it is bypassed, there is no Default/System proxy set, so we will return null:
            if (webProxy.IsBypassed(new Uri(HttpClientHandlerBuilderNew.SECRET_URL))) {
                return null;
            }
            else
            {
                return webProxy;
            }
        }
    }
}
