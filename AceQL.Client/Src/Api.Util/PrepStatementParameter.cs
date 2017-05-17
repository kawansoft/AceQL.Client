/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2017,  KawanSoft SAS
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
﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Util
{

    /// <summary>
    /// Class PrepStatementParameter. Tool to build the JSON of statement_parameters for POST requests.
    /// </summary>
    internal class PrepStatementParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrepStatementParameter"/> class.
        /// </summary>
        internal PrepStatementParameter()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrepStatementParameter"/> class.
        /// </summary>
        /// <param name="paramIndex">Index of the parameter.</param>
        /// <param name="paramType">Type of the parameter.</param>
        /// <param name="paramValue">The parameter value.</param>
        internal PrepStatementParameter(int paramIndex, String paramType, String paramValue)
        {
            param_index = paramIndex;
            param_type = paramType;
            param_value = paramValue;
        }

        /// <summary>
        /// Gets or sets the index of the parameter.
        /// </summary>
        /// <value>The index of the parameter.</value>
        internal int param_index { get; set; }
        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        internal string param_type { get; set; }
        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>The parameter value.</value>
        internal string param_value { get; set; }
    }
}
