using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using PCLStorage;

namespace AceQL.Client.api.file
{
    /// <summary>
    /// Equivalent in our Portable Class Library of Windows System.IO.File for most important methods.
    /// Instead of paths, files are composed of a simple name (without path separator), and are stored in
    /// a folder name.
    /// Methods are all async, as storage methods are async on some OS.
    /// Implementation uses https://github.com/dsplaisted/PCLStorage.
    /// </summary>
    public class PortableFile
    {

        /// <summary>
        /// Says if a file exists in a folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>If the folder and the file exist, else false.</returns>
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
        /// Deletes silentely the specified file in the specified folder.
        /// No Exceptions are raised if folder or file does no exists.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        public static async Task DeleteAsync(string folderName, string fileName)
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
                return;
            }

            IFile file = null;

            try
            {
                file = await folder.GetFileAsync(fileName).ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
                return;
            }

            if (file == null)
            {
                return;
            }

            await file.DeleteAsync();
        }

        /// <summary>
        /// Creates or overwrites the specified file. Folder is created if it does not exist.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>A System.IO.Stream that provides read/write access to the file specified.</returns>
        public static async Task<Stream> CreateAsync(string folderName, String fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFile file = await CreateIFileAsync(folderName, fileName);

            Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false);
            return stream;
        }

        /// <summary>
        /// Creates or opens a file for writing UTF-8 encoded text.
        /// Folder is created if it does not exist.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>A System.IO.StreamWriter that writes to the specified file using UTF-8 encoding.</returns>
        public static async Task<StreamWriter> CreateTextAsync(string folderName, String fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFile file = await CreateIFileAsync(folderName, fileName);

            Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false);
            StreamWriter streamWriter = new StreamWriter(stream);
            return streamWriter;

        }

        /// <summary>
        /// Opens an existing file for reading.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>A read-only System.IO.FileStream on the specified path.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The folder does not exist, or the file was not found in the specified folder.</exception>
        public static async Task<Stream> OpenReadAsync(string folderName, string fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFile file = null;

            try
            {
                file = await GetIFileAsync(folderName, fileName).ConfigureAwait(false);
            }
            catch (FileNotFoundException exception)
            {
                throw new FileNotFoundException(exception.Message);
            }

            // If implementation changes:
            if (file == null)
            {
                throw new FileNotFoundException("file does not exist: " + folderName + "/" + fileName);
            }

            Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read).ConfigureAwait(false);
            return stream;
        }


        /// <summary>
        /// Opens an existing UTF-8 encoded text file in read access for reading.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>A System.IO.TextReader on the specified file.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The folder does not exist, or the file was not found in the specified folder.</exception>
        public static async Task<TextReader> OpenTextAsync(string folderName, string fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFile file = null;

            try
            {
                file = await GetIFileAsync(folderName, fileName).ConfigureAwait(false);
            }
            catch (FileNotFoundException exception)
            {
                throw new FileNotFoundException(exception.Message);
            }

            // If implementation changes:
            if (file == null)
            {
                throw new FileNotFoundException("file does not exist: " + folderName + "/" + fileName);
            }

            Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read).ConfigureAwait(false);
            TextReader textTexreader = new StreamReader(stream);
            return textTexreader;
        }

        /// <summary>
        /// Writes text to a file, overwriting any existing data. Folder is created if it does not exist.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <param name="contents">The content to write to the file</param>
        public static async Task WriteAllTextAsync(string folderName, string fileName, string contents)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFile file = await CreateIFileAsync(folderName, fileName).ConfigureAwait(false);
            await file.WriteAllTextAsync(contents).ConfigureAwait(false);
        }


        /// <summary>
        /// Reads the contents of a file as a string.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>The contents of the file</returns>
        /// <exception cref="System.IO.FileNotFoundException">The file was not found in the specified folder.</exception>
        public static async Task<string> ReadAllTextAsync(string folderName, string fileName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            IFile file = null;

            try
            {
                file = await GetIFileAsync(folderName, fileName).ConfigureAwait(false);
            }
            catch (FileNotFoundException exception)
            {
                throw new FileNotFoundException(exception.Message);
            }

            // If implementation changes
            if (file == null)
            {
                throw new FileNotFoundException("file does not exist: " + folderName + "/" + fileName);
            }

            return await file.ReadAllTextAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Create a IFile with a name in a folder.  Folder is created if it does not exist.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>A new IFile.</returns>
        private static async Task<IFile> CreateIFileAsync(String folderName, String fileName)
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
        /// Gets an existing IFile.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Simple name of the file. Example: myfile.txt.</param>
        /// <returns>An existing IFile.</returns>
        /// <exception cref="System.IO.FileNotFoundException">If the folder does not exist or the file was not found in the specified folder.</exception>
        internal static async Task<IFile> GetIFileAsync(String folderName, String fileName)
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

    }
}
