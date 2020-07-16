using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests.Test
{
    public class MyProxyInfo
    {
        private readonly string proxyUsername;
        private readonly string proxyPassword;

        public MyProxyInfo()
        {
            String[] lines = File.ReadAllLines("i:\\neotunnel.txt");
            proxyUsername = lines[0];
            proxyPassword = lines[1];
        }

        public string ProxyPassword { get => proxyPassword;}
        public string ProxyUsername { get => proxyUsername;}
    }
}
