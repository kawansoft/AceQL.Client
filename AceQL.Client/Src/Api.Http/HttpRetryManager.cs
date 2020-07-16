using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api.Http
{
    /// <summary>
    /// Class HttpRetryManager. Allows to define retry options. Fist implementation is for 407 retry. 
    /// </summary>
    internal static class HttpRetryManager
    {
        /// <summary>
        /// The proxy authentication call limit. Defaults to one retry.
        /// </summary>
        private static int proxyAuthenticationCallLimit = 1;

        /// <summary>
        /// Gets or sets the proxy authentication call limit. This is the limit of retry when an HTTP call return 407
        /// </summary>
        /// <value>The proxy authentication call limit.</value>
        public static int ProxyAuthenticationCallLimit { get => proxyAuthenticationCallLimit; set => proxyAuthenticationCallLimit = value; }
    }
}
