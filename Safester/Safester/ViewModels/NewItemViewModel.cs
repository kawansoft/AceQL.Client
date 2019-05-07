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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Safester.CryptoLibrary.Api;
using Safester.Models;
using Safester.Network;
using Safester.Utils;
using Xamarin.Forms;

namespace Safester.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public ObservableCollection<Recipient> ToRecipients { get; set; }
        public ObservableCollection<Recipient> CcRecipients { get; set; }
        public ObservableCollection<Recipient> BccRecipients { get; set; }
        public ObservableCollection<Attachment> Attachments { get; set; }

        public Command SendMessgeCommand { get; set; }
        public Command SaveDraftCommand { get; set; }

        public Action<bool> Finished { get; set; }
        private List<Recipient> MailRecipients { get; set; }
        private List<PgpPublicKey> MailKeys { get; set; }

        public string BodyEncrypted { get; set; }

        public NewItemViewModel()
        {
            SendMessgeCommand = new Command(async () => await ExecuteSendMessgeCommand());
            SaveDraftCommand = new Command(async () => await ExecuteSaveDraftCommand());

            ToRecipients = new ObservableCollection<Recipient>();
            CcRecipients = new ObservableCollection<Recipient>();
            BccRecipients = new ObservableCollection<Recipient>();
            Attachments = new ObservableCollection<Attachment>();
        }

        async Task ExecuteSendMessgeCommand()
        {
            try
            {
                MailKeys = new List<PgpPublicKey>();
                var senderKeyInfo = await ApiManager.SharedInstance().GetPublicKey(App.CurrentUser.UserEmail, App.CurrentUser.Token, App.CurrentUser.UserEmail);
                if (senderKeyInfo == null)
                {
                    Finished?.Invoke(false);
                    return;
                }

                // Add sender pgp public key
                MailKeys.Add(PgpPublicKeyGetter.ReadPublicKey(senderKeyInfo.publicKey));

                int recpos = 0;
                MailRecipients = new List<Recipient>();

                if (ToRecipients != null)
                {
                    foreach (var item in ToRecipients)
                    {
                        if (string.IsNullOrWhiteSpace(item.recipientEmailAddr) == false)
                            await GetKeyAndAddRecipient(item, 1, recpos++);
                    }
                }

                if (CcRecipients != null)
                {
                    foreach (var item in CcRecipients)
                    {
                        if (string.IsNullOrWhiteSpace(item.recipientEmailAddr) == false)
                            await GetKeyAndAddRecipient(item, 2, recpos++);
                    }
                }

                if (BccRecipients != null)
                {
                    foreach (var item in BccRecipients)
                    {
                        if (string.IsNullOrWhiteSpace(item.recipientEmailAddr) == false)
                            await GetKeyAndAddRecipient(item, 3, recpos++);
                    }
                }

                if (string.IsNullOrEmpty(Body))
                    Body = string.Empty;
                else
                    Body = Body.Replace("\n", "<br>");

                var jsonData = new SenderMailMessage
                {
                    senderEmailAddr = App.CurrentUser.UserEmail,
                    subject = HttpUtility.HtmlEncode(Subject),//App.KeyEncryptor.Encrypt(MailKeys, HttpUtility.HtmlEncode(Subject)),
                    body = App.KeyEncryptor.Encrypt(MailKeys, HttpUtility.HtmlEncode(Body)),
                };

                jsonData.attachments = new List<Attachment>();
                var fileList = new List<KeyValuePair<string, Stream>>();

                if (Attachments != null)
                {
                    int pos = 1;
                    foreach (var attachment in Attachments)
                    {
                        string encfilepath = string.Empty;
                        if (Utils.Utils.EncryptFile(App.KeyEncryptor, MailKeys, attachment.filepath, attachment.filename, "fileenc", out encfilepath) == false)
                        {
                            Finished?.Invoke(false);
                            return;
                        }

                        var streamContent = File.Open(encfilepath, FileMode.Open, FileAccess.Read);
                        FileInfo info = new FileInfo(encfilepath);
                        jsonData.attachments.Add(new Attachment
                        {
                            attachPosition = pos++,
                            filename = attachment.filename + ".pgp",
                            size = info.Length,
                        });

                        fileList.Add(new KeyValuePair<string, Stream>(attachment.filename + ".pgp", streamContent));
                    }
                }

                jsonData.recipients = MailRecipients;

                jsonData.size = 0;

                var jsonStringData = JsonConvert.SerializeObject(jsonData);
                ApiManager.SharedInstance().SendMessage(App.CurrentUser.UserEmail, App.CurrentUser.Token, jsonStringData, fileList, (success, result) =>
                {
                    try
                    {
                        if (fileList != null)
                        {
                            foreach (var file in fileList)
                            {
                                file.Value.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    if (success)
                        Utils.Utils.SaveDataToFile(App.Recipients, Utils.Utils.KEY_FILE_RECIPIENTS);

                    BodyEncrypted = Utils.Utils.EncryptMessageData(App.KeyEncryptor, MailKeys, Body);
                    Finished?.Invoke(success);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Finished?.Invoke(false);
            }
        }

        async Task ExecuteSaveDraftCommand()
        {
            Finished?.Invoke(true);
        }

        private async Task GetKeyAndAddRecipient(Recipient item, int type, int pos)
        {
            //string recipientName = string.Empty;
            //string recipientEmail = string.Empty;

            //Utils.Utils.ParseEmailString(item, out recipientName, out recipientEmail);
            if (MailRecipients.Any(x => x.recipientEmailAddr.Equals(item.recipientEmailAddr)))
            {
                return;
            }

            var keyInfo = await ApiManager.SharedInstance().GetPublicKey(App.CurrentUser.UserEmail, App.CurrentUser.Token, item.recipientEmailAddr);
            if (keyInfo == null)
            {
                keyInfo = await ApiManager.SharedInstance().GetPublicKey(App.CurrentUser.UserEmail, App.CurrentUser.Token, "contact@safelogic.com");
                if (keyInfo == null)
                    return;
            }

            var newRecipient = new Recipient
            {
                recipientEmailAddr = item.recipientEmailAddr,
                recipientName = item.recipientName,
                recipientPosition = pos,
                recipientType = type
            };

            Utils.Utils.AddOrUpdateRecipient(newRecipient);
            MailRecipients.Add(newRecipient);

            // Add recipient pgp public key
            MailKeys.Add(PgpPublicKeyGetter.ReadPublicKey(keyInfo.publicKey));
        }
    }
}
