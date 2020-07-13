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
﻿using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Util
{
    /// <summary>
    /// Console emulator as in Portable Class Library we don't have access to Console.
    /// </summary>
    static internal class ConsoleEmul
    {
        private const string CONSOLE_EMUL = "CONSOLE_EMUL";

        internal static void WriteLine()
        {
            System.Diagnostics.Debug.WriteLine(CONSOLE_EMUL);
        }

        internal static void WriteLine(string s)
        {
            System.Diagnostics.Debug.WriteLine("CONSOLE_EMUL" + " " + s);
        }
    }
}
