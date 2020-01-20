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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Specifies how a command string is interpreted.
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// An SQL text command. (Default.)
        /// </summary>
        Text = 1,

        /// <summary>
        /// The name of a stored procedure.
        /// </summary>
        StoredProcedure = 4,

        /// <summary>
        /// The name of a table.
        /// </summary>
        TableDirect = 512
    }
}
