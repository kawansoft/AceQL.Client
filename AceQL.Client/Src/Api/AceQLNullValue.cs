using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Allows to pass a null value as parameter for remote SQL execution.
    /// </summary>
    public class AceQLNullValue
    {
        /// <summary>
        /// The enum value of the null type.
        /// </summary>
        private readonly AceQLNullType aceQLNullType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aceQLNullType">The type to associate the null value to.</param>
        public AceQLNullValue(AceQLNullType aceQLNullType)
        {
            this.aceQLNullType = aceQLNullType;
        }

        /// <summary>
        /// Returns the type to use on remote server for the null value.
        /// </summary>
        public AceQLNullType GetAceQLNullType()
        {
            return this.aceQLNullType;
        }
    }
}
