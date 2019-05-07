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
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Safester.Services;
using Xamarin.Forms;

namespace Safester.Views
{
    public partial class SignatureInputPage : PopupPage
    {
        public SignatureInputPage()
        {
            InitializeComponent();

            var _settingsService = DependencyService.Get<SettingsService>();
            entrySignature.Text = _settingsService.LoadSettings("mobile_signature");
        }

        void Save_Clicked(object sender, System.EventArgs e)
        {
            var _settingsService = DependencyService.Get<SettingsService>();
            _settingsService.SaveSettings("mobile_signature", string.IsNullOrEmpty(entrySignature.Text) ? "" : entrySignature.Text);
            PopupNavigation.Instance.PopAsync();
        }

        void Cancel_Clicked(object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}
