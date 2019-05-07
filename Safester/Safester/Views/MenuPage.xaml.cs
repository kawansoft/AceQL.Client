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
﻿using Safester.Models;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Safester.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.Android)
            {
                menuItems = new List<HomeMenuItem>
                {
                    new HomeMenuItem {Id = MenuItemType.Inbox, Title=AppResources.Inbox, Image="inbox.png" },
                    new HomeMenuItem {Id = MenuItemType.Sent, Title=AppResources.Sent, Image="inbox_out.png" },
                    new HomeMenuItem {Id = MenuItemType.Drafts, Title=AppResources.Drafts, Image="document.png" },
                    //new HomeMenuItem {Id = MenuItemType.Trash, Title="Trash", Image="recycle.png" },
                    new HomeMenuItem {Id = MenuItemType.Contacts, Title=AppResources.Contacts, Image="business.png" },
                    new HomeMenuItem {Id = MenuItemType.Settings, Title=AppResources.Settings, Image="settings.png" },
                    new HomeMenuItem {Id = MenuItemType.ChangePass, Title=AppResources.ChangePass, Image="password.png" },
                    new HomeMenuItem {Id = MenuItemType.About, Title=AppResources.About, Image="information.png" },
                    new HomeMenuItem {Id = MenuItemType.Logout, Title=AppResources.Logout, Image="log_out.png" }
                };
            }
            else
            {
                menuItems = new List<HomeMenuItem>
                {
                    new HomeMenuItem {Id = MenuItemType.Inbox, Title=AppResources.Inbox, Image="inbox.png" },
                    new HomeMenuItem {Id = MenuItemType.Sent, Title=AppResources.Sent, Image="inbox_out.png" },
                    new HomeMenuItem {Id = MenuItemType.Drafts, Title=AppResources.Drafts, Image="document.png" },
                    new HomeMenuItem {Id = MenuItemType.Contacts, Title=AppResources.Contacts, Image="business.png" },
                    new HomeMenuItem {Id = MenuItemType.Settings, Title=AppResources.Settings, Image="settings.png" },
                    new HomeMenuItem {Id = MenuItemType.Logout, Title=AppResources.Logout, Image="log_out.png" }
                };
            }

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var item = (HomeMenuItem)e.SelectedItem;
                if (item.Id == MenuItemType.Logout)
                    ListViewMenu.SelectedItem = null;

                await RootPage.NavigateFromMenu(item);
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            labelName.Text = App.UserSettings.name;
            labelEmail.Text = App.CurrentUser.UserEmail;
        }
    }
}