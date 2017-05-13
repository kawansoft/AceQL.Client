using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using PCLStorage;

namespace AceQL.Client.Api.File
{
    /// <summary>
    /// Equivalent in our Portable Class Library of Windows System.IO.File for some important methods not included
    /// in PCLStorage, such as AppendAllTextAsync() or GetLengthAsync().
    /// Methods are all async, as storage methods are async on some OS.
    /// Implementation uses PCLStorage (https://github.com/dsplaisted/PCLStorage).
    /// </summary>
    public class PortableFile
    {
      
        /// <summary>
        /// Opens a file, appends the specified string to the file, and then closes the file. If the file does not exist, this method creates a file, writes the specified string to the file, then closes the file.
        /// </summary>
        /// <param name="file">The file on which to trace</param>
        /// <param name="contents">The string to append to the file.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The file is null.</exception>
        public static async Task AppendAllTextAsync(IFile file, string contents)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file is null!");
            }

            var bufferArray = Encoding.UTF8.GetBytes(contents);
            using (Stream streamToWrite = await file.OpenAsync(FileAccess.ReadAndWrite).ConfigureAwait(false))
            {
                streamToWrite.Position = streamToWrite.Length;
                if (streamToWrite.CanWrite)
                {
                    await streamToWrite.WriteAsync(bufferArray, 0, bufferArray.Length).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Returns the file length.
        /// </summary>
        /// <returns>The file length.</returns>
        /// <exception cref="System.ArgumentNullException">The file is null.</exception>
        public async Task<long> GetLengthAsync(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file is null!");
            }

            long theLength = 0;

            using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read))
            {
                while (stream.ReadByte() > 0)
                {
                    theLength++;
                }
            }

            return theLength;
        }

    }
}
