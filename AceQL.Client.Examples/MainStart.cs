﻿using AceQL.Client.Samples;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Samples
{
    class MainStart
    {
        static void Main(string[] args)
        {
            int mainToLaunch = 2;

            if (mainToLaunch == 1)
            {
                MyRemoteConnection.TheMain(args);
            }
            else if (mainToLaunch == 2)
            {
                AceQLExample.TheMain(args);
            }
        }
    }
}
