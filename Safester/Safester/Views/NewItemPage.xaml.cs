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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Safester.Models;
using Safester.ViewModels;
using System.Linq;
using System.Collections.ObjectModel;
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;
using Acr.UserDialogs;
using Newtonsoft.Json;
using Safester.Services;
using Plugin.Media;
using System.IO;

namespace Safester.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewItemPage : ContentPage
    {
        private NewItemViewModel viewModel { get; set; }
        private bool isDraft { get; set; }
        private bool isSending { get; set; }
        private bool isAppearing { get; set; }

        public DraftMessage DraftMessageData { get; set; }

        public NewItemPage(DraftMessage data = null)
        {
            InitializeComponent();

            DraftMessageData = data;
            viewModel = new NewItemViewModel();
            if (data != null)
            {
                viewModel.Subject = data.subject;
                if (data.attachments != null && data.attachments.Count > 0)
                {
                    viewModel.Attachments = new ObservableCollection<Attachment>();
                    foreach (var item in data.attachments)
                        viewModel.Attachments.Add(item);
                }
            }

            BindingContext = viewModel;

            viewModel.Finished = FinishedAction;

            listAttachment.ItemSelected += async (object sender, SelectedItemChangedEventArgs e) =>
            {
                if (e.SelectedItem == null)
                    return;

                bool result = await DisplayAlert(AppResources.Warning, AppResources.RemoveAttachment, AppResources.Yes, AppResources.No);
                if (result)
                    viewModel.Attachments.Remove(e.SelectedItem as Attachment);

                listAttachment.SelectedItem = null;
            };

            suggestBoxTo.DataSource = App.Recipients;
            suggestBoxCc.DataSource = App.Recipients;
            suggestBoxBcc.DataSource = App.Recipients;

            UpdateDraftData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (isAppearing == false)
            {
                isAppearing = true;
                editorBody.Focus();
            }
        }

        private void UpdateDraftData()
        {
            if (DraftMessageData != null)
            {
                if (DraftMessageData.ToRecipients != null && DraftMessageData.ToRecipients.Count > 0)
                {
                    suggestBoxTo.SelectedItem = DraftMessageData.ToRecipients;
                }
                if (DraftMessageData.CcRecipients != null && DraftMessageData.CcRecipients.Count > 0)
                {
                    suggestBoxCc.SelectedItem = DraftMessageData.CcRecipients;
                }
                if (DraftMessageData.BccRecipients != null && DraftMessageData.BccRecipients.Count > 0)
                {
                    suggestBoxBcc.SelectedItem = DraftMessageData.BccRecipients;
                }

                if (string.IsNullOrEmpty(DraftMessageData.subject) == false)
                    entrySubject.Text = DraftMessageData.subject;

                if (string.IsNullOrEmpty(DraftMessageData.body) == false)
                    editorBody.Text = DraftMessageData.body;
            }

            if (string.IsNullOrEmpty(editorBody.Text))
            {
                var _settingsService = DependencyService.Get<SettingsService>();
                var enableSignature = _settingsService.LoadSettings("enable_mobile_signature");
                if (string.IsNullOrEmpty(enableSignature) == false && enableSignature.Equals("1"))
                {
                    editorBody.Text = string.Format("\n\n{0}", _settingsService.LoadSettings("mobile_signature"));
                }
            }
        }

        void Save_Clicked(object sender, EventArgs e)
        {
            if (DraftMessageData == null)
                DraftMessageData = new DraftMessage();

            if (DraftMessageData.Id == 0)
                DraftMessageData.Id = App.DraftMessages.Count + 1;

            SyncSuggestDataWithVM();

            DraftMessageData.ToRecipients = viewModel.ToRecipients;
            DraftMessageData.CcRecipients = viewModel.CcRecipients;
            DraftMessageData.BccRecipients = viewModel.BccRecipients;

            if (string.IsNullOrEmpty(entrySubject.Text) == false)
                DraftMessageData.subject = entrySubject.Text;
            if (string.IsNullOrEmpty(editorBody.Text) == false)
                DraftMessageData.body = editorBody.Text;

            if (viewModel.Attachments != null)
            {
                DraftMessageData.attachments = viewModel.Attachments;
            }

            Utils.Utils.AddOrUpdateDraft(DraftMessageData);
            Utils.Utils.SaveDataToFile(App.DraftMessages, Utils.Utils.KEY_FILE_DRAFTMESSAGES);

            DisplayAlert("", AppResources.SaveSuccess, AppResources.OK);
            Navigation.PopAsync();
        }

        void Send_Clicked(object sender, EventArgs e)
        {
            if (isSending == true)
                return;

            SyncSuggestDataWithVM();

            if (viewModel.ToRecipients.Count == 0)
            {
                DisplayAlert(AppResources.Warning, AppResources.InputReceiverEmail, AppResources.OK);
                return;
            }

            if (string.IsNullOrEmpty(viewModel.Subject))
            {
                DisplayAlert(AppResources.Warning, AppResources.InputSubject, AppResources.OK);
                return;
            }

            viewModel.Body = editorBody.Text;
            if (string.IsNullOrEmpty(viewModel.Body))
                viewModel.Body = string.Empty;

            isSending = true;
            ShowLoading(true);
            isDraft = false;
            viewModel.SendMessgeCommand.Execute(null);
        }

        async void AddFile_Clicked(object sender, System.EventArgs e)
        {
            string option = await DisplayActionSheet("", AppResources.Cancel, null, (Device.RuntimePlatform == Device.iOS) ? new string[] { AppResources.TakePhoto, AppResources.SelectPhoto, AppResources.PickFile } : new string[] { AppResources.PickFile });
            if (string.IsNullOrEmpty(option))
                return;

            string filename = string.Empty;
            string filepath = string.Empty;

            if (option.Equals(AppResources.TakePhoto)) // Take Photo from Camera
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    return;
                }

                filename = String.Format("camera_{0}.jpg", System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = filename,
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 1024,
                    CompressionQuality = 80,
                });

                if (file != null)
                    filepath = file.Path;
            }
            else if (option.Equals(AppResources.SelectPhoto))// Take Photo from Library
            { 
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    CompressionQuality = 80,
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 1024,
                });

                if (file != null)
                {
                    filepath = file.Path;
                    filename = filepath.Substring(filepath.LastIndexOf('/') + 1);
                }
            }
            else if (option.Equals(AppResources.PickFile))// Pick file from the device
            {
                FileData filedata = new FileData();
                var crossFileData = CrossFilePicker.Current;
                filedata = await crossFileData.PickFile();

                if (filedata == null || string.IsNullOrEmpty(filedata.FilePath))
                    return;

                filepath = filedata.FilePath;
                filename = filedata.FilePath.Substring(filedata.FilePath.LastIndexOf('/') + 1);
            }

            if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(filepath))
                return;

            if (viewModel.Attachments.Any(x => string.IsNullOrEmpty(x.filepath) && x.filepath.Equals(filepath)))
            {
                await DisplayAlert("", AppResources.FileAdded, AppResources.OK);
                return;
            }

            FileInfo info = new FileInfo(filepath);
            viewModel.Attachments.Add(new Attachment
            {
                filepath = filepath,
                filename = filename,
                size = info.Length,
            });
        }

        void ClearAll_Clicked(object sender, System.EventArgs e)
        {
            viewModel.Attachments = new ObservableCollection<Attachment>();
        }

        async void FinishedAction(bool success)
        {
            isSending = false;
            ShowLoading(false);
            if (success)
            {
                if (isDraft == false)
                {
                    editorBody.Text = viewModel.BodyEncrypted;
                    await DisplayAlert(AppResources.Success, AppResources.MessageEncrypted, AppResources.OK);

                    if (DraftMessageData != null && DraftMessageData.Id > 0)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            int idx = App.DraftMessages.IndexOf(DraftMessageData);
                            App.DraftMessages.RemoveAt(idx);
                            Utils.Utils.SaveDataToFile(App.DraftMessages, Utils.Utils.KEY_FILE_DRAFTMESSAGES);
                            await Navigation.PopAsync();
                        });
                    }
                    else
                    {
                        await Navigation.PopAsync();
                    }
                }
                else
                {
                    await DisplayAlert(AppResources.Success, AppResources.SaveSuccess, AppResources.OK);
                }
            }
            else
            {
                await DisplayAlert(AppResources.Warning, AppResources.ErrorOccured, AppResources.OK);
            }
        }

        private void SyncSuggestDataWithVM()
        {
            string lastRecipientName = string.Empty;

            viewModel.ToRecipients = new ObservableCollection<Recipient>();
            if (suggestBoxTo.SelectedItem != null)
            {
                var selectedItems = suggestBoxTo.SelectedItem as ObservableCollection<object>;
                foreach (var item in selectedItems)
                {
                    viewModel.ToRecipients.Add(item as Recipient);
                    lastRecipientName = (item as Recipient).ToString();
                }
            }

            if (string.IsNullOrEmpty(suggestBoxTo.Text) == false && suggestBoxTo.Text.Equals(lastRecipientName) == false)
            {
                string recipientName = string.Empty;
                string recipientEmail = string.Empty;

                Utils.Utils.ParseEmailString(suggestBoxTo.Text, out recipientName, out recipientEmail);
                viewModel.ToRecipients.Add(new Recipient { recipientEmailAddr = recipientEmail, recipientName = recipientName });
            }

            lastRecipientName = string.Empty;
            viewModel.CcRecipients = new ObservableCollection<Recipient>();
            if (suggestBoxCc.SelectedItem != null)
            {
                var selectedItems = suggestBoxCc.SelectedItem as ObservableCollection<object>;
                foreach (var item in selectedItems)
                {
                    viewModel.CcRecipients.Add(item as Recipient);
                    lastRecipientName = (item as Recipient).ToString();
                }
            }
            if (string.IsNullOrEmpty(suggestBoxCc.Text) == false && suggestBoxCc.Text.Equals(lastRecipientName) == false)
            {
                string recipientName = string.Empty;
                string recipientEmail = string.Empty;

                Utils.Utils.ParseEmailString(suggestBoxCc.Text, out recipientName, out recipientEmail);
                viewModel.CcRecipients.Add(new Recipient { recipientEmailAddr = recipientEmail, recipientName = recipientName });
            }

            lastRecipientName = string.Empty;
            viewModel.BccRecipients = new ObservableCollection<Recipient>();
            if (suggestBoxBcc.SelectedItem != null)
            {
                var selectedItems = suggestBoxBcc.SelectedItem as ObservableCollection<object>;
                foreach (var item in selectedItems)
                {
                    viewModel.BccRecipients.Add(item as Recipient);
                    lastRecipientName = (item as Recipient).ToString();
                }
            }

            if (string.IsNullOrEmpty(suggestBoxBcc.Text) == false && suggestBoxBcc.Text.Equals(lastRecipientName) == false)
            {
                string recipientName = string.Empty;
                string recipientEmail = string.Empty;

                Utils.Utils.ParseEmailString(suggestBoxBcc.Text, out recipientName, out recipientEmail);
                viewModel.BccRecipients.Add(new Recipient { recipientEmailAddr = recipientEmail, recipientName = recipientName });
            }
        }

        private void ShowLoading(bool isShowing)
        {
            if (isShowing)
                UserDialogs.Instance.Loading(AppResources.Pleasewait, null, null, true);
            else
                UserDialogs.Instance.Loading().Hide();
        }
    }
}