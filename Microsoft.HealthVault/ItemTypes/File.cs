// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents information about a file.
    /// </summary>
    ///
    public class File : ThingBase
    {
        /// <summary>
        /// Creates a <see cref="File"/> item instance using the specified file path and content
        /// type.
        /// </summary>
        ///
        /// <param name="path">
        /// The path of the file to associate with this <see cref="File"/> instance.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the file. This is usually something like "image/jpg", "application/csv",
        /// or other mime type.
        /// </param>
        ///
        /// <returns>
        /// A new instance of the <see cref="File"/> thing with data populated from the
        /// specified file path.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="path"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="contentType"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters,
        /// and file names must be less than 260 characters.
        /// </exception>
        ///
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid (for example, it is on an unmapped drive).
        /// </exception>
        ///
        /// <exception cref="IOException">
        /// An I/O error occurred while opening the file.
        /// </exception>
        ///
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// path specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="path"/> was not found.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// <paramref name="path"/> is in an invalid format.
        /// </exception>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        ///
        public static File CreateFromFilePath(string path, CodableValue contentType)
        {
            File file = new File();
            file.SetContent(path, contentType);
            return file;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="File"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public File()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="File"/> class with the
        /// specified name and size.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the file.
        /// </param>
        ///
        /// <param name="size">
        /// The file size in bytes.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="size"/> parameter is negative or zero.
        /// </exception>
        ///
        public File(string name, long size)
            : base(TypeId)
        {
            Name = name;
            Size = size;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("bd0403c5-4ae2-4b0e-a8db-1888678e4528");

        /// <summary>
        /// Populates this <see cref="File"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the file data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a file node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator fileNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("file");

            Validator.ThrowInvalidIfNull(fileNav, Resources.FileUnexpectedNode);

            _name = fileNav.SelectSingleNode("name").Value;
            _size = fileNav.SelectSingleNode("size").ValueAsLong;

            XPathNavigator contentTypeNav =
                fileNav.SelectSingleNode("content-type");

            if (contentTypeNav != null)
            {
                _contentType = new CodableValue();
                _contentType.ParseXml(contentTypeNav);
            }
        }

        /// <summary>
        /// Writes the file data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the file data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            if (string.IsNullOrEmpty(_name))
            {
                throw new ThingSerializationException(Resources.FileNameNotSet);
            }

            Validator.ThrowSerializationIfNull(_size, Resources.FileSizeNotSet);

            // <file>
            writer.WriteStartElement("file");

            writer.WriteElementString("name", _name);
            writer.WriteElementString("size", _size.ToString());

            if (_contentType != null)
            {
                _contentType.WriteXml("content-type", writer);
            }

            // </file>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the file including the extension if
        /// available.
        /// </summary>
        ///
        /// <value>
        /// A string representing the file name.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b> or empty on set.
        /// </exception>
        ///
        public string Name
        {
            get { return _name; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Name");
                _name = value;
            }
        }

        private string _name;

        /// <summary>
        /// Gets or sets the file size.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the file size.
        /// </value>
        ///
        /// <remarks>
        /// This property must be set before the item is created or updated.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public long Size
        {
            get
            {
                return _size.HasValue ? (long)_size : 0;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Size), Resources.FileSizeNotPositive);
                }

                _size = value;
            }
        }

        private long? _size;

        /// <summary>
        /// Gets or sets the type of content contained in the file.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> representing the file content type.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the content type should not be stored.
        /// </remarks>
        ///
        public CodableValue ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        private CodableValue _contentType;

        /// <summary>
        /// Gets the content of the file item.
        /// </summary>
        ///
        public byte[] Content
        {
            get
            {
                byte[] result = null;
                BlobStore store = GetBlobStore(default(HealthRecordAccessor));
                if (store.Count > 0 && store[string.Empty] != null)
                {
                    result = store[string.Empty].ReadAllBytes();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a stream to read the file contents.
        /// </summary>
        ///
        public Stream ContentStream
        {
            get
            {
                Stream result = null;
                BlobStore store = GetBlobStore(default(HealthRecordAccessor));
                if (store.Count > 0 && store[string.Empty] != null)
                {
                    result = store[string.Empty].GetReaderStream();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a string representation of the file item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the file item.
        /// </returns>
        ///
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Sets the content of the file instance using the specified file.
        /// </summary>
        ///
        /// <param name="path">
        /// The path of the file to associate with this <see cref="File"/> instance.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the file.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="path"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="contentType"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters,
        /// and file names must be less than 260 characters.
        /// </exception>
        ///
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid (for example, it is on an unmapped drive).
        /// </exception>
        ///
        /// <exception cref="IOException">
        /// An I/O error occurred while opening the file.
        /// </exception>
        ///
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// path specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="path"/> was not found.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// <paramref name="path"/> is in an invalid format.
        /// </exception>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        ///
        public void SetContent(string path, CodableValue contentType)
        {
            Validator.ThrowIfStringNullOrEmpty(path, "path");
            Validator.ThrowIfArgumentNull(contentType, nameof(contentType), Resources.FileContentTypeMustBeSpecified);

            FileInfo fileInfo = new FileInfo(path);
            Size = fileInfo.Length;
            Name = fileInfo.Name;
            ContentType = contentType;

            BlobStore store = GetBlobStore(default(HealthRecordAccessor));
            Blob blob = store.NewBlob(string.Empty, ContentType.Text);

            byte[] content = System.IO.File.ReadAllBytes(path);
            blob.WriteInline(content);
        }
    }
}
