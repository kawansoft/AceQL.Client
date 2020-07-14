using AceQL.Client.Src.Api.Http;
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
    public static class ProxyLoopTest
    {
        public static async Task Test()
        {
            HttpManager httpManager = ProxyHttpManagerBuilder.BuildHttpHttpManager();

            int i = 0;
            while (i < 1000000)
            {
                i++;

                String url = "http://www.runsafester.net:8081/aceql";

                String responseString = await httpManager.CallWithGetAsync(url).ConfigureAwait(false);
                AceQLConsole.WriteLine();
                AceQLConsole.WriteLine("response status " + httpManager.HttpStatusCode);
                AceQLConsole.WriteLine(responseString);

                if (!httpManager.HttpStatusCode.Equals(HttpStatusCode.OK))
                {
                    AceQLConsole.WriteLine("WARNING: not a 200 response: " + httpManager.HttpStatusCode);
                    return;
                }

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                responseString = await httpManager.CallWithPostAsyncReturnString(new Uri(url), parameters);
                AceQLConsole.WriteLine();
                AceQLConsole.WriteLine("response status " + httpManager.HttpStatusCode);
                AceQLConsole.WriteLine(responseString);

                if (!httpManager.HttpStatusCode.Equals(HttpStatusCode.OK))
                {
                    AceQLConsole.WriteLine("WARNING: not a 200 response: " + httpManager.HttpStatusCode);
                    return;
                }

                AceQLConsole.WriteLine();
            }
            httpManager.Dispose();

        }
    }
}
