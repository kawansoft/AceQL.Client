using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests.Test
{
    /// <summary>
    /// Class for getting with JSON the out parameter indexes and values.
    /// </summary>
    internal class ParametersOutIndex
    {
        /// <summary>
        /// The parameter name
        /// </summary>
        private string parameterIndex;

        /// <summary>
        /// The value
        /// </summary>
        private object theValue = null;

        /// <summary>
        /// Specifies the out parameter index.
        /// </summary>
        public string ParameterIndex { get => parameterIndex; set => parameterIndex = value; }

        /// <summary>
        /// Specifies the out parameter value.
        /// </summary>
        public object Value { get => theValue; set => theValue = value; }
    }
}
