using AceQL.Client.Api.File;
using AceQL.Client.Api.Util;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using PCLStorage;

namespace AceQL.Client.Api.Util
{
    /// <summary>
    /// Counts rows in a JSON file.
    /// </summary>
    internal class RowCounter
    {
        private bool traceOn;
        private IFile file;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">The Result Set JSON file to count the rows for.</param>
        /// <exception cref="System.ArgumentNullException">The file is null.</exception>
        public RowCounter(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file is null!");
            }

            this.file = file;
        }


        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <returns>System.Int32.</returns>
        internal async Task<int> CountAsync()
        {
            Trace();
            using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read).ConfigureAwait(false))
            {
                TextReader textReader = new StreamReader(stream);
                var reader = new JsonTextReader(textReader);

                while (reader.Read())
                {
                    if (reader.Value == null)
                    {
                        continue;
                    }

                    if (reader.TokenType != JsonToken.PropertyName || !reader.Value.Equals("row_count"))
                    {
                        continue;
                    }

                    if (reader.Read())
                    {
                        String rowCountStr = reader.Value.ToString();
                        int rowCount = Int32.Parse(rowCountStr);

                        Trace();
                        Trace("rowCount: " + rowCount);
                        return rowCount;
                    }
                    else
                    {
                        return 0;
                    }

                }

            }

            return 0;

        }


        /**
         * Says if trace is on
         * 
         * @return true if trace is on
         */
        /// <summary>
        /// Determines whether [is trace on].
        /// </summary>
        /// <returns><c>true</c> if [is trace on]; otherwise, <c>false</c>.</returns>
        internal bool IsTraceOn()
        {
            return traceOn;
        }

        /**
         * Sets the trace on/off
         * 
         * @param traceOn
         *            if true, trace will be on
         */
        /// <summary>
        /// Sets the trace on.
        /// </summary>
        /// <param name="traceOn">if set to <c>true</c> [trace on].</param>
        internal void SetTraceOn(bool traceOn)
        {
            this.traceOn = traceOn;
        }

        /// <summary>
        /// Traces this instance.
        /// </summary>
        private void Trace()
        {
            if (traceOn)
            {
                ConsoleEmul.WriteLine();
            }
        }

        /// <summary>
        /// Traces the specified string.
        /// </summary>
        /// <param name="theString">The string to trace.</param>
        private void Trace(String theString)
        {
            if (traceOn)
            {
                ConsoleEmul.WriteLine(theString);
            }
        }
    }
}