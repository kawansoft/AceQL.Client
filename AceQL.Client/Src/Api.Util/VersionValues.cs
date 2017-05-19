/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2017,  KawanSoft SAS
 * (http://www.kawansoft.com). All rights reserved.                                
 *                                                                               
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */

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
//13/05/2017 13:53 NDP: Version is marked as beta
//13/05/2017 14:28 NDP: Clean code loops
//13/05/2017 15:30 NDP: GetUniqueResultSetFile() renamed to GetUniqueResultSetFileAsync()
//13/05/2017 15:46 NDP: No .ConfigureAwait(false) in examples code
//13/05/2017 17:49 NDP: Add clean <cref> in comments
//13/05/2017 19:02 NDP: Add AceQLCredential class & AceQLIsolationLevel renamed to IsolationLevel
//13/05/2017 21:17 NDP: Add AceQLConnection() constructor and AceQLConnection get/set Credential & ConnectionString properties
//13/05/2017 21:45 NDP: AceQLConnection & AceQLHttpApi: clean comments
//13/05/2017 22:09 NDP: AceQLConnection: CancellationTokenSource, if set, is used on all http calls
//14/05/2017 03:03 NDP: AceQLHttpApi.CallWithGetReturnStreamAsync: fix but on twice http calls
//15/05/2017 15:11 NDP: AceQLConnection is now closed with AceQLConnection.CloseAsync
//15/05/2017 16:16 NDP: Add all possible constructors to AceCommand for user comfort
//15/05/2017 17:02 NDP: Add ReadAsync to AceQLDataReader
//15/05/2017 17:33 NDP: AceQLHttpApi.DecodeConnectionString: treat case some empty alone ";" in connection strings. Test AceQLCredential usage  
//15/05/2017 17:53 NDP: Add AceQLHttpApi.Prepare
//15/05/2017 18:44 NDP: AceQLCommand & AceQLConnection: add Async(CancellationToken cancellationToken) version
//15/05/2017 19:05 NDP: XxxAsync(CancellationToken cancellationToken) versions all done 
//15/05/2017 19:23 NDP: Clean some comments
//15/05/2017 19:45 NDP: Clean some comments
//15/05/2017 20:10 NDP: Fix spelling error in comments 
//15/05/2017 20:48 NDP: AceQLDataReader: clean comments
//15/05/2017 20:58 NDP: AceQLDataReader: clean comments
//16/05/2017 10:40 NDP: RowParser: rewrite read() which was buggy after refactoring
//16/05/2017 11:32 NDP: AceQLParameterCollection: clean comments
//16/05/2017 11:45 NDP: PortableFile & AceQLCommand: clean comments
//16/05/2017 11:57 NDP: ProgressIndicator: clean comments
//16/05/2017 13:15 NDP: AceQLException: rename errorId to errorType
//16/05/2017 15:18 NDP: AceQLParameterCollection.AddBlob renamed to AceQLParameterCollection.AddWithValue
//16/05/2017 16:19 NDP: AceQLDataReader: clean management of IsDBNull()
//16/05/2017 18:17 NDP: AceQLException: use cleaner names for public properties
//16/05/2017 21:23 NDP: Clean AceQLParameterCollection comments
//17/05/2017 14:28 NDP: AceQLParameterCollection: support add parameter version with stream for BLOB, without length
//17/05/2017 15:44 NDP: Clean comments everywhere and add new line for long comments
//17/05/2017 17:14 NDP: Remove header notices
//17/05/2017 17:14 NDP: Add Apache 2.0 notice to all files
//17/05/2017 17:39 NDP: PortableFile: clean comments
//17/05/2017 18:17 NDP: AceQLException: clean comments
//17/05/2017 19:42 NDP: All classes named "*Analyser*" renamed ro "*Analyzer*" because of meaning of US word
//17/05/2017 21:12 NDP: AceQLException: all constructor are public, as user may want to reuse it.
//17/05/2017 22:59 NDP: AceQLTransaction: clean comments
//17/05/2017 01:09 NDP: Src folder is root for source files
//17/05/2017 01:27 NDP: Simplify Null Check 
//17/05/2017 01:38 NDP: Simplify Null Check
//17/05/2017 01:45 NDP: Replace all NotSupportedException by NotSupportedException (Microsoft Code Analysis);
//17/05/2017 02:07 NDP: Rewrite Dispose methods as recommended by code analysis and https://msdn.microsoft.com/library/ms244737.aspx?f=255&MSPPError=-2147217396
//17/05/2017 02:14 NDP: AceQLHttpApi: clean comments
//17/05/2017 02:25 NDP: ProgressIndicator: move in Api Src folder
//18/05/2017 17:58 NDP: AceQLCredential is sealed and uses now a char array for password
//18/05/2017 19:10 NDP: Rename classes and suppress AceQLParameterCollection.AddWithNullValue
//18/05/2017 19:29 NDP: AceQLParameter: clean comments
//19/05/2017 01:19 NDP: FormUploadStream: test if progressIndicator is null before setting Value to 100
//19/05/2017 03:23 NDP: Clean AceQLConnection.SetProgressIndicator comments
//19/05/2017 04:44 NDP: Simplify null check as recommended by Microsoft Code Analysis
//19/05/2017 04:58 NDP: Simplify null check as recommended by Microsoft Code Analysis
//19/05/2017 05:01 NDP: Simplify null check as recommended by Microsoft Code Analysis
//19/05/2017 10:50 NDP: AceQLProgressIndicator: Value is read only.
// 
namespace AceQL.Client.Api.Util
{
    internal class VersionValues
    {
        internal static readonly String PRODUCT = "AceQL HTTP Client SDK";
        internal static readonly String VERSION = "v1.0.1-beta";
        internal static readonly String DATE = "19-may-2017";
    }
}
