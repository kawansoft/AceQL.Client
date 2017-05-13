// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-22-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="JavaSqlTypes.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace AceQL.Client.Api
{

    /// <summary>
    /// Enum of SQL types. To use when setting a value to NULL with <see cref="AceQLCommand"/>.Parameter.AddNullValue.
    /// </summary>
    public enum SqlType
    {
        /// <summary>
        /// SQL type BIT.
        /// </summary>
        BIT = -7,

        /// <summary>
        /// SQL type TINYINT.
        /// </summary>
        TINYINT = -6,

        /// <summary>
        /// SQL type SMALLINT.
        /// </summary>
        SMALLINT = 5,

        /// <summary>
        /// SQL type INTEGER.
        /// </summary>
        INTEGER = 4,

        /// <summary>
        /// SQL type BIGINT.
        /// </summary>
        BIGINT = -5,

        /// <summary>
        /// SQL type FLOAT.
        /// </summary>
        FLOAT = 6,

        /// <summary>
        /// SQL type REAL.
        /// </summary>
        REAL = 7,

        /// <summary>
        /// SQL type DOUBLE.
        /// </summary>
        DOUBLE = 8,

        /// <summary>
        /// SQL type NUMERIC.
        /// </summary>
        NUMERIC = 2,

        /// <summary>
        /// SQL type DECIMAL.
        /// </summary>
        DECIMAL = 3,

        /// <summary>
        /// SQL type CHAR.
        /// </summary>
        CHAR = 1,

        /// <summary>
        /// SQL type VARCHAR.
        /// </summary>
        VARCHAR = 12,

        ///// <summary>
        ///// SQL type LONGVARCHAR.
        ///// </summary>
        //LONGVARCHAR = -1,

        /// <summary>
        /// SQL type DATE.
        /// </summary>
        DATE = 91,

        /// <summary>
        /// SQL type TIME.
        /// </summary>
        TIME = 92,

        /// <summary>
        /// SQL type TIMESTAMP.
        /// </summary>
        TIMESTAMP = 93,

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
        BLOB = 2004,

        /// <summary>
        /// SQL type CLOB.
        /// </summary>
        CLOB = 2005,

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
        BOOLEAN = 16,

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
