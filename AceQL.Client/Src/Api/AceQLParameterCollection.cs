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

using AceQL.Client.Api.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Represents a collection of parameters associated with an <see cref="AceQLCommand"/>
    /// and their respective mappings to columns.
    /// </summary>
    public class AceQLParameterCollection : IList, ICollection, IEnumerable
    {
        /// <summary>
        /// The AceQL Parameters
        /// </summary>
        private readonly List<AceQLParameter> aceqlParameters = new List<AceQLParameter>();
        private readonly string cmdText;

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLParameterCollection"/> class.
        /// </summary>
        /// <param name="cmdText">The SQL command text.</param>
        /// <exception cref="System.ArgumentNullException">If cmdText is null.</exception>
        internal AceQLParameterCollection(string cmdText)
        {
            this.cmdText = cmdText ?? throw new ArgumentNullException("cmdText is null!");
        }

        /// <summary>
        /// Specifies the number of items in the collection.
        /// </summary>
        /// <value>The number of items in the collection.</value>
        public int Count
        {
            get
            {
                return aceqlParameters.Count;
            }
        }

        /// <summary>
        /// Specifies the <see cref="T:System.Object" /> to be used to synchronize access to the collection. Not implemented.
        /// </summary>
        /// <value>The synchronize root.</value>
        /// <exception cref="System.NotSupportedException"></exception>
        public object SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Specifies whether the collection is a fixed size. Not implemented.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
        /// <exception cref="System.NotSupportedException"></exception>
        public bool IsFixedSize
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Specifies whether the collection is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Specifies whether the collection is synchronized. Not implemented.
        /// </summary>
        /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
        /// <exception cref="System.NotSupportedException"></exception>
        public bool IsSynchronized
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public string CmdText => cmdText;

        /// <summary>
        /// Returns parameter at specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>the <see cref="AceQLParameter"/> at index.</returns>
        public object this[int index]
        {
            get
            {
                return aceqlParameters[index];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Removes all <see cref="AceQLParameter"/> objects from the <see cref="AceQLParameterCollection"/>
        /// </summary>
        public void Clear()
        {
            aceqlParameters.Clear();
        }

        /// <summary>
        /// Determines whether the specified parameter name is in this <see cref="AceQLParameterCollection"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>true if the <see cref="AceQLParameterCollection"/> contains the value; otherwise false.</returns>
        public bool Contains(string value)
        {
            foreach(AceQLParameter aceQLParameter in aceqlParameters)
            {
                if (aceQLParameter.ParameterName.Equals(value)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified System.Object is in this <see cref="AceQLParameterCollection"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>true if the <see cref="AceQLParameterCollection"/> contains the value; otherwise false.</returns>
        public bool Contains(object value)
        {
            foreach (AceQLParameter aceQLParameter in aceqlParameters)
            {
                if (aceQLParameter.Value.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// Not implemented.
        /// </summary>
        /// <param name="array">The one-dimensional<see cref="Array"/>that is the destination of the elements copied from <see cref="ICollection"/>. 
        ///                     The <see cref="Array"/>must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Exposes the <see cref="IEnumerable"/>.GetEnumerator" method, which supports a simple iteration over a collection by a .NET Framework data provider.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return aceqlParameters.GetEnumerator();
        }

        /// <summary>
        ///  Gets the location of the specified <see cref="AceQLParameter"/> with the specified name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns> The case-sensitive name of the <see cref="AceQLParameter"/> to find.</returns>
        /// <exception cref="System.ArgumentNullException">If parameterName is null.</exception>
        public int IndexOf(string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName is null!");
            }

            for (int i = 0; i < aceqlParameters.Count; i++)
            {
                AceQLParameter aceQLParameter = aceqlParameters[i];
                if (aceQLParameter.ParameterName.Equals(parameterName))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        ///  Adds a value to the end of the <see cref="AceQLParameterCollection"/>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value to be added. Cannot ne bull.</param>
        /// <exception cref="System.ArgumentNullException">If parameterName or value is null.</exception>
        public void AddWithValue(string parameterName, object value)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName is null!");
            }

            AceQLParameter aceQLParameter = new AceQLParameter(parameterName, value);
            aceqlParameters.Add(aceQLParameter);

        }

        /// <summary>
        /// Adds a value to the end of the <see cref="AceQLParameterCollection"/>.
        /// To be used for Blobs insert or update.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="stream">The Blob stream to read. Cannot be null.</param>
        /// <param name="length">The Blob stream length.</param>
        /// <exception cref="System.ArgumentNullException">If parameterName or stream is null.</exception>
        public void AddWithValue(string parameterName, Stream stream, long length)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName is null!");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream is null!");
            }

            AceQLParameter aceQLParameter = new AceQLParameter(parameterName, stream, length);
            aceqlParameters.Add(aceQLParameter);
        }

        /// <summary>
        /// Adds the specified <see cref="AceQLParameter"/>. object to the <see cref="AceQLParameterCollection"/>.
        /// </summary>
        /// <param name="value">The <see cref="AceQLParameter"/> to add to the collection.</param>
        /// <exception cref="System.ArgumentNullException">If value is null.</exception>
        public void Add(AceQLParameter value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value is null!");
            }

            aceqlParameters.Add(value);
        }


        /// <summary>
        ///  Gets the location of the specified <see cref="Object"/> within the collection.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to find.</param>
        /// <returns> The zero-based location of the specified <see cref="Object"/> that is a <see cref="AceQLParameter"/> within the collection. Returns -1 when the object does not exist in the <see cref="AceQLParameterCollection"/>.</returns>
        /// <exception cref="System.ArgumentNullException">If parameterName is null.</exception>
        public int IndexOf(object value)
        {
            for (int i = 0; i < aceqlParameters.Count; i++)
            {
                AceQLParameter aceQLParameter = aceqlParameters[i];
                if (aceQLParameter.Value.Equals(value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Inserts an <see cref="Object"/>into the <see cref="AceQLParameterCollection"/> at the specified index. 
        /// Not implemented.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">An <see cref="Object"/> to be inserted in the <see cref="AceQLParameterCollection"/>.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the specified <see cref="AceQLParameter"/> from the collection.
        /// </summary>
        /// <param name="value">The object to Remove from the collection.</param>
        public void Remove(object value)
        {
            for (int i = 0; i < aceqlParameters.Count; i++)
            {
                AceQLParameter aceQLParameter = aceqlParameters[i];
                if (aceQLParameter.Value.Equals(value))
                {
                    aceqlParameters.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        ///  Removes the <see cref="AceQLParameter"/> from the <see cref="AceQLParameterCollection"/>
        ///  at the specified parameter name.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="AceQLParameter"/> to Remove.</param>
        public void RemoveAt(string parameterName)
        {
            for (int i = 0; i < aceqlParameters.Count; i++)
            {
                AceQLParameter aceQLParameter = aceqlParameters[i];
                if (aceQLParameter.ParameterName.Equals(parameterName))
                {
                    aceqlParameters.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        ///  Removes the <see cref="AceQLParameter"/> from the <see cref="AceQLParameterCollection"/>
        ///  at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="AceQLParameter"/> object to Remove.</param>
        public void RemoveAt(int index)
        {
            aceqlParameters.RemoveAt(index);
        }

        /// <summary>
        /// Gets the <see cref="AceQLParameter"/> for it's name.
        /// </summary>
        /// <param name="parameterName">Name of the <see cref="AceQLParameter"/>.</param>
        /// <returns>The <see cref="AceQLParameter"/> for the parameter name.</returns>
        public AceQLParameter GetAceQLParameter(string parameterName)
        {
            for (int i = 0; i < aceqlParameters.Count; i++)
            {
                AceQLParameter aceQLParameter = aceqlParameters[i];
                if (aceQLParameter.ParameterName.Equals(parameterName))
                {
                    return aceQLParameter;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="AceQLParameter"/> at the index.
        /// </summary>
        /// <param name="index">The index of <see cref="AceQLParameter"/>.</param>
        /// <returns>The <see cref="AceQLParameter"/> at index position.</returns>
        protected AceQLParameter GetParameter(int index)
        {
            return aceqlParameters[index];
        }


        /// <summary>
        ///  Adds the specified <see cref="AceQLParameter"/> object to the <see cref="AceQLParameterCollection"/>. Not implemented.
        /// </summary>
        /// <param name="value">An <see cref="Object"/>.</param>
        /// <returns>The index of the new <see cref="AceQLParameter"/> object.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public int Add(object value)
        {
            throw new NotSupportedException();
        }
    }
}
