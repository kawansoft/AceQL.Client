using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api
{
    /// <summary>
    /// Specifies the type of a parameter within a query.
    /// </summary>
    public enum ParameterDirection
    {
        /// <summary>
        /// The parameter is an input parameter.
        /// </summary>
        Input = 1,

        /// <summary>
        /// The parameter is an output parameter.
        /// </summary>
        Output = 2,

        /// <summary>
        /// The parameter is capable of both input and output.
        /// </summary>
        InputOutput = 3,

        /// <summary>
        /// The parameter represents a return value from an operation such as a stored procedure,
        /// built-in function, or user-defined function.
        /// </summary>
        ReturnValue = 6
    }
}
