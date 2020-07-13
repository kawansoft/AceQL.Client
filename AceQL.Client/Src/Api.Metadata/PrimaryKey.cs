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

namespace AceQL.Client.Api.Metadata
{

    /// <summary>
    /// Class PrimaryKey. A SQL Primary Key with it's defining elements.
    /// Implements the <see cref="AceQL.Client.Api.Metadata.CatalogAndSchema" />
    /// </summary>
    /// <seealso cref="AceQL.Client.Api.Metadata.CatalogAndSchema" />
    public class PrimaryKey : CatalogAndSchema
    {

        /// <summary>
        /// The table name
        /// </summary>
        private string tableName;
        /// <summary>
        /// The column name
        /// </summary>
        private string columnName;
        /// <summary>
        /// The key sequence
        /// </summary>
        private int keySequence;
        /// <summary>
        /// The primary key name
        /// </summary>
        private string primaryKeyName;

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public virtual string TableName
        {
            get
            {
                return tableName;
            }
            set
            {
                this.tableName = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public virtual string ColumnName
        {
            get
            {
                return columnName;
            }
            set
            {
                this.columnName = value;
            }
        }
        /// <summary>
        /// Gets or sets the key sequence.
        /// </summary>
        /// <value>The key sequence.</value>
        public virtual int KeySequence
        {
            get
            {
                return keySequence;
            }
            set
            {
                this.keySequence = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the primary key.
        /// </summary>
        /// <value>The name of the primary key.</value>
        public virtual string PrimaryKeyName
        {
            get
            {
                return primaryKeyName;
            }
            set
            {
                this.primaryKeyName = value;
            }
        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + ((string.ReferenceEquals(primaryKeyName, null)) ? 0 : primaryKeyName.GetHashCode());
            result = prime * result + ((string.ReferenceEquals(tableName, null)) ? 0 : tableName.GetHashCode());
            return result;
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Objet à comparer avec l'objet actif.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            PrimaryKey other = (PrimaryKey)obj;
            if (string.ReferenceEquals(primaryKeyName, null))
            {
                if (!string.ReferenceEquals(other.primaryKeyName, null))
                {
                    return false;
                }
            }
            else if (!primaryKeyName.Equals(other.primaryKeyName))
            {
                return false;
            }
            if (string.ReferenceEquals(tableName, null))
            {
                if (!string.ReferenceEquals(other.tableName, null))
                {
                    return false;
                }
            }
            else if (!tableName.Equals(other.tableName))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "PrimaryKey [tableName=" + tableName + ", columnName=" + columnName + ", keySequence=" + keySequence + ", primaryKeyName=" + primaryKeyName + ", getCatalog()=" + Catalog + ", getSchema()=" + Schema + "]";
        }

    }
}
