// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates radiology
    /// laboratory results.
    /// </summary>
    ///
    public class RadiologyLabResults : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RadiologyLabResults"/> class
        /// with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public RadiologyLabResults()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RadiologyLabResults"/> class
        /// with the specified date.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time for the radiology laboratory results.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public RadiologyLabResults(HealthServiceDateTime when)
            : base(TypeId)
        {
            When = when;
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
            new Guid("E4911BD3-61BF-4E10-AE78-9C574B888B8F");

        /// <summary>
        /// Populates this radiology laboratory results instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the radiology laboratory results data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a radiology laboratory results node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("radiology-lab-results");

            Validator.ThrowInvalidIfNull(itemNav, Resources.RadiologyLabResultsUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // <title>
            _title =
                XPathHelper.GetOptNavValue(itemNav, "title");

            // <anatomic-site>
            _anatomicSite =
                XPathHelper.GetOptNavValue(itemNav, "anatomic-site");

            // <result-text>
            _resultText =
                XPathHelper.GetOptNavValue(itemNav, "result-text");
        }

        /// <summary>
        /// Writes the radiology laboratory results data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the radiology laboratory results data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// The <see cref="When"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.RadiologyLabResultsWhenNotSet);

            // <radiology-lab-results>
            writer.WriteStartElement("radiology-lab-results");

            // <when>
            _when.WriteXml("when", writer);

            // <title>
            XmlWriterHelper.WriteOptString(
                writer,
                "title",
                _title);

            // <anatomic-site>
            XmlWriterHelper.WriteOptString(
                writer,
                "anatomic-site",
                _anatomicSite);

            // <result-text>
            XmlWriterHelper.WriteOptString(
                writer,
                "result-text",
                _resultText);

            // </radiology-lab-result>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the radiology laboratory results occurred.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the title for the radiology laboratory results.
        /// </summary>
        ///
        /// <value>
        /// A string representing the title.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the title should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Title
        {
            get { return _title; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Title");
                _title = value;
            }
        }

        private string _title;

        /// <summary>
        /// Gets or sets the anatomic site for the radiology laboratory results.
        /// </summary>
        ///
        /// <value>
        /// A string representing the site.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the site should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string AnatomicSite
        {
            get { return _anatomicSite; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "AnatomicSite");
                _anatomicSite = value;
            }
        }

        private string _anatomicSite;

        /// <summary>
        /// Gets or sets the result text for the radiology laboratory results.
        /// </summary>
        ///
        /// <value>
        /// A string representing the text.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the result text should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string ResultText
        {
            get { return _resultText; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "ResultText");
                _resultText = value;
            }
        }

        private string _resultText;

        /// <summary>
        /// Gets a string representation of the radiology lab results.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the radiology lab results.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (Title != null)
            {
                result = Title;
            }

            return result;
        }
    }
}
