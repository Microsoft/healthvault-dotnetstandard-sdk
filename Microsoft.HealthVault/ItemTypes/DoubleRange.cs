// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a data range based on the <see cref="double"/> type.
    /// </summary>
    ///
    /// <remarks>
    /// A range consists of a minimum range value and a maximum range value of type
    /// <see cref="double"/>
    /// </remarks>
    ///
    public class DoubleRange : Range<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DoubleRange"/> class with
        /// default values.
        /// </summary>
        ///
        public DoubleRange()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="DoubleRange"/> class with the specified
        /// minimum and maximum range values.
        /// </summary>
        ///
        /// <param name="minRange">
        /// The minimum value for the range.
        /// </param>
        ///
        /// <param name="maxRange">
        /// The maximum value for the range.
        /// </param>
        ///
        public DoubleRange(double minRange, double maxRange)
            : base(minRange, maxRange)
        {
        }

        /// <summary>
        /// Reads the value from the specified <see cref="XPathNavigator"/> as a
        /// <see cref="double"/>.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The <see cref="XPathNavigator"/> to read the value from.
        /// </param>
        ///
        /// <returns>
        /// The value as a <see cref="double"/>.
        /// </returns>
        ///
        /// <remarks>
        /// Derived classes must override and read the value from the XML and convert it to the
        /// type for the range.
        /// </remarks>
        ///
        protected override double ReadRangeValue(XPathNavigator navigator)
        {
            return navigator.ValueAsDouble;
        }

        /// <summary>
        /// Writes the specified range value to the specified writer with the specified node name.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the node to use when writing the value.
        /// </param>
        ///
        /// <param name="value">
        /// The value to be written.
        /// </param>
        ///
        /// <param name="writer">
        /// The writer to write the value element to.
        /// </param>
        ///
        protected override void WriteRangeValue(string nodeName, double value, XmlWriter writer)
        {
            writer.WriteElementString(nodeName, XmlConvert.ToString(value));
        }

        /// <summary>
        /// Initializes the minimum range value to its default value.
        /// </summary>
        ///
        /// <returns>
        /// <see cref="double.MinValue"/>
        /// </returns>
        ///
        protected override double DefaultMinValue => double.MinValue;

        /// <summary>
        /// Gets the maximum range value to it's default value.
        /// </summary>
        ///
        /// <returns>
        /// <see cref="double.MaxValue"/>
        /// </returns>
        ///
        protected override double DefaultMaxValue => double.MaxValue;

        /// <summary>
        /// Returns the range as a string.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the range.
        /// </returns>
        ///
        /// <remarks>
        /// This method is not locale aware.
        /// </remarks>
        ///
        public override string ToString()
        {
            return string.Format(
                Resources.Range,
                MinRange,
                MaxRange);
        }
    }
}
