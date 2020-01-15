﻿/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2018,  KawanSoft SAS
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
    /// Class JdbcDatabaseMetaDataDto.
    /// </summary>
    internal class JdbcDatabaseMetaDataDto
    {

        /// <summary>
        /// The JDBC database meta data
        /// </summary>
        private JdbcDatabaseMetaData jdbcDatabaseMetaData = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="JdbcDatabaseMetaDataDto"/> class.
        /// </summary>
        /// <param name="jdbcDatabaseMetaData">The JDBC database meta data.</param>
        public JdbcDatabaseMetaDataDto(JdbcDatabaseMetaData jdbcDatabaseMetaData)
        {
            this.jdbcDatabaseMetaData = jdbcDatabaseMetaData;
        }

        /// <summary>
        /// Gets the JDBC database meta data.
        /// </summary>
        /// <value>The JDBC database meta data.</value>
        public JdbcDatabaseMetaData JdbcDatabaseMetaData { get => jdbcDatabaseMetaData; }
    }
}