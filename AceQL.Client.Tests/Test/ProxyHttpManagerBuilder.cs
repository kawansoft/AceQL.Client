using AceQL.Client.Src.Api.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests
{
    /// <summary>
    /// Class ProxyHttpManagerBuilder. Builds a HttpManafer that uses an authenticated proxy
    /// </summary>
    public class ProxyHttpManagerBuilder
    {
        public static HttpManager BuildHttpHttpManager()
        {
            ICredentials proxyCredentials = BuildProxyCredentials();

            String proxyUri = "http://localhost:8080";
            HttpManager httpManager = new HttpManager(proxyUri, proxyCredentials, 0, false);
            return httpManager;
        }

        public static ICredentials BuildProxyCredentials()
        {
            String[] lines = File.ReadAllLines("i:\\neotunnel.txt");
            string proxyUsername = lines[0];
            string proxyPassword = lines[1];
            ICredentials proxyCredentials = new NetworkCredential(proxyUsername, proxyPassword);
            return proxyCredentials;
        }
    }
}
