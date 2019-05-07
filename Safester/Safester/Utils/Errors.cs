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
namespace Safester.Utils
{
    public static class Errors
    {
        public static string NO_ERROR = "ok";
        public static string API_ERROR = "ko";

        // Register API
        public static string REGISTER_ACCOUNT_EXISTS = "error_account_already_exists";
        public static string REGISTER_EMAIL_INVALID = "error_invalid_email_address";
        public static string REGISTER_COUPON_INVALID = "error_invalid_coupon";

        // Login API
        public static string LOGIN_ACCOUNT_PENDING = "error_account_pending_validation";

        public static bool IsErrorExist(string response)
        {
            if (!string.IsNullOrEmpty(response) && response.StartsWith(Errors.API_ERROR, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public static bool IsApiSuccess(string response)
        {
            if (!string.IsNullOrEmpty(response) && response.StartsWith(Errors.NO_ERROR, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
