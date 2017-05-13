
using AceQL.Client.Api.File;
using AceQL.Client.Api.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Http
{
    /// <summary>
    /// Allows file upload (Blobs) using streaming and supporting progress indicators
    /// </summary>
    internal class FormUploadStream
    {
        internal static bool DEBUG = true;
        private long tempLen = 0;

        /// <summary>
        /// Uploads a file using a blob reference
        /// </summary>
        /// <param name="url"></param>
        /// <param name="proxyUri"></param>
        /// <param name="credentials"></param>
        /// <param name="timeout"></param>
        /// <param name="blobId"></param>
        /// <param name="stream"></param>
        /// <param name="totalLength"></param> 
        /// <param name="progressIndicator"></param>
        /// <param name="cancellationTokenSource"></param> 
        /// <returns></returns>
        internal async Task<HttpResponseMessage> UploadAsync(String url, String proxyUri, ICredentials credentials, int timeout, String blobId, Stream stream, long totalLength, ProgressIndicator progressIndicator, CancellationTokenSource cancellationTokenSource)
        {
            HttpClientHandler handler = AceQLHttpApi.BuildHttpClientHandler(proxyUri, credentials);

            ProgressMessageHandler processMessageHander = new ProgressMessageHandler(handler);

            HttpClient httpClient = new HttpClient(processMessageHander);

            if (timeout != 0)
            {
                long nanoseconds = 1000000 * timeout;
                httpClient.Timeout = new TimeSpan(nanoseconds / 100);
            }

            processMessageHander.HttpSendProgress += (sender, e) =>
            {
                //if (DEBUG) ConsoleEmul.WriteLine(DateTime.Now + " progress.IntValue: " + progress.IntValue);

                int num = e.ProgressPercentage;
                this.tempLen += e.BytesTransferred;

                if (progressIndicator != null)
                {
                    if (totalLength > 0 && tempLen > totalLength / 100)
                    {
                        tempLen = 0;
                    }
                    int cpt = progressIndicator.Value;
                    cpt++;
                    progressIndicator.Value = Math.Min(99, cpt);
                    if (DEBUG) ConsoleEmul.WriteLine(DateTime.Now + " progressHolder.Progress: " + progressIndicator.Value);
                }
                else
                {
                    if (DEBUG) ConsoleEmul.WriteLine(DateTime.Now + " num: " + num);
                }

            };

            StringContent stringContentBlobId = new StringContent(blobId);

            try
            {
                var multipart = new MultipartFormDataContent();
                multipart.Add(stringContentBlobId, '"' + "blob_id" + '"');
                multipart.Add(new StreamContent(stream), '"' + "file" + '"', '"' + blobId + '"');

                await AceQLHttpApi.TraceAsync();
                await AceQLHttpApi.TraceAsync("----------------------------------------");
                await AceQLHttpApi.TraceAsync("url     : " + url);
                await AceQLHttpApi.TraceAsync("blob_id : " + blobId);
                await AceQLHttpApi.TraceAsync("----------------------------------------");

                if (DEBUG)
                {
                    ConsoleEmul.WriteLine();
                    ConsoleEmul.WriteLine("url     : " + url);
                    ConsoleEmul.WriteLine("blob_id : " + blobId);
                }

                HttpResponseMessage response = null;

                if (cancellationTokenSource == null)
                {
                    response = await httpClient.PostAsync(url, multipart);
                }
                else
                {
                    response = await httpClient.PostAsync(url, multipart, cancellationTokenSource.Token);
                }

                progressIndicator.Value = 100;
                //if (!response.IsSuccessStatusCode)
                //{
                //    ConsoleEmul.WriteLine("FAILURE");
                //    return null;
                //}

                return response;
            }
            finally
            {
                stream.Dispose();
            }
        }
    }

}
