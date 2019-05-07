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
using Safester.Models;
using Xamarin.Forms;

namespace Safester.Views
{
    public partial class DraftItemsPage : ContentPage
    {
        public DraftItemsPage()
        {
            InitializeComponent();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as DraftMessage;
            if (item == null)
                return;

            var itemsPage = new NewItemPage(item);
            await Navigation.PushAsync(itemsPage);

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.DraftMessages != null)
            {
                foreach (var item in App.DraftMessages)
                {
                    item.ShowToRecipients = string.Empty;
                    if (item.ToRecipients != null && item.ToRecipients.Count > 0)
                        item.ShowToRecipients = string.Join(";", item.ToRecipients);
                }
            }
            ItemsListView.ItemsSource = App.DraftMessages;
        }

        async void ComposeItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewItemPage());
        }
    }
}
