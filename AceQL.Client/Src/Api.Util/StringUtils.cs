using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api.Util
{
    /// <summary>
    /// Class StringUtils. Utilities fro String management.
    /// </summary>
    internal class StringUtils
    {
        /// <summary>
        /// Gets the substring before the first occurrence of a separator. The separator is not returned.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>System.String.</returns>
        internal static string SubstringBefore(string str, string separator)
        {
            if (str == null || str.Length == 0)
            {
                return str;
            }

            int commaIndex = str.IndexOf(separator);

            if (commaIndex <= 0)
            {
                return str;
            }
            else
            {
                return str.Substring(0, commaIndex);
            }

        }
    }
}
