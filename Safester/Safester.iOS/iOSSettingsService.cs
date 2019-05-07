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
using AVFoundation;
using Foundation;
using System.IO;
using Safester.iOS;
using Safester.Services;

[assembly: Xamarin.Forms.Dependency(typeof(iOSSettingsService))]
namespace Safester.iOS
{
	public class iOSSettingsService : SettingsService
	{
		public void SaveSettings(String key, String value)
		{
			NSUserDefaults.StandardUserDefaults.SetString(value, key);

			NSUserDefaults.StandardUserDefaults.Synchronize();
		}

		public String LoadSettings(String key)
		{
			String strResult = NSUserDefaults.StandardUserDefaults.StringForKey(key);
            if (string.IsNullOrEmpty(strResult))
                strResult = string.Empty;

			return strResult;
		}

        public string GetAppVersionName()
        {
            return "1.2.3"; //NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
        }

        public void AskContactsPermission(Action ContactsGrantedAction)
        {
            ContactsGrantedAction?.Invoke();
        }
    }
}

