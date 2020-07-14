using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests.Test
{
    public static class ConnectionStringCurrent
    {

        public static Boolean useLocal = false;
        public static Boolean useLdapAuth = false;

        public static Boolean useAuthenticatedProxy = true;

        public static string Build()
        {
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
                    connectionString = ConnectionStringBuilderFactory.CreateDefaultRemote(useAuthenticatedProxy);
                }
                else
                {
                    connectionString = ConnectionStringBuilderFactory.CreateDefaultRemoteLdapAuth(useAuthenticatedProxy);
                }
            }

            AceQLConsole.WriteLine("connectionString: " + connectionString);
            return connectionString;
        }

    }
}
