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
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Safester.CryptoLibrary.Api;
using Safester.Models;
using Safester.Network;
using Safester.Utils;
using Xamarin.Forms;

namespace Safester.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public Command LoadSettingsCommand { get; set; }
        public Command SaveSettingsCommand { get; set; }

        public Action<bool> ResponseAction { get; set; }

        public SettingsViewModel()
        {
            LoadSettingsCommand = new Command(async () => await ExecuteLoadSettingsCommand());
            SaveSettingsCommand = new Command(async () => await ExecuteSaveSettingsCommand());
        }

        async Task ExecuteLoadSettingsCommand()
        {
            try
            {
                var settings = await ApiManager.SharedInstance().GetUserSettings(App.CurrentUser.UserEmail, App.CurrentUser.Token);
                if (settings != null)
                {
                    App.UserSettings = settings;
                    Utils.Utils.SaveDataToFile(App.UserSettings, Utils.Utils.KEY_FILE_USERSETTINGS);
                    ResponseAction?.Invoke(true);
                }
                else
                {
                    ResponseAction?.Invoke(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        async Task ExecuteSaveSettingsCommand()
        {
            try
            {
                var result = await ApiManager.SharedInstance().SetUserSettings(App.CurrentUser.UserEmail, App.CurrentUser.Token, App.UserSettings);
                if (string.IsNullOrEmpty(result) == false)
                {
                    if (result.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        Utils.Utils.SaveDataToFile(App.UserSettings, Utils.Utils.KEY_FILE_USERSETTINGS);
                        ResponseAction?.Invoke(true);
                    }
                    else
                    {
                        ResponseAction?.Invoke(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
