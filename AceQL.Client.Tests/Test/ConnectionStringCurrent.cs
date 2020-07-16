using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests.Test
{
    public static class ConnectionStringCurrent
    {

        public static readonly Boolean useLocal ;
        public static readonly Boolean useLdapAuth;
        public static readonly Boolean useAuthenticatedProxy;

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
                    connectionString = ConnectionStringBuilderFactory.CreateDefaultRemoteLdapAuth(useAuthenticatedProxy);
                }
                else
                {
                    connectionString = ConnectionStringBuilderFactory.CreateDefaultRemote(useAuthenticatedProxy);
                }
            }

            AceQLConsole.WriteLine("connectionString: " + connectionString);
            return connectionString;
        }

    }
}
