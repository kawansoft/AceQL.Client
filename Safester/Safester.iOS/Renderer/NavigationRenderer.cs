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
using Safester.iOS.Renderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]

namespace Safester.iOS.Renderer
{
    public class CustomNavigationRenderer : NavigationRenderer
    {

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            try
            {
                var logo = NavigationBar.TopItem.RightBarButtonItem.Image;
                if (logo == null) return;

                if (logo.RenderingMode == UIImageRenderingMode.AlwaysOriginal)
                {
                    return;
                }

                foreach (var item in NavigationBar.TopItem.RightBarButtonItems)
                {
                    item.Image = item.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
