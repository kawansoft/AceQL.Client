/*
 * This file is part of Safester.                                    
 * Copyright (C) 2019, KawanSoft SAS
 * (https://www.Safester.net). All rights reserved.                                
 *                                                                               
 * Safester is free software; you can redistribute it and/or                 
 * modify it under the terms of the GNU Lesser General Public                    
 * License as published by the Free Software Foundation; either                  
 * version 2.1 of the License, or (at your option) any later version.            
 *                                                                               
 * Safester is distributed in the hope that it will be useful,               
 * but WITHOUT ANY WARRANTY; without even the implied warranty of                
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU             
 * Lesser General Public License for more details.                               
 *                                                                               
 * You should have received a copy of the GNU Lesser General Public              
 * License along with this library; if not, write to the Free Software           
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  
 * 02110-1301  USA
 * 
 * Any modifications to this file must keep this entire header
 * intact.
 */
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Safester.CryptoLibrary.Api;
using Safester.Models;
using Safester.Utils;

namespace Safester.Network
{
	public class ApiManager
	{
		private static ApiManager _instance = null;
		public static ApiManager SharedInstance()
		{
			if (_instance == null)
				_instance = new ApiManager();

			return _instance;
		}

		HttpClient client;
        private HttpStatusCode httpStatusCode;

        public HttpClient _client
        {
            get
            {
                if (client == null)
                {
                    client = new HttpClient();
                    client.BaseAddress = new Uri(SERVER_BASEURL);

                    client.MaxResponseContentBufferSize = long.MaxValue; // no limit for the download size
                    client.Timeout = TimeSpan.FromSeconds(60 * 60 * 5); // 5 hours timeout
                }
                return client;
            }
            set
            {
                client = value;
            }
        }        

		#region CONST VALUES
		private const string SERVER_BASEURL = "https://www.runsafester.net/";
		#endregion

		public ApiManager()
		{
		}
        
		public async void Login(string userName, string passWord, Action<bool, string> callback)
		{
            var passPhrase = PassphraseUtil.ComputeHashAndSaltedPassphrase(userName, passWord);

            var postData = new List<KeyValuePair<string, string>>();
			postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("passphrase", passPhrase));

            string result = string.Empty;
            result = await CallWithPostAsync("api/login", postData);

            if (string.IsNullOrEmpty(result) == true)
                callback(false, AppResources.CANNOT_CONNECT_SERVER);
            else
            {
                try
                {
                    var ret = (UserInfo)JsonConvert.DeserializeObject(result, typeof(UserInfo));
                    if (Errors.IsApiSuccess(ret.status))
                    {
                        App.CurrentUser.PassPhrase = PassphraseUtil.ComputeHashAndSaltedPassphrase(userName, passWord);
                        App.CurrentUser.Token = ret.token;
                        callback(true, ret.errorMessage);
                    }
                    else
                        callback(false, ret.errorMessage);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    callback(false, (ex == null) ? "" : ex.ToString());
                }
            }
		}

        public async void Register(string userName, string emailAddr, string passWord, string coupon, Action<bool, string> callback)
        {
            // Get the Hexa hashPassphrase computed from passphrzse 
            String hashPassphrase = PassphraseUtil.ComputeHashAndSaltedPassphrase(emailAddr, passWord);

            //1) First generate key pair 
            PgpKeyPairGenerator pgpKeyPairGenerator = new PgpKeyPairGenerator(emailAddr, passWord.ToCharArray(), PublicKeyAlgorithm.RSA, PublicKeyLength.BITS_2048);
            PgpKeyPairHolder pgpKeyPairHolder = pgpKeyPairGenerator.Generate();

            string privKey = pgpKeyPairHolder.PrivateKeyRing;
            string pubKey = pgpKeyPairHolder.PublicKeyRing;

            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("name", userName));
            postData.Add(new KeyValuePair<string, string>("emailAddress", emailAddr));
            postData.Add(new KeyValuePair<string, string>("hashPassphrase", hashPassphrase));
            postData.Add(new KeyValuePair<string, string>("privKey", privKey));
            postData.Add(new KeyValuePair<string, string>("pubKey", pubKey));

