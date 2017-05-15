// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Application
{
    /// <summary>
    /// Represents configuration data for an application which can be read from a file or stream
    /// and has an associated content type.
    /// </summary>
    ///
    /// <remarks>
    /// HealthVault applications can be configured with logos, privacy statements, and terms of
    /// use statements that can be read from a file or stream. This class wraps the content and
    /// content type for that configuration.
    /// </remarks>
    ///
    internal class ApplicationBinaryConfiguration
    {
        /// <summary>
        /// Constructs a <see cref="ApplicationBinaryConfiguration"/> with default values.
        /// </summary>
        ///
        public ApplicationBinaryConfiguration()
        {
        }

        /// <summary>
        /// Constructs a <see cref="ApplicationBinaryConfiguration"/> with the specified file and
        /// content type.
        /// </summary>
        ///
        /// <param name="binaryConfigurationFilePath">
        /// A local path to a file to use as the content.
        /// </param>
        ///
        /// <param name="contentType">
        /// The MIME type of the content in the file specified by
        /// <paramref name="binaryConfigurationFilePath"/>.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="binaryConfigurationFilePath"/> or <paramref name="contentType"/> is
        /// <b>null</b> or empty,<br/>
        /// or <paramref name="binaryConfigurationFilePath"/> contains one or more invalid characters.
        /// </exception>
        ///
        /// <exception cref="PathTooLongException">
        /// If <paramref name="binaryConfigurationFilePath"/> is too long.
        /// </exception>
        ///
        /// <exception cref="DirectoryNotFoundException">
        /// If <paramref name="binaryConfigurationFilePath"/> is invalid (for example, it is on an
        /// unmapped drive).
        /// </exception>
        ///
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="binaryConfigurationFilePath"/> is a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// <paramref name="binaryConfigurationFilePath"/> specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="binaryConfigurationFilePath"/> was not found.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// <paramref name="binaryConfigurationFilePath"/> is in an invalid format.
        /// </exception>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        ///
        public ApplicationBinaryConfiguration(string binaryConfigurationFilePath, string contentType)
        {
            Validator.ThrowIfStringNullOrEmpty(binaryConfigurationFilePath, "binaryConfigurationFilePath");
            Validator.ThrowIfStringNullOrEmpty(contentType, "contentType");

            BinaryConfigurationContent = File.ReadAllBytes(binaryConfigurationFilePath);
            ContentType = contentType;
        }

        /// <summary>
        /// Constructs a <see cref="ApplicationBinaryConfiguration"/> with the specified stream and
        /// content type.
        /// </summary>
        ///
        /// <param name="binaryConfigurationContent">
        /// A stream containing the content.
        /// </param>
        ///
        /// <param name="contentType">
        /// The MIME type of the content in the stream specified by
        /// <paramref name="binaryConfigurationContent"/>.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="binaryConfigurationContent"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="contentType"/> is
        /// <b>null</b> or empty.
        /// </exception>
        ///
        public ApplicationBinaryConfiguration(Stream binaryConfigurationContent, string contentType)
        {
            Validator.ThrowIfArgumentNull(binaryConfigurationContent, nameof(binaryConfigurationContent), Resources.ApplicationBinaryConfigurationContentStreamRequired);

            Validator.ThrowIfStringNullOrEmpty(contentType, "contentType");

            BinaryConfigurationContent = new byte[binaryConfigurationContent.Length];
            int numberOfBytesToRead = (int)binaryConfigurationContent.Length;
            int bytesRead = 0;

            while (numberOfBytesToRead > 0)
            {
                int bytesReceived =
                    binaryConfigurationContent.Read(
                        BinaryConfigurationContent,
                        bytesRead,
                        numberOfBytesToRead);
                bytesRead += bytesReceived;
                numberOfBytesToRead -= bytesReceived;
            }

            ContentType = contentType;
        }

        /// <summary>
        /// Creates an instance from XML.
        /// </summary>
        ///
        /// <param name="containerNav">
        /// The container nav.
        /// </param>
        ///
        /// <param name="outerElement">
        /// The outer element.
        /// </param>
        ///
        /// <param name="dataElement">
        /// The data element.
        /// </param>
        ///
        /// <param name="contentTypeElement">
        /// The content type element.
        /// </param>
        ///
        /// <returns>
        /// Configuration data for an application which can be read from a file or stream
        /// and has an associated content type.
        /// </returns>
        ///
        internal static ApplicationBinaryConfiguration CreateFromXml(
            XPathNavigator containerNav,
            string outerElement,
            string dataElement,
            string contentTypeElement)
        {
            ApplicationBinaryConfiguration binaryConfig = null;
            XPathNavigator outerNav = containerNav.SelectSingleNode(outerElement);
            if (outerNav != null)
            {
                binaryConfig = new ApplicationBinaryConfiguration();
                binaryConfig.CultureSpecificBinaryConfigurationContents.PopulateFromXml(
                    outerNav,
                    dataElement);
                binaryConfig.ContentType = outerNav.SelectSingleNode(contentTypeElement).Value;
            }

            return binaryConfig;
        }

        internal void AppendRequestParameters(
            XmlWriter writer,
            string outerElementName,
            string dataElementName)
        {
            writer.WriteStartElement(outerElementName);

            CultureSpecificBinaryConfigurationContents.AppendLocalizedElements(
                writer,
                dataElementName);

            writer.WriteElementString("content-type", ContentType);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the binary content.
        /// </summary>
        ///
        public byte[] BinaryConfigurationContent
        {
            get { return CultureSpecificBinaryConfigurationContents.BestValue; }
            set { CultureSpecificBinaryConfigurationContents.DefaultValue = value; }
        }

        /// <summary>
        ///     Gets a dictionary of language specifiers and localized content.
        /// </summary>
        public CultureSpecificByteArrayDictionary CultureSpecificBinaryConfigurationContents { get; } = new CultureSpecificByteArrayDictionary();

        /// <summary>
        /// Gets or sets the MIME type of the content.
        /// </summary>
        ///
        public string ContentType { get; set; }
    }
}
