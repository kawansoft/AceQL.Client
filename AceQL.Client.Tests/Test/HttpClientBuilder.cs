using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests
{
    class HttpClientBuilder
    {
        public static HttpClient buildHttpClient()
        {

            String[] lines = File.ReadAllLines("i:\\neotunnel.txt"); 

            String proxyUri0 = "http://localhost:8080";
            string proxyUsername = lines[0];
            string proxyPassword = lines[1];
            ICredentials proxyCredentials = new NetworkCredential(proxyUsername, proxyPassword);

            WebProxy proxy = new WebProxy(proxyUri0);
            proxy.Credentials = proxyCredentials;

            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            };

            httpClientHandler.PreAuthenticate = true;
            httpClientHandler.UseDefaultCredentials = false;

            HttpClient httpClient = new HttpClient(httpClientHandler, false);
            return httpClient;
        }
    }
}
