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

using AceQL.Client.Api;
using AceQL.Client.Api.Metadata;
using AceQL.Client.Api.Metadata.Dto;
using AceQL.Client.Tests.Test;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests
{
    public static class MainStart
    {

        public static void Main(string[] args)
        {
            int mainToLaunch = 6;

            if (mainToLaunch == 1)
            {
                AceQLTest.TheMain(args);
                AceQLTestClose.TheMain(args);
            }
            else if (mainToLaunch == 2)
            {
                AceQLTestColumnAsKeyName.TheMain(args);
            }
            else if (mainToLaunch == 3)
            {
                JsonTest.TheMain(args);
            }
            else if (mainToLaunch == 4)
            {
                MySqlStoredProcedureTest.TheMain(args);
            }
            else if (mainToLaunch == 5)
            {
                SqlServerStoredProcedureTest.TheMain(args);
            }
            else if (mainToLaunch == 6)
            {
                AceQLTest.TheMain(args);
                AceQLTestMetadata.TheMain();
            }
            else if (mainToLaunch == 7)
            {
                AceQLTestSessionId.TheMain();
                AceQLTestMetadata.TheMain();
            }
            else if (mainToLaunch == 8)
            {
                AceQLTestClose.TheMain(args);
            }

        }

    }
}
