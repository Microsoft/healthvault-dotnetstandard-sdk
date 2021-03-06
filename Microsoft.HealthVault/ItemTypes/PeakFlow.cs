// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing that encapsulates a single peak flow reading.
    /// </summary>
    ///
    /// <remarks>
    /// Peak flow measures are typically collected on a daily basis by patients to track their
    /// lung function.
    /// </remarks>
    ///
    public class PeakFlow : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PeakFlow"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public PeakFlow()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PeakFlow"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="when">
        /// The date and time when the peak flow reading occurred.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public PeakFlow(ApproximateDateTime when)
            : base(TypeId)
        {
            When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("5d8419af-90f0-4875-a370-0f881c18f6b3");

        /// <summary>
        /// Populates this <see cref="PeakFlow"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the peack flow data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in the <paramref name="typeSpecificXml"/> parameter
        /// is not a peak-flow node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "peak-flow");

            Validator.ThrowInvalidIfNull(itemNav, Resources.PeakFlowUnexpectedNode);

            _when = new ApproximateDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _peakExpiratoryFlow =
                XPathHelper.GetOptNavValue<FlowMeasurement>(
                    itemNav,
                    "pef");

            _fev1 =
                XPathHelper.GetOptNavValue<VolumeMeasurement>(
                    itemNav,
                    "fev1");

            _fev6 =
                XPathHelper.GetOptNavValue<VolumeMeasurement>(
                    itemNav,
                    "fev6");

            _measurementFlags.Clear();
            XPathNodeIterator measurementFlagsIterator = itemNav.Select("measurement-flags");
            foreach (XPathNavigator flagNav in measurementFlagsIterator)
            {
                CodableValue flag = new CodableValue();
                flag.ParseXml(flagNav);

                _measurementFlags.Add(flag);
            }
        }

        /// <summary>
        /// Writes the peak flow data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the peak flow data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.PeakFlowWhenNotSet);

            // <peak-flow>
            writer.WriteStartElement("peak-flow");

            // <when>
            _when.WriteXml("when", writer);

            XmlWriterHelper.WriteOpt(
                writer,
                "pef",
                _peakExpiratoryFlow);

            XmlWriterHelper.WriteOpt(
                writer,
                "fev1",
                _fev1);

            XmlWriterHelper.WriteOpt(
                writer,
                "fev6",
                _fev6);

            foreach (CodableValue flag in _measurementFlags)
            {
                flag.WriteXml("measurement-flags", writer);
            }

            // </peak-flow>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time when the peak flow reading occurred.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="ApproximateDateTime"/> instance representing the date
        /// and time.
        /// </returns>
        ///
        /// <remarks>
        /// The value defaults to the current year only.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ApproximateDateTime When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private ApproximateDateTime _when = new ApproximateDateTime();

        /// <summary>
        /// Gets or sets the peak expiratory flow measured in liters per
        /// second (L/s).
        /// </summary>
        ///
        /// <returns>
        /// A number representing the peak flow.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the peak expiratory flow should not
        /// be stored.
        /// </remarks>
        ///
        public FlowMeasurement Pef
        {
            get { return _peakExpiratoryFlow; }
            set { _peakExpiratoryFlow = value; }
        }

        private FlowMeasurement _peakExpiratoryFlow;

        /// <summary>
        /// Gets or sets the forced expiratory volume in one second, measured in
        /// liters (L).
        /// </summary>
        ///
        /// <returns>
        /// A number representing the volume.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the forced expiratory volume should not
        /// be stored.
        /// </remarks>
        ///
        public VolumeMeasurement Fev1
        {
            get { return _fev1; }
            set { _fev1 = value; }
        }

        private VolumeMeasurement _fev1;

        /// <summary>
        /// Gets or sets the forced expiratory volume in six seconds, measured in
        /// liters (L).
        /// </summary>
        ///
        /// <returns>
        /// A number representing the volume.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the forced expiratory volume should not
        /// be stored.
        /// </remarks>
        ///
        public VolumeMeasurement Fev6
        {
            get { return _fev6; }
            set { _fev6 = value; }
        }

        private VolumeMeasurement _fev6;

        /// <summary>
        /// Gets a collection of additional information about the measurement.
        /// </summary>
        ///
        /// <returns>
        /// A collection of <see cref="CodableValue"/> representing the flags.
        /// </returns>
        ///
        /// <remarks>
        /// Examples: incomplete measurement.
        /// </remarks>
        ///
        public Collection<CodableValue> MeasurementFlags => _measurementFlags;

        private readonly Collection<CodableValue> _measurementFlags = new Collection<CodableValue>();

        /// <summary>
        /// Gets a string representation of the peak flow reading.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the peak flow reading.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (Pef != null && Fev1 != null && Fev6 != null)
            {
                result =
                    string.Format(
                        Resources.PeakFlowToStringFormatPefFev1Fev6,
                        Pef.ToString(),
                        Fev1.ToString(),
                        Fev6.ToString());
            }
            else if (Pef != null && Fev1 != null)
            {
                result =
                    string.Format(
                        Resources.PeakFlowToStringFormatPefFev1,
                        Pef.ToString(),
                        Fev1.ToString());
            }
            else if (Pef != null && Fev6 != null)
            {
                result =
                    string.Format(
                        Resources.PeakFlowToStringFormatPefFev6,
                        Pef.ToString(),
                        Fev6.ToString());
            }
            else if (Fev1 != null && Fev6 != null)
            {
                result =
                    string.Format(
                        Resources.PeakFlowToStringFormatFev1Fev6,
                        Fev1.ToString(),
                        Fev6.ToString());
            }
            else if (Pef != null)
            {
                result =
                    string.Format(
                        Resources.PeakFlowToStringFormatPef,
                        Pef.ToString());
            }
            else if (Fev1 != null)
            {
                result =
                    string.Format(
                        Resources.PeakFlowToStringFormatFev1,
                        Fev1.ToString());
            }
            else if (Fev6 != null)
            {
                result =
                    string.Format(
                        Resources.PeakFlowToStringFormatFev6,
                        Fev6.ToString());
            }

            return result;
        }
    }
}
