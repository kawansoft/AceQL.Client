using System;

//29/04/2017 20:38 NDP: C#: begin remove all System.Data references
//30/04/2017 21:17 NDP: C#: version use streams fro Blob, no more file because of Portable Library project
//30/04/2017 21:23 NDP: C#: Suppress JavaTypesConverter & JavaSqlTypes
//30/04/2017 22:32 NDP: C#: Clean var names, all namespace are now AceQL.Client & add  PortableFile & PortableFileInfo
//30/04/2017 22:33 NDP: C#: All code is compatible with Portable Class Library!
//01/05/2017 16:01 NDP: C#: Yeah... Conversion to iOS, Android, Windows Portable Class Library done!
//02/05/2017 15:45 NDP: C# clean XML comments
//02/05/2017 17:40 NDP: C# Use HttpClient for all http request
//02/05/2017 18:01 NDP: C# Clean XML comments
//03/05/2017 01:32 NDP: C# Clean XML comments
//03/05/2017 15:22 NDP: C# Clean XML comments & MyRemoteConnection: clean code
//03/05/2017 18:18 NDP: C# Clean XML comment & throw ArgumentNullException for null passed parameters
//03/05/2017 20:05 NDP: C# Clean XML comment & remove AceQLTransaction public constructor
//04/05/2017 11:13 NDP: C# Format MyRemoteConnection
//04/05/2017 17:49 NDP: C# Pass stateless mode to /connect API
//04/05/2017 17:49 NDP: C# AceQConnection: database is now before username
//05/05/2017 18:06 NDP: C# AceQLHttpApi: new method to use if set system proxy and use, if set, credentials.
//06/05/2017 01:19 NDP: C# api.file package is now public.
//06/05/2017 02:06 NDP: C# Add PortableFile.GetFolderPathAsync()
//06/05/2017 13:30 NDP: C# PortableFile & PortableFileInfo: hide PCLStorage FileNotFoundException
//06/05/2017 14:43 NDP: C# PortableFile & PortableFileInfo are now in api namespace
//06/05/2017 14:55 NDP: C# Add PCLStorage License
//06/05/2017 15:25 NDP: C# Clean PortableFile comments
//06/05/2017 13:28 NDP: C# PortableFile: do not create folder when accessing in read mode to a file.
//06/05/2017 15:59 NDP: C# Directory info are in PortableFileInfo to be consistent with Windows implementation
//09/05/2017 20:55 NDP: C# AceQLConnection: add SetCancellationTokenSource
//10/05/2017 19:00 NDP: C# Allow to pass ProxyUri in connection string and use it instead of System.Net.WebRequest.DefaultWebProxy
//10/05/2017 19:32 NDP: C# AceQLClientSdk is the new Visual C# project
//10/05/2017 22:08 NDP: C# Releases
//10/05/2017 22:20 NDP: C# BeginTransaction renamed to BeginTransactionAsync 
//11/05/2017 12:24 NDP: C# Correct XML comments spelling
//11/05/2017 12:39 NDP: C# Packages first letters are now uppercase

namespace AceQL.Client.Api.Util
{
    internal class VersionValues
    {
        internal static readonly String PRODUCT = "AceQL HTTP Client SDK";
        internal static readonly String VERSION = "v1.0.2";
        internal static readonly String DATE = "11-may-2017";
    }
}