            if (string.IsNullOrWhiteSpace(coupon) == false)
                postData.Add(new KeyValuePair<string, string>("coupon", coupon));

            string result = string.Empty;
            result = await CallWithPostAsync("api/register", postData);

            if (string.IsNullOrEmpty(result) == true)
                callback(false, AppResources.CANNOT_CONNECT_SERVER);
            else
            {
                try
                {
                    var ret = (BaseResult)JsonConvert.DeserializeObject(result, typeof(BaseResult));
                    if (Errors.IsApiSuccess(ret.status))
                        callback(true, ret.errorMessage);
                    else
                        callback(false, ret.errorMessage);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    callback(false, (ex == null) ? "" : ex.ToString());
                }
            }
        }

        public async void ListMessages(string userName, string token, int directoryId, int limit, int offset, Action<bool, MessagesResultInfo> callback)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));
            postData.Add(new KeyValuePair<string, string>("directoryId", directoryId.ToString()));
            postData.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
            postData.Add(new KeyValuePair<string, string>("offset", offset.ToString()));

            string result = string.Empty;
            result = await CallWithPostAsync("api/listMessages", postData);

            if (string.IsNullOrEmpty(result) == true)
                callback(false, new MessagesResultInfo { errorMessage = AppResources.CANNOT_CONNECT_SERVER });
            else
            {
                try
                {
                    var ret = (MessagesResultInfo)JsonConvert.DeserializeObject(result, typeof(MessagesResultInfo));
                    if (Errors.IsApiSuccess(ret.status))
                    {
                        callback(true, ret);
                    }
                    else
                        callback(false, ret);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    callback(false, null);
                }
            }
        }

        public async Task<bool> DeleteMessage(string userName, string token, int messageId, int directoryId)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));
            postData.Add(new KeyValuePair<string, string>("messageId", messageId.ToString()));
            postData.Add(new KeyValuePair<string, string>("directoryId", directoryId.ToString()));

            string result = string.Empty;
            result = await CallWithPostAsync("api/deleteMessage", postData);

            if (string.IsNullOrEmpty(result) == true)
                return false;
            else
            {
                try
                {
                    var ret = (BaseResult)JsonConvert.DeserializeObject(result, typeof(BaseResult));
                    if (Errors.IsApiSuccess(ret.status))
                    {
                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return false;
                }
            }
        }

        public async void GetMessageDetail(string userName, string token, int messageId, Action<bool, MessageDetailInfo> callback)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));
            postData.Add(new KeyValuePair<string, string>("messageId", messageId.ToString()));

            string result = string.Empty;
            result = await CallWithPostAsync("api/getMessage", postData);

            if (string.IsNullOrEmpty(result) == true)
                callback(false, new MessageDetailInfo { errorMessage = AppResources.CANNOT_CONNECT_SERVER });
            else
            {
                try
                {
                    var ret = (MessageDetailInfo)JsonConvert.DeserializeObject(result, typeof(MessageDetailInfo));
                    if (Errors.IsApiSuccess(ret.status))
                    {
                        callback(true, ret);
                    }
                    else
                        callback(false, ret);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    callback(false, null);
                }
            }
        }

        public async void GetMessageAttachment(string userName, string token, int messageId, int attachPosition, Action<bool, Stream> callback)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));
            postData.Add(new KeyValuePair<string, string>("messageId", messageId.ToString()));
            postData.Add(new KeyValuePair<string, string>("attachPosition", attachPosition.ToString()));

            Stream result = null;
            result = await CallWithStreamResultPostAsync("api/getAttachment", postData);

            if (result == null)
                callback(false, null);
            else
            {
                callback(true, result);
            }
        }

        public async void SetMessageRead(string userName, string token, string senderEmail, int messageId, Action<bool, string> callback)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));
            postData.Add(new KeyValuePair<string, string>("messageId", messageId.ToString()));
            postData.Add(new KeyValuePair<string, string>("senderEmailAddress", senderEmail));

            string result = string.Empty;
            result = await CallWithPostAsync("api/setMessageRead", postData);

            if (string.IsNullOrEmpty(result) == true)
                callback(false, "");
            else
            {
                try
                {
                    var ret = (BaseResult)JsonConvert.DeserializeObject(result, typeof(BaseResult));
                    if (Errors.IsErrorExist(ret.status))
                    {
                        callback(false, ret.errorMessage);
                    }
                    else
                        callback(true, "");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    callback(false, null);
                }
            }
        }

        public async void GetAddressBook(string userName, string token, Action<bool, RecipientsBookInfo> callback)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));

            string result = string.Empty;
            result = await CallWithPostAsync("api/getCompletionAddressBook", postData);

            if (string.IsNullOrEmpty(result) == true)
                callback(false, null);
            else
            {
                try
                {
                    var ret = (RecipientsBookInfo)JsonConvert.DeserializeObject(result, typeof(RecipientsBookInfo));
                    if (Errors.IsErrorExist(ret.status))
                    {
                        callback(false, ret);
                    }
                    else
                        callback(true, null);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    callback(false, null);
                }
            }
        }

        public async void AddAddressBook(string userName, string token, string emailAddr, string name, Action<bool, string> callback)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));
            postData.Add(new KeyValuePair<string, string>("emailAddress", emailAddr));
            postData.Add(new KeyValuePair<string, string>("name", name));

            string result = string.Empty;
            result = await CallWithPostAsync("api/setCompletionEntry", postData);

            if (string.IsNullOrEmpty(result) == true)
                callback(false, "");
            else
            {
                try
                {
                    var ret = (BaseResult)JsonConvert.DeserializeObject(result, typeof(BaseResult));
                    if (Errors.IsErrorExist(ret.status))
                    {
                        callback(false, ret.errorMessage);
                    }
                    else
                        callback(true, "");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    callback(false, null);
                }
            }
        }

        public async void SendMessage(string userName, string token, string jsonElements, List<KeyValuePair<string, Stream>> attachments, Action<bool, string> callback)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));
            postData.Add(new KeyValuePair<string, string>("jsonMessageElements", jsonElements));

            string result = string.Empty;
            result = await CallWithFilePostAsync("api/sendMessage", postData, attachments);

            if (string.IsNullOrEmpty(result) == true)
                callback(false, "");
            else
            {
                try
                {
                    var ret = (BaseResult)JsonConvert.DeserializeObject(result, typeof(BaseResult));
                    if (Errors.IsErrorExist(ret.status))
                    {
                        callback(false, ret.errorMessage);
                    }
                    else
                        callback(true, "");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    callback(false, null);
                }
            }
        }

        public async Task<SettingsInfo> GetUserSettings(string userName, string token)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));

            string result = string.Empty;
            result = await CallWithPostAsync("api/getUserSettings", postData);

            if (string.IsNullOrEmpty(result) == true)
                return null;
            else
            {
                try
                {
                    var ret = (SettingsInfo)JsonConvert.DeserializeObject(result, typeof(SettingsInfo));
                    if (Errors.IsApiSuccess(ret.status))
                        return ret;
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        public async Task<string> SetUserSettings(string userName, string token, SettingsInfo settings)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));
            postData.Add(new KeyValuePair<string, string>("name", settings.name));
            postData.Add(new KeyValuePair<string, string>("notificationOn", settings.notificationOn.ToString()));
            postData.Add(new KeyValuePair<string, string>("notificationEmail", HttpUtility.HtmlEncode(settings.notificationEmail)));

            string result = string.Empty;
            result = await CallWithPostAsync("api/setUserSettings", postData);

            if (string.IsNullOrEmpty(result) == true)
                return AppResources.CANNOT_CONNECT_SERVER;
            else
            {
                try
                {
                    var ret = (BaseResult)JsonConvert.DeserializeObject(result, typeof(BaseResult));
                    if (Errors.IsApiSuccess(ret.status))
                        return Errors.NO_ERROR;
                    else
                        return ret.errorMessage;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return AppResources.CANNOT_CONNECT_SERVER;
                }
            }
        }

        #region KEY APIS
        public async Task<KeyInfo> GetPublicKey(string userName, string token, string userEmailAddr)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));
            postData.Add(new KeyValuePair<string, string>("userEmailAddr", userEmailAddr));

            string result = string.Empty;
            result = await CallWithPostAsync("api/getPublicKey", postData);

            if (string.IsNullOrEmpty(result) == true)
                return null;
            else
            {
                try
                {
                    var ret = (KeyInfo)JsonConvert.DeserializeObject(result, typeof(KeyInfo));
                    if (Errors.IsApiSuccess(ret.status))
                        return ret;
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        public async void GetPrivateKey(string userName, string token, Action<bool, KeyInfo> callback)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", userName));
            postData.Add(new KeyValuePair<string, string>("token", token));

            string result = string.Empty;
            result = await CallWithPostAsync("api/getPrivateKey", postData);

            if (string.IsNullOrEmpty(result) == true)
                callback(false, null);
            else
            {
                try
                {
                    var ret = (KeyInfo)JsonConvert.DeserializeObject(result, typeof(KeyInfo));
                    if (Errors.IsApiSuccess(ret.status))
                    {
                        callback(true, ret);
                    }
                    else
                        callback(false, null);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    callback(false, null);
                }
            }
        }
        #endregion

        #region CALL APIS
        public async Task<string> CallWithPostAsync(string theUrl, List<KeyValuePair<string, string>> parameters)
        {
            if (string.IsNullOrEmpty(theUrl) || parameters == null)
                return "";

            string result = string.Empty;

            try
            {
                HttpContent content = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = null;
                response = await _client.PostAsync(theUrl, content);

                httpStatusCode = response.StatusCode;
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                System.Threading.Thread.Sleep(50);

                if (stream != null)
                {
                    result = new StreamReader(stream).ReadToEnd();
                    Console.WriteLine("Payload: {0}", result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ApiGetResult: " + ex);
            }

            return result;
        }

        public async Task<byte[]> CallWithByteResultPostAsync(string theUrl, List<KeyValuePair<string, string>> parameters)
        {
            if (string.IsNullOrEmpty(theUrl) || parameters == null)
                return null;

            string result = string.Empty;

            try
            {
                HttpContent content = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = null;
                response = await _client.PostAsync(theUrl, content);

                httpStatusCode = response.StatusCode;
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                System.Threading.Thread.Sleep(50);
                
                if (stream != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return null;
        }

        public async Task<Stream> CallWithStreamResultPostAsync(string theUrl, List<KeyValuePair<string, string>> parameters)
        {
            if (string.IsNullOrEmpty(theUrl) || parameters == null)
                return null;

            string result = string.Empty;

            try
            {
                HttpContent content = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = null;
                response = await _client.PostAsync(theUrl, content);

                httpStatusCode = response.StatusCode;
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                System.Threading.Thread.Sleep(50);

                return stream;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return null;
        }

        public async Task<string> CallWithFilePostAsync(string theUrl, List<KeyValuePair<string, string>> parameters, List<KeyValuePair<string, Stream>> files)
        {
            if (string.IsNullOrEmpty(theUrl) || parameters == null)
                return "";

            string result = string.Empty;

            try
            {
                MultipartFormDataContent content = new MultipartFormDataContent();

                foreach (var keyValuePair in parameters)
                    content.Add(new StringContent(keyValuePair.Value, Encoding.UTF8), string.Format("\"{0}\"", keyValuePair.Key));

                foreach (var filedata in files)
                    content.Add(new StreamContent(filedata.Value), "file", filedata.Key);

                HttpResponseMessage response = null;
                response = await _client.PostAsync(theUrl, content);

                httpStatusCode = response.StatusCode;
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                System.Threading.Thread.Sleep(50);

                if (stream != null)
                {
                    result = new StreamReader(stream).ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return result;
        }
        #endregion
    }
}
