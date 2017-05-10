using AceQL.Client.api.file;
using AceQL.Client.api.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.api.http
{

    /// <summary>
    /// Implements multipart/form-data POST in C# http://www.ietf.org/rfc/rfc2388.txt
    /// 
    /// Adapted by NDP from http://www.briangrinstead.com/blog/multipart-form-post-in-c
    /// Only change: 
    /// - NOT using (Stream requestStream = request.GetRequestStream()) because WebException is not 
    ///    thrown correctly and a NullReferenceException is thrown instead.
    /// </summary>
    internal class FormUpload
    {
        internal const string USER_AGENT = "AceQL SDK";
        private static readonly Encoding encoding = Encoding.UTF8;

        private string postUrl = null;
        private ICredentials credentials = null;
        //private string userAgent = null;
        private Dictionary<string, object> postParameters = null;
        private bool uploadOnePass = false;

        // The total bytes to upload
        private int fileLength = 0;

        // The current uploaded bytes percent
        private int percent = 0;
        private int timeout;



        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postUrl">the POST URL.</param>
        /// <param name="credentials">the Credentials info.</param>
        /// <param name="postParameters">the parameters of the Upload</param>
        /// <param name="timeout">The read/write timeout in milliseconds.</param>
        /// <param name="uploadOnePass">if true upload is done by writing all bytes once on thre stream, elses by per byte for progress indicator</param>
        /// 
        /// 
        internal FormUpload(string postUrl, ICredentials credentials, Dictionary<string, object> postParameters, int timeout, bool uploadOnePass)
        {
            this.postUrl = postUrl;
            this.credentials = credentials;
            //this.userAgent = USER_AGENT;
            this.postParameters = postParameters;
            this.timeout = timeout;
            this.uploadOnePass = uploadOnePass;

            // Get the file length
            foreach (var param in postParameters)
            {
                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;
                    this.fileLength = fileToUpload.File.Length;
                }
            }
        }

        /// <summary>
        /// Allows to Upload a document.
        /// </summary>
        /// <returns>the web response.</returns>
        internal async Task<HttpWebResponse> MultipartFormDataPostAsync()
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return await PostFormAsync(contentType, formData).ConfigureAwait(false);
        }

        private async Task<HttpWebResponse> PostFormAsync(string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (timeout != 0)
            {
                //request.ReadWriteTimeout = timeout;
            }

            if (request == null)
            {
                throw new ArgumentNullException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            //request.UserAgent = userAgent;

            // Always set Credentials, even if null
            // Because otw previous value setting is kept
            request.Proxy.Credentials = credentials;

            //request.ContentLength = formData.Length;

            // Send the form data to the request.

            //NDP: NO! WebException is not thrown correctly and a NullReference is thrown instead.
            //using (Stream requestStream = request.GetRequestStream())
            //{
            //    requestStream.Write(formData, 0, formData.Length);
            //    requestStream.Close();
            //}

            Stream requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false);

            if (uploadOnePass)
            {
                requestStream.Write(formData, 0, formData.Length);
                this.percent = 101;
            }
            else
            {
                StreamWriter streamWriter = null;

                try
                {
                    String fileName = "FormUploadOut.txt";
                    Stream stream = await PortableFile.CreateAsync(Parms.ACEQL_PCL_FOLDER, fileName).ConfigureAwait(false);

                    streamWriter = new StreamWriter(stream);

                    int cpt = 0;
                    for (int i = 0; i < formData.Length; i++)
                    {
                        requestStream.WriteByte(formData[i]);
                        requestStream.Flush();
                        cpt++;

                        if (cpt > fileLength / 100)
                        {
                            percent++;
                            cpt = 0;

                            streamWriter.WriteLine(DateTime.Now + " " + percent);
                        }
                    }
                }
                finally
                {
                    // Force percent > 100
                    percent = 101;

                    if (streamWriter != null)
                    {
                        streamWriter.Dispose();
                    }
                }
            }

            requestStream.Dispose();

            return await request.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Dispose();

            return formData;
        }

        internal class FileParameter
        {
            internal byte[] File { get; set; }
            internal string FileName { get; set; }
            internal string ContentType { get; set; }
            internal FileParameter(byte[] file) : this(file, null) { }
            internal FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            internal FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }
        }

        /// <summary>
        /// The already upoaded length
        /// </summary>
        internal int Percent
        {
            get
            {
                return percent;
            }
        }
    }

}
