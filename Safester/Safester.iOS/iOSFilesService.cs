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
using Safester.Services;
using Safester.iOS;
using UIKit;
using Foundation;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(iOSFilesService))]
namespace Safester.iOS
{
    public class iOSFilesService : IFilesService
    {
        public void OpenUri(string uri)
        {
            try
            {
                var PreviewController = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(uri));
                PreviewController.Delegate = new CustomInteractionDelegate(UIApplication.SharedApplication.KeyWindow.RootViewController);

                Device.BeginInvokeOnMainThread(() =>
                {
                    PreviewController.PresentPreview(true);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string GetDownloadFolder()
        {
            var pathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            pathFile = Path.Combine(pathFile, "Downloads");

            try
            {
                Directory.CreateDirectory(pathFile);
            }
            catch (Exception ex)
            {

            }

            return pathFile;
        }
    }

    public class CustomInteractionDelegate : UIDocumentInteractionControllerDelegate
    {
        UIViewController parent;
        public CustomInteractionDelegate(UIViewController controller)
        {
            parent = controller;
        }

        public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
        {
            return parent;
        }
    }
}

