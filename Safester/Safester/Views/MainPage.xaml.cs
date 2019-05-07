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
﻿using Acr.UserDialogs;
using Safester.Models;
using Safester.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Safester.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        //Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;
            Detail = new NavigationPage(new ItemsPage(MenuItemType.Inbox, AppResources.Inbox));
        }

        public async Task NavigateFromMenu(HomeMenuItem item)
        {
            IsPresented = false;

            switch (item.Id)
            {
                case MenuItemType.Inbox:
                    Detail = new NavigationPage(new ItemsPage(MenuItemType.Inbox, AppResources.Inbox));
                    break;
                case MenuItemType.Sent:
                    Detail = new NavigationPage(new ItemsPage(MenuItemType.Sent, AppResources.Sent));
                    break;
                case MenuItemType.Drafts:
                    Detail = new NavigationPage(new DraftItemsPage());
                    break;
                case MenuItemType.Contacts:
                    ImportContacts();
                    break;
                case MenuItemType.Settings:
                    Detail = new NavigationPage(new SettingsPage());
                    break;
                case MenuItemType.About:
                    Detail = new NavigationPage(new AboutPage());
                    break;
                case MenuItemType.Trash:
                    await DisplayAlert(AppResources.Warning, AppResources.AvailableSoon, AppResources.OK);
                    break;
                case MenuItemType.ChangePass:
                    await DisplayAlert(AppResources.ChangePass, AppResources.ChangePassHint, AppResources.OK);
                    break;
                case MenuItemType.Logout:
                    bool result = await DisplayAlert("", AppResources.ConfirmLogout, AppResources.Yes, AppResources.No);
                    if (result)
                    {
                        var _settingsService = DependencyService.Get<SettingsService>();
                        _settingsService.SaveSettings("rememberuser", "0");

                        LoginPage.CurrentUserEmail = App.CurrentUser.UserEmail;
                        LoginPage.CurrentUserPassword = String.Empty;
                        LoginPage.NeedsUpdating = true;
                        App.Current.MainPage = new NavigationPage(new LoginPage());
                    }
                    break;
            }

            if (Device.RuntimePlatform == Device.Android)
                await Task.Delay(100);
        }

        private async void ImportContacts()
        {
            bool result = await UserDialogs.Instance.ConfirmAsync(AppResources.ImportContacts, "Safester", AppResources.OK, AppResources.Cancel);
            if (result)
            {
                var _settingsService = DependencyService.Get<SettingsService>();
                _settingsService.AskContactsPermission(ImportContactsWithPermission);
            }
        }

        private void ImportContactsWithPermission()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                UserDialogs.Instance.Loading(AppResources.ImportContactsHint, null, null, true);

                await Utils.Utils.GetPhoneContacts();

                UserDialogs.Instance.Loading().Hide();

                await DisplayAlert("Safester", AppResources.ImportContactsSuccess, AppResources.OK);
            });
        }
    }
}