using System;

namespace AceQL.Client.api.util
{
    internal class Version
    {
        internal static String GetVersion()
        {
            return VersionValues.PRODUCT + " " + VersionValues.VERSION + " - " + VersionValues.DATE;
        }
    }
}
