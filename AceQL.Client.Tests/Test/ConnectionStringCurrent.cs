using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests.Test
{
    public static class ConnectionStringCurrent
    {

        public static Boolean useLocal;
        public static Boolean useLdapAuth;
        public static int typeAuthenticatedProxy;

        public static string Build()
        {
            String file = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\aceql.client.ini";
            PropFileReader propFileReader = new PropFileReader(file);

            useLocal = Boolean.Parse(propFileReader.getProperty("useLocal"));
            useLdapAuth = Boolean.Parse(propFileReader.getProperty("useLdapAuth"));
            typeAuthenticatedProxy = int.Parse(propFileReader.getProperty("typeAuthenticatedProxy"));

            AceQLConsole.WriteLine("useLocal              : " + useLocal);
            AceQLConsole.WriteLine("useLdapAuth           : " + useLdapAuth);
            AceQLConsole.WriteLine("typeAuthenticatedProxy: " + typeAuthenticatedProxy);
            AceQLConsole.WriteLine();

            String connectionString = null;

            if (useLocal)
            {
                if (useLdapAuth)
                {
                    connectionString = ConnectionStringBuilderFactory.CreateDefaultLocalLdapAuth();
                }
                else
                {
                    connectionString = ConnectionStringBuilderFactory.CreateDefaultLocal();
                }
            }
            else
            {
                if (useLdapAuth)
                {
                    connectionString = ConnectionStringBuilderFactory.CreateDefaultRemoteLdapAuth(typeAuthenticatedProxy);
                }
                else
                {
                    connectionString = ConnectionStringBuilderFactory.CreateDefaultRemote(typeAuthenticatedProxy);
                }
            }

            AceQLConsole.WriteLine("connectionString: " + connectionString);
            return connectionString;
        }

    }
}
