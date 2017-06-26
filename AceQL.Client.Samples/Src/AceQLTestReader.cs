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

using AceQL.Client.Api;
using AceQL.Client.Api.File;
using PCLStorage;
using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AceQL.Client.Samples
{
    /// <summary>
    /// Tests AceQL client SDK by calling all APIs.
    /// </summary>
    class AceQLTestReader
    {
        private const string ACEQL_PCL_FOLDER = "AceQLPclFolder";

        public static void TheMain(string[] args)
        {
            try
            {

                DoIt(args).Wait();

                Console.WriteLine();
                Console.WriteLine("Press enter to close....");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }

        }


        static async Task DoIt(string[] args)
        {
            try
            {
                Console.WriteLine(new DateTime() + " AceQLTestReader start...");
                //Stream readStream = File.OpenRead("C:\\Users\\Nicolas de Pomereu\\Desktop\\result-set.txt");
                Stream readStream = File.OpenRead("C:\\Users\\Nicolas de Pomereu\\Desktop\\original.txt"); 

                IFile file = await GetUniqueResultSetFileAsync().ConfigureAwait(false);
                AceQLDataReader dataReader = null; // new AceQLDataReader(file, readStream, 3, null);

                // Our dataReader must be disposed to delete underlying downloaded files
               
                    //await dataReader.ReadAsync(new CancellationTokenSource().Token)
                while (dataReader.Read())
                {
                    Console.WriteLine();
                    int i = 0;
                    Console.WriteLine("GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++) + "\n"
                        + "GetValue: " + dataReader.GetValue(i++));
                }

                readStream.Dispose();

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }

        }

        /// <summary>
        /// Generates a unique File on the system for the downloaded result set content.
        /// </summary>
        /// <returns>A unique File on the system.</returns>
        private static async Task<IFile> GetUniqueResultSetFileAsync()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("AceQLPclFolder",
                CreationCollisionOption.OpenIfExists).ConfigureAwait(false);

            String fileName = Guid.NewGuid().ToString() + "-result-set.txt";
            IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);

            return file;
        }

    }
}
