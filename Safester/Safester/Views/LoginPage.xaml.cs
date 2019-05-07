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
using Safester.CryptoLibrary.Api;
using Safester.Custom.Effects;
using Safester.Network;
using Safester.Services;
using Safester.Utils;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Safester.Views
{
    public partial class LoginPage : ContentPage
    {
        public static string CurrentUserEmail { get; set; }
        public static string CurrentUserPassword { get; set; }
        public static bool NeedsUpdating { get; set; }

        bool rememberUser = false;

        private SettingsService _settingsService { get; set; }

        public LoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            TapGestureRecognizer loginGesture = new TapGestureRecognizer();
			loginGesture.Tapped += StartBtn_Clicked;
			ImgLogin.GestureRecognizers.Add(loginGesture);

            TapGestureRecognizer signupGesture = new TapGestureRecognizer();
            signupGesture.Tapped += SignupGesture_Tapped;
            lblSignUp.GestureRecognizers.Add(signupGesture);

            TapGestureRecognizer rememberGesture = new TapGestureRecognizer();
            rememberGesture.Tapped += RememberGesture_Tapped;
            stackRemember.GestureRecognizers.Add(rememberGesture);

            NavigationPage.SetHasNavigationBar(this, false);

            _settingsService = DependencyService.Get<SettingsService>();
			lblVersion.Text = AppResources.Version + _settingsService.GetAppVersionName();

            var isFirstRun = _settingsService.LoadSettings("app_first_run");
            if (string.IsNullOrEmpty(isFirstRun))
            {
                _settingsService.SaveSettings("app_first_run", "1");
                _settingsService.SaveSettings("mobile_signature", (Device.RuntimePlatform == Device.Android) ? AppResources.SentFromAndroid : AppResources.SentFromiOS);
            }

            rememberUser = _settingsService.LoadSettings("rememberuser").Equals("1");
            if (rememberUser)
            {
                CurrentUserEmail = entryUserName.Text = _settingsService.LoadSettings("useremail");
                string passwordSetting = _settingsService.LoadSettings("password");
                string encrypted = _settingsService.LoadSettings("password_encrypted");
                if (string.IsNullOrEmpty(encrypted) == false && encrypted.Equals("1"))
                    passwordSetting = Utils.Utils.GetDecryptedPassphrase(passwordSetting);

                CurrentUserPassword = entryPassword.Text = passwordSetting;
            }

            entryPassword.Effects.Add(new ShowHidePassEffect());
            ChangeUIOption();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (NeedsUpdating)
            {
                entryUserName.Text = CurrentUserEmail;
                entryPassword.Text = CurrentUserPassword;
            }

            NeedsUpdating = false;
        }

        async void StartBtn_Clicked(object sender, EventArgs e)
		{
            if (string.IsNullOrEmpty(entryUserName.Text))
            {
                await DisplayAlert(AppResources.Warning, AppResources.ErrorInputUserName, AppResources.OK);
                return;
            }
            if (string.IsNullOrEmpty(entryPassword.Text))
            {
                await DisplayAlert(AppResources.Warning, AppResources.ErrorInputUserPassPhrase, AppResources.OK);
                return;
            }

            ShowLoading(true);
            ApiManager.SharedInstance().Login(entryUserName.Text, entryPassword.Text, (success, message) =>
            {
                if (success == true)
                {
                    App.CurrentUser.UserName = entryUserName.Text;
                    App.CurrentUser.UserEmail = entryUserName.Text;
                    App.CurrentUser.UserPassword = entryPassword.Text.ToCharArray();

                    ApiManager.SharedInstance().GetPrivateKey(App.CurrentUser.UserName, App.CurrentUser.Token, (suc, keyInfo) => {
                        ShowLoading(false);
                        if (suc)
                        {
                            App.CurrentUser.PrivateKey = keyInfo.privateKey;
                            App.KeyDecryptor = new Decryptor(App.CurrentUser.PrivateKey, App.CurrentUser.UserPassword);

                            _settingsService.SaveSettings("rememberuser", rememberUser ? "1" : "0");
                            if (rememberUser)
                            {
                                _settingsService.SaveSettings("useremail", entryUserName.Text);
                                _settingsService.SaveSettings("password_encrypted", "1");
                                _settingsService.SaveSettings("password", Utils.Utils.GetEncryptedPassphrase(entryPassword.Text));
                            }

                            App.Current.MainPage = new MainPage();
                        }
                        else
                        {
                            DisplayAlert(AppResources.Error, keyInfo.errorMessage, AppResources.OK);
                        }
                    });
                }
                else
                {
                    ShowLoading(false);

                    if (message.StartsWith(Errors.LOGIN_ACCOUNT_PENDING, StringComparison.OrdinalIgnoreCase))
                    {
                        var alertMsg = AppResources.ALERT_CONFIRM_EMAIL.Replace("\\n", "\n");
                        UserDialogs.Instance.Alert(string.Format(alertMsg, ""));
                    }
                    else
                    {
                        DisplayAlert(AppResources.Error, message, AppResources.OK);
                    }
                }
            });
		}

		void SignupGesture_Tapped(object sender, EventArgs e)
		{
            Navigation.PushAsync(new RegisterPage());
		}

		void RememberGesture_Tapped(object sender, EventArgs e)
		{
            rememberUser = !rememberUser;
            ChangeUIOption();
		}

        void ChangeUIOption()
        {
            imgRemember.Source = ImageSource.FromFile(rememberUser ? "check_box_checked_small.png" : "check_box_uncheck_small.png");            
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
