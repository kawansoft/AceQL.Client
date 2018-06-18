using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests.Test
{
    /// <summary>
    /// Class for getting with JSON the out parameter names and values.
    /// </summary>
    internal class ParametersOutName
    {
        /// <summary>
        /// The parameter name
        /// </summary>
        private string parameterName;

        /// <summary>
        /// The value
        /// </summary>
        private object theValue = null;

        /// <summary>
        /// Specifies the out parameter name.
        /// </summary>
        public string ParameterName { get => parameterName; set => parameterName = value; }

        /// <summary>
        /// Specifies the out parameter value.
        /// </summary>
        public object Value { get => theValue; set => theValue = value; }
    }
}
