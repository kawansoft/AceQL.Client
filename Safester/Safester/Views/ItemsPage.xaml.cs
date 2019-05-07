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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Safester.Models;
using Safester.Views;
using Safester.ViewModels;
using System.Collections;
using Safester.Services;
using Acr.UserDialogs;

namespace Safester.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        public MenuItemType ItemType { get; set; }
        public static bool NeedForceReload { get; set; }
        private ItemsViewModel viewModel;

        public ItemsPage(MenuItemType itemType, string title)
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
            ItemType = itemType;
            viewModel.DirectoryId = ItemType;
            viewModel.Title = title;

            ItemsListView.ItemAppearing += InfiniteListView_ItemAppearing;

            viewModel.LoadingAction = (isShowing) =>
            {
                if (isShowing)
                    UserDialogs.Instance.Loading(AppResources.Pleasewait, null, null, true);
                else
                    UserDialogs.Instance.Loading().Hide();
            };

            viewModel.LoadingFinished = () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ItemsListView.IsRefreshing = false; 
                });
            };
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Message;
            if (item == null)
                return;

            viewModel.MarkMessageAsRead(item);
            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item, ItemType)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        void InfiniteListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var items = ItemsListView.ItemsSource as IList;

            var _settingsService = DependencyService.Get<SettingsService>();
            var messagesPerScroll = _settingsService.LoadSettings("messages_per_scroll");
            int limit = Utils.Utils.GetCountPerScroll(messagesPerScroll);

            if (items != null && e.Item == items[items.Count - 1] && items.Count >= limit)
            {
                if (viewModel.LoadMoreCommand != null)
                    viewModel.LoadMoreCommand.Execute(null);
            }
        }

        async void ComposeItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewItemPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0 || NeedForceReload)
                viewModel.LoadItemsCommand.Execute(null);

            NeedForceReload = false;
        }

        async void OnDelete(object sender, System.EventArgs e)
        {
            bool result = await DisplayAlert(AppResources.Warning, AppResources.DeleteMail, "Yes", "Cancel");
            if (result)
            {
                var mi = ((MenuItem)sender);
                viewModel.DeleteCommand.Execute((mi.CommandParameter as Message).messageId);
            }
        }
    }
}