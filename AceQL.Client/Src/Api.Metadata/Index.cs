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

using AceQL.Client.Api.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Metadata
{
    /// <summary>
    /// Class Index. A SQL Index with it's defining elements.
    /// Implements the <see cref="AceQL.Client.Api.Metadata.CatalogAndSchema" />
    /// </summary>
    /// <seealso cref="AceQL.Client.Api.Metadata.CatalogAndSchema" />
    public class Index : CatalogAndSchema
    {
        /// <summary>
        /// The table index statistic
        /// </summary>
        public static readonly string tableIndexStatistic = "tableIndexStatistic";
        /// <summary>
        /// The table index clustered
        /// </summary>
        public static readonly string tableIndexClustered = "tableIndexClustered";
        /// <summary>
        /// The table index hashed
        /// </summary>
        public static readonly string tableIndexHashed = "tableIndexHashed";
        /// <summary>
        /// The table index other
        /// </summary>
        public static readonly string tableIndexOther = "tableIndexOther";

        /// <summary>
        /// The index name
        /// </summary>
        private string indexName;
        /// <summary>
        /// The table name
        /// </summary>
        private string tableName;
        /// <summary>
        /// The non unique
        /// </summary>
        private bool nonUnique;
        /// <summary>
        /// The index qualifier
        /// </summary>
        private string indexQualifier;
        /// <summary>
        /// The type
        /// </summary>
        private string type ;
        /// <summary>
        /// The ordinal position
        /// </summary>
        private int ordinalPosition;
        /// <summary>
        /// The column name
        /// </summary>
        private string columnName;
        /// <summary>
        /// The ascending or descending
        /// </summary>
        private string ascendingOrDescending;
        /// <summary>
        /// The cardinality
        /// </summary>
        private long cardinality;
        /// <summary>
        /// The pages
        /// </summary>
        private long pages;
        /// <summary>
        /// The filter condition
        /// </summary>
        private string filterCondition;

        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        /// <value>The name of the index.</value>
        public string IndexName { get => indexName; set => indexName = value; }
        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get => tableName; set => tableName = value; }
        /// <summary>
        /// Gets or sets a value indicating whether [non unique].
        /// </summary>
        /// <value><c>true</c> if [non unique]; otherwise, <c>false</c>.</value>
        public bool NonUnique { get => nonUnique; set => nonUnique = value; }
        /// <summary>
        /// Gets or sets the index qualifier.
        /// </summary>
        /// <value>The index qualifier.</value>
        public string IndexQualifier { get => indexQualifier; set => indexQualifier = value; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get => type; set => type = value; }
        /// <summary>
        /// Gets or sets the ordinal position.
        /// </summary>
        /// <value>The ordinal position.</value>
        public int OrdinalPosition { get => ordinalPosition; set => ordinalPosition = value; }
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get => columnName; set => columnName = value; }
        /// <summary>
        /// Gets or sets the ascending or descending.
        /// </summary>
        /// <value>The ascending or descending.</value>
        public string AscendingOrDescending { get => ascendingOrDescending; set => ascendingOrDescending = value; }
        /// <summary>
        /// Gets or sets the cardinality.
        /// </summary>
        /// <value>The cardinality.</value>
        public long Cardinality { get => cardinality; set => cardinality = value; }
        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public long Pages { get => pages; set => pages = value; }
        /// <summary>
        /// Gets or sets the filter condition.
        /// </summary>
        /// <value>The filter condition.</value>
        public string FilterCondition { get => filterCondition; set => filterCondition = value; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Objet à comparer avec l'objet actif.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var index = obj as Index;
            return index != null &&
                   indexName == index.indexName &&
                   tableName == index.tableName;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            var hashCode = -699929044;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(indexName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(tableName);
            return hashCode;
        }

        /// <summary>
        /// Retourne une chaîne qui représente l'objet actif.
        /// </summary>
        /// <returns>Chaîne qui représente l'objet actif.</returns>
        public override String ToString()
        {
            return "Index [indexName=" + indexName + ", tableName=" + tableName + ", nonUnique=" + nonUnique
                + ", indexQualifier=" + indexQualifier + ", type=" + type + ", ordinalPosition=" + ordinalPosition
                + ", columnName=" + columnName + ", ascendingOrDescending=" + ascendingOrDescending
                + ", cardinality=" + cardinality + ", pages=" + pages + ", filterCondition=" + filterCondition
                + ", getCatalog()=" + Catalog + ", getSchema()=" + Schema + "]";
        }
    }
}
