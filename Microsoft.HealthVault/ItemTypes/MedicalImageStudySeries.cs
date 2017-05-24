﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents the details of a specific series of images in a medical image study.
    /// </summary>
    public class MedicalImageStudySeries : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudySeries"/> class with default values.
        /// </summary>
        ///
        public MedicalImageStudySeries()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudySeries"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="acquisitionDateTime">
        /// The date and time that the image was acquired.
        /// </param>
        /// <param name="images">
        /// Medical image study series images.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="images"/> parameter is <b>null</b> or doesn't contain any images.
        /// </exception>
        ///
        public MedicalImageStudySeries(
            HealthServiceDateTime acquisitionDateTime,
            ICollection<MedicalImageStudySeriesImage> images)
        {
            Validator.ThrowIfArgumentNull(images, nameof(images), Resources.ImagesMandatory);

            if (images.Count == 0)
            {
                throw new ArgumentException(Resources.ImagesMandatory, nameof(images));
            }

            AcquisitionDateTime = acquisitionDateTime;
            foreach (MedicalImageStudySeriesImage image in images)
            {
                images.Add(image);
            }
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the medical image study series.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            if (_acquisitionDateTime == null)
            {
                _acquisitionDateTime = new HealthServiceDateTime();
            }

            _acquisitionDateTime.ParseXml(navigator.SelectSingleNode("acquisition-datetime"));

            _description = XPathHelper.GetOptNavValue(navigator, "description");

            _images.Clear();

            XPathNodeIterator imageIterator = navigator.Select("images");
            foreach (XPathNavigator imageNav in imageIterator)
            {
                MedicalImageStudySeriesImage image = new MedicalImageStudySeriesImage();
                image.ParseXml(imageNav);
                _images.Add(image);
            }

            _institutionName = XPathHelper.GetOptNavValue<Organization>(navigator, "institution-name");
            _referringPhysician = XPathHelper.GetOptNavValue<PersonItem>(navigator, "referring-physician");
            _modality = XPathHelper.GetOptNavValue<CodableValue>(navigator, "modality");
            _bodyPart = XPathHelper.GetOptNavValue<CodableValue>(navigator, "body-part");
            _previewBlobName = XPathHelper.GetOptNavValue(navigator, "preview-blob-name");
            _seriesInstanceUID = XPathHelper.GetOptNavValue(navigator, "series-instance-uid");
        }

        /// <summary>
        /// Writes the XML representation of the medical image study series into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the medical image study series should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="AcquisitionDateTime"/> is <b>null</b>.
        /// If <see cref="Images"/> is <b>null</b> or doesn't contain any image.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_acquisitionDateTime, Resources.AcquisitionDateTimeMandatory);

            if (_images == null || _images.Count == 0)
            {
                throw new ThingSerializationException(Resources.ImagesMandatory);
            }

            writer.WriteStartElement(nodeName);

            // acquisition-datetime
            _acquisitionDateTime.WriteXml("acquisition-datetime", writer);

            // description
            XmlWriterHelper.WriteOptString(writer, "description", _description);

            // images
            foreach (MedicalImageStudySeriesImage image in _images)
            {
                XmlWriterHelper.WriteOpt(writer, "images", image);
            }

            // institution-name
            XmlWriterHelper.WriteOpt(writer, "institution-name", _institutionName);

            // referring-physician
            XmlWriterHelper.WriteOpt(writer, "referring-physician", _referringPhysician);

            // modality
            XmlWriterHelper.WriteOpt(writer, "modality", _modality);

            // body-part
            XmlWriterHelper.WriteOpt(writer, "body-part", _bodyPart);

            // preview-blob-name
            XmlWriterHelper.WriteOptString(writer, "preview-blob-name", _previewBlobName);

            // series-instance-uid
            XmlWriterHelper.WriteOptString(writer, "series-instance-uid", _seriesInstanceUID);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time that the image was acquired.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tags (0008, 0022) and (0008, 0032).
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime AcquisitionDateTime
        {
            get { return _acquisitionDateTime; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(AcquisitionDateTime), Resources.AcquisitionDateTimeMandatory);
                _acquisitionDateTime = value;
            }
        }

        private HealthServiceDateTime _acquisitionDateTime;

        /// <summary>
        /// Gets or sets a description of the series.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0008, 103E).
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Description
        {
            get { return _description; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }

        private string _description;

        /// <summary>
        /// Gets and sets medical images.
        /// </summary>
        ///
        public Collection<MedicalImageStudySeriesImage> Images => _images;

        private readonly Collection<MedicalImageStudySeriesImage> _images = new Collection<MedicalImageStudySeriesImage>();

        /// <summary>
        /// Gets or sets the name of the institution where the images were acquired.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0008, 0080).
        ///
        /// If there is no information about the institution the value should be set
        /// to <b>null</b>.
        /// </remarks>
        ///
        public Organization InstitutionName
        {
            get { return _institutionName; }
            set { _institutionName = value; }
        }

        private Organization _institutionName;

        /// <summary>
        /// Gets or sets the physician who ordered the study.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0008, 0090).
        ///
        /// If there is no information about the physician the value should be set
        /// to <b>null</b>.
        /// </remarks>
        ///
        public PersonItem ReferringPhysician
        {
            get { return _referringPhysician; }
            set { _referringPhysician = value; }
        }

        private PersonItem _referringPhysician;

        /// <summary>
        /// Gets or sets the method (or modality) in which the images were acquired.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0008, 0060).
        ///
        /// If there is no information about the modality the value should be set to <b>null</b>.
        ///
        /// The preferred vocabulary is dicom.modality.
        /// </remarks>
        ///
        public CodableValue Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        private CodableValue _modality;

        /// <summary>
        /// Gets or sets the body part that was imaged.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0018, 0015).
        ///
        /// If there is no information about the body part the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public CodableValue BodyPart
        {
            get { return _bodyPart; }
            set { _bodyPart = value; }
        }

        private CodableValue _bodyPart;

        /// <summary>
        /// Gets or sets the name of the BLOB holding a smaller version of the image
        /// suitable for web viewing or email.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no preview BLOB name the value should be set to <b>null</b>.
        /// Previews should be stored using the jpeg or png format.
        /// It is recommended that the shorter dimension of the image be no less than 500 pixels in size.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string PreviewBlobName
        {
            get { return _previewBlobName; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "PreviewBlobName");
                _previewBlobName = value;
            }
        }

        private string _previewBlobName;

        /// <summary>
        /// Gets or sets the series instance UID.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0020,000E)
        ///
        /// If there is no series instance UID, the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string SeriesInstanceUID
        {
            get { return _seriesInstanceUID; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "SeriesInstanceUID");
                _seriesInstanceUID = value;
            }
        }

        private string _seriesInstanceUID;

        /// <summary>
        /// Gets a string representation of the medical image study series.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the medical image study series.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (!string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Description.Trim()))
            {
                result.Append(Description);

                if (result.Length > 0)
                {
                    result.Append(" ");
                }

                result.Append(Resources.OpenParen);

                if (BodyPart != null)
                {
                    result.Append(BodyPart.Text);
                }

                if (AcquisitionDateTime != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(Resources.ListSeparator);
                    }

                    result.Append(AcquisitionDateTime);
                }

                if (InstitutionName != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(Resources.ListSeparator);
                    }

                    result.Append(InstitutionName.Name);
                }

                if (ReferringPhysician != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(Resources.ListSeparator);
                    }

                    result.Append(ReferringPhysician.Name);
                }

                result.Append(Resources.CloseParen);
            }

            return result.ToString();
        }
    }
}
