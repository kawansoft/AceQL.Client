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
using Safester.Views;
using Safester.Models;
using Safester.CryptoLibrary.Api;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Plugin.Multilingual;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Safester
{
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }
        public static SettingsInfo UserSettings { get; set; }

        public static Decryptor KeyDecryptor;
        public static Encryptor KeyEncryptor;

        // Local Temp Data
        public static ObservableCollection<Recipient> Recipients { get; set; }
        public static ObservableCollection<DraftMessage> DraftMessages { get; set; }

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("ODU3NjVAMzEzNzJlMzEyZTMwZ1hwRDBmQkVIRWszNE5STkNGUmQ4RkJRdVA0U1J6aFp0aFlFUUNGci9IZz0=");
            //("Nzg2MjJAMzEzNjJlMzQyZTMwU3BVRzZxdkhmUXhmYU1iYlNBQmUvaTRoZnlCU3NlK291Q0hkcnBpYkZPYz0=");

            InitializeComponent();
            AppResources.Culture = CrossMultilingual.Current.DeviceCultureInfo;

            bool armor = false;
            bool withIntegrityCheck = true;
            KeyEncryptor = new Encryptor(armor, withIntegrityCheck);

            CurrentUser = new User();

            Recipients = Utils.Utils.LoadDataFromFile<ObservableCollection<Recipient>>(Utils.Utils.KEY_FILE_RECIPIENTS);
            if (Recipients == null)
                Recipients = new ObservableCollection<Recipient>();

            UserSettings = Utils.Utils.LoadDataFromFile<SettingsInfo>(Utils.Utils.KEY_FILE_USERSETTINGS);
            if (UserSettings == null)
                UserSettings = new SettingsInfo();

            DraftMessages = Utils.Utils.LoadDataFromFile<ObservableCollection<DraftMessage>>(Utils.Utils.KEY_FILE_DRAFTMESSAGES);
            if (DraftMessages == null)
                DraftMessages = new ObservableCollection<DraftMessage>();

            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
