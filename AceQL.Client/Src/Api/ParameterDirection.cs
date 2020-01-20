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
    /// Specifies the type of a parameter within a query.
    /// </summary>
    public enum ParameterDirection
    {
        /// <summary>
        /// The parameter is an input parameter.
        /// </summary>
        Input = 1,

        /// <summary>
        /// The parameter is an output parameter.
        /// </summary>
        Output = 2,

        /// <summary>
        /// The parameter is capable of both input and output.
        /// </summary>
        InputOutput = 3,

        /// <summary>
        /// The parameter represents a return value from an operation such as a stored procedure,
        /// built-in function, or user-defined function.
        /// </summary>
        ReturnValue = 6
    }
}
