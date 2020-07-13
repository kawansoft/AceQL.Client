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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Metadata
{
    /// <summary>
    /// Class Column. A SQL Column with it's defining elements.
    /// Implements the <see cref="AceQL.Client.Api.Metadata.CatalogAndSchema" />
    /// </summary>
    /// <seealso cref="AceQL.Client.Api.Metadata.CatalogAndSchema" />
    public class Column : CatalogAndSchema
    {

        public static readonly string columnNoNulls = "columnNoNulls";
        public static readonly string columnNullable = "columnNullable";
        public static readonly string columnNullableUnknown = "columnNullableUnknown";

        /**
     * <pre>
     * <code>
    databaseMetaData.getColumns(customer_auto) 1: null                                1.TABLE_CAT String => table catalog (may be null)
    databaseMetaData.getColumns(customer_auto) 2: public                              2.TABLE_SCHEM String => table schema (may be null)
    databaseMetaData.getColumns(customer_auto) 3: customer_auto                       3.TABLE_NAME String => table name
    databaseMetaData.getColumns(customer_auto) 4: phone                               4.COLUMN_NAME String => column name

    databaseMetaData.getColumns(customer_auto) 5: 12                                  5.DATA_TYPE int => SQL type from java.sql.Types
    databaseMetaData.getColumns(customer_auto) 6: varchar                             6.TYPE_NAME String => Data source dependent type name,for a UDT the type name is fu
    databaseMetaData.getColumns(customer_auto) 7: 32                                  7.COLUMN_SIZE int => column size.
    databaseMetaData.getColumns(customer_auto) 8: null                                8.BUFFER_LENGTH is not used.
    databaseMetaData.getColumns(customer_auto) 9: 0                                   9.DECIMAL_DIGITS int => the number of fractional digits. Null is returned for data
    databaseMetaData.getColumns(customer_auto) 10: 10                                 10.NUM_PREC_RADIX int => Radix (typically either 10 or 2)
    databaseMetaData.getColumns(customer_auto) 11: 1                                  11.NULLABLE int => is NULL allowed. â—¦ columnNoNulls - might not allow NULL values
    databaseMetaData.getColumns(customer_auto) 12: null                               12.REMARKS String => comment describing column (may be null)
    databaseMetaData.getColumns(customer_auto) 13: null                               13.COLUMN_DEF String => default value for the column, which should be interpreted as a string when the value is enclosed in single quotes (may be null)
    databaseMetaData.getColumns(customer_auto) 14: null                               14.SQL_DATA_TYPE int => unused
    databaseMetaData.getColumns(customer_auto) 15: null                               15.SQL_DATETIME_SUB int => unused
    databaseMetaData.getColumns(customer_auto) 16: 32                                 16.CHAR_OCTET_LENGTH int => for char types themaximum number of bytes in the column
    databaseMetaData.getColumns(customer_auto) 17: 8                                  17.ORDINAL_POSITION int => index of column in table(starting at 1)
    databaseMetaData.getColumns(customer_auto) 18: YES                                18.IS_NULLABLE String => ISO rules are used to determine the nullability for a column. â—¦ YES --- if the column can include NULLs
    databaseMetaData.getColumns(customer_auto) 19: null                               19.SCOPE_CATALOG String => catalog of table that is the scopeof a reference attribute (null if DATA_TYPE isn't REF)
    databaseMetaData.getColumns(customer_auto) 20: null                               20.SCOPE_SCHEMA String => schema of table that is the scopeof a reference attribute (null if the DATA_TYPE isn't REF)
    databaseMetaData.getColumns(customer_auto) 21: null                               21.SCOPE_TABLE String => table name that this the scopeof a reference attribute (null if the DATA_TYPE isn't REF)
    databaseMetaData.getColumns(customer_auto) 22: null                               22.SOURCE_DATA_TYPE short => source type of a distinct type or user-generatedRef type, SQL type from java.sql.Types (null if
    databaseMetaData.getColumns(customer_auto) 23: NO                                 23.IS_AUTOINCREMENT String => Indicates whether this column is auto incremented â—¦ YES --- if the column is auto incremented
     </code>
     * </pre>
     *
     * !
     */

        /// <summary>
        /// The column name
        /// </summary>
        private String columnName;
        /// <summary>
        /// The table name
        /// </summary>
        private String tableName;
        /// <summary>
        /// The type name
        /// </summary>
        private String typeName;
        /// <summary>
        /// The size
        /// </summary>
        private int size;
        /// <summary>
        /// The decimal digits
        /// </summary>
        private int decimalDigits;
        /// <summary>
        /// The radix
        /// </summary>
        private int radix;
        /// <summary>
        /// The nullable
        /// </summary>
        private String nullable;
        /// <summary>
        /// The remarks
        /// </summary>
        private String remarks;
        /// <summary>
        /// The default value
        /// </summary>
        private String defaultValue;
        /// <summary>
        /// The character octet length
        /// </summary>
        private int charOctetLength;
        /// <summary>
        /// The ordinal position
        /// </summary>
        private int ordinalPosition;
        /// <summary>
        /// The is nullable
        /// </summary>
        private String isNullable;
        /// <summary>
        /// The scope catalog
        /// </summary>
        private String scopeCatalog;
        /// <summary>
        /// The scope schema
        /// </summary>
        private String scopeSchema;
        /// <summary>
        /// The scope table
        /// </summary>
        private String scopeTable;
        /// <summary>
        /// The source data type
        /// </summary>
        private short sourceDataType;
        /// <summary>
        /// The is autoincrement
        /// </summary>
        private String isAutoincrement;

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get => columnName; set => columnName = value; }
        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get => tableName; set => tableName = value; }
        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName { get => typeName; set => typeName = value; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get => size; set => size = value; }
        /// <summary>
        /// Gets or sets the decimal digits.
        /// </summary>
        /// <value>The decimal digits.</value>
        public int DecimalDigits { get => decimalDigits; set => decimalDigits = value; }
        /// <summary>
        /// Gets or sets the radix.
        /// </summary>
        /// <value>The radix.</value>
        public int Radix { get => radix; set => radix = value; }
        /// <summary>
        /// Gets or sets the nullable.
        /// </summary>
        /// <value>The nullable.</value>
        public string Nullable { get => nullable; set => nullable = value; }
        /// <summary>
        /// Gets or sets the remarks.
        /// </summary>
        /// <value>The remarks.</value>
        public string Remarks { get => remarks; set => remarks = value; }
        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue { get => defaultValue; set => defaultValue = value; }
        /// <summary>
        /// Gets or sets the length of the character octet.
        /// </summary>
        /// <value>The length of the character octet.</value>
        public int CharOctetLength { get => charOctetLength; set => charOctetLength = value; }
        /// <summary>
        /// Gets or sets the ordinal position.
        /// </summary>
        /// <value>The ordinal position.</value>
        public int OrdinalPosition { get => ordinalPosition; set => ordinalPosition = value; }
        /// <summary>
        /// Gets or sets the is nullable.
        /// </summary>
        /// <value>The is nullable.</value>
        public string IsNullable { get => isNullable; set => isNullable = value; }
        /// <summary>
        /// Gets or sets the scope catalog.
        /// </summary>
        /// <value>The scope catalog.</value>
        public string ScopeCatalog { get => scopeCatalog; set => scopeCatalog = value; }
        /// <summary>
        /// Gets or sets the scope schema.
        /// </summary>
        /// <value>The scope schema.</value>
        public string ScopeSchema { get => scopeSchema; set => scopeSchema = value; }
        /// <summary>
        /// Gets or sets the scope table.
        /// </summary>
        /// <value>The scope table.</value>
        public string ScopeTable { get => scopeTable; set => scopeTable = value; }
        /// <summary>
        /// Gets or sets the type of the source data.
        /// </summary>
        /// <value>The type of the source data.</value>
        public short SourceDataType { get => sourceDataType; set => sourceDataType = value; }
        /// <summary>
        /// Gets or sets the is autoincrement.
        /// </summary>
        /// <value>The is autoincrement.</value>
        public string IsAutoincrement { get => isAutoincrement; set => isAutoincrement = value; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Objet à comparer avec l'objet actif.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var column = obj as Column;
            return column != null &&
                   tableName == column.tableName &&
                   scopeCatalog == column.scopeCatalog &&
                   scopeSchema == column.scopeSchema;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            var hashCode = -8313490;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(tableName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(scopeCatalog);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(scopeSchema);
            return hashCode;
        }



        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "Column [columnName=" + columnName + ", tableName=" + tableName + ", typeName=" + typeName + ", size="
                + size + ", decimalDigits=" + decimalDigits + ", radix=" + radix + ", nullable=" + nullable
                + ", remarks=" + remarks + ", defaultValue=" + defaultValue + ", charOctetLength=" + charOctetLength
                + ", ordinalPosition=" + ordinalPosition + ", isNullable=" + isNullable + ", scopeCatalog="
                + scopeCatalog + ", scopeSchema=" + scopeSchema + ", scopeTable=" + scopeTable + ", sourceDataType="
                + sourceDataType + ", isAutoincrement=" + isAutoincrement + ", getCatalog()=" + ScopeCatalog
                + ", getSchema()=" + ScopeSchema + "]";
        }
    }
}
