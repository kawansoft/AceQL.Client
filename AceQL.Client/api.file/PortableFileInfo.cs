
using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AceQL.Client.Api.File
{
    /// <summary>
    /// Equivalent in our Portable Class Library of Windows System.IO.FileInfo for most important methods.
    /// Instead of paths, files are composed of a simple name (without path separator), and are stored in
    /// a folder name.
    /// Includes the static GetRootFolder() method to get the path of the root folder in the current implementation.
    /// Methods are all async, as storage methods are async on some OS.
    /// Implementation uses https://github.com/dsplaisted/PCLStorage.
    /// </summary>
    public class PortableFileInfo
    {
        private string fileName = null;
        private string folderName = null;

        /// <summary>
        /// Consructor to use for GetDirectoryNameAsync() call.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        public PortableFileInfo(string folderName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            this.folderName = folderName;
        }

        /// <summary>
        /// Constructor to use for GetLengthAsync() call.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">the file name, as myfile.txt.</param>
        public PortableFileInfo(string folderName, string fileName)
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
        /// Returns the root folder path on the current file system.
        /// </summary>
        /// <returns>The root folder path on the current file system.</returns>
        public static string GetRootFolder()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            return rootFolder.Path;
        }

        /// <summary>
        /// Gets a string representing the folder's full path. Creates the folder if it does not exist.
        /// </summary>
        /// <returns>A string representing the folder's full path.</returns>
        public async Task<string> GetDirectoryNameAsync()
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(folderName,
                CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
            return folder.Path;
        }

        /// <summary>
        /// Returns the file length.
        /// </summary>
        /// <returns>The file length.</returns>
        public async Task<long> GetLengthAsync()
        {
            if (fileName == null)
            {
                throw new FileNotFoundException("File not found because PortableFileInfo(string folderName) constructor was used instead of PortableFileInfo(string folderName, string fileName).");
            }

            IFile file = null;

            try
            {
                file = await PortableFile.GetIFileAsync(folderName, fileName).ConfigureAwait(false); 
            }
            catch (FileNotFoundException exception)
            {
                throw new FileNotFoundException(exception.Message);
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