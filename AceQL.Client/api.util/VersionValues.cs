using System;

//29/04/2017 20:38 NDP: begin remove all System.Data references
//30/04/2017 21:17 NDP: version use streams fro Blob, no more file because of Portable Library project
//30/04/2017 21:23 NDP: Suppress JavaTypesConverter & JavaSqlTypes
//30/04/2017 22:32 NDP: Clean var names, all namespace are now AceQL.Client & add  PortableFile & PortableFileInfo
//30/04/2017 22:33 NDP: All code is compatible with Portable Class Library!
//01/05/2017 16:01 NDP: Yeah... Conversion to iOS, Android, Windows Portable Class Library done!
//02/05/2017 15:45 NDP: clean XML comments
//02/05/2017 17:40 NDP: Use HttpClient for all http request
//02/05/2017 18:01 NDP: Clean XML comments
//03/05/2017 01:32 NDP: Clean XML comments
//03/05/2017 15:22 NDP: Clean XML comments & MyRemoteConnection: clean code
//03/05/2017 18:18 NDP: Clean XML comment & throw ArgumentNullException for null passed parameters
//03/05/2017 20:05 NDP: Clean XML comment & remove AceQLTransaction public constructor
//04/05/2017 11:13 NDP: Format MyRemoteConnection
//04/05/2017 17:49 NDP: Pass stateless mode to /connect API
//04/05/2017 17:49 NDP: AceQConnection: database is now before username
//05/05/2017 18:06 NDP: AceQLHttpApi: new method to use if set system proxy and use, if set, credentials.
//06/05/2017 01:19 NDP: api.file package is now public.
//06/05/2017 02:06 NDP: Add PortableFile.GetFolderPathAsync()
//06/05/2017 13:30 NDP: PortableFile & PortableFileInfo: hide PCLStorage FileNotFoundException
//06/05/2017 14:43 NDP: PortableFile & PortableFileInfo are now in api namespace
//06/05/2017 14:55 NDP: Add PCLStorage License
//06/05/2017 15:25 NDP: Clean PortableFile comments
//06/05/2017 13:28 NDP: PortableFile: do not create folder when accessing in read mode to a file.
//06/05/2017 15:59 NDP: Directory info are in PortableFileInfo to be consistent with Windows implementation
//09/05/2017 20:55 NDP: AceQLConnection: add SetCancellationTokenSource
//10/05/2017 19:00 NDP: Allow to pass ProxyUri in connection string and use it instead of System.Net.WebRequest.DefaultWebProxy
//10/05/2017 19:32 NDP: AceQLClientSdk is the new Visual project
//10/05/2017 22:08 NDP: Releases
//10/05/2017 22:20 NDP: BeginTransaction renamed to BeginTransactionAsync 
//11/05/2017 12:24 NDP: Correct XML comments spelling
//11/05/2017 12:39 NDP: Packages first letters are now uppercase
//11/05/2017 17:43 NDP: Correct XML comments spelling
//11/05/2017 23:16 NDP: AceQLConnection.GetCancellationTokenSource(): add missing XML comments on return
//11/05/2017 23:16 NDP: update VERSION to 1.0.1
//12/05/2017 19:18 NDP: PortableFile: add AppendAllTextAsync() & All Traces use now AppendAllTextAsync() instead of debug ConsoleEmul.WriteLine()
//12/05/2017 19:59 NDP: PortableFile: add CopyAsync() methods
//13/05/2017 00:22 NDP: Use as much as possible directly PCLStorage API
//13/05/2017 00:27 NDP: Delete class FormUpload
//13/05/2017 02:04 NDP: PortableFile entirely rewritten: no more hiding or IFile/IFolder
//13/05/2017 02:28 NDP: License is now Apache 2.0
//13/05/2017 02:29 NDP: Clean comments
//13/05/2017 02:50 NDP: Remove unnecessary methods from PortableFile
//13/05/2017 02:54 NDP: Clean PortableFile comments
//13/05/2017 13:47 NDP: ProgressHolder renamed to ProgressIndicator

namespace AceQL.Client.Api.Util
{
    internal class VersionValues
    {
        internal static readonly String PRODUCT = "AceQL HTTP Client SDK";
        internal static readonly String VERSION = "v1.0.1-beta.1";
        internal static readonly String DATE = "13-may-2017";
    }
}
