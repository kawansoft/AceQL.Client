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

namespace AceQL.Client.Api
{
    /// <summary>
    /// Allows to pass a null value as parameter for remote SQL execution.
    /// </summary>
    public class AceQLNullValue
    {
        /// <summary>
        /// The enum value of the null type.
        /// </summary>
        private readonly AceQLNullType aceQLNullType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aceQLNullType">The type to associate the null value to.</param>
        public AceQLNullValue(AceQLNullType aceQLNullType)
        {
            this.aceQLNullType = aceQLNullType;
        }

        /// <summary>
        /// Returns the type to use on remote server for the null value.
        /// </summary>
        public AceQLNullType GetAceQLNullType()
        {
            return this.aceQLNullType;
        }
    }
}
