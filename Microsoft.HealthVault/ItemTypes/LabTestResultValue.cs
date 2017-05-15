// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A clinical value within a laboratory result.
    /// </summary>
    ///
    /// <remarks>
    /// This type defines a clinical value within a laboratory result,
    /// including value, unit, reference, and toxic ranges.
    /// </remarks>
    ///
    public class LabTestResultValue : ItemBase
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="LabTestResultValue"/>
        /// class with default values.
        /// </summary>
        ///
        public LabTestResultValue()
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="LabTestResultValue"/>
        /// class with the specified measurement.
        /// </summary>
        ///
        /// <param name="measurement">
        /// The value of the lab results.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="measurement"/> is <b> null </b>.
        /// </exception>
        ///
        public LabTestResultValue(GeneralMeasurement measurement)
        {
            Measurement = measurement;
        }

        /// <summary>
        /// Populates this <see cref="LabTestResultValue"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the lab test result value type data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the first node in <paramref name="navigator"/> is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // measurement
            _measurement = new GeneralMeasurement();
            _measurement.ParseXml(navigator.SelectSingleNode("measurement"));

            // ranges
            XPathNodeIterator rangesIterator = navigator.Select("ranges");

            _ranges = new Collection<TestResultRange>();
            foreach (XPathNavigator rangeNav in rangesIterator)
            {
                TestResultRange range = new TestResultRange();
                range.ParseXml(rangeNav);
                _ranges.Add(range);
            }

            // flag
            XPathNodeIterator flagsIterator = navigator.Select("flag");

            _flag = new Collection<CodableValue>();
            foreach (XPathNavigator flagNav in flagsIterator)
            {
                CodableValue singleflag = new CodableValue();
                singleflag.ParseXml(flagNav);
                _flag.Add(singleflag);
            }
        }

        /// <summary>
        /// Writes the lab test result value type data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the node to write the XML.</param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the lab test result value type data to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> is <b> null </b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b> null </b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Measurement"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_measurement, Resources.LabTestResultValueTypeMeasurementNotSet);

            // <lab-test-result-value-type>
            writer.WriteStartElement(nodeName);

            // measurement
            _measurement.WriteXml("measurement", writer);

            // ranges
            for (int index = 0; index < _ranges.Count; ++index)
            {
                XmlWriterHelper.WriteOpt(
                    writer,
                    "ranges",
                    _ranges[index]);
            }

            // flag
            for (int index = 0; index < _flag.Count; ++index)
            {
                XmlWriterHelper.WriteOpt(
                    writer,
                    "flag",
                    _flag[index]);
            }

            // </lab-test-result-value-type>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets measurement.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public GeneralMeasurement Measurement
        {
            get { return _measurement; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Measurement), Resources.LabTestResultValueTypeMeasurementNotSet);
                _measurement = value;
            }
        }

        private GeneralMeasurement _measurement;

        /// <summary>
        /// Gets the ranges that are associated with this test.
        /// </summary>
        ///
        public Collection<TestResultRange> Ranges => _ranges;

        private Collection<TestResultRange> _ranges = new Collection<TestResultRange>();

        /// <summary>
        /// Gets a collection containing the flags for laboratory results.
        /// </summary>
        ///
        /// <value>
        /// Example values are normal, high, low.
        /// </value>
        ///
        public Collection<CodableValue> Flag => _flag;

        private Collection<CodableValue> _flag = new Collection<CodableValue>();

        /// <summary>
        /// Gets a string representation of the lab test result value type item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the lab test result value type item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);
            result.Append(_measurement);
            if (_ranges != null)
            {
                for (int index = 0; index < _ranges.Count; ++index)
                {
                    result.AppendFormat(
                        Resources.ListFormat,
                        _ranges[index].ToString());
                }
            }

            if (_flag != null)
            {
                for (int index = 0; index < _flag.Count; ++index)
                {
                    if (result.Length > 0)
                    {
                        result.Append(
                            Resources.ListSeparator);
                    }

                    result.Append(_flag[index]);
                }
            }

            return result.ToString();
        }
    }
}
