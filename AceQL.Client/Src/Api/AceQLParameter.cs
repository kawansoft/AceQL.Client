/*
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


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Represents a parameter to an <see cref="AceQLCommand"/>.
    /// </summary>
    public class AceQLParameter
    {
        /// <summary>
        /// The parameter name
        /// </summary>
        private string parameterName;
        /// <summary>
        /// The value
        /// </summary>
        private object value = null;

        /// <summary>
        /// The database type
        /// </summary>
        private AceQLNullType sqlType;

        private bool isNullValue = false;

        /// <summary>
        /// The length of the BLOB to upload
        /// </summary>
        private long blobLength = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLParameter"/> class to pass a NULL value to the remote
        /// database.
        /// </summary>
        /// <param name="parameterName">Name of the parameter to set with a NULL value.</param>
        /// <param name="value">The <see cref="AceQLNullType"/> value.</param>
        /// <exception cref="System.ArgumentNullException">If parameterName is null.</exception>
        public AceQLParameter(string parameterName, AceQLNullType value)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName is null!");
            }

            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }

            this.parameterName = parameterName;

            IsNullValue = true;
            SqlType = sqlType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLParameter"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">If parameterName or value is null.</exception>
        public AceQLParameter(string parameterName, object value)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName is null!");
            }

            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }

            this.parameterName = parameterName;
            this.value = value;

            if (value == null)
            {
                throw new ArgumentNullException("Parameter value cannot be null!");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLParameter"/> class.
        /// To be used for Blobs.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The Blob stream. Cannot be null.</param>
        /// <param name="length">The Blob stream length.</param>
        /// <exception cref="System.ArgumentNullException">If parameterName or value is null.</exception>
        public AceQLParameter(string parameterName, Stream value, long length) : this(parameterName, value)
        {
            this.blobLength = length;
        }


        /// <summary>
        /// Gets name of the <see cref="AceQLParameter"/> name.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string ParameterName
        {
            get
            {
                return parameterName;
            }
        }


        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        /// <value>The value of the parameter.</value>
        public object Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Says it the <see cref="AceQLParameter"/> has been asked to be null with <see cref="AceQLParameterCollection"/>.AddNull.
        /// </summary>
        internal bool IsNullValue
        {
            get
            {
                return isNullValue;
            }

            set
            {
                isNullValue = value;
            }
        }

        /// <summary>
        /// The SQL type
        /// </summary>
        internal AceQLNullType SqlType
        {
            get
            {
                return sqlType;
            }

            set
            {
                sqlType = value;
            }
        }

        /// <summary>
        /// The BLOB length to upload
        /// </summary>
        internal long BlobLength
        {
            get
            {
                return blobLength;
            }

        }

        /// <summary>
        /// Gets a string that contains <see cref="AceQLParameter"/>.ParameterName
        /// </summary>
        /// <returns>A string that contains <see cref="AceQLParameter"/></returns>
        public override string ToString()
        {
            //NO! Must return parameterName!
            //return "name: " + parameterName + " / " + "DbType: " + dbType + " / " + "value: " + value;
            return parameterName;
        }
    }
}
