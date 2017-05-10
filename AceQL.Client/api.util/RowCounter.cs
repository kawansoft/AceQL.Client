using AceQL.Client.api.file;
using AceQL.Client.api.util;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AceQL.Client.api
{
    /// <summary>
    /// Counts rows in a JSON file.
    /// </summary>
    internal class RowCounter
    {
        private string fileName;
        private bool traceOn;
        private string folderName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="folderName">The folder name.</param>
        /// <param name="fileName">The file name.</param>
        internal RowCounter(string folderName, string fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            this.folderName = folderName;
            this.fileName = fileName;
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <returns>System.Int32.</returns>
        internal async Task<int> CountAsync()
        {
            if (!await PortableFile.ExistsAsync(folderName, fileName).ConfigureAwait(false))
            {
                throw new System.IO.FileNotFoundException("JSON file does not exist: " + fileName);
            }

            Trace();
            using (TextReader textReader = await PortableFile.OpenTextAsync(folderName, fileName).ConfigureAwait(false))
            {
                using (JsonTextReader reader = new JsonTextReader(textReader))
                {
                    while (reader.Read())
                    {
                        if (reader.Value != null)
                        {
                            if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("row_count"))
                            {
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