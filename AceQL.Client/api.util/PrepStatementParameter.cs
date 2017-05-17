
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Util
{

    /// <summary>
    /// Class PrepStatementParameter. Tool to build the JSON of statement_parameters for POST requests.
    /// </summary>
    internal class PrepStatementParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrepStatementParameter"/> class.
        /// </summary>
        internal PrepStatementParameter()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrepStatementParameter"/> class.
        /// </summary>
        /// <param name="paramIndex">Index of the parameter.</param>
        /// <param name="paramType">Type of the parameter.</param>
        /// <param name="paramValue">The parameter value.</param>
        internal PrepStatementParameter(int paramIndex, String paramType, String paramValue)
        {
            param_index = paramIndex;
            param_type = paramType;
            param_value = paramValue;
        }

        /// <summary>
        /// Gets or sets the index of the parameter.
        /// </summary>
        /// <value>The index of the parameter.</value>
        internal int param_index { get; set; }
        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        internal string param_type { get; set; }
        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>The parameter value.</value>
        internal string param_value { get; set; }
    }
}
