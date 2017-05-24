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
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents information about a health record that is not considered personally
    /// identifiable.
    /// </summary>
    ///
    public class Basic : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Basic"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public Basic()
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
            new Guid("bf516a61-5252-4c28-a979-27f45f62f78d");

        /// <summary>
        /// Populates this <see cref="Basic"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the basic data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a basic node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator basicNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("basic");

            Validator.ThrowInvalidIfNull(basicNav, Resources.BasicUnexpectedNode);

            XPathNavigator genderNav =
                basicNav.SelectSingleNode("gender");

            if (genderNav != null)
            {
                string genderString = genderNav.Value;
                if (string.Equals(
                        genderString,
                        "m",
                        StringComparison.Ordinal))
                {
                    _gender = ItemTypes.Gender.Male;
                }
                else if (
                    string.Equals(
                        genderString,
                        "f",
                        StringComparison.Ordinal))
                {
                    _gender = ItemTypes.Gender.Female;
                }
                else
                {
                    _gender = ItemTypes.Gender.Unknown;
                }
            }

            XPathNavigator birthYearNav =
                basicNav.SelectSingleNode("birthyear");

            if (birthYearNav != null)
            {
                _birthYear = birthYearNav.ValueAsInt;
            }

            XPathNavigator countryNav =
                basicNav.SelectSingleNode("country");

            if (countryNav != null)
            {
                _country = countryNav.Value;
            }

            XPathNavigator postalNav =
                basicNav.SelectSingleNode("postcode");

            if (postalNav != null)
            {
                _postalCode = postalNav.Value;
            }

            XPathNavigator cityNav =
                basicNav.SelectSingleNode("city");

            if (cityNav != null)
            {
                _city = cityNav.Value;
            }

            XPathNavigator stateNav = basicNav.SelectSingleNode("state");
            if (stateNav != null)
            {
                _stateOrProvince = stateNav.Value;
            }

            XPathNavigator dayOfWeekNav =
                basicNav.SelectSingleNode("firstdow");

            if (dayOfWeekNav != null)
            {
                _firstDayOfWeek = (DayOfWeek)(dayOfWeekNav.ValueAsInt - 1);
            }

            XPathNodeIterator languageIterator =
                basicNav.Select("language");

            foreach (XPathNavigator languageNav in languageIterator)
            {
                Language newLanguage = new Language();
                newLanguage.ParseXml(languageNav);

                _languages.Add(newLanguage);
            }
        }

        /// <summary>
        /// Writes the basic data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the basic data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <basic>
            writer.WriteStartElement("basic");

            if (_gender != null)
            {
                if (_gender == ItemTypes.Gender.Male)
                {
                    writer.WriteElementString("gender", "m");
                }
                else if (_gender == ItemTypes.Gender.Female)
                {
                    writer.WriteElementString("gender", "f");
                }
            }

            if (_birthYear != null)
            {
                writer.WriteElementString(
                    "birthyear",
                    ((int)_birthYear).ToString(CultureInfo.InvariantCulture));
            }

            if (_country != null)
            {
                writer.WriteElementString("country", _country);
            }

            if (_postalCode != null)
            {
                writer.WriteElementString("postcode", _postalCode);
            }

            if (_city != null)
            {
                writer.WriteElementString("city", _city);
            }

            if (!string.IsNullOrEmpty(_stateOrProvince))
            {
                writer.WriteElementString("state", _stateOrProvince);
            }

            if (_firstDayOfWeek != null)
            {
                // The DayOfWeek enum starts at 0 whereas the XSD starts at
                // 1.

                writer.WriteElementString(
                    "firstdow",
                    ((int)_firstDayOfWeek + 1).ToString(CultureInfo.InvariantCulture));
            }

            foreach (Language language in _languages)
            {
                language.WriteXml("language", writer);
            }

            // </basic>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the gender of the person.
        /// </summary>
        ///
        /// <value>
        /// The person's gender.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the gender should not be stored.
        /// </remarks>
        ///
        public Gender? Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        private Gender? _gender;

        /// <summary>
        /// Gets or sets the birth year of the person.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the year.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the birth year should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 1000 or greater than
        /// 3000 when setting the value.
        /// </exception>
        ///
        public int? BirthYear
        {
            get { return _birthYear; }

            set
            {
                if (value != null && ((int)value < 1000 || (int)value > 3000))
                {
                    throw new ArgumentOutOfRangeException(nameof(BirthYear), Resources.BasicBirthYearOutOfRange);
                }

                _birthYear = value;
            }
        }

        private int? _birthYear;

        /// <summary>
        /// Gets or sets the country of residence.
        /// </summary>
        ///
        /// <value>
        /// A string representing the two-letter ISO3166-2 code for the
        /// country/region.
        /// </value>
        ///
        public string Country
        {
            get { return _country; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Country");
                _country = value;
            }
        }

        private string _country;

        /// <summary>
        /// Gets or sets the postal code of the country of residence.
        /// </summary>
        ///
        /// <value>
        /// A string representing the postal code.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string PostalCode
        {
            get { return _postalCode; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "PostalCode");
                _postalCode = value;
            }
        }

        private string _postalCode;

        /// <summary>
        /// Gets or sets the city of residence.
        /// </summary>
        ///
        /// <value>
        /// A string representing the city.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string City
        {
            get { return _city; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "City");
                _city = value;
            }
        }

        private string _city;

        /// <summary>
        /// Gets or sets the state or province of residence.
        /// </summary>
        ///
        /// <value>
        /// A string representing the state or province.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the state
        /// should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string StateOrProvince
        {
            get { return _stateOrProvince; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "StateOrProvince");
                _stateOrProvince = value;
            }
        }

        private string _stateOrProvince;

        /// <summary>
        /// Gets or sets the preferred first day of the week.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="DayOfWeek"/> instance representing the day.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the first day of the week
        /// should not be stored.
        /// </remarks>
        ///
        public DayOfWeek? FirstDayOfWeek
        {
            get { return _firstDayOfWeek; }
            set { _firstDayOfWeek = value; }
        }

        private DayOfWeek? _firstDayOfWeek;

        /// <summary>
        /// Gets the language(s) the person speaks.
        /// </summary>
        ///
        /// <value>
        /// A list of the languages.
        /// </value>
        ///
        public IList<Language> Languages => _languages;

        private readonly List<Language> _languages = new List<Language>();

        /// <summary>
        /// Gets a string representation of the basic item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the basic item.
        /// </returns>
        ///
        public override string ToString()
        {
            if (Gender != null || BirthYear != null)
            {
                if (Gender != null && BirthYear != null)
                {
                    string localizedGenderString = ResourceUtilities.ResourceManager.GetString(Gender.ToString());
                    return string.Format(
                        Resources.BasicToStringFormatGenderAndBirthYear,
                        localizedGenderString,
                        BirthYear);
                }

                if (BirthYear != null)
                {
                    return string.Format(
                        Resources.BasicToStringFormatBirthYear,
                        BirthYear);
                }

                return ResourceUtilities.ResourceManager.GetString(Gender.ToString());
            }

            if (PostalCode != null || Country != null)
            {
                if (PostalCode != null && Country != null)
                {
                    return string.Format(
                        Resources.BasicToStringFormatPostalCodeAndCountry,
                        PostalCode,
                        Country);
                }

                if (Country != null)
                {
                    return Country;
                }

                return PostalCode;
            }

            if (CommonData.Note != null)
            {
                return CommonData.Note;
            }

            return Resources.BasicToStringSeeDetails;
        }
    }
}
