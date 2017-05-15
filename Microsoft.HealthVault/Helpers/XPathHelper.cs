// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Helpers
{
    internal static class XPathHelper
    {
        #region mandatory

        internal static DateTime GetDateTime(XPathNavigator nav, string elementName)
        {
            string navValue = nav.SelectSingleNode(elementName).Value;

            DateTime result = DateTime.MaxValue;
            try
            {
                result =
                    DateTime.Parse(
                        navValue,
                        DateTimeFormatInfo.InvariantInfo,
                        DateTimeStyles.AdjustToUniversal);
            }
            catch (FormatException)
            {
            }

            return result;
        }

        internal static EnumType GetEnumByName<EnumType>(
            XPathNavigator nav,
            string elementName,
            EnumType defaultValue)
        {
            string navValue = nav.SelectSingleNode(elementName).Value;

            EnumType result = defaultValue;
            try
            {
                result =
                    (EnumType)Enum.Parse(typeof(EnumType), navValue);
            }
            catch (FormatException)
            {
            }

            return result;
        }

        internal static EnumType GetEnumByNumber<EnumType>(
            XPathNavigator nav,
            string elementName,
            EnumType defaultValue)
        {
            EnumType result = defaultValue;

            try
            {
                int navValue = nav.SelectSingleNode(elementName).ValueAsInt;
                IEnumerator values = Enum.GetValues(typeof(EnumType)).GetEnumerator();

                while (values.MoveNext())
                {
                    int enumValue = (int)values.Current;

                    if (enumValue == navValue)
                    {
                        result = (EnumType)values.Current;
                        break;
                    }
                }
            }
            catch (FormatException)
            {
            }

            return result;
        }

        internal static decimal GetDecimal(XPathNavigator nav, string elementName)
        {
            string navValue = nav.SelectSingleNode(elementName).Value;

            decimal result;

            if (decimal.TryParse(navValue, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return decimal.MaxValue;
        }

        #endregion mandatory

        #region optionals

        internal static bool? GetOptNavValueAsBool(
            XPathNavigator nav,
            string elementName)
        {
            bool? result = null;

            XPathNavigator valueNav =
                nav.SelectSingleNode(elementName);
            if (valueNav != null)
            {
                result = valueNav.ValueAsBoolean;
            }

            return result;
        }

        internal static Guid? GetOptNavValueAsGuid(
            XPathNavigator nav,
            string elementName)
        {
            Guid? result = null;

            XPathNavigator valueNav =
                nav.SelectSingleNode(elementName);
            if (valueNav != null)
            {
                try
                {
                    result = new Guid(valueNav.Value);
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            return result;
        }

        internal static int? GetOptNavValueAsInt(
            XPathNavigator nav,
            string elementName)
        {
            int? result = null;

            XPathNavigator valueNav =
                nav.SelectSingleNode(elementName);
            if (valueNav != null)
            {
                result = valueNav.ValueAsInt;
            }

            return result;
        }

        internal static uint? GetOptNavValueAsUInt(
            XPathNavigator nav,
            string elementName)
        {
            uint? result = null;

            XPathNavigator valueNav =
                nav.SelectSingleNode(elementName);
            if (valueNav != null)
            {
                uint parsedResult;
                if (!uint.TryParse(valueNav.Value, out parsedResult))
                {
                    return null;
                }

                result = parsedResult;
            }

            return result;
        }

        internal static long? GetOptNavValueAsLong(
            XPathNavigator nav,
            string elementName)
        {
            long? result = null;

            XPathNavigator valueNav =
                nav.SelectSingleNode(elementName);
            if (valueNav != null)
            {
                result = valueNav.ValueAsLong;
            }

            return result;
        }

        internal static double? GetOptNavValueAsDouble(
            XPathNavigator nav,
            string elementName)
        {
            double? result = null;

            XPathNavigator valueNav =
                nav.SelectSingleNode(elementName);
            if (valueNav != null)
            {
                result = valueNav.ValueAsDouble;
            }

            return result;
        }

        internal static string GetOptNavValue(
            XPathNavigator nav,
            string elementName)
        {
            string result = null;

            XPathNavigator valueNav =
                nav.SelectSingleNode(elementName);
            if (valueNav != null)
            {
                result = valueNav.Value;
            }

            return result;
        }

        internal static Uri GetOptNavValueAsUri(
            XPathNavigator nav,
            string elementName)
        {
            Uri result = null;

            XPathNavigator valueNav =
                nav.SelectSingleNode(elementName);
            if (valueNav != null)
            {
                try
                {
                    result = new Uri(valueNav.Value, UriKind.RelativeOrAbsolute);
                }
                catch (UriFormatException)
                {
                }
            }

            return result;
        }

        internal static DataType GetOptNavValue<DataType>(
            XPathNavigator nav,
            string elementName)
            where DataType : ItemBase, new()
        {
            DataType result = default(DataType);

            XPathNavigator valueNav =
                nav.SelectSingleNode(elementName);
            if (valueNav != null)
            {
                result = new DataType();
                result.ParseXml(valueNav);
            }

            return result;
        }

        #endregion optionals

        /// <summary>
        /// Parse an attribute on the navigator as boolean.
        /// </summary>
        /// <remarks>
        /// The parsed attributed is returned if present, otherwise the
        /// <paramref name="defaultValue"/> is returned.
        /// </remarks>
        /// <param name="navigator">The navigator to use.</param>
        /// <param name="attributeName">The attribute to get.</param>
        /// <param name="defaultValue">The default value.</param>
        internal static bool ParseAttributeAsBoolean(
            XPathNavigator navigator,
            string attributeName,
            bool defaultValue)
        {
            bool result = defaultValue;

            string attributeString =
                navigator.GetAttribute(attributeName, string.Empty);

            if (attributeString != string.Empty)
            {
                try
                {
                    result = XmlConvert.ToBoolean(attributeString);
                }
                catch (FormatException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// Parse an attribute on the navigator as a long integer.
        /// </summary>
        /// <remarks>
        /// The parsed attribute value is returned as a nullable long integer if present,
        /// otherwise the <paramref name="defaultValue"/> is returned.
        /// </remarks>
        /// <param name="navigator">The navigator.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        internal static long? ParseAttributeAsLong(
            XPathNavigator navigator,
            string attributeName,
            long? defaultValue)
        {
            long? result = defaultValue;

            string attributeString =
                navigator.GetAttribute(attributeName, string.Empty);

            if (attributeString != string.Empty)
            {
                try
                {
                    result = XmlConvert.ToInt64(attributeString);
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// Parse an attribute on the navigator as a DateTime.
        /// </summary>
        /// <remarks>
        /// The nullable value argument is returned as a DateTime if present, otherwise the
        /// <paramref name="defaultValue"/> is returned.
        /// </remarks>
        /// <param name="navigator">The navigator.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        internal static DateTime ParseAttributeAsDateTime(
            XPathNavigator navigator,
            string attributeName,
            DateTime defaultValue)
        {
            DateTime result = defaultValue;

            string attributeString =
                navigator.GetAttribute(attributeName, string.Empty);

            if (attributeString != string.Empty)
            {
                DateTime.TryParse(
                    attributeString,
                    DateTimeFormatInfo.InvariantInfo,
                    DateTimeStyles.AdjustToUniversal,
                    out result);
            }

            return result;
        }

        /// <summary>
        /// Parse an attribute on the navigator as an enum.
        /// </summary>
        /// <remarks>
        /// The attribute is returned as the enum type T if present, otherwise is the
        /// <paramref name="defaultValue"/> is returned.
        /// </remarks>
        /// <param name="navigator">The navigator.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="defaultValue">The default value</param>
        internal static T ParseAttributeAsEnum<T>(
            XPathNavigator navigator,
            string attributeName,
            T defaultValue)
        {
            T result = defaultValue;

            string attributeString =
                navigator.GetAttribute(attributeName, string.Empty);

            if (attributeString != string.Empty)
            {
                try
                {
                    result =
                        (T)Enum.Parse(
                            typeof(T),
                            attributeString);
                }
                catch (ArgumentException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// Parse into a ItemBase-derived collection
        /// </summary>
        /// <typeparam name="T">The ItemBase-derived class</typeparam>
        /// <param name="nav">The navigator</param>
        /// <param name="itemPath">The xpath for the item</param>
        /// <returns>Returns a collection of parsed data types</returns>
        internal static Collection<T> ParseXmlCollection<T>(XPathNavigator nav, string itemPath)
            where T : ItemBase, new()
        {
            XPathNodeIterator itemIterator = nav.Select(itemPath);

            Collection<T> items = new Collection<T>();

            foreach (XPathNavigator itemNav in itemIterator)
            {
                T t = new T();
                t.ParseXml(itemNav);
                items.Add(t);
            }

            return items;
        }
    }
}
