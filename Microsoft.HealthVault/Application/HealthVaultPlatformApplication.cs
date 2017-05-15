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
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Application
{
    /// <summary>
    /// Provides low-level access to the HealthVault message operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformApplication.Current to a derived class to intercept all message calls.
    /// </remarks>
    internal class HealthVaultPlatformApplication
    {
        internal static HealthVaultPlatformApplication Current { get; private set; } = new HealthVaultPlatformApplication();

        #region GetAuthorizedPeople

        /// <summary>
        /// Gets information about people authorized for an application.
        /// </summary>
        ///
        /// <remarks>
        /// The returned IEnumerable iterator will access the HealthVault service
        /// across the network. See <see cref="GetAuthorizedPeopleSettings"/> for applicable
        /// settings.
        /// </remarks>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application-level. </param>
        ///
        /// <param name="settings">
        /// The <see cref="GetAuthorizedPeopleSettings" /> object used to configure the
        /// IEnumerable iterator returned by this method.
        /// </param>
        ///
        /// <returns>
        /// An IEnumerable iterator of <see cref="PersonInfo"/> objects representing
        /// people authorized for the application.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. The retrieval can be retried from the
        /// current position by calling this method again and using the last successfully
        /// retrieved person Id for <see cref="GetAuthorizedPeopleSettings.StartingPersonId"/>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null.
        /// </exception>
        ///
        public virtual IEnumerable<Task<PersonInfo>> GetAuthorizedPeopleAsync(
            IHealthVaultConnection connection,
            GetAuthorizedPeopleSettings settings)
        {
            Validator.ThrowIfArgumentNull(settings, nameof(settings), Resources.GetAuthorizedPeopleSettingsNull);

            bool moreResults = true;
            Guid cursor = settings.StartingPersonId;
            DateTime authCreatedSinceDate = settings.AuthorizationsCreatedSince;
            int batchSize = settings.BatchSize;

            while (moreResults)
            {
                Collection<PersonInfo> personInfos = null;

                // For the first one in the batch we need to go out and get it, then return the first item
                yield return Task.Run(async () =>
                {
                    GetAuthorizedPeopleResult getPeopleResult = await GetAuthorizedPeopleAsync(
                        connection,
                        cursor,
                        authCreatedSinceDate,
                        batchSize).ConfigureAwait(false);

                    personInfos = getPeopleResult.People;
                    moreResults = getPeopleResult.MoreResults;

                    if (personInfos.Count > 0)
                    {
                        cursor = personInfos[personInfos.Count - 1].PersonId;
                    }

                    return personInfos[0];
                });

                // For the rest in the batch, we can immediately return a result.
                for (int i = 1; i < personInfos.Count; i++)
                {
                    yield return Task.FromResult(personInfos[i]);
                }
            }
        }

        /// <summary>
        /// Ensures all the authorized people for the application are returned
        /// </summary>
        ///
        /// <param name="connection">Connection</param>
        /// <param name="settings">Settings</param>
        /// <returns>List of persons</returns>
        public virtual async Task<IList<PersonInfo>> EnsureGetAuthorizedPeopleAsync(
            IHealthVaultConnection connection,
            GetAuthorizedPeopleSettings settings)
        {
            var peopleTasks = GetAuthorizedPeopleAsync(connection, settings);
            var people = new List<PersonInfo>();

            foreach (var personTask in peopleTasks)
            {
                await personTask;
                people.Add(personTask.Result);
            }

            return people;
        }

        internal static async Task<GetAuthorizedPeopleResult> GetAuthorizedPeopleAsync(
            IHealthVaultConnection connection,
            Guid personIdCursor,
            DateTime authCreatedSinceDate,
            int numResults)
        {
            if (numResults < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numResults), Resources.GetAuthorizedPeopleNumResultsNegative);
            }

            StringBuilder parameters = new StringBuilder(256);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(parameters, settings))
            {
                writer.WriteStartElement("parameters");

                if (personIdCursor != Guid.Empty)
                {
                    writer.WriteElementString("person-id-cursor", personIdCursor.ToString());
                }

                if (authCreatedSinceDate != DateTime.MinValue)
                {
                    writer.WriteElementString(
                        "authorizations-created-since",
                        SDKHelper.XmlFromDateTime(authCreatedSinceDate));
                }

                if (numResults != 0)
                {
                    writer.WriteElementString("num-results", numResults.ToString(CultureInfo.InvariantCulture));
                }

                writer.WriteEndElement(); // parameters
                writer.Flush();
            }

            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetAuthorizedPeople, 1, parameters.ToString()).ConfigureAwait(false);

            Collection<PersonInfo> personInfos = new Collection<PersonInfo>();

            XPathExpression navExp =
                 SDKHelper.GetInfoXPathExpressionForMethod(
                     responseData.InfoNavigator, "GetAuthorizedPeople");
            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(navExp);
            XPathNavigator nav = infoNav.SelectSingleNode("response-results/person-info");

            if (nav != null)
            {
                do
                {
                    PersonInfo personInfo = PersonInfo.CreateFromXml(nav);
                    personInfos.Add(personInfo);
                }
                while (nav.MoveToNext("person-info", string.Empty));

                nav.MoveToNext();
            }
            else
            {
                nav = infoNav.SelectSingleNode("response-results/more-results");
            }

            return new GetAuthorizedPeopleResult(personInfos, nav.ValueAsBoolean);
        }

        #endregion GetAuthorizedPeople

        #region GetApplicationInfo

        /// <summary>
        /// Gets the application configuration information for the calling application.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application level. </param>
        ///
        /// <param name="allLanguages">
        /// A boolean value indicating whether the localized values all languages should be
        /// returned, just one language. This affects all properties which can have multiple
        /// localized values, including <see cref="ApplicationInfo.CultureSpecificNames"/>,
        /// <see cref="ApplicationInfo.CultureSpecificDescriptions"/>,
        /// <see cref="ApplicationInfo.CultureSpecificAuthorizationReasons"/>,
        /// <see cref="ApplicationInfo.LargeLogo"/>,
        /// <see cref="ApplicationInfo.SmallLogo"/>,
        /// <see cref="ApplicationInfo.PrivacyStatement"/>,
        /// <see cref="ApplicationInfo.TermsOfUse"/>,
        /// and <see cref="ApplicationInfo.DtcSuccessMessage"/>
        /// </param>
        ///
        /// <returns>
        /// An ApplicationInfo object for the calling application.
        /// </returns>
        ///
        /// <remarks>
        /// This method always calls the HealthVault service to get the latest
        /// information. It returns installation configuration about the calling
        /// application.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public virtual async Task<ApplicationInfo> GetApplicationInfoAsync(
            IHealthVaultConnection connection,
            bool allLanguages)
        {
            string parameters = allLanguages ? "<all-languages>true</all-languages>" : null;

            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetApplicationInfo, 2, parameters).ConfigureAwait(false);

            XPathExpression xPathExpression = SDKHelper.GetInfoXPathExpressionForMethod(
                    responseData.InfoNavigator, "GetApplicationInfo");

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(xPathExpression);

            XPathNavigator appInfoNav = infoNav.SelectSingleNode("application");

            ApplicationInfo appInfo = null;
            if (appInfoNav != null)
            {
                appInfo = ApplicationInfo.CreateFromInfoXml(appInfoNav);
            }

            return appInfo;
        }

        #endregion

        #region GetUpdatedRecordsForApplication

        /// <summary>
        /// Gets a list of health record IDs for the current application,
        /// that optionally have been updated since a specified date.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application level. </param>
        ///
        /// <param name="updatedDate">
        /// Date that is used to filter health record IDs according to whether or not they have
        /// been updated since the specified date.
        /// </param>
        ///
        /// <returns>
        /// List of health record IDs filtered by any specified input parameters.
        /// </returns>
        ///
        public virtual async Task<IList<Guid>> GetUpdatedRecordsForApplicationAsync(
            IHealthVaultConnection connection,
            DateTime? updatedDate)
        {
            string parameters = GetUpdateDateParameters(updatedDate);

            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetUpdatedRecordsForApplication, 2, parameters).ConfigureAwait(false);
            return ParseGetUpdatedRecordsForApplicationResponseRecordIds(responseData);
        }

        /// <summary>
        /// Gets a list of <see cref="HealthRecordUpdateInfo"/> objects for the current application,
        /// that optionally have been updated since a specified date.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application level. </param>
        ///
        /// <param name="updatedDate">
        /// Date that is used to filter health record IDs according to whether or not they have
        /// been updated since the specified date.
        /// </param>
        ///
        /// <returns>
        /// List of <see cref="HealthRecordUpdateInfo"/> objects filtered by any specified input parameters.
        /// </returns>
        ///
        public virtual async Task<IList<HealthRecordUpdateInfo>> GetUpdatedRecordInfoForApplicationAsync(
            IHealthVaultConnection connection,
            DateTime? updatedDate)
        {
            string parameters = GetUpdateDateParameters(updatedDate);

            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetUpdatedRecordsForApplication, 2, parameters).ConfigureAwait(false);
            return ParseGetUpdatedRecordsForApplicationResponseHealthRecordUpdateInfos(responseData);
        }

        private string GetUpdateDateParameters(DateTime? updateDate)
        {
            return updateDate == null ? null : $"<update-date>{SDKHelper.XmlFromDateTime(updateDate.Value)}</update-date>";
        }

        private static IList<Guid> ParseGetUpdatedRecordsForApplicationResponseRecordIds(HealthServiceResponseData response)
        {
            IList<Guid> recordIds = new List<Guid>();

            XPathNodeIterator iterator = response.InfoNavigator.SelectSingleNode("updated-records").Select("updated-record");
            foreach (XPathNavigator navigator in iterator)
            {
                recordIds.Add(ParseHealthRecordUpdateInfo(navigator).RecordId);
            }

            return recordIds;
        }

        private static IList<HealthRecordUpdateInfo> ParseGetUpdatedRecordsForApplicationResponseHealthRecordUpdateInfos(
            HealthServiceResponseData response)
        {
            IList<HealthRecordUpdateInfo> healthRecordPersonUpdateInfos = new List<HealthRecordUpdateInfo>();

            XPathNodeIterator iterator = response.InfoNavigator.SelectSingleNode("updated-records").Select("updated-record");
            foreach (XPathNavigator navigator in iterator)
            {
                healthRecordPersonUpdateInfos.Add(ParseHealthRecordUpdateInfo(navigator));
            }

            return healthRecordPersonUpdateInfos;
        }

        private static HealthRecordUpdateInfo ParseHealthRecordUpdateInfo(XPathNavigator navigator)
        {
            Guid recordId = new Guid(navigator.SelectSingleNode("record-id").Value);
            Guid personId = new Guid(navigator.SelectSingleNode("person-id").Value);
            DateTime lastUpdateDate = DateTime.Parse(
                                        navigator.SelectSingleNode("update-date").Value,
                                        DateTimeFormatInfo.InvariantInfo,
                                        DateTimeStyles.AdjustToUniversal);
            long latestOperationSequenceNumber = navigator.SelectSingleNode("latest-operation-sequence-number").ValueAsLong;
            return new HealthRecordUpdateInfo(recordId, lastUpdateDate, personId, latestOperationSequenceNumber);
        }

        #endregion

        #region NewSignupCode

        /// <summary>
        /// Generates a new signup code that should be passed to HealthVault Shell in order
        /// to create a new user account.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application level. </param>
        ///
        /// <returns>
        /// A signup code that can be used to create an account.
        /// </returns>
        ///
        public virtual async Task<string> NewSignupCodeAsync(IHealthVaultConnection connection)
        {
            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.NewSignupCode, 1).ConfigureAwait(false);

            XPathExpression infoPath = SDKHelper.GetInfoXPathExpressionForMethod(responseData.InfoNavigator, "NewSignupCode");

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(infoPath);
            return infoNav.SelectSingleNode("signup-code").Value;
        }

        #endregion

        internal class GetAuthorizedPeopleResult
        {
            public GetAuthorizedPeopleResult(Collection<PersonInfo> people, bool moreResults)
            {
                People = people;
                MoreResults = moreResults;
            }

            public Collection<PersonInfo> People { get; }

            public bool MoreResults { get; }
        }
    }
}
