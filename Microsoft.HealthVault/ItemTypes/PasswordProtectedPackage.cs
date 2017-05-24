// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Provides metadata about the encryption algorithm and parameters used to
    /// protect some data with a password.
    /// </summary>
    ///
    /// <remarks>
    /// The <see cref="PasswordProtectedPackage"/> item type defines the metadata for the
    /// encryption algorithm used to protect data with a password. The
    /// application should generate a password (or take it from the user) and
    /// encrypt the desired data. This data should be set in a <see cref="Blob"/> created off the
    /// <see cref="BlobStore"/> retrieved from the
    /// <see cref="ThingBase.GetBlobStore(HealthRecordAccessor)"/>.
    /// The properties of the Blob should be set with the parameters required
    /// to decrypt the data. These parameters are application dependant but
    /// should adhere to standard practices in dealing with PKCS5v2 data.
    /// </remarks>
    ///
    internal class PasswordProtectedPackage : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PasswordProtectedPackage"/> class
        /// with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public PasswordProtectedPackage()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PasswordProtectedPackage"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="algorithm">
        /// The name of the algorithm used to protect the data.
        /// </param>
        ///
        /// <param name="salt">
        /// A string representing the encoding of the bytes that were used as
        /// the salt when protecting the data.
        /// </param>
        ///
        /// <param name="keyLength">
        /// The number of bits used by the algorithm.
        /// </param>
        ///
        /// <remarks>
        /// In general, the salt is a series of bytes encoded in an
        /// application-dependent way. The length of the salt must match the
        /// algorithm. It is recommended that the salt encoding be base64.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The <paramref name="salt"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="keyLength"/> parameter is negative or zero.
        /// </exception>
        ///
        public PasswordProtectedPackage(
            PasswordProtectAlgorithm algorithm,
            string salt,
            int keyLength)
            : base(TypeId)
        {
            PasswordProtectAlgorithm = algorithm;
            Salt = salt;
            KeyLength = keyLength;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static readonly new Guid TypeId =
            new Guid("c9287326-bb43-4194-858c-8b60768f000f");

        /// <summary>
        /// Populates this PasswordProtectedPackage instance from the data
        /// in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the file data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in the <paramref name="typeSpecificXml"/>
        /// parameter is not a file node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator packageNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "password-protected-package/encrypt-algorithm");

            Validator.ThrowInvalidIfNull(packageNav, Resources.PackageUnexpectedNode);

            _algorithmName =
                packageNav.SelectSingleNode("algorithm-name").Value;

            switch (_algorithmName)
            {
                case "none":
                    PasswordProtectAlgorithm = PasswordProtectAlgorithm.None;
                    break;

                case "hmac-sha1-3des":
                    PasswordProtectAlgorithm = PasswordProtectAlgorithm.HmacSha13Des;
                    break;

                case "hmac-sha256-aes256":
                    PasswordProtectAlgorithm = PasswordProtectAlgorithm.HmacSha256Aes256;
                    break;

                default:
                    PasswordProtectAlgorithm = PasswordProtectAlgorithm.Unknown;
                    break;
            }

            _salt = packageNav.SelectSingleNode("parameters/salt").Value;
            _hashIterations =
                packageNav.SelectSingleNode(
                    "parameters/iteration-count").ValueAsInt;

            _keyLength =
                packageNav.SelectSingleNode(
                    "parameters/key-length").ValueAsInt;
        }

        /// <summary>
        /// Writes the file data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the password protected package to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            if (PasswordProtectAlgorithm == PasswordProtectAlgorithm.Unknown && _algorithmName == null)
            {
                throw new ThingSerializationException(Resources.PackageAlgorithmNotSet);
            }

            if (string.IsNullOrEmpty(_salt))
            {
                throw new ThingSerializationException(Resources.PackageSaltNotSet);
            }

            if (_keyLength < 1)
            {
                throw new ThingSerializationException(Resources.PackageKeyLengthNotSet);
            }

            // <password-protected-package>
            writer.WriteStartElement("password-protected-package");

            // <encrypt-algorithm>
            writer.WriteStartElement("encrypt-algorithm");

            switch (PasswordProtectAlgorithm)
            {
                case PasswordProtectAlgorithm.None:
                    writer.WriteElementString("algorithm-name", "none");
                    break;

                case PasswordProtectAlgorithm.HmacSha13Des:
                    writer.WriteElementString("algorithm-name", "hmac-sha1-3des");
                    break;

                case PasswordProtectAlgorithm.HmacSha256Aes256:
                    writer.WriteElementString("algorithm-name", "hmac-sha256-aes256");
                    break;

                case PasswordProtectAlgorithm.Unknown:
                    writer.WriteElementString("algorithm-name", _algorithmName);
                    break;
            }

            // <parameters>
            writer.WriteStartElement("parameters");

            writer.WriteElementString("salt", _salt);
            writer.WriteElementString(
                "iteration-count",
                _hashIterations.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString(
                "key-length",
                _keyLength.ToString(CultureInfo.InvariantCulture));

            // </parameters>
            writer.WriteEndElement();

            // </encrypt-algorithm>
            writer.WriteEndElement();

            // </password-protected-package>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the algorithm used to encrypt the package.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="PasswordProtectAlgorithm"/>
        /// representing the algorithm.
        /// </value>
        ///
        public PasswordProtectAlgorithm PasswordProtectAlgorithm { get; set; } = PasswordProtectAlgorithm.None;

        private string _algorithmName;

        /// <summary>
        /// Gets or sets the salt used when encrypting the package.
        /// </summary>
        ///
        /// <value>
        /// A string representing the salt.
        /// </value>
        ///
        /// <remarks>
        /// In general, the salt is a series of bytes encoded in an
        /// application-dependent way. The length of the salt must match the
        /// algorithm. It is recommended that the salt encoding be base64.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace on set.
        /// </exception>
        ///
        public string Salt
        {
            get { return _salt; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Salt");
                Validator.ThrowIfStringIsWhitespace(value, "Salt");
                _salt = value;
            }
        }

        private string _salt;

        /// <summary>
        /// Gets or sets the number of hash iterations taken when protecting
        /// the package.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the number of iterations. The default value
        /// is 20000 iterations.
        /// </value>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public int HashIterations
        {
            get
            {
                return _hashIterations;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(HashIterations), Resources.PackageHashIterationOutOfRange);
                }

                _hashIterations = value;
            }
        }

        // Default to 20k (same as PFX in Windows)
        private int _hashIterations = 20000;

        /// <summary>
        /// Gets or sets the key length in bits.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the key length.
        /// </value>
        ///
        /// <remarks>
        /// The value should match that of the algorithm, for example, 168 bits
        /// for 3DES and 256 bits for AES256.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than one.
        /// </exception>
        ///
        public int KeyLength
        {
            get { return _keyLength; }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(KeyLength), Resources.PackageKeyLengthOutOfRange);
                }

                _keyLength = value;
            }
        }

        private int _keyLength;

        /// <summary>
        /// Gets a string representation of the password protected package definition.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the password protected package.
        /// </returns>
        ///
        public override string ToString()
        {
            string result;
            switch (PasswordProtectAlgorithm)
            {
                case PasswordProtectAlgorithm.HmacSha13Des:
                    result = "hmac-sha1-3des";
                    break;

                case PasswordProtectAlgorithm.HmacSha256Aes256:
                    result = "hmac-sha256-aes256";
                    break;

                case PasswordProtectAlgorithm.None:
                    result = "none";
                    break;

                default:
                    result = "unknown";
                    break;
            }

            return result;
        }
    }
}
