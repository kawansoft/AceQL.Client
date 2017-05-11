// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-25-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="RowParser.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using AceQL.Client.Api.File;
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
    /// Class RowParser.
    /// We pass a TextReader instead of a PortableFile as we want the methods to be all sync for end user.
    /// </summary>
    internal class RowParser
    {
        private const string COL_INDEX = "idx";
        private const string COL_TYPE = "typ";
        private const string COL_NAME = "nam";
        private const string COL_VALUE = "val";

        /// <summary>
        /// The file containing the result set.
        /// </summary>
        private string fileName;

        /// <summary>
        /// The text reader
        /// </summary>
        TextReader textReader;
        /// <summary>
        /// The reader
        /// </summary>
        JsonTextReader reader;

        /// <summary>
        /// The trace on
        /// </summary>
        private bool traceOn = false;
        /// <summary>
        /// The values per col index
        /// </summary>
        private Dictionary<int, object> valuesPerColIndex;
        private Dictionary<int, string> typesPerColIndex;
        private Dictionary<int, string> colNamesPerColIndex;
        private Dictionary<string, int> colIndexesPerColName;
        private string folderName;


        /// <summary>
        /// Initializes a new instance of the <see cref="RowParser" /> class.
        /// </summary>
        /// <param name="folderName">The folder name.</param>
        /// <param name="fileName">The file name. Passe for file deletion.</param>
        /// <param name="textTreader">The textTreader to use, corresponds to filename.</param>
        /// <exception cref="System.ArgumentNullException">fileName is null!</exception>
        ///<exception cref="System.ArgumentNullException">textTreader is null!</exception>
        internal RowParser(string folderName, string fileName, TextReader textTreader)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName is null!");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null!");
            }

            if (textTreader == null)
            {
                throw new ArgumentNullException("textTreader is null!");
            }

            this.folderName = folderName;
            this.fileName = fileName;
            this.textReader = textTreader;
            this.reader = new JsonTextReader(textReader);
        }


        internal Dictionary<int, string> GetTypesPerColIndex()
        {
            return typesPerColIndex;
        }

        internal Dictionary<int, object> GetValuesPerColIndex()
        {
            return valuesPerColIndex;
        }


        internal Dictionary<string, int> GetColIndexesPerColName()
        {
            return colIndexesPerColName;
        }

        internal Dictionary<int, string> GetColNamesPerColIndex()
        {
            return colNamesPerColIndex;
        }


        /// <summary>
        /// Builds the row number.
        /// </summary>
        /// <param name="rowNum">The row number.</param>
        internal void BuildRowNum(int rowNum)
        {
            //NO: Never happens
            //if (reader == null)
            //{
            //    textReader = PortableFile.OpenText(fileName);
            //    reader = new JsonTextReader(textReader);
            //}

            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("row_" + rowNum))
                    {
                        int colIndex = 0;
                        String colName = null;
                        String colType = null;

                        valuesPerColIndex = new Dictionary<int, object>();
                        typesPerColIndex = new Dictionary<int, string>();
                        colNamesPerColIndex = new Dictionary<int, string>();
                        colIndexesPerColName = new Dictionary<string, int>();

                        while (reader.Read())
                        {
                            // We are done at end of row
                            if (reader.TokenType.Equals(JsonToken.EndArray))
                            {
                                return;
                            }

                            if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_INDEX))
                            {
                                if (reader.Read())
                                {
                                    String colIndexStr = reader.Value.ToString();
                                    colIndex = Int32.Parse(colIndexStr);

                                    Trace();
                                    Trace("" + colIndex);
                                }
                                else
                                {
                                    return;
                                }
                            }

                            if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_TYPE))
                            {
                                if (reader.Read())
                                {
                                    String colIndexStr = reader.Value.ToString();
                                    colType = reader.Value.ToString();
                                    Trace("" + colType);
                                }
                                else
                                {
                                    return;
                                }
                            }

                            if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_NAME))
                            {
                                if (reader.Read())
                                {
                                    String colIndexStr = reader.Value.ToString();
                                    colName = reader.Value.ToString();
                                    Trace("" + colName);
                                }
                                else
                                {
                                    return;
                                }
                            }

                            if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(COL_VALUE))
                            {
                                if (reader.Read())
                                {
                                    String colValue = reader.Value.ToString();

                                    if (colValue != null)
                                    {
                                        colValue = colValue.Trim();
                                    }

                                    // because it's start at 0 on C# insted of 1 in JDBC
                                    colIndex--;

                                    Trace("" + colValue);
                                    valuesPerColIndex.Add(colIndex, colValue);
                                    typesPerColIndex.Add(colIndex, colType);

                                    colNamesPerColIndex.Add(colIndex, colName);
                                    colIndexesPerColName.Add(colName, colIndex);

                                }
                                else
                                {
                                    return;
                                }
                            }
                        }

                    }
                }

            }
        }

        /**
         * Says if trace is on
         * 
         * @return true if trace is on
         */
        /// <summary>
        /// Determines whether [is trace on].
        /// </summary>
        /// <returns><c>true</c> if [is trace on]; otherwise, <c>false</c>.</returns>
        internal bool IsTraceOn()
        {
            return traceOn;
        }

        /**
         * Sets the trace on/off
         * 
         * @param traceOn
         *            if true, trace will be on
         */
        /// <summary>
        /// Sets the trace on.
        /// </summary>
        /// <param name="traceOn">if set to <c>true</c> [trace on].</param>
        internal void SetTraceOn(bool traceOn)
        {
            this.traceOn = traceOn;
        }

        /// <summary>
        /// Traces this instance.
        /// </summary>
        private void Trace()
        {
            if (traceOn)
            {
                ConsoleEmul.WriteLine();
            }
        }

        /// <summary>
        /// Traces the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        private void Trace(String s)
        {
            if (traceOn)
            {
                ConsoleEmul.WriteLine(s);
            }
        }

        internal void ResetParser()
        {
            if (reader != null)
            {
                reader.Close();
            }

            // Reinit parser:
            reader = null;
        }

        internal void Close()
        {
            if (reader != null)
            {
                reader.Close();
            }

            if (textReader != null)
            {
                textReader.Dispose();
            }

            // Reinit parser:
            reader = null;

            // Delete the file in fire and forget mode

            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                PortableFile.DeleteAsync(folderName, fileName);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception exception)
            {
                ConsoleEmul.WriteLine(exception.ToString());
            }

        }


    }
}
