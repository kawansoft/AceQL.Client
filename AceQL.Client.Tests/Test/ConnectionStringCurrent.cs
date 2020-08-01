using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests.Test
{
    public static class ConnectionStringCurrent
    {

        public static readonly Boolean useLocal = false;
        public static readonly Boolean useLdapAuth;
        public static readonly int typeAuthenticatedProxy = ConnectionStringBuilderFactory.AUTHENTICATED_PROXY_CREDENTIAL_CACHE;

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
