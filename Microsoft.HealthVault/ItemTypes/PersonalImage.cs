// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a personal image associated with the health record.
    /// </summary>
    ///
    /// <remarks>
    /// There can be only one personal image for each health record.
    /// </remarks>
    ///
    public class PersonalImage : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PersonalImage"/> class with
        /// default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public PersonalImage()
            : base(TypeId)
        {
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
            new Guid("a5294488-f865-4ce3-92fa-187cd3b58930");

        /// <summary>
        /// Populates this <see cref="PersonalImage"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the personal image data from.
        /// </param>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
        }

        /// <summary>
        /// Writes the file data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the file data to.
        /// </param>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("personal-image");
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the image from the byte stream into the BlobStore
        /// of the item.
        /// </summary>
        ///
        /// <param name="imageStream">
        /// The stream from which to retrieve the image bytes.
        /// </param>
        ///
        /// <param name="mimeType">
        /// The mime type for the image.
        /// </param>
        ///
        /// <remarks>
        /// The image is streamed in using the <see cref="BlobStore"/> for this item.
        /// <br/>
        /// This method does not close the stream.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="imageStream"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="imageStream"/> parameter length is greater than
        /// <see cref="int.MaxValue"/>.
        /// </exception>
        ///
        public void WriteImage(Stream imageStream, string mimeType)
        {
            Validator.ThrowIfArgumentNull(imageStream, nameof(imageStream), Resources.ImageStreamNull);

            if (imageStream.Length > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(imageStream), Resources.ImageStreamToLarge);
            }

            long bytesToRead = imageStream.Length;
            byte[] imageBytes = new byte[bytesToRead];
            long imageByteOffset = 0;
            int bytesRead = 0;

            if (imageStream.CanSeek && imageStream.Position != 0)
            {
                imageStream.Seek(0, SeekOrigin.Begin);
            }

            do
            {
                bytesRead =
                    imageStream.Read(
                        imageBytes,
                    (int)imageByteOffset,
                        (int)bytesToRead);
                if (bytesRead == 0)
                {
                    break;
                }

                bytesToRead -= bytesRead;
            }
            while (bytesRead > 0);

            Blob blob =
                GetBlobStore(default(HealthRecordAccessor)).NewBlob(string.Empty, mimeType);
            blob.WriteInline(imageBytes);
        }

        /// <summary>
        /// Reads the image from the <see cref="Blob"/>
        /// as a stream.
        /// </summary>
        ///
        /// <remarks>
        /// An application must explicitly request that the blob information is returned
        /// when querying for an personal image. It can do this by asking for the BlobPayload section
        /// in the Sections property of the <see cref="HealthRecordView"/> class (filter.View).
        /// </remarks>
        ///
        /// <returns>
        /// A Stream containing the image bytes. It is the caller's
        /// responsibility to close this stream.
        /// May return null if the blob payload is not requested or the personal
        /// image has no data...
        /// </returns>
        ///
        public Stream ReadImage()
        {
            BlobStore store = GetBlobStore(default(HealthRecordAccessor));
            Blob blob = store[string.Empty];

            if (blob == null)
            {
                return null;
            }

            return blob.GetReaderStream();
        }
    }
}
