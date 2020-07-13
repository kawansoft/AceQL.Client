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
﻿
namespace AceQL.Client.Api
{
    internal static class Factor
    {
        // We multiply original Jav enum value per -10000 to be sure the int value is never used as 
        // prepared statement parameter by user?
        internal const int FACTOR = -10000;
    }

    /// <summary>
    /// Specifies the SQL type to pass to the server when setting a value to NULL with <see cref="AceQLParameter"/>.
    /// </summary>
    public enum AceQLNullType
    {
        /// <summary>
        /// SQL type BIT.
        /// </summary>
        BIT = -7 * Factor.FACTOR,

        /// <summary>
        /// SQL type TINYINT.
        /// </summary>
        TINYINT = -6 * Factor.FACTOR,

        /// <summary>
        /// SQL type BIGINT.
        /// </summary>
        BIGINT = -5 * Factor.FACTOR,

        /// <summary>
        /// SQL type SMALLINT.
        /// </summary>
        SMALLINT = 5 * Factor.FACTOR,

        /// <summary>
        /// SQL type INTEGER.
        /// </summary>
        INTEGER = 4 * Factor.FACTOR,

        /// <summary>
        /// SQL type FLOAT.
        /// </summary>
        FLOAT = 6  * Factor.FACTOR,

        /// <summary>
        /// SQL type REAL.
        /// </summary>
        REAL = 7 * Factor.FACTOR,

        /// <summary>
        /// SQL type DOUBLE.
        /// </summary>
        DOUBLE = 8 * Factor.FACTOR,

        /// <summary>
        /// SQL type NUMERIC.
        /// </summary>
        NUMERIC = 2 * Factor.FACTOR,

        /// <summary>
        /// SQL type DECIMAL.
        /// </summary>
        DECIMAL = 3 * Factor.FACTOR,

        /// <summary>
        /// SQL type CHAR.
        /// </summary>
        CHAR = 1 * Factor.FACTOR,

        /// <summary>
        /// SQL type VARCHAR.
        /// </summary>
        VARCHAR = 12 * Factor.FACTOR,

        ///// <summary>
        ///// SQL type LONGVARCHAR.
        ///// </summary>
        //LONGVARCHAR = -1,

        /// <summary>
        /// SQL type DATE.
        /// </summary>
        DATE = 91 * Factor.FACTOR,

        /// <summary>
        /// SQL type TIME.
        /// </summary>
        TIME = 92 * Factor.FACTOR,

        /// <summary>
        /// SQL type TIMESTAMP.
        /// </summary>
        TIMESTAMP = 93 * Factor.FACTOR,

        ///// <summary>
        ///// SQL type BINARY.
        ///// </summary>
        //BINARY = -2,

        ///// <summary>
        ///// SQL type VARBINARY.
        ///// </summary>
        //VARBINARY = -3,

        ///// <summary>
        ///// SQL type LONGVARBINARY.
        ///// </summary>
        //LONGVARBINARY = -4,

        ///// <summary>
        ///// The null
        ///// </summary>
        //NULL = 0,

        ///// <summary>
        ///// The other
        ///// </summary>
        //OTHER = 1111,

        ///// <summary>
        ///// The java object
        ///// </summary>
        //JAVA_OBJECT = 2000,

        ///// <summary>
        ///// The distinct
        ///// </summary>
        //DISTINCT = 2001,

        ///// <summary>
        ///// The structure
        ///// </summary>
        //STRUCT = 2002,

        ///// <summary>
        ///// The array
        ///// </summary>
        //ARRAY = 2003,

        /// <summary>
        /// SQL type BLOB.
        /// </summary>
        BLOB = 2004 * Factor.FACTOR,

        /// <summary>
        /// SQL type CLOB.
        /// </summary>
        CLOB = 2005 * Factor.FACTOR,

        ///// <summary>
        ///// The reference
        ///// </summary>
        //REF = 2006,

        ///// <summary>
        ///// The datalink
        ///// </summary>
        //DATALINK = 70,

        /// <summary>
        /// SQL type BOOLEAN.
        /// </summary>
        BOOLEAN = 16 * Factor.FACTOR,

        //------------------------- JDBC 4.0 -----------------------------------

        ///// <summary>
        ///// The rowid
        ///// </summary>
        //ROWID = -8,

        ///// <summary>
        ///// The nchar
        ///// </summary>
        //NCHAR = -15,

        ///// <summary>
        ///// The nvarchar
        ///// </summary>
        //NVARCHAR = -9,

        ///// <summary>
        ///// SQL type LONGNVARCHAR
        ///// </summary>
        //LONGNVARCHAR = -16,

        ///// <summary>
        ///// The nclob
        ///// </summary>
        //NCLOB = 2011,


        ///// <summary>
        ///// The SQLXML
        ///// </summary>
        //SQLXML = 2009

    }

}
