using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Class that allows to store the progress between 0 and 100 for a Blob/Clob upload.
    /// </summary>
    public class ProgressIndicator
    {
        private int progress = 0;

        /// <summary>
        /// The progress value between 0 and 100.
        /// </summary>
        public int Value
        {
            get
            {
                return progress;
            }

            set
            {
                progress = value;
            }
        }

        /// <summary>
        /// Returns the string representation of the progress value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return progress.ToString();
        }
    }
}
