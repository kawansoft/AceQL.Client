using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Http
{
    /// <summary>
    /// Class to allow to pass a proxyUri, if we don't want to use the default System.Net.WebRequest.DefaultWebProxy.
    /// </summary>
    internal class Proxy : IWebProxy
    {
        private ICredentials credentials = null;
        String proxyUri = null;

        /// <summary>
        /// Builds an IWebProxy implementation.
        /// </summary>
        /// <param name="proxyUri">The proxy URI. Example: http://localhost:8080.</param>
        /// 
        public Proxy(String proxyUri)
        {
            if (proxyUri == null)
            {
                throw new ArgumentNullException("proxyUri is null!");
            }

            this.proxyUri = proxyUri;

        }

        public ICredentials Credentials
        {
            get
            {
                return credentials;       
            }

            set
            {
                this.credentials = value; 
            }
        }

        public Uri GetProxy(Uri destination)
        {
            return new Uri(this.proxyUri);
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}
