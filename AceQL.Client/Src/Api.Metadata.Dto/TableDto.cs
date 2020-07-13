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


namespace AceQL.Client.Api.Metadata.Dto
{

    /// <summary>
    /// Contains the list of tables of the database.
    /// @author Nicolas de Pomereu
    /// </summary>
    internal class TableDto
    {

        /// <summary>
        /// The status
        /// </summary>
        private readonly string status = "OK";
        /// <summary>
        /// The table
        /// </summary>
        private readonly Table table;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDto"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public TableDto(Table table)
        {
            this.table = table;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        public virtual string Status
        {
            get
            {
                return status;
            }
        }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <value>The table.</value>
        public virtual Table Table
        {
            get
            {
                return table;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "TableDto [status=" + status + ", table=" + table + "]";
        }

    }
}
