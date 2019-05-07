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
using System.IO;
using Android.Content;
using Plugin.CurrentActivity;
using Safester.Services;
using Safester.Droid;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android.Content.PM;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidSettingsService))]
namespace Safester.Droid
{
	public class AndroidSettingsService : SettingsService
	{
		public void SaveSettings(string key, string value)
		{
			//store
			var prefs = Android.App.Application.Context.GetSharedPreferences(key, FileCreationMode.Private);
			var prefEditor = prefs.Edit();
			prefEditor.PutString(key, value);
			prefEditor.Commit();
		}

		public string LoadSettings(string key)
		{
			//retreive 
			var prefs = Android.App.Application.Context.GetSharedPreferences(key, FileCreationMode.Private);
			var value = prefs.GetString(key, "");

			return value;
		}

		public string GetAppVersionName()
		{
            return "1.2.2";
		}

        public void AskContactsPermission(Action ContactsGrantedAction)
        {
            if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Android.Manifest.Permission.ReadContacts) != Permission.Granted)
            {
                if (CrossCurrentActivity.Current.Activity is MainActivity)
                    (CrossCurrentActivity.Current.Activity as MainActivity).ContactPermissionGranted = ContactsGrantedAction;

                ActivityCompat.RequestPermissions(CrossCurrentActivity.Current.Activity, new String[] {
                Android.Manifest.Permission.ReadContacts}, 1);
            }
            else
            {
                ContactsGrantedAction?.Invoke();
            }
        }
	}
}

