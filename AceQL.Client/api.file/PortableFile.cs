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
    /// Instead of paths, files are defined as of a simple name (without path separator), and are stored in
    /// a folder name.
    /// Methods are all async, as storage methods are async on some OS.
    /// Implementation uses https://github.com/dsplaisted/PCLStorage.
    /// </summary>
    public class PortableFile
    {

        /// <summary>
        /// Says if a file exists in a folder.
        /// </summary>
        /// <param name="folderName">Name of the folder, without path separators.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>If the folder and the file exist, else false.</returns>
        /// <exception cref="System.ArgumentNullException">The file name or folder name is null.</exception>
        public static async Task<bool> ExistsAsync(string folderName, string fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFolder folder = null;
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                folder = await rootFolder.GetFolderAsync(folderName).ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            IFile file = null;

            try
            {
                file = await folder.GetFileAsync(fileName).ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            if (file == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Creates a file with a name in a folder. Folder is created if it does not exist.
        /// </summary>
        /// <param name="folderName">Name of the folder, without path separators.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>A new file.</returns>
        public static async Task<IFile> CreateFileAsync(String folderName, String fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(folderName,
                CreationCollisionOption.OpenIfExists).ConfigureAwait(false);

            IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);
            return file;
        }

        /// <summary>
        /// Gets an existing file.
        /// </summary>
        /// <param name="folderName">Name of the folder, without path separators.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>An existing file instance.</returns>
        /// <exception cref="System.IO.FileNotFoundException">If the folder does not exist or the file was not found in the specified folder.</exception>
        public static async Task<IFile> GetFileAsync(String folderName, String fileName)
        {

            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.GetFolderAsync(folderName).ConfigureAwait(false);
            IFile file = await folder.GetFileAsync(fileName).ConfigureAwait(false);
            return file;
        }


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
