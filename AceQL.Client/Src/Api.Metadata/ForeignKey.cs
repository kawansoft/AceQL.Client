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
    /// Class ForeignKey. A SQL Foreign Key.
    /// Implements the <see cref="AceQL.Client.Api.Metadata.CatalogAndSchema" />
    /// </summary>
    /// <seealso cref="AceQL.Client.Api.Metadata.CatalogAndSchema" />
    public class ForeignKey : CatalogAndSchema
    {
        /// <summary>
        /// The imported key cascade
        /// </summary>
        public static readonly string importedKeyCascade = "importedKeyCascade";
        /// <summary>
        /// The imported key restrict
        /// </summary>
        public static readonly string importedKeyRestrict = "importedKeyRestrict";
        /// <summary>
        /// The imported key set null
        /// </summary>
        public static readonly string importedKeySetNull = "importedKeySetNull";
        /// <summary>
        /// The imported key no action
        /// </summary>
        public static readonly string importedKeyNoAction = "importedKeyNoAction";
        /// <summary>
        /// The imported key set default
        /// </summary>
        public static readonly string importedKeySetDefault = "importedKeySetDefault";
        /// <summary>
        /// The imported key initially deferred
        /// </summary>
        public static readonly string importedKeyInitiallyDeferred = "importedKeyInitiallyDeferred";
        /// <summary>
        /// The imported key initially immediate
        /// </summary>
        public static readonly string importedKeyInitiallyImmediate = "importedKeyInitiallyImmediate";
        /// <summary>
        /// The imported key not deferrable
        /// </summary>
        public static readonly string importedKeyNotDeferrable = "importedKeyNotDeferrable";

        /// <summary>
        /// The primary key table
        /// </summary>
        private String primaryKeyTable;
        /// <summary>
        /// The primary key column
        /// </summary>
        private String primaryKeyColumn;
        /// <summary>
        /// The foreign key catalog
        /// </summary>
        private String foreignKeyCatalog;
        /// <summary>
        /// The foreign key schema
        /// </summary>
        private String foreignKeySchema;
        /// <summary>
        /// The foreign key table
        /// </summary>
        private String foreignKeyTable;
        /// <summary>
        /// The foreign key column
        /// </summary>
        private String foreignKeyColumn;
        /// <summary>
        /// The key sequence
        /// </summary>
        private int keySequence;
        /// <summary>
        /// The update rule
        /// </summary>
        private String updateRule;
        /// <summary>
        /// The delete rule
        /// </summary>
        private String deleteRule;
        /// <summary>
        /// The foreign key name
        /// </summary>
        private String foreignKeyName;
        /// <summary>
        /// The primary key name
        /// </summary>
        private String primaryKeyName;
        /// <summary>
        /// The deferrability
        /// </summary>
        private int deferrability;

        /// <summary>
        /// Gets or sets the primary key table.
        /// </summary>
        /// <value>The primary key table.</value>
        public string PrimaryKeyTable { get => primaryKeyTable; set => primaryKeyTable = value; }
        /// <summary>
        /// Gets or sets the primary key column.
        /// </summary>
        /// <value>The primary key column.</value>
        public string PrimaryKeyColumn { get => primaryKeyColumn; set => primaryKeyColumn = value; }
        /// <summary>
        /// Gets or sets the foreign key catalog.
        /// </summary>
        /// <value>The foreign key catalog.</value>
        public string ForeignKeyCatalog { get => foreignKeyCatalog; set => foreignKeyCatalog = value; }
        /// <summary>
        /// Gets or sets the foreign key schema.
        /// </summary>
        /// <value>The foreign key schema.</value>
        public string ForeignKeySchema { get => foreignKeySchema; set => foreignKeySchema = value; }
        /// <summary>
        /// Gets or sets the foreign key table.
        /// </summary>
        /// <value>The foreign key table.</value>
        public string ForeignKeyTable { get => foreignKeyTable; set => foreignKeyTable = value; }
        /// <summary>
        /// Gets or sets the foreign key column.
        /// </summary>
        /// <value>The foreign key column.</value>
        public string ForeignKeyColumn { get => foreignKeyColumn; set => foreignKeyColumn = value; }
        /// <summary>
        /// Gets or sets the key sequence.
        /// </summary>
        /// <value>The key sequence.</value>
        public int KeySequence { get => keySequence; set => keySequence = value; }
        /// <summary>
        /// Gets or sets the update rule.
        /// </summary>
        /// <value>The update rule.</value>
        public string UpdateRule { get => updateRule; set => updateRule = value; }
        /// <summary>
        /// Gets or sets the delete rule.
        /// </summary>
        /// <value>The delete rule.</value>
        public string DeleteRule { get => deleteRule; set => deleteRule = value; }
        /// <summary>
        /// Gets or sets the name of the foreign key.
        /// </summary>
        /// <value>The name of the foreign key.</value>
        public string ForeignKeyName { get => foreignKeyName; set => foreignKeyName = value; }
        /// <summary>
        /// Gets or sets the name of the primary key.
        /// </summary>
        /// <value>The name of the primary key.</value>
        public string PrimaryKeyName { get => primaryKeyName; set => primaryKeyName = value; }
        /// <summary>
        /// Gets or sets the deferrability.
        /// </summary>
        /// <value>The deferrability.</value>
        public int Deferrability { get => deferrability; set => deferrability = value; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Objet à comparer avec l'objet actif.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var key = obj as ForeignKey;
            return key != null &&
                   foreignKeyTable == key.foreignKeyTable &&
                   foreignKeyName == key.foreignKeyName;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            var hashCode = 2080242271;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(foreignKeyTable);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(foreignKeyName);
            return hashCode;
        }


        /// <summary>
        /// Retourne une chaîne qui représente l'objet actif.
        /// </summary>
        /// <returns>Chaîne qui représente l'objet actif.</returns>
        public override String ToString()
        {
            return "ForeignKey [primaryKeyTable=" + primaryKeyTable + ", primaryKeyColumn=" + primaryKeyColumn
                + ", foreignKeyCatalog=" + foreignKeyCatalog + ", foreignKeySchema=" + foreignKeySchema
                + ", foreignKeyTable=" + foreignKeyTable + ", foreignKeyColumn=" + foreignKeyColumn + ", keySequence="
                + keySequence + ", updateRule=" + updateRule + ", deleteRule=" + deleteRule + ", foreignKeyName="
                + foreignKeyName + ", primaryKeyName=" + primaryKeyName + ", deferrability=" + deferrability
                + ", getCatalog()=" + Catalog + ", getSchema()=" + Schema + "]";
        }
    }
}
