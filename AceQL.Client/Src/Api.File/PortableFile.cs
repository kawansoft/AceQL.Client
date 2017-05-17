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
﻿using System;

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
    /// Class a <see cref="PortableFile"/>
    /// <para/>
    /// Equivalent in our Portable Class Library of Windows System.IO.File for some important methods not included
    /// in PCLStorage, such as AppendAllTextAsync() or GetLengthAsync().
    /// <para/>
    /// Methods are all async, as storage methods are async on some OS.
    /// Implementation uses PCLStorage (https://github.com/dsplaisted/PCLStorage).
    /// </summary>
    public class PortableFile
    {

        /// <summary>
        /// Opens a <see cref="IFile"/>, appends the specified string to the file, and then closes the file.
        /// If the file does not exist, this method creates a file, writes the specified string to the file, then closes the file.
        /// </summary>
        /// <param name="file">The <see cref="IFile"/> to append the specified string to.</param>
        /// <param name="contents">The string to append to the <see cref="IFile"/>.</param>
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
        /// Returns the <see cref="IFile" /> length.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The <see cref="IFile" /> length.</returns>
        /// <exception cref="ArgumentNullException">file is null!</exception>
        public async Task<long> GetLengthAsync(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file is null!");
            }

            long theLength = 0;

            using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read).ConfigureAwait(false))
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
