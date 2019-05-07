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
using System.Threading.Tasks;
using Acr.UserDialogs;
using Safester.Custom.Effects;
using Safester.Network;
using Safester.Utils;
using Xamarin.Forms;

namespace Safester.Views
{
    public partial class RegisterPage : ContentPage
    {
        private bool _isCreating { get; set; }

        public RegisterPage()
        {
            InitializeComponent();

            entryPassword.Effects.Add(new ShowHidePassEffect());
            entryConfirmPassword.Effects.Add(new ShowHidePassEffect());
        }

        void Save_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entryName.Text) == true)
            {
                DisplayAlert(AppResources.Error, AppResources.ErrorInputUserName, AppResources.OK);
                return;
            }

            if (string.IsNullOrWhiteSpace(entryEmail.Text) == true)
            {
                DisplayAlert(AppResources.Error, AppResources.ErrorInputUserEmail, AppResources.OK);
                return;
            }

            if (entryEmail.Text.Contains(" ") == true)
            {
                DisplayAlert(AppResources.Error, AppResources.ErrorInputUserEmail1, AppResources.OK);
                return;
            }

            if (string.IsNullOrEmpty(entryPassword.Text) == true)
            {
                DisplayAlert(AppResources.Error, AppResources.ErrorInputUserPassPhrase, AppResources.OK);
                return;
            }

            if (string.IsNullOrEmpty(entryConfirmPassword.Text) == true)
            {
                DisplayAlert(AppResources.Error, AppResources.ErrorInputUserPassPhrase, AppResources.OK);
                return;
            }

            if (entryPassword.Text.Equals(entryConfirmPassword.Text) == false)
            {
                DisplayAlert(AppResources.Error, AppResources.ErrorInputUserPassPhrase1, AppResources.OK);
                return;
            }

            if (entryPassword.Text.Length < 10)
            {
                DisplayAlert(AppResources.Error, AppResources.ErrorInputUserPassPhrase2, AppResources.OK);
                return;
            }

            if (_isCreating)
                return;

            ShowLoading(true);

            Task.Run(() =>
            {
                ApiManager.SharedInstance().Register(entryName.Text, entryEmail.Text, entryPassword.Text, entryCoupon.Text, (success, message) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ShowLoading(false);
                        if (success == false)
                        {
                            if (message.StartsWith(Errors.REGISTER_ACCOUNT_EXISTS, StringComparison.OrdinalIgnoreCase))
                            {
                                UserDialogs.Instance.Alert(string.Format(AppResources.ALERT_ACCOUNT_EXIST, ""));
                            }
                            else if (message.StartsWith(Errors.REGISTER_EMAIL_INVALID, StringComparison.OrdinalIgnoreCase))
                            {
                                UserDialogs.Instance.Alert(string.Format(AppResources.ALERT_EMAIL_INVALID, ""));
                            }
                            else if (message.StartsWith(Errors.REGISTER_COUPON_INVALID, StringComparison.OrdinalIgnoreCase))
                            {
                                UserDialogs.Instance.Alert(string.Format(AppResources.ALERT_COUPON_INVALID, ""));
                            }
                            else
                            {
                                UserDialogs.Instance.Alert(string.Format(message, ""));
                            }
                        }
                        else
                        {
                            var alertMsg = AppResources.ALERT_REGISTER_SUCCESS.Replace("\\n", "\n");
                            UserDialogs.Instance.Alert(string.Format(alertMsg, entryEmail.Text));

                            LoginPage.CurrentUserEmail = entryEmail.Text;
                            LoginPage.CurrentUserPassword = String.Empty;
                            LoginPage.NeedsUpdating = true;

                            Navigation.PopAsync();
                        }
                    });
                });
            });
        }

        private void ShowLoading(bool isShowing)
        {
            _isCreating = isShowing;
            if (isShowing)
                UserDialogs.Instance.Loading(AppResources.Pleasewait, null, null, true);
            else
                UserDialogs.Instance.Loading().Hide();
        }
    }
}
