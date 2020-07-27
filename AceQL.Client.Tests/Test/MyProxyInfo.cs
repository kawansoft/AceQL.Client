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

        public static readonly String NEOTUNNEL_SAVE_TXT = "i:\\neotunnel_SAVE.txt";

        private readonly string proxyUsername;
        private readonly string proxyPassword;


     public MyProxyInfo()
        {
            String[] lines = File.ReadAllLines("i:\\neotunnel.txt");
            proxyUsername = lines[0];
            proxyPassword = lines[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyProxyInfo"/> class.
        /// </summary>
        /// <param name="filepath">The filepath of username & password</param>
       public MyProxyInfo(String filepath)
        {
            String[] lines = File.ReadAllLines(filepath);
            proxyUsername = lines[0];
            proxyPassword = lines[1];
        }

        public string ProxyPassword { get => proxyPassword;}
        public string ProxyUsername { get => proxyUsername;}
    }
}
