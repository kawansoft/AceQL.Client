
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
using System;

namespace AceQL.Client.Api.Util
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   The main supported SQL types. </summary>
    ///
    /// <remarks>   Nicolas De Pomereu, 19/05/2017. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    internal static class AceQLTypes
    {
        internal static readonly String BINARY = "BINARY";
        internal static readonly String BIT = "BIT";
        internal static readonly String BLOB = "BLOB";
        internal static readonly String CHAR = "CHAR";
        internal static readonly String CHARACTER = "CHARACTER";
        internal static readonly String CLOB = "CLOB";
        internal static readonly String DATE = "DATE";
        internal static readonly String DECIMAL = "DECIMAL";
        internal static readonly String DOUBLE_PRECISION = "DOUBLE PRECISION";
        internal static readonly String FLOAT = "FLOAT";
        internal static readonly String INTEGER = "INTEGER";
        internal static readonly String LONGVARBINARY = "LONGVARBINARY";
        internal static readonly String LONGVARCHAR = "LONGVARCHAR";
        internal static readonly String NUMERIC = "NUMERIC";
        internal static readonly String REAL = "REAL";
        internal static readonly String SMALLINT = "SMALLINT";
        internal static readonly String TIME = "TIME";
        internal static readonly String TIMESTAMP = "TIMESTAMP";
        internal static readonly String TINYINT = "TINYINT";
        internal static readonly String URL = "URL";
        internal static readonly String VARBINARY = "VARBINARY";
        internal static readonly String VARCHAR = "VARCHAR";

        internal static bool IsDateTimeType(string colType)
        {
            if (colType.Equals(AceQLTypes.DATE) || colType.Equals(AceQLTypes.TIME) || colType.Equals(AceQLTypes.TIMESTAMP))  {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool IsStringType(string colType)
        {
            if (colType.Equals(AceQLTypes.CHAR) || colType.Equals(AceQLTypes.CHARACTER) || colType.Equals(AceQLTypes.VARCHAR))  {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
