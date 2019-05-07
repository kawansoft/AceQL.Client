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
using Safester.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Xamarin")]
[assembly: ExportEffect(typeof(ShowHidePassEffect), "ShowHidePassEffect")]
namespace Safester.iOS.Effects
{
    public class ShowHidePassEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            ConfigureControl();
        }

        protected override void OnDetached()
        {

        }

        private void ConfigureControl()
        {
            if (Control != null)
            {
                UITextField vUpdatedEntry = (UITextField)Control;
                var buttonRect = UIButton.FromType(UIButtonType.Custom);
                buttonRect.SetImage(new UIImage("eye_icon"), UIControlState.Normal);
                buttonRect.TouchUpInside += (object sender, EventArgs e1) =>
                {
                    if (vUpdatedEntry.SecureTextEntry)
                    {
                        vUpdatedEntry.SecureTextEntry = false;
                        buttonRect.SetImage(new UIImage("eye_not_icon"), UIControlState.Normal);
                    }
                    else
                    {
                        vUpdatedEntry.SecureTextEntry = true;
                        buttonRect.SetImage(new UIImage("eye_icon"), UIControlState.Normal);
                    }
                };

                vUpdatedEntry.ShouldChangeCharacters += (textField, range, replacementString) =>
                {
                    string text = vUpdatedEntry.Text;
                    var result = text.Substring(0, (int)range.Location) +
                     replacementString + text.Substring((int)range.Location + (int)range.Length);
                    vUpdatedEntry.Text = result;
                    return false;
                };

                buttonRect.Frame = new CoreGraphics.CGRect(10.0f, 0.0f, 15.0f, 15.0f);
                buttonRect.ContentMode = UIViewContentMode.Right;

                UIView paddingViewRight = new UIView(new System.Drawing.RectangleF
                                          (5.0f, -5.0f, 30.0f, 18.0f));
                paddingViewRight.Add(buttonRect);
                paddingViewRight.ContentMode = UIViewContentMode.BottomRight;


                vUpdatedEntry.LeftView = paddingViewRight;
                vUpdatedEntry.LeftViewMode = UITextFieldViewMode.Always;

                Control.Layer.CornerRadius = 4;
                Control.Layer.BorderColor = new CoreGraphics.CGColor(255, 255, 255);
                Control.Layer.MasksToBounds = true;
                vUpdatedEntry.TextAlignment = UITextAlignment.Left;
            }
        }
    }
}
