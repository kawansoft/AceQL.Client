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
using System.ComponentModel;
using Newtonsoft.Json;

namespace Safester.Models
{
    public class MessagesResultInfo : BaseResult
    {
        public List<Message> messages { get; set; }
    }

    public class Message : BindingModel
    {
        public long messageId { get; set; }
        public string senderEmailAddr { get; set; }
        public string senderName { get; set; }
        public List<Recipient> recipients { get; set; }
        public long date { get; set; }
        public long size { get; set; }
        public string subject { get; set; }
        public bool hasAttachs { get; set; }

        private bool isRead;
        public bool IsRead { get { return isRead; } set
            {
                isRead = value; OnPropertyChanged("IsRead");
            } }

    }

    public class Recipient
    {
        public string recipientEmailAddr { get; set; }
        public string recipientName { get; set; }
        public int recipientPosition { get; set; }
        public int recipientType { get; set; }

        [JsonIgnore]
        public string displayName {
            get
            {
                return string.Format("{0}", string.IsNullOrWhiteSpace(recipientName) ? recipientEmailAddr : recipientName);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", string.IsNullOrWhiteSpace(recipientName) ? recipientEmailAddr : recipientName);
        }
    }

    public class MessageDetailInfo : BaseResult
    {
        public long message_id { get; set; }
        public string body { get; set; }
        public List<Attachment> attachments { get; set; }
    }

    public class Attachment
    {
        public int attachPosition { get; set; }
        public string filename { get; set; }

        public string filepath { get; set; }

        public long size { get; set; }
    }

    public class RecipientsBookInfo : BaseResult
    {
        public List<RecipientBook> addressBookEntries { get; set; }
    }

    public class RecipientBook
    {
        public string emailAddress { get; set; }
        public string name { get; set; }
    }

    public class SenderMailMessage
    {
        public string senderEmailAddr { get; set; }
        public List<Recipient> recipients { get; set; }
        public long size { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public List<Attachment> attachments { get; set; }
    }

    // Local Draft Temp
    public class DraftMessage
    {
        public int Id { get; set; }
        public ObservableCollection<Recipient> ToRecipients { get; set; }
        public ObservableCollection<Recipient> CcRecipients { get; set; }
        public ObservableCollection<Recipient> BccRecipients { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public ObservableCollection<Attachment> attachments { get; set; }

        [JsonIgnore]
        public string ShowToRecipients { get; set; }
    }
}
