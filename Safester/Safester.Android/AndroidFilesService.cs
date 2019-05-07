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
using Android.Content;
using Plugin.CurrentActivity;
using Safester.Services;
using Safester.Droid;
using Android.Support.V4.Content;
using Android.Webkit;
using Android.Content.PM;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidFilesService))]
namespace Safester.Droid
{
    public class AndroidFilesService : IFilesService
    {
        public void OpenUri(string uri)
        {
            Java.IO.File f = new Java.IO.File(uri);
            f.SetReadable(true);

            var context = Forms.Context.ApplicationContext;
            var targetUri = FileProvider.GetUriForFile(context, "com.app.Safester.provider", f);

            var intent = new Intent(Intent.ActionView);
            MimeTypeMap myMime = MimeTypeMap.Singleton;
            String mimeType = myMime.GetMimeTypeFromExtension(fileExt(uri));
            intent.SetDataAndType(targetUri, mimeType);
            intent.SetFlags(ActivityFlags.NewTask);

            intent.AddFlags(ActivityFlags.GrantReadUriPermission);

            PackageManager packageManager = Forms.Context.PackageManager;
            if (intent.ResolveActivity(packageManager) != null)
            {
                context.StartActivity(intent);
            }
            else
            {
                // no app found that can handle the intent
                Toast.MakeText(context, "No handler for this type of file", ToastLength.Long).Show();
            }
        }

        public string GetDownloadFolder()
        {
            var pathFile = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            var absolutePath = pathFile.AbsolutePath;

            return absolutePath;
        }

        private string fileExt(string url)
        {
            if (url.IndexOf("?", StringComparison.OrdinalIgnoreCase) > -1)
            {
                url = url.Substring(0, url.IndexOf("?", StringComparison.OrdinalIgnoreCase));
            }
            if (url.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) == -1)
            {
                return null;
            }
            else
            {
                String ext = url.Substring(url.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1);
                if (ext.IndexOf("%", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    ext = ext.Substring(0, ext.IndexOf("%", StringComparison.OrdinalIgnoreCase));
                }
                if (ext.IndexOf("/", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    ext = ext.Substring(0, ext.IndexOf("/", StringComparison.OrdinalIgnoreCase));
                }
                return ext.ToLower();

            }
        }
    }
}

