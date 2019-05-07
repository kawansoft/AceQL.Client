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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Safester.Models;
using Safester.ViewModels;
using System.IO;
using Safester.Services;
using Acr.UserDialogs;
using System.Linq;
using Xamarin.Essentials;

namespace Safester.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel { get; set; }
        bool isNeedOpenFile { get; set; }

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
            Initialize();
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            viewModel = new ItemDetailViewModel(new Message(), MenuItemType.Inbox);
            BindingContext = viewModel;

            Initialize();
        }

        private void Initialize()
        {
            listAttachment.BindingContext = viewModel;

            viewModel.BodyUpdated = BodyUpdatedAction;
            viewModel.DownloadFinished = DownloadFinishedAction;

            listAttachment.ItemSelected += ListAttachment_ItemSelected;
            switchShowOriginal.Toggled += SwitchShowOriginal_Toggled;
            htmlLabel.LongClicked = (copyFlag) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    { 
                        await Clipboard.SetTextAsync(htmlLabel.PlainText);

                        UserDialogs.Instance.Alert(AppResources.ClipboardSuccess);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            };
        }

        void SwitchShowOriginal_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                htmlLabel.Text = viewModel.BodyOriginal;
            }
            else
            {
                htmlLabel.Text = viewModel.Body;
            }
        }

        private void BodyUpdatedAction()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                htmlLabel.Text = viewModel.Body;
            });
        }

        private void DownloadFinishedAction(Attachment item, string filePath)
        {
            ShowLoading(false);
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (item == null || string.IsNullOrWhiteSpace(filePath))
                {
                    await DisplayAlert(AppResources.Warning, AppResources.FileDownloadFailure, AppResources.OK);
                    return;
                }

                string alertMsg = string.Empty; 
                if (Device.RuntimePlatform == Device.iOS)
                    alertMsg = AppResources.FileDownloadToFolderiOS.Replace("\\n", "\n");
                else
                    alertMsg = AppResources.FileDownloadToFolderAndroid.Replace("\\n", "\n");

                if (isNeedOpenFile == false)
                    await DisplayAlert(AppResources.Success, alertMsg, AppResources.OK);
                else
                {
                    var filesService = DependencyService.Get<IFilesService>();
                    filesService.OpenUri(filePath);
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Item.hasAttachs == false)
            {
                layoutAttachment.IsVisible = false;
                layoutAttachment.HeightRequest = 0;
            }

            viewModel.LoadDataCommand.Execute(null);
        }

        async void ListAttachment_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var itemData = e.SelectedItem as Attachment;
            if (itemData == null)
                return;

            listAttachment.SelectedItem = null;
            string option = await DisplayActionSheet("", AppResources.Cancel, null, new string[] { AppResources.OpenFile, AppResources.DownloadFile } );
            if (string.IsNullOrEmpty(option))
                return;

            if (option.Equals(AppResources.DownloadFile, StringComparison.OrdinalIgnoreCase)) // Download Only
            {
                isNeedOpenFile = false;
                ShowLoading(true);
                viewModel.LoadAttachmentCommand.Execute(itemData.attachPosition);
            }
            else if (option.Equals(AppResources.OpenFile, StringComparison.OrdinalIgnoreCase)) // Download and open
            {
                isNeedOpenFile = true;
                ShowLoading(true);
                viewModel.LoadAttachmentCommand.Execute(itemData.attachPosition);
            }
        }

        private async void DeleteItem_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert(AppResources.Warning, AppResources.DeleteMail, AppResources.Yes, AppResources.Cancel);
            if (result)
            {
                ShowLoading(true);
                result = await viewModel.DeleteItemsCommand((int)viewModel.Item.messageId);
                ShowLoading(false);

                if (result == true)
                {
                    ItemsPage.NeedForceReload = true;
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert(AppResources.Warning, AppResources.TryAgain, AppResources.OK);
                }
            }
        }

        private void ReplyItem_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.FromRecipient))
                return;

            var recipient = ProcessReplyRecipient(viewModel.FromRecipient);
            if (recipient == null)
                return;

            if (viewModel.IsBodyLoaded == false)
                return;

            var draftMessage = new DraftMessage { Id = 0 };
            draftMessage.ToRecipients = new System.Collections.ObjectModel.ObservableCollection<Recipient>();
            draftMessage.ToRecipients.Add(recipient);
            draftMessage.CcRecipients = new System.Collections.ObjectModel.ObservableCollection<Recipient>();
            draftMessage.BccRecipients = new System.Collections.ObjectModel.ObservableCollection<Recipient>();
            draftMessage.attachments = null;
            draftMessage.subject = "Re:" + viewModel.Subject;
            draftMessage.body = "\n\n----- original message --------\n" + htmlLabel.PlainText;
            if (string.IsNullOrEmpty(draftMessage.body) == false)
                draftMessage.body = draftMessage.body.Replace("<br>", "\n");

            Navigation.PushAsync(new NewItemPage(draftMessage));
        }

        private void ReplyAllItem_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.FromRecipient))
                return;

            if (viewModel.IsBodyLoaded == false)
                return;

            var draftMessage = new DraftMessage { Id = 0 };
            draftMessage.ToRecipients = new System.Collections.ObjectModel.ObservableCollection<Recipient>();
            var recipient = ProcessReplyRecipient(viewModel.FromRecipient);
            if (recipient == null)
                return;
            draftMessage.ToRecipients.Add(recipient);

            if (string.IsNullOrEmpty(viewModel.ToRecipients) == false)
            {
                string[] recipients = viewModel.ToRecipients.Split(';');
                if (recipients != null)
                {
                    foreach (var item in recipients)
                    {
                        if (string.IsNullOrWhiteSpace(item) == false)
                        {
                            recipient = ProcessReplyRecipient(item);
                            if (recipient != null && !recipient.recipientEmailAddr.Equals(App.CurrentUser.UserEmail, StringComparison.OrdinalIgnoreCase))
                                draftMessage.ToRecipients.Add(recipient);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(viewModel.CcRecipients) == false)
            {
                string[] recipients = viewModel.CcRecipients.Split(';');
                if (recipients != null)
                {
                    foreach (var item in recipients)
                    {
                        if (string.IsNullOrWhiteSpace(item) == false)
                        {
                            recipient = ProcessReplyRecipient(item);
                            if (recipient != null && !recipient.recipientEmailAddr.Equals(App.CurrentUser.UserEmail, StringComparison.OrdinalIgnoreCase))
                                draftMessage.ToRecipients.Add(recipient);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(viewModel.BccRecipients) == false)
            {
                string[] recipients = viewModel.BccRecipients.Split(';');
                if (recipients != null)
                {
                    foreach (var item in recipients)
                    {
                        if (string.IsNullOrWhiteSpace(item) == false)
                        {
                            recipient = ProcessReplyRecipient(item);
                            if (recipient != null && !recipient.recipientEmailAddr.Equals(App.CurrentUser.UserEmail, StringComparison.OrdinalIgnoreCase))
                                draftMessage.ToRecipients.Add(recipient);
                        }
                    }
                }
            }

            draftMessage.CcRecipients = new System.Collections.ObjectModel.ObservableCollection<Recipient>();
            draftMessage.BccRecipients = new System.Collections.ObjectModel.ObservableCollection<Recipient>();
            draftMessage.attachments = null;
            draftMessage.subject = "Re:" + viewModel.Subject;
            draftMessage.body = "\n\n----- original message --------\n" + htmlLabel.PlainText;
            if (string.IsNullOrEmpty(draftMessage.body) == false)
                draftMessage.body = draftMessage.body.Replace("<br>", "\n");

            Navigation.PushAsync(new NewItemPage(draftMessage));
        }

        private Recipient ProcessReplyRecipient(string recpStr)
        {
            string userName;
            string userEmail;

            Utils.Utils.ParseEmailString(recpStr, out userName, out userEmail);
            if (string.IsNullOrEmpty(userEmail))
                return null;

            var recipient = App.Recipients.Where(x => x.recipientEmailAddr.Equals(userEmail)).Select(x => x).FirstOrDefault();

            if (recipient == null)
            {
                recipient = new Recipient
                {
                    recipientName = string.Empty,
                    recipientEmailAddr = userEmail,
                    recipientType = 0,
                    recipientPosition = 0,
                };

                Utils.Utils.AddOrUpdateRecipient(recipient);
            }

            return recipient;
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