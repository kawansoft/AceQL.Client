using System;

namespace AceQL.Client.Api.Util
{
    internal class Version
    {
        internal static String GetVersion()
        {
            return VersionValues.PRODUCT + " " + VersionValues.VERSION + " - " + VersionValues.DATE;
        }
    }
}
