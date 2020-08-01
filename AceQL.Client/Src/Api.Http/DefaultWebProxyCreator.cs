﻿using AceQL.Client.Api;
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
    /// Class DefaultWebProxyCreator. Allows to get the Default Or System proxy in use. 
    /// </summary>
    internal class DefaultWebProxyCreator
    {

        /// <summary>
        /// Gets the  the Default Or System proxy in use. Will lreturn null if no Default/System proxy is in use.
        /// </summary>
        /// <returns>System.Net.IWebProxy.</returns>
        public static IWebProxy GetWebProxy()
        {
            IWebProxy webProxy = null;

            // See if end user has forced to use a System.Net.WebRequest.GetSystemWebProxy()
            if (AceQLConnection.GetSystemWebProxy() != null)
            {
                webProxy = AceQLConnection.GetSystemWebProxy();
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
