using AceQL.Client.Tests.Test;
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
    public class HttpClientLoopTest
    {
        public static async Task Test()
        {
            HttpClient httpClient = HttpClientBuilder.buildHttpClient();

            int i = 0;
            while (i < 10)
            {
                i++;

                String url = "http://www.runsafester.net:8081/aceql";
                HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                Stream stream = await response.Content.ReadAsStreamAsync();
                var responseString = new StreamReader(stream).ReadToEnd();
                AceQLConsole.WriteLine("response " + responseString);

                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode.Equals(HttpStatusCode.ProxyAuthenticationRequired))
                {
                    AceQLConsole.WriteLine("REBUILD HttpClient");
                    httpClient.Dispose();
                    httpClient = HttpClientBuilder.buildHttpClient();
                }

                AceQLConsole.WriteLine();

                url = "http://www.runsafester.net";
                response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                stream = await response.Content.ReadAsStreamAsync();
                responseString = new StreamReader(stream).ReadToEnd();
                AceQLConsole.WriteLine("response " + responseString);

                AceQLConsole.WriteLine();
                //Thread.Sleep(100);
            }
            httpClient.Dispose();

        }
    }
}
