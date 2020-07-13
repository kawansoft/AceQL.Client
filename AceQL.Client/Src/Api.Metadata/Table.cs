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
    /// Class Table. A SQL Table with it's defining elements.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// The table
        /// </summary>
        public static readonly string TABLE = "TABLE";
        /// <summary>
        /// The view
        /// </summary>
        public static readonly string VIEW = "VIEW";

        /// <summary>
        /// The table name
        /// </summary>
        private string tableName;
        /// <summary>
        /// The table type
        /// </summary>
        private string tableType;
        /// <summary>
        /// The remarks
        /// </summary>
        private string remarks;

        /// <summary>
        /// The columns
        /// </summary>
        private List<Column> columns;
        /// <summary>
        /// The primary keys
        /// </summary>
        private List<PrimaryKey> primaryKeys;
        /// <summary>
        /// The indexes
        /// </summary>
        private List<Index> indexes;

        /// <summary>
        /// The importedforeign keys
        /// </summary>
        private List<ImportedKey> importedforeignKeys;
        /// <summary>
        /// The exportedforeign keys
        /// </summary>
        private List<ExportedKey> exportedforeignKeys;

        /// <summary>
        /// The catalog
        /// </summary>
        private string catalog;
        /// <summary>
        /// The schema
        /// </summary>
        private string schema;

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get => tableName; set => tableName = value; }
        /// <summary>
        /// Gets or sets the type of the table.
        /// </summary>
        /// <value>The type of the table.</value>
        public string TableType { get => tableType; set => tableType = value; }
        /// <summary>
        /// Gets or sets the remarks.
        /// </summary>
        /// <value>The remarks.</value>
        public string Remarks { get => remarks; set => remarks = value; }
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<Column> Columns { get => columns; set => columns = value; }
        /// <summary>
        /// Gets or sets the primary keys.
        /// </summary>
        /// <value>The primary keys.</value>
        public List<PrimaryKey> PrimaryKeys { get => primaryKeys; set => primaryKeys = value; }
        /// <summary>
        /// Gets or sets the indexes.
        /// </summary>
        /// <value>The indexes.</value>
        public List<Index> Indexes { get => indexes; set => indexes = value; }
        /// <summary>
        /// Gets or sets the importedforeign keys.
        /// </summary>
        /// <value>The importedforeign keys.</value>
        public List<ImportedKey> ImportedforeignKeys { get => importedforeignKeys; set => importedforeignKeys = value; }
        /// <summary>
        /// Gets or sets the exportedforeign keys.
        /// </summary>
        /// <value>The exportedforeign keys.</value>
        public List<ExportedKey> ExportedforeignKeys { get => exportedforeignKeys; set => exportedforeignKeys = value; }
        /// <summary>
        /// Gets or sets the catalog.
        /// </summary>
        /// <value>The catalog.</value>
        public string Catalog { get => catalog; set => catalog = value; }
        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public string Schema { get => schema; set => schema = value; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Objet à comparer avec l'objet actif.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var table = obj as Table;
            return table != null &&
                   tableName == table.tableName &&
                   catalog == table.catalog &&
                   schema == table.schema;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            var hashCode = 431174090;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(tableName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(catalog);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(schema);
            return hashCode;
        }

        /// <summary>
        /// Retourne une chaîne qui représente l'objet actif.
        /// </summary>
        /// <returns>Chaîne qui représente l'objet actif.</returns>
        public override String ToString()
        {
            return "Table [tableName=" + tableName + ", tableType=" + tableType + ", remarks=" + remarks + ", columns="
                + string.Join(",", (object[])columns.ToArray()) + ", primaryKeys=" + string.Join(",", (object[])primaryKeys.ToArray())
                + ", indexes=" + string.Join(",", (object[])indexes.ToArray()) + ", importedforeignKeys="
                + string.Join(",", (object[])importedforeignKeys.ToArray())
                + ", exportedforeignKeys=" + string.Join(",", (object[])exportedforeignKeys.ToArray()) + ", catalog=" + catalog
                + ", schema=" + schema + "]";
        }
    }
}
