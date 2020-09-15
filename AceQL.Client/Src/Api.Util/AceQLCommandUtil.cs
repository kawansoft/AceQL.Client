/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2020,  KawanSoft SAS
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


using AceQL.Client.Src.Api.Util;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Util
{
    /// <summary>
    /// Class AceQLCommandUtil.
    /// </summary>
    internal class AceQLCommandUtil
    {
        public static readonly string[] PARM_SEPARATORS = { "(", ")", ";", " ", "+", "-", "/", "*", "=", "\'", "\"", "?", "!", ":", "#", "&", "-", "<", ">", "{", "}", "[", "]", "|", "%", "," };

        internal static readonly bool DEBUG;

        /// <summary>
        /// The command text
        /// </summary>
        private string cmdText;
        /// <summary>
        /// The parameters
        /// </summary>
        private readonly AceQLParameterCollection Parameters;

        /// <summary>
        /// The BLOB ids
        /// </summary>
        private readonly List<String> blobIds = new List<string>();
        /// <summary>
        /// The BLOB file streams
        /// </summary>
        private readonly List<Stream> blobStreams = new List<Stream>();

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
            HashSet<String> theParamsSetInSqlCommand = GetValidParamsInSqlCommand();
            CheckAllParametersExistInSqlCommand(theParamsSetInSqlCommand, this.Parameters);

            Dictionary<string, string> parametersList = new Dictionary<string, string>();

            // For each parameter get the dbType
            int paramIndex = 0;
            foreach (var parameterName in theParamsSetInSqlCommand)
            {
                AceQLParameter aceQLParameter = this.Parameters.GetAceQLParameter(parameterName);
                paramIndex++;

                Debug("");
                Debug("parameterName       : " + parameterName);
                Debug("aceQLParameter.Value: " + aceQLParameter.Value + ":");
                Debug("paramIndex          : " + paramIndex);

                AceQLNullType aceQLNullType = aceQLParameter.SqlNullType;
                Object ParmValue = aceQLParameter.Value;

                //Reconvert SqlType original Java value by diving per 10000 and multiplying per -1:
                int sqlType = (int)aceQLNullType;
                sqlType = sqlType / (int)AceQLNullType.CHAR;

                // For OUT parameters that may be null value
                if (ParmValue == null)
                {
                    ParmValue = "NULL";
                }

                if (aceQLParameter.IsNullValue)
                {
                    String paramType = "TYPE_NULL" + sqlType;
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, "NULL");
                }
                else if (ParmValue is Stream)
                {
                    // All streams are blob for now
                    // This will be enhanced in future version

                    String blobId = BuildUniqueBlobId();

                    blobIds.Add(blobId);
                    blobStreams.Add((Stream)ParmValue);
                    blobLengths.Add(aceQLParameter.BlobLength);

                    String paramType = "BLOB";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, blobId);
                }
                else if (ParmValue is string || ParmValue is String)
                {
                    String paramType = "VARCHAR";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ParmValue.ToString());
                }
                else if (ParmValue is long)
                {
                    String paramType = "BIGINT";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ParmValue.ToString());
                }
                else if (ParmValue is int)
                {
                    String paramType = "INTEGER";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ParmValue.ToString());
                }
                else if (ParmValue is short)
                {
                    String paramType = "TINYINT";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ParmValue.ToString());
                }
                else if (ParmValue is bool || ParmValue is Boolean)
                {
                    String paramType = "BIT";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ParmValue.ToString());
                }
                else if (ParmValue is float)
                {
                    String paramType = "REAL";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ParmValue.ToString().Replace(",", "."));
                }
                else if (ParmValue is double || ParmValue is Double)
                {
                    String paramType = "DOUBLE_PRECISION";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ParmValue.ToString().Replace(",", "."));
                }
                else if (ParmValue is DateTime)
                {
                    String paramType = "TIMESTAMP";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ConvertToTimestamp((DateTime)ParmValue));
                }
                else if (ParmValue is TimeSpan)
                {
                    String paramType = "TIME";
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, ConvertToTimestamp((DateTime)ParmValue));
                }
                else
                {
                    throw new AceQLException("Type of value is not supported. Value: " + ParmValue + " / Type: " + ParmValue.GetType(), 2, (Exception)null, HttpStatusCode.OK);
                }

                if (!aceQLParameter.IsNullValue && !(ParmValue is Stream))
                {

                    if (aceQLParameter.Direction == Api.ParameterDirection.InputOutput)
                    {
                        parametersList.Add("param_direction_" + paramIndex, "inout");
                        parametersList.Add("out_param_name_" + paramIndex, aceQLParameter.ParameterName);
                    }
                    else if (aceQLParameter.Direction == Api.ParameterDirection.Output)
                    {
                        parametersList.Add("param_direction_" + paramIndex, "out");
                        parametersList.Add("out_param_name_" + paramIndex, aceQLParameter.ParameterName);
                    }
                    else
                    {
                        // Defaults to "in" on server
                    }
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

            //\AppData\Local\KawanSoft\AceQL.Client.Samples\3.0.0.0\AceQLPclFolder

            string pathTraceTxt = "Trace_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + "_" + Guid.NewGuid().ToString() + ".txt";
            IFile file = await folder.CreateFileAsync(pathTraceTxt, CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
            return file;
        }

        /// <summary>
        /// Checks all parameters exist in SQL command. Will throw an Exception if a parameter is missing.
        /// </summary>
        /// <param name="theParamsSetInSqlCommand">The parameters set in SQL command.</param>
        /// <param name="parameters">The parameters.</param>
        private static void CheckAllParametersExistInSqlCommand(HashSet<string> theParamsSetInSqlCommand, AceQLParameterCollection parameters)
        {
            HashSet<string> parameterNames = new HashSet<string>();

            for (int i = 0; i < parameters.Count; i++)
            {
                String theParm = parameters[i].ToString();
                parameterNames.Add(theParm);
            }

            List<string> theParamsListInSqlCommand = theParamsSetInSqlCommand.ToList();

            foreach (string theParamInSqlCommand in theParamsListInSqlCommand)
            {
                if (!parameterNames.Contains(theParamInSqlCommand))
                {
                    throw new ArgumentException("Missing parameter value for parameter name: " + theParamInSqlCommand);
                }
            }
        }

        /// <summary>
        /// Gets the valid parameters.
        /// </summary>
        /// <returns>HashSet&lt;System.String&gt;.</returns>
        private HashSet<string> GetValidParamsInSqlCommand()
        {
            HashSet<string> theParamsSet = new HashSet<string>();
            String[] splits = cmdText.Split(PARM_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

            foreach (string splitted in splits)
            {
                if (!splitted.StartsWith("@"))
                {
                    continue;
                }

                if (theParamsSet.Contains(splitted.Trim()))
                {
                    throw new ArgumentException("This parameter is duplicate in SQL command: " + splitted.Trim());
                }

                theParamsSet.Add(splitted.Trim());

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
            double theDouble = (TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc) - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds;
            String theTimeString = theDouble.ToString(CultureInfo.InvariantCulture);

            // Remove "." or ',' depending on Locale:
            theTimeString = StringUtils.SubstringBefore(theTimeString, ",");
            theTimeString = StringUtils.SubstringBefore(theTimeString, ".");
            return theTimeString;
        }

        private static void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }
    }


}
