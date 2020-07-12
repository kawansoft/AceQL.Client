using AceQL.Client.Api.File;
using AceQL.Client.Api.Util;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Src.Api.Util
{
    /// <summary>
    /// Class Tracer. Allows to create a single instance for a trace action.
    /// </summary>
    internal class SimpleTracer
    {
        /// <summary>
        /// The trace on
        /// </summary>
        private bool traceOn = false;

        /// <summary>
        /// The trace file for debug
        /// </summary>
        private IFile file = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTracer"/> class.
        /// </summary>
        public SimpleTracer()
        {
        }

        /// <summary>
        /// Says if trace is on
        /// </summary>
        /// <returns>true if trace is on</returns>
        internal bool IsTraceOn()
        {
            return traceOn;
        }

        /// <summary>
        /// Sets the trace on/off
        /// </summary>
        /// <param name="traceOn">if true, trace will be on; else race will be off</param>
        internal void SetTraceOn(bool traceOn)
        {
            this.traceOn = traceOn;
        }

        /// <summary>
        /// Traces this instance.
        /// </summary>
        internal async Task TraceAsync()
        {
            await TraceAsync("").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the name of the trace file.
        /// </summary>
        /// <returns>The name name fo the trace file</returns>
        internal async Task<String> GetTraceFileName()
        {
            if (file == null)
            {
                file = await AceQLCommandUtil.GetTraceFileAsync().ConfigureAwait(false);
            }

            return file.Name;
        }

        /// <summary>
        /// Traces the specified string.
        /// </summary>
        /// <param name="contents">The string to trace</param>
        internal async Task TraceAsync(String contents)
        {
            if (traceOn)
            {
                if (file == null)
                {
                    file = await AceQLCommandUtil.GetTraceFileAsync().ConfigureAwait(false);
                }
                contents = DateTime.Now + " " + contents;
                await PortableFile.AppendAllTextAsync(file, "\r\n" + contents).ConfigureAwait(false);
            }
        }
    }
}
