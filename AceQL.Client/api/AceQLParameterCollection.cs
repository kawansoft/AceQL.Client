// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-22-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="AceQLParameterCollection.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using AceQL.Client.Api.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Represents a collection of parameters associated with an AceQL.Client.api.AceQLCommand 
    /// and their respective mappings to columns.
    /// </summary>
    public class AceQLParameterCollection : IList, ICollection, IEnumerable
    {
        internal static bool DEBUG = false;

        /// <summary>
        /// The AceQL Parameters
        /// </summary>
        private List<AceQLParameter> aceqlParameters = new List<AceQLParameter>();
        private string cmdText;

        /// <summary>
        /// Initializes a new instance of the <see cref="AceQLParameterCollection"/> class.
        /// </summary>
        /// <param name="cmdText">The SQL command text.</param>
        /// <exception cref="System.ArgumentNullException">If cmdText is null.</exception>
        internal AceQLParameterCollection(string cmdText)
        {
            if (cmdText == null)
            {
                throw new ArgumentNullException("cmdText is null!");
            }

            this.cmdText = cmdText;
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
        /// <exception cref="System.NotImplementedException"></exception>
        public object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Specifies whether the collection is a fixed size. Not implemented.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsFixedSize
        {
            get
            {
                throw new NotImplementedException();
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
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns parameter at specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>the AceQLParameter at index.</returns>
        public object this[int index]
        {
            get
            {
                return aceqlParameters[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Removes all values from the AceQLParameterCollection" />.
        /// </summary>
        public void Clear()
        {
            aceqlParameters.Clear();
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
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
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
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
        /// Copies the elements of the System.Collections.ICollection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.ICollection. The System.Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Exposes the <see cref="M:System.Collections.IEnumerable.GetEnumerator" /> method, which supports a simple iteration over a collection by a .NET Framework data provider.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return aceqlParameters.GetEnumerator();
        }

        /// <summary>
        /// Get index of a parameter name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The index of the parameter name.</returns>
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
        /// Add a null value for a parameter, and precise the parameter's SQL type.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="sqlType">The SQL type of the parameter.</param>
        /// <exception cref="System.ArgumentNullException">If parameterName is null.</exception>
        public void AddNullValue(string parameterName, SqlType sqlType)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName is null!");
            }

            AceQLParameter aceQLParameter = new AceQLParameter(parameterName, "NULL");
            aceQLParameter.IsNullValue = true;
            aceQLParameter.SqlType = sqlType;
            aceqlParameters.Add(aceQLParameter);
            
            //debug(parameterName + " SqlType: " + aceQLParameter.SqlType);
        }

        /// <summary>
        /// Adds the parameter name with it's value.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value. Cannot ne bull.</param>
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
        /// Adds the BLOB parameter as a stream.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="stream">The BLOB stream to read.</param>
        /// <param name="length">The BLOB stream length.</param>
        /// <exception cref="System.ArgumentNullException">If parameterName or stream is null.</exception>
        public void AddBlob(string parameterName, Stream stream, long length)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName is null!");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream is null!");
            }

            AceQLParameter aceQLParameter = new AceQLParameter(parameterName, stream);
            aceQLParameter.SqlType = SqlType.BLOB;
            aceQLParameter.BlobLength = length;
            aceqlParameters.Add(aceQLParameter);
            Debug(parameterName + " SqlType: " + aceQLParameter.SqlType);
        }

        /// <summary>
        /// Adds the specified parameter.
        /// </summary>
        /// <param name="value">The value.</param>
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
        /// Get index of parameter value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The index of the value.</returns>
        public int IndexOf(object value)
        {
            //if (value == null)
            //{
            //    throw new ArgumentNullException("value is null!");
            //}

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
        /// Inserts value at the specified index. Not implemented.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified parameter value.
        /// </summary>
        /// <param name="value">The value.</param>
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
        /// Removes the specified parameter name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
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
        /// Removes the parameter at the specified index .
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            aceqlParameters.RemoveAt(index);
        }

        /// <summary>
        /// Gets the parameter for it's name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The AceQLParameter for the parameter name.</returns>
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
        /// Gets the parameter at the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The AceQLParameter at index position.</returns>
        protected AceQLParameter GetParameter(int index)
        {
            return aceqlParameters[index];
        }


        /// <summary>
        /// Adds the specified value. Not implemented.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        private static void Debug(string s)
        {
            if (DEBUG)
            {
                ConsoleEmul.WriteLine(DateTime.Now + " " + s);
            }
        }
    }
}
