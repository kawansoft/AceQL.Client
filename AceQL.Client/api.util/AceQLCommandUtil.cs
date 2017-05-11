// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-22-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="AceQLCommandUtil.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            if (cmdText == null)
            {
                throw new ArgumentNullException("cmdText is null!");
            }

            if (Parameters == null)
            {
                throw new ArgumentNullException("Parameters is null!");
            }

            this.cmdText = cmdText;
            this.Parameters = Parameters;
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
                //DbType dbType = aceQLParameter.DbType;
                SqlType sqlType = aceQLParameter.SqlType;
                Object value = aceQLParameter.Value;

                Debug("paramIndex: " + paramIndex);
                Debug("value     : " + value + ":");

                if (aceQLParameter.IsNullValue)
                {
                    String paramType = "TYPE_NULL" + (int) sqlType;
                    parametersList.Add("param_type_" + paramIndex, paramType);
                    parametersList.Add("param_value_" + paramIndex, "NULL");
                }
                else if (sqlType == SqlType.BLOB)
                {
                    Stream stream = (Stream)value;

                    String blobId = AceQLCommandUtil.BuildBlobId();

                    blobIds.Add(blobId);
                    blobStreams.Add(stream);
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

        internal static string BuildBlobId()
        {
            String blobId = Guid.NewGuid().ToString() + ".blob";
            return blobId;
        }

        internal static string BuildResultSetFileName()
        {
            String fileName = Guid.NewGuid().ToString() + "-result-set.txt";
            return fileName;
        }

        /// <summary>
        /// Gets the prepared statement parameters dictionnary.
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
