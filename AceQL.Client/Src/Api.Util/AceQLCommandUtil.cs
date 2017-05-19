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
﻿

using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Util
{
    /// <summary>
    /// Class AceQLCommandUtil.
    /// </summary>
    internal class AceQLCommandUtil
    {
        internal static bool DEBUG = false;

        /// <summary>
        /// The command text
        /// </summary>
        private string cmdText;
        /// <summary>
        /// The parameters
        /// </summary>
        private AceQLParameterCollection Parameters;

        /// <summary>
        /// The BLOB ids
        /// </summary>
        private List<String> blobIds = new List<string>();
        /// <summary>
        /// The BLOB file streams
        /// </summary>
        private List<Stream> blobStreams = new List<Stream>();

        private List<long> blobLengths = new List<long>();

        /// <summary>
        /// Gets the BLOB ids.
        /// </summary>
        /// <value>The BLOB ids.</value>
        internal List<string> BlobIds
        {
            get
            {
                return blobIds;
            }
        }

        /// <summary>
        /// Gets the BLOB streams.
        /// </summary>
        /// <value>The BLOB streams.</value>
        internal List<Stream> BlobStreams
        {
            get
            {
                return blobStreams;
            }
        }


        internal List<long> BlobLengths
        {
            get
            {
                return blobLengths;
            }

            set
            {
                blobLengths = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLCommandUtil"/> class.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="Parameters">The parameters.</param>
        /// <exception cref="System.ArgumentNullException">
        /// cmdText is null!
        /// or
        /// Parameters is null!
        /// </exception>
        internal AceQLCommandUtil(String cmdText, AceQLParameterCollection Parameters)
        {
            this.cmdText = cmdText ?? throw new ArgumentNullException("cmdText is null!");
            this.Parameters = Parameters ?? throw new ArgumentNullException("Parameters is null!");
        }


        /// <summary>
        /// Gets the prepared statement parameters.
        /// </summary>
        /// <returns>The Parameters List</returns>
        internal Dictionary<string, string> GetPreparedStatementParameters()
        {
            Dictionary<String, int> paramsIndexInPrepStatement = GetPreparedStatementParametersDic();

            //List<PrepStatementParameter> parametersList = new List<PrepStatementParameter>();

            Dictionary<string, string> parametersList = new Dictionary<string, string>();

            // For each parameter 1) get the index 2) get the dbType
            foreach (KeyValuePair<String, int> parameter in paramsIndexInPrepStatement)
            {
                AceQLParameter aceQLParameter = Parameters.GetAceQLParameter(parameter.Key);
                int paramIndex = parameter.Value;
                AceQLNullType sqlType = aceQLParameter.SqlType;
                Object value = aceQLParameter.Value;

                Debug("paramIndex: " + paramIndex);
                Debug("value     : " + value + ":");

                if (aceQLParameter.IsNullValue)
                {
                    String paramType = "TYPE_NULL" + (int) sqlType;
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, "NULL");
                }
                else if (value is Stream)
                {
                    // All streams are blob for now
                    // This will be enhanced in future version

                    String blobId = BuildUniqueBlobId();

                    blobIds.Add(blobId);
                    blobStreams.Add((Stream)value);
                    blobLengths.Add(aceQLParameter.BlobLength);

                    String paramType = "BLOB";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, blobId);
                }
                else if (value is string || value is String)
                {
                    String paramType = "VARCHAR";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, value.ToString());
                }
                else if (value is long)
                {
                    String paramType = "BIGINT";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, value.ToString());
                }
                else if (value is int)
                {
                    String paramType = "INTEGER";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, value.ToString());
                }
                else if (value is short)
                {

                    String paramType = "TINYINT";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, value.ToString());
                }
                else if (value is bool || value is Boolean)
                {
                    String paramType = "BIT";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, value.ToString());
                }
                else if (value is float)
                {
                    //parametersList.Add(new PrepStatementParameter(paramIndex, "REAL", value.ToString()));

                    String paramType = "REAL";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, value.ToString());
                }
                else if (value is double || value is Double)
                {
                    String paramType = "DOUBLE_PRECISION";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, value.ToString());
                }
                else if (value is DateTime)
                {
                    String paramType = "TIMESTAMP";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ConvertToTimestamp((DateTime)value));
                }
                else
                {
                    String paramType = "VARCHAR";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, value.ToString());
                }

            }

            return parametersList;
        }

        /// <summary>
        /// Builds a unique Blob ID.
        /// </summary>
        /// <returns>a unique Blob ID.</returns>
        internal static string BuildUniqueBlobId()
        {
            String blobId = Guid.NewGuid().ToString() + ".blob";
            return blobId;
        }

        /// <summary>
        /// Returns the file corresponding to the trace file. Value is: AceQLPclFolder/trace.txt.
        /// </summary>
        /// <returns>the file corresponding to the trace file.</returns>
        internal static async Task<IFile> GetTraceFileAsync()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(Parms.ACEQL_PCL_FOLDER,
                CreationCollisionOption.OpenIfExists).ConfigureAwait(false);

            IFile file = await folder.CreateFileAsync(Parms.TRACE_TXT, CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
            return file;
        }

        /// <summary>
        /// Gets the prepared statement parameters dictionary.
        /// </summary>
        /// <returns>Dictionary&lt;String, System.Int32&gt;.</returns>
        /// <exception cref="System.ArgumentException">Invalid parameter not exists in SQL command: " + theParm</exception>
        internal Dictionary<String, int> GetPreparedStatementParametersDic()
        {
            HashSet<String> theParamsSet = GetValidParams(); 

            SortedDictionary<int, String> paramsIndexOf = new SortedDictionary<int, String>();

            for (int i = 0; i < Parameters.Count; i++)
            {
                if (DEBUG)
                {
                    //ConsoleEmul.WriteLine(Parameters[i] + " / " + Parameters[i].Value);
                }

                String theParm = Parameters[i].ToString();

                if (! theParamsSet.Contains(theParm)) {
                    throw new ArgumentException("Invalid parameter that not exists in SQL command: " + theParm); 
                }

                int index = cmdText.IndexOf(theParm);

                paramsIndexOf.Add(index, theParm);
            }

            // Build the parameters
            Dictionary<String, int> paramsIndexInPrepStatement = new Dictionary<String, int>();

            int parameterIndex = 0;
            foreach (KeyValuePair<int, String> p in paramsIndexOf)
            {
                parameterIndex++;
                paramsIndexInPrepStatement.Add(p.Value, parameterIndex);
            }
            return paramsIndexInPrepStatement;
        }

        /// <summary>
        /// Gets the valid parameters.
        /// </summary>
        /// <returns>HashSet&lt;System.String&gt;.</returns>
        private HashSet<string> GetValidParams()
        {
            HashSet<string> theParamsSet = new HashSet<string>();
            char[] separators = { '@', ',', '(', ')', ' ', '=' };
            String[] splits = cmdText.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splits.Count(); i++)
            {
                String validParam = "@" + splits[i].Trim();

                if (cmdText.Contains(validParam))
                {
                    theParamsSet.Add(validParam);
                    //ConsoleEmul.WriteLine(validParam);
                }
            }

            return theParamsSet;
        }

        /// <summary>
        /// Replaces the parms with question marks.
        /// </summary>
        /// <returns>System.String.</returns>
        internal string ReplaceParmsWithQuestionMarks()
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                //ConsoleEmul.WriteLine(Parameters[i] + " / " + Parameters[i].Value + " / " + Parameters[i].DbType);

                String theParm = Parameters[i].ToString();
                cmdText = cmdText.Replace(theParm, "?");
            }

            return cmdText;
        }


        /// <summary>
        /// Dates the time to unix timestamp.
        /// </summary>
        /// <param name="dateTime">The UNIX date time in milliseconds</param>
        /// <returns>String.</returns>
        internal static String ConvertToTimestamp(DateTime dateTime)
        {
            double theDouble = (TimeZoneInfo.ConvertTime (dateTime, TimeZoneInfo.Utc) - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;

            theDouble = theDouble * 1000;
            String theTimeString = theDouble.ToString();
            int commaIndex = theTimeString.IndexOf(",");

            if (commaIndex <= 0)
            {
                return theTimeString;
            }

            return theTimeString.Substring(0, commaIndex);
        }

        private void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }
    }


}
