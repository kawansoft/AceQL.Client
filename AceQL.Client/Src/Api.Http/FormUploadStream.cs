/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2020,  KawanSoft SAS
 * (http://www.kawansoft.com). All rights reserved.                                
 *                                                                               
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */
﻿

using AceQL.Client.Api.File;
using AceQL.Client.Api.Util;
using AceQL.Client.Src.Api.Http;
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
        internal static readonly bool DEBUG;

        private long tempLen;
        private int proxyAuthenticationCallCount;

        /// <summary>
        /// Uploads a file using a blob reference.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="proxyUri"></param>
        /// <param name="credentials"></param>
        /// <param name="timeout"></param>
        /// <param name="enableDefaultSystemAuthentication"></param>
        /// <param name="blobId"></param>
        /// <param name="stream"></param>
        /// <param name="totalLength"></param> 
        /// <param name="progressIndicator"></param>
        /// <param name="cancellationToken"></param> 
        /// <param name="useCancellationToken"></param>
        /// <param name="requestHeaders">The request headers to add to all requests.</param>
        /// <returns></returns>
        internal async Task<HttpResponseMessage> UploadAsync(String url, String proxyUri, ICredentials credentials,
            int timeout, bool enableDefaultSystemAuthentication, String blobId, Stream stream, long totalLength, 
            AceQLProgressIndicator progressIndicator, CancellationToken cancellationToken, bool useCancellationToken, Dictionary<string, string> requestHeaders)
        {
            HttpClientHandler handler = HttpClientHandlerBuilderNew.Build(proxyUri, credentials, enableDefaultSystemAuthentication);
            ProgressMessageHandler processMessageHander = new ProgressMessageHandler(handler);
            HttpClient httpClient = new HttpClient(processMessageHander);
            HttpManager.AddRequestHeaders(httpClient, requestHeaders);

            if (timeout != 0)
            {
                long nanoseconds = 1000000 * timeout;
                httpClient.Timeout = new TimeSpan(nanoseconds / 100);
            }

            processMessageHander.HttpSendProgress += (sender, e) =>
            {
                int num = e.ProgressPercentage;
                this.tempLen += e.BytesTransferred;

                if (progressIndicator != null)
                {
                    if (totalLength > 0 && tempLen > totalLength / 100)
                    {
                        tempLen = 0;
                    }
                    int cpt = progressIndicator.Percent;
                    cpt++;
                    progressIndicator.SetValue(Math.Min(99, cpt));

                    if (DEBUG)
                    {
                        ConsoleEmul.WriteLine(DateTime.Now + " progressHolder.Progress: " + progressIndicator.Percent);
                    }
                }
                else
                {
                    if (DEBUG)
                    {
                        ConsoleEmul.WriteLine(DateTime.Now + " num: " + num);
                    }
                }
            };
            
            StringContent stringContentBlobId = new StringContent(blobId);

            try
            {
                var multipart = new MultipartFormDataContent();
                multipart.Add(stringContentBlobId, '"' + "blob_id" + '"');
                multipart.Add(new StreamContent(stream), '"' + "file" + '"', '"' + blobId + '"');

                if (DEBUG)
                {
                    ConsoleEmul.WriteLine();
                    ConsoleEmul.WriteLine("url     : " + url);
                    ConsoleEmul.WriteLine("blob_id : " + blobId);
                }

                HttpResponseMessage response = null;

                if (!useCancellationToken)
                {
                    response = await httpClient.PostAsync(url, multipart).ConfigureAwait(false);
                }
                else
                {
                    response = await httpClient.PostAsync(url, multipart, cancellationToken).ConfigureAwait(false);
                }

                // Allows a retry for 407, because can happen time to time with Web proxies 
                if (response.StatusCode.Equals(HttpStatusCode.ProxyAuthenticationRequired))
                {
                    while (proxyAuthenticationCallCount < HttpRetryManager.ProxyAuthenticationCallLimit)
                    {
                        proxyAuthenticationCallCount++;
                        if (!useCancellationToken)
                        {
                            response = await httpClient.PostAsync(url, multipart).ConfigureAwait(false);
                        }
                        else
                        {
                            response = await httpClient.PostAsync(url, multipart, cancellationToken).ConfigureAwait(false);
                        }

                        if (!response.StatusCode.Equals(HttpStatusCode.ProxyAuthenticationRequired))
                        {
                            proxyAuthenticationCallCount = 0;
                            break;
                        }
                    }
                }

                if (progressIndicator != null)
                {
                    progressIndicator.SetValue(100);
                }

                return response;
            }
            finally
            {
                stream.Dispose();
                httpClient.Dispose();
            }
        }
    }

}
