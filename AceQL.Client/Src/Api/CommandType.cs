using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api
{
    /// <summary>
    /// Specifies how a command string is interpreted.
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// An SQL text command. (Default.)
        /// </summary>
        Text = 1,

        /// <summary>
        /// The name of a stored procedure.
        /// </summary>
        StoredProcedure = 4,

        /// <summary>
        /// The name of a table.
        /// </summary>
        TableDirect = 512
    }
}
