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
using Acr.UserDialogs;
using Rg.Plugins.Popup.Services;
using Safester.Services;
using Safester.ViewModels;
using Xamarin.Forms;

namespace Safester.Views
{
    public partial class SettingsPage : ContentPage
    {
        private SettingsViewModel viewModel { get; set; }

        public SettingsPage()
        {
            InitializeComponent();

            viewModel = new SettingsViewModel();

            btnSignature.Clicked += (sender, e) =>
            {
                var popupPage = new SignatureInputPage();
                PopupNavigation.Instance.PushAsync(popupPage);
            };

            switchSignature.Toggled += (sender, e) =>
            {
                if (switchSignature.IsToggled)
                    btnSignature.IsEnabled = true;
                else
                    btnSignature.IsEnabled = false;
            };
        }

        void Save_Clicked(object sender, EventArgs e)
        {
            UpdateUI(false);

            ShowLoading(true);

            viewModel.ResponseAction = (success) =>
            {
                ShowLoading(false);
                if (success == false)
                    DisplayAlert(AppResources.Error, AppResources.ErrorOccured, AppResources.OK);
                else
                {
                    DisplayAlert(AppResources.Success, AppResources.SettingsSaved, AppResources.OK);
                    Utils.Utils.SaveDataToFile(App.UserSettings, Utils.Utils.KEY_FILE_USERSETTINGS);
                }
            };

            viewModel.SaveSettingsCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateUI(true);
            ShowLoading(true);

            viewModel.ResponseAction = (success) =>
            {
                ShowLoading(false);
                if (success == false)
                    DisplayAlert(AppResources.Error, AppResources.ErrorOccured, AppResources.OK);
                else
                {
                    UpdateUI(true);
                    Utils.Utils.SaveDataToFile(App.UserSettings, Utils.Utils.KEY_FILE_USERSETTINGS);
                }
            };

            viewModel.LoadSettingsCommand.Execute(null);
        }

        private void UpdateUI(bool loading)
        {
            var _settingsService = DependencyService.Get<SettingsService>();
            if (loading)
            {
                entryName.Text = App.UserSettings.name;
                entryProduct.Text = Utils.Utils.GetProductName(App.UserSettings.product);
                entryCrypto.Text = App.UserSettings.cryptographyInfo;
                entryMailBoxSize.Text = Utils.Utils.GetSizeString(App.UserSettings.mailboxSize);
                entryEmail.Text = App.UserSettings.notificationEmail;
                switchNotification.IsToggled = App.UserSettings.notificationOn;

                var messagesPerScroll = _settingsService.LoadSettings("messages_per_scroll");
                entryCountPerScroll.Text = Utils.Utils.GetCountPerScroll(messagesPerScroll).ToString();

                var enableSignature = _settingsService.LoadSettings("enable_mobile_signature");
                if (string.IsNullOrEmpty(enableSignature) == false && enableSignature.Equals("1"))
                    switchSignature.IsToggled = true;
                else
                    switchSignature.IsToggled = false;

                //entrySignature.Text = _settingsService.LoadSettings("mobile_signature");
            }
            else
            {
                App.UserSettings.name = entryName.Text;
                App.UserSettings.notificationEmail = entryEmail.Text;
                App.UserSettings.notificationOn = switchNotification.IsToggled;
                //_settingsService.SaveSettings("mobile_signature", string.IsNullOrEmpty(entrySignature.Text) ? "" : entrySignature.Text);
                _settingsService.SaveSettings("messages_per_scroll", string.IsNullOrEmpty(entryCountPerScroll.Text) ? "" : entryCountPerScroll.Text);

                _settingsService.SaveSettings("enable_mobile_signature", switchSignature.IsToggled ? "1" : "0");
            }

            if (switchSignature.IsToggled)
                btnSignature.IsEnabled = true;
            else
                btnSignature.IsEnabled = false;
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
