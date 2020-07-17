
/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2020,  KawanSoft SAS
 * (http://www.kawansoft.com). All rights reserved.                                
 *                                                                               
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */

using System;

namespace AceQL.Client.Api
{
    /// <summary>
    /// <see cref="AceQLProgressIndicator"/> Class. Allows to get the percentage progress between 0 and 100 for Blob/Clob upload.
    /// </summary>
    public class AceQLProgressIndicator
    {
        /// <summary>   The perccent progress value set by upload thread.</summary>
        private int percent;

        /// <summary>
        /// Gets the upload progress value between 0 and 100.
        /// </summary>
        public int Percent
        {
            get
            {
                return percent;
            }
        }

        /// <summary>
        /// Returns the string representation of the progress value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return percent.ToString();
        }


        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        internal void SetValue(int value)
        {
            percent = value;
        }
    }
}
