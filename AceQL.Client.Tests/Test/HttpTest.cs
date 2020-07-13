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
    public class HttpTest
    {
        private HttpStatusCode httpStatusCode;

        /// <summary>
        /// Executes a POST with parameters and returns a Stream
        /// </summary>
        /// <param name="theUrl">The Url.</param>
        /// <param name="parameters">The request parameters.</param>
        /// <returns>Stream.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// action is null!
        /// or
        /// postParameters is null!
        /// </exception>
        public async Task<Stream> CallWithPostAsync(Uri theUrl, Dictionary<string, string> parameters)
        {
            if (theUrl == null)
            {
                throw new ArgumentNullException("urlWithaction is null!");
            }


            if (parameters == null)
            {
                throw new ArgumentNullException("postParameters is null!");
            }

            HttpClient httpClient = new HttpClient();

            MultipartFormDataContent formData = new MultipartFormDataContent();

            // This is the postdata
            var postData = new List<KeyValuePair<string, string>>();

            foreach (var param in parameters)
            {
                postData.Add(new KeyValuePair<string, string>(param.Key, param.Value));
            }

            HttpContent content = new FormUrlEncodedContent(postData);

            HttpResponseMessage response = null;

            response = await httpClient.PostAsync(theUrl, content);

            this.httpStatusCode = response.StatusCode;
            AceQLConsole.WriteLine("this.httpStatusCode: " + this.httpStatusCode);
            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Test the HTTP code
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task DoIt()
        {

            string url = "https://www.runsafester.net/api/login";
            Uri theUri = new Uri(url);

            AceQLConsole.WriteLine("calling: " + url);

            Dictionary<string, string> parametersMap = new Dictionary<string, string>
            {
                { "username",  "brunopaul88@outlook.com"},
                { "passphrase",  "82223bafcd814f5d5600"},
            };

            AceQLConsole.WriteLine("calling parameters:");

            foreach (KeyValuePair<string, string> kvp in parametersMap)
            {
                AceQLConsole.WriteLine("Key/Value = " +  kvp.Key + "/" +  kvp.Value);
            }
            AceQLConsole.WriteLine();

            HttpTest httpTest = new HttpTest();

            String result = null;

            using (Stream input = await httpTest.CallWithPostAsync(theUri, parametersMap).ConfigureAwait(false))
            {
                if (input != null)
                {
                    result = new StreamReader(input).ReadToEnd();
                }
            }

            AceQLConsole.WriteLine("result: " + result);
            Console.ReadLine();

        }

    }
}
