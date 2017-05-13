// ***********************************************************************
// Assembly         : AceQL.Client
// Author           : Nicolas de Pomereu
// Created          : 02-24-2017
//
// Last Modified By : Nicolas de Pomereu
// Last Modified On : 02-25-2017
// ***********************************************************************
// <copyright file="AceQLParameter.cs" company="KawanSoft">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api
{
    /// <summary>
    /// Represents a parameter to an AceQL.Client.api.AceQLCommand.
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
        private SqlType sqlType;

        private bool isNullValue = false;

        /// <summary>
        /// The length of the BLOB to upload
        /// </summary>
        private long blobLength = 0;

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
                throw new ArgumentNullException("Parameter value cannot be null! Use AceQLCommand.Parameters.addNullValue() to pass a null value.");
            }
        }


        /// <summary>
        /// Gets name of the <see cref="AceQLParameter"/> name.
        /// </summary>
        /// <value>The name of the parameter.</value>
        /// <exception cref="System.NotImplementedException"></exception>
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
        internal SqlType SqlType
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

            set
            {
                blobLength = value;
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
