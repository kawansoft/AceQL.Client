﻿using AceQL.Client.Src.Api.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api.Http
{
    internal class HttpManager
    {
        /// <summary>
        /// The Proxy Uri, if we don't want 
        /// </summary>
        private string proxyUri = null;

        /// <summary>
        /// The credentials
        /// </summary>
        private ICredentials proxyCredentials = null;

        private CancellationToken cancellationToken;
        private bool useCancellationToken = false;

        /// <summary>
        /// The timeout in milliseconds
        /// </summary>
        private int timeout = 0;
        private bool enableDefaultSystemAuthentication = false;

        /// <summary>
        /// The HTTP status code
        /// </summary>
        internal HttpStatusCode httpStatusCode;

        /// <summary>
        /// Says it use has passed a CancellationToken
        /// </summary>
        public bool UseCancellationToken { get => useCancellationToken; }

        private SimpleTracer simpleTracer = new SimpleTracer();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpManager"/> class.
        /// </summary>
        /// <param name="proxyUri">The proxy URI.</param>
        /// <param name="proxyCredentials">The proxy credentials.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="enableDefaultSystemAuthentication">if set to <c>true</c> [enable default system authentication].</param>
        public HttpManager(string proxyUri, ICredentials proxyCredentials, int timeout, bool enableDefaultSystemAuthentication)
        {
            this.proxyUri = proxyUri;
            this.proxyCredentials = proxyCredentials;
            this.timeout = timeout;
            this.enableDefaultSystemAuthentication = enableDefaultSystemAuthentication;
        }

        /// <summary>
        /// To be call at end of each of each public aysnc(CancellationToken) calls to reset to false the usage of a CancellationToken with http calls
        /// and some reader calls.
        /// </summary>
        internal void ResetCancellationToken()
        {
            this.useCancellationToken = false;
        }

        /// <summary>
        /// Sets the CancellationToken asked by user to pass for the current public xxxAsync() call api.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken asked by user to pass for the current public xxxAsync() call api.</param>
        internal void SetCancellationToken(CancellationToken cancellationToken)
        {
            this.useCancellationToken = true;
            this.cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Calls the with get return stream.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Stream.</returns>
        internal async Task<Stream> CallWithGetReturnStreamAsync(String url)
        {
            HttpClient httpClient = new HttpClient(HttpClientHandlerBuilder.Build(proxyUri, proxyCredentials, enableDefaultSystemAuthentication));

            if (timeout != 0)
            {
                long nanoseconds = 1000000 * timeout;
                httpClient.Timeout = new TimeSpan(nanoseconds / 100);
            }

            HttpResponseMessage response = null;

            if (!UseCancellationToken)
            {
                response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            }
            else
            {
                response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            }

            this.httpStatusCode = response.StatusCode;

            HttpContent content = response.Content;

            return await content.ReadAsStreamAsync().ConfigureAwait(false);

        }

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

            HttpClient httpClient = new HttpClient(HttpClientHandlerBuilder.Build(proxyUri, proxyCredentials, enableDefaultSystemAuthentication));

            if (timeout != 0)
            {
                long nanoseconds = 1000000 * timeout;
                httpClient.Timeout = new TimeSpan(nanoseconds / 100);
            }

            MultipartFormDataContent formData = new MultipartFormDataContent();

            // This is the postdata
            var postData = new List<KeyValuePair<string, string>>();

            await simpleTracer.TraceAsync().ConfigureAwait(false);
            await simpleTracer.TraceAsync("----------------------------------------").ConfigureAwait(false);
            await simpleTracer.TraceAsync(theUrl.ToString()).ConfigureAwait(false);

            foreach (var param in parameters)
            {
                postData.Add(new KeyValuePair<string, string>(param.Key, param.Value));
                //await TraceAsync("param: " + param.Key + "/" + param.Value);
            }
            await simpleTracer.TraceAsync("----------------------------------------").ConfigureAwait(false);

            HttpContent content = new FormUrlEncodedContent(postData);

            HttpResponseMessage response = null;

            if (!UseCancellationToken)
            {
                response = await httpClient.PostAsync(theUrl, content);
            }
            else
            {
                response = await httpClient.PostAsync(theUrl, content, cancellationToken);
            }

            this.httpStatusCode = response.StatusCode;
            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Calls the with get.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>String.</returns>
        /// <exception cref="System.ArgumentNullException">url is null!</exception>
        public async Task<string> CallWithGetAsync(String url)
        {

            if (url == null)
            {
                throw new ArgumentNullException("url is null!");
            }

            using (Stream stream = await CallWithGetReturnStreamAsync(url).ConfigureAwait(false))
            {
                if (stream == null)
                {
                    return null;
                }

                var responseString = new StreamReader(stream).ReadToEnd();

                await simpleTracer.TraceAsync().ConfigureAwait(false);
                await simpleTracer.TraceAsync("----------------------------------------").ConfigureAwait(false);
                await simpleTracer.TraceAsync(url).ConfigureAwait(false);
                await simpleTracer.TraceAsync(responseString).ConfigureAwait(false);
                await simpleTracer.TraceAsync("----------------------------------------").ConfigureAwait(false);
                
                return responseString;
            }

        }

        /// <summary>
        /// Allowso the parent AceQLHttpApi to set the simple tracer.
        /// </summary>
        /// <param name="simpleTracer">The simple tracer.</param>
        internal void SetSimpleTracer(SimpleTracer simpleTracer)
        {
            this.simpleTracer = simpleTracer;
        }

        /// <summary>
        /// Executes a POST with parameters and returns a Srring
        /// </summary>
        /// <param name="theUrl">The Url.</param>
        /// <param name="parametersMap">The request parameters.</param>
        /// <returns>Stream.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// action is null!
        /// or
        /// postParameters is null!
        /// </exception>
        public async Task<string> CallWithPostAsyncReturnString(Uri theUrl, Dictionary<string, string> parametersMap)
        {
            String result = null;

            using (Stream input = await CallWithPostAsync(theUrl, parametersMap).ConfigureAwait(false))
            {
                if (input != null)
                {
                    result = new StreamReader(input).ReadToEnd();
                }
            }

            return result;
        }

    }
}
