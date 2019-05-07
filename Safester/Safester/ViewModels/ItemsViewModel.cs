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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Safester.Models;
using Safester.Views;
using Safester.Network;
using Safester.Services;
using System.Web;

namespace Safester.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Message> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command LoadMoreCommand { get; set; }
        public Command<int> DeleteCommand { get; set; }

        public MenuItemType DirectoryId { get; set; }

        public Action<bool> LoadingAction { get; set; }
        public Action LoadingFinished { get; set; }

        private int limit { get; set; }
        private int offset { get; set; }

        public ItemsViewModel()
        {
            Items = new ObservableCollection<Message>();

            var _settingsService = DependencyService.Get<SettingsService>();
            var messagesPerScroll = _settingsService.LoadSettings("messages_per_scroll");
            limit = Utils.Utils.GetCountPerScroll(messagesPerScroll);

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            LoadMoreCommand = new Command(async () => await ExecuteLoadMoreItemsCommand());
            DeleteCommand = new Command<int>(async (id) => await DeleteItemsCommand(id));
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LoadingAction?.Invoke(true);
            try
            {
                Items.Clear();
                offset = 0;
                ApiManager.SharedInstance().ListMessages(App.CurrentUser.UserName, App.CurrentUser.Token, (int)DirectoryId, limit, offset, (success, result) =>
                {
                    IsBusy = false;
                    LoadingAction?.Invoke(false);

                    if (success && result != null)
                    {
                        foreach (var item in result.messages)
                        {
                            AddMessageToTheList(item);
                        }

                        LoadingFinished?.Invoke();
                        OnPropertyChanged("Items");
                        offset = Items.Count;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        async Task ExecuteLoadMoreItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LoadingAction?.Invoke(true);

            try
            {
                ApiManager.SharedInstance().ListMessages(App.CurrentUser.UserName, App.CurrentUser.Token, (int)DirectoryId, limit, offset, (success, result) =>
                {
                    IsBusy = false;
                    LoadingAction?.Invoke(false);
                    if (success && result != null)
                    {
                        foreach (var item in result.messages)
                        {
                            AddMessageToTheList(item);
                        }

                        LoadingFinished?.Invoke();
                        OnPropertyChanged("Items");
                        offset = Items.Count;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        async Task DeleteItemsCommand(int id)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LoadingAction?.Invoke(true);

            try
            {
                var result = await ApiManager.SharedInstance().DeleteMessage(App.CurrentUser.UserName, App.CurrentUser.Token, id, (int)DirectoryId);

                IsBusy = false;
                LoadingAction?.Invoke(false);

                if (result == true)
                {
                    await ExecuteLoadItemsCommand();
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void MarkMessageAsRead(Message item)
        {
            if (Items == null)
                return;

            var index = Items.IndexOf(item);
            if (index == -1)
                return;

            Items[index].IsRead = true;
        }

        private void AddMessageToTheList(Message item)
        {
            item.subject = HttpUtility.HtmlDecode(Utils.Utils.DecryptMessageData(App.KeyDecryptor, item.subject, true));
            if (DirectoryId != MenuItemType.Inbox)
                item.IsRead = true;

            Items.Add(item);
        }
    }
}