// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Provides low-level access to the HealthVault message operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformInformation.Current to a derived class to intercept all message calls.
    /// </remarks>
    internal class HealthVaultPlatformInformation
    {
        internal static HealthVaultPlatformInformation Current { get; private set; } = new HealthVaultPlatformInformation();

        #region GetServiceDefinitionAsync

        /// <summary>
        /// Gets information about the HealthVault service.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service. This
        /// includes:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="ServiceInfo"/> instance that contains the service version, SDK
        /// assemblies versions and URLs, method information, and so on.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public virtual async Task<ServiceInfo> GetServiceDefinitionAsync(IHealthVaultConnection connection)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.ConnectionNull);
            return await GetServiceDefinitionAsync(connection, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets information about the HealthVault service only if it has been updated since
        /// the specified update time.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation.</param>
        ///
        /// <param name="lastUpdatedTime">
        /// The time of the last update to an existing cached copy of <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service, if there were updates
        /// since the specified <paramref name="lastUpdatedTime"/>.  If there were no updates
        /// the method returns <b>null</b>.
        /// This includes:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// </remarks>
        ///
        /// <returns>
        /// If there were updates to the service information since the specified <paramref name="lastUpdatedTime"/>,
        /// a <see cref="ServiceInfo"/> instance that contains the service version, SDK
        /// assemblies versions and URLs, method information, and so on.  Otherwise, if there were no updates,
        /// returns <b>null</b>.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public virtual async Task<ServiceInfo> GetServiceDefinitionAsync(IHealthVaultConnection connection, DateTime lastUpdatedTime)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.ConnectionNull);

            StringBuilder requestBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(requestBuilder, SDKHelper.XmlUnicodeWriterSettings))
            {
                writer.WriteElementString(
                    "updated-date",
                    SDKHelper.XmlFromUtcDateTime(lastUpdatedTime.ToUniversalTime()));

                writer.Flush();
            }

            string requestParams = requestBuilder.ToString();
            return await GetServiceDefinitionAsync(connection, requestParams).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation.</param>
        ///
        /// <param name="responseSections">
        /// A bitmask of one or more <see cref="ServiceInfoSections"/> which specify the
        /// categories of information to be populated in the <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service. Depending on the specified
        /// <paramref name="responseSections"/>, this will include some or all of:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        ///
        /// Retrieving only the sections you need will give a faster response time than
        /// downloading the full response.
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="ServiceInfo"/> instance that contains some or all of the service version,
        /// SDK assemblies versions and URLs, method information, and so on, depending on which
        /// information categories were specified.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public virtual async Task<ServiceInfo> GetServiceDefinitionAsync(
            IHealthVaultConnection connection,
            ServiceInfoSections responseSections)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.ConnectionNull);
            string requestParams = CreateServiceDefinitionRequestParameters(responseSections, null);
            return await GetServiceDefinitionAsync(connection, requestParams).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories if the requested information has been updated since the specified
        /// update time.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation.</param>
        ///
        /// <param name="responseSections">
        /// A bitmask of one or more <see cref="ServiceInfoSections"/> which specify the
        /// categories of information to be populated in the <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <param name="lastUpdatedTime">
        /// The time of the last update to an existing cached copy of <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service, if there were updates
        /// since the specified <paramref name="lastUpdatedTime"/>.  If there were no updates
        /// the method returns <b>null</b>.
        /// Depending on the specified
        /// <paramref name="responseSections"/>, this will include some or all of:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        ///
        /// Retrieving only the sections you need will give a faster response time than
        /// downloading the full response.
        /// </remarks>
        ///
        /// <returns>
        /// If there were updates to the service information since the specified <paramref name="lastUpdatedTime"/>,
        /// a <see cref="ServiceInfo"/> instance that contains some or all of the service version,
        /// SDK  assemblies versions and URLs, method information, and so on, depending on which
        /// information categories were specified.  Otherwise, if there were no updates, returns
        /// <b>null</b>.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public virtual async Task<ServiceInfo> GetServiceDefinitionAsync(
            IHealthVaultConnection connection,
            ServiceInfoSections responseSections,
            DateTime lastUpdatedTime)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.ConnectionNull);
            string requestParams = CreateServiceDefinitionRequestParameters(responseSections, lastUpdatedTime);
            return await GetServiceDefinitionAsync(connection, requestParams).ConfigureAwait(false);
        }

        private static string CreateServiceDefinitionRequestParameters(
            ServiceInfoSections responseSections,
            DateTime? lastUpdatedTime)
        {
            StringBuilder requestBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(requestBuilder, SDKHelper.XmlUnicodeWriterSettings))
            {
                if (lastUpdatedTime != null)
                {
                    writer.WriteElementString(
                        "updated-date",
                        SDKHelper.XmlFromUtcDateTime(lastUpdatedTime.Value.ToUniversalTime()));
                }

                writer.WriteStartElement("response-sections");

                if ((responseSections & ServiceInfoSections.Platform) == ServiceInfoSections.Platform)
                {
                    writer.WriteElementString("section", "platform");
                }

                if ((responseSections & ServiceInfoSections.Shell) == ServiceInfoSections.Shell)
                {
                    writer.WriteElementString("section", "shell");
                }

                if ((responseSections & ServiceInfoSections.Topology) == ServiceInfoSections.Topology)
                {
                    writer.WriteElementString("section", "topology");
                }

                if ((responseSections & ServiceInfoSections.XmlOverHttpMethods) == ServiceInfoSections.XmlOverHttpMethods)
                {
                    writer.WriteElementString("section", "xml-over-http-methods");
                }

                if ((responseSections & ServiceInfoSections.MeaningfulUse) == ServiceInfoSections.MeaningfulUse)
                {
                    writer.WriteElementString("section", "meaningful-use");
                }

                writer.WriteEndElement();

                writer.Flush();
            }

            return requestBuilder.ToString();
        }

        private static async Task<ServiceInfo> GetServiceDefinitionAsync(IHealthVaultConnection connection, string parameters)
        {
            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetServiceDefinition, 2, parameters).ConfigureAwait(false);

            if (responseData.InfoNavigator.HasChildren)
            {
                XPathExpression infoPath = SDKHelper.GetInfoXPathExpressionForMethod(
                    responseData.InfoNavigator,
                    "GetServiceDefinition2");

                XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(infoPath);

                return ServiceInfo.CreateServiceInfo(infoNav);
            }

            return null;
        }

        #endregion GetServiceDefinitionAsync

        #region GetHealthRecordItemType

        /// <summary>
        /// Removes all item type definitions from the client-side cache.
        /// </summary>
        ///
        public virtual void ClearItemTypeCache()
        {
            lock (sectionCache)
            {
                sectionCache.Clear();
            }
        }

        private readonly Dictionary<string, Dictionary<ThingTypeSections, Dictionary<Guid, ThingTypeDefinition>>>
            sectionCache = new Dictionary<string, Dictionary<ThingTypeSections, Dictionary<Guid, ThingTypeDefinition>>>();

        /// <summary>
        /// Gets the definitions for one or more thing type definitions
        /// supported by HealthVault.
        /// </summary>
        ///
        /// <param name="typeIds">
        /// A collection of health item type IDs whose details are being requested. Null
        /// indicates that all health item types should be returned.
        /// </param>
        ///
        /// <param name="sections">
        /// A collection of ThingTypeSections enumeration values that indicate the type
        /// of details to be returned for the specified health item records(s).
        /// </param>
        ///
        /// <param name="imageTypes">
        /// A collection of strings that identify which health item record images should be
        /// retrieved.
        ///
        /// This requests an image of the specified mime type should be returned. For example,
        /// to request a GIF image, "image/gif" should be specified. For icons, "image/vnd.microsoft.icon"
        /// should be specified. Note, not all health item records will have all image types and
        /// some may not have any images at all.
        ///
        /// If '*' is specified, all image types will be returned.
        /// </param>
        ///
        /// <param name="lastClientRefreshDate">
        /// A <see cref="DateTime"/> instance that specifies the time of the last refresh
        /// made by the client.
        /// </param>
        ///
        /// <param name="connection">
        /// A connection to the HealthVault service.
        /// </param>
        ///
        /// <returns>
        /// The type definitions for the specified types, or empty if the
        /// <paramref name="typeIds"/> parameter does not represent a known unique
        /// type identifier.
        /// </returns>
        ///
        /// <remarks>
        /// This method calls the HealthVault service if the types are not
        /// already in the client-side cache.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="typeIds"/> is <b>null</b> and empty, or
        /// <paramref name="typeIds"/> is <b>null</b> and member in <paramref name="typeIds"/> is
        /// <see cref="System.Guid.Empty"/>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public virtual async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds,
            ThingTypeSections sections,
            IList<string> imageTypes,
            DateTime? lastClientRefreshDate,
            IHealthVaultConnection connection)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.TypeManagerConnectionNull);

            if ((typeIds != null && typeIds.Contains(Guid.Empty)) ||
                (typeIds != null && typeIds.Count == 0))
            {
                throw new ArgumentException(Resources.TypeIdEmpty, nameof(typeIds));
            }

            if (lastClientRefreshDate != null)
            {
                return await GetHealthRecordItemTypeDefinitionByDateAsync(
                    typeIds,
                    sections,
                    imageTypes,
                    lastClientRefreshDate.Value,
                    connection).ConfigureAwait(false);
            }

            return await GetHealthRecordItemTypeDefinitionNoDateAsync(
                typeIds,
                sections,
                imageTypes,
                connection).ConfigureAwait(false);
        }

        private async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionNoDateAsync(
            IList<Guid> typeIds,
            ThingTypeSections sections,
            IList<string> imageTypes,
            IHealthVaultConnection connection)
        {
            StringBuilder requestParameters = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            Dictionary<Guid, ThingTypeDefinition> cachedThingTypes = null;

            string cultureName = CultureInfo.CurrentCulture.Name;
            bool sendRequest = false;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                if ((typeIds != null) && (typeIds.Count > 0))
                {
                    foreach (Guid id in typeIds)
                    {
                        lock (sectionCache)
                        {
                            if (!sectionCache.ContainsKey(cultureName))
                            {
                                sectionCache.Add(
                                    cultureName,
                                    new Dictionary<ThingTypeSections, Dictionary<Guid, ThingTypeDefinition>>());
                            }

                            if (!sectionCache[cultureName].ContainsKey(sections))
                            {
                                sectionCache[cultureName].Add(sections, new Dictionary<Guid, ThingTypeDefinition>());
                            }

                            if (sectionCache[cultureName][sections].ContainsKey(id))
                            {
                                if (cachedThingTypes == null)
                                {
                                    cachedThingTypes =
                                        new Dictionary<Guid, ThingTypeDefinition>();
                                }

                                cachedThingTypes[id] = sectionCache[cultureName][sections][id];
                            }
                            else
                            {
                                sendRequest = true;

                                writer.WriteStartElement("id");

                                writer.WriteString(id.ToString("D"));

                                writer.WriteEndElement();
                            }
                        }
                    }
                }
                else
                {
                    lock (sectionCache)
                    {
                        if (!sectionCache.ContainsKey(cultureName))
                        {
                            sectionCache.Add(
                                cultureName,
                                new Dictionary<ThingTypeSections, Dictionary<Guid, ThingTypeDefinition>>());
                        }

                        if (!sectionCache[cultureName].ContainsKey(sections))
                        {
                            sectionCache[cultureName].Add(sections, new Dictionary<Guid, ThingTypeDefinition>());
                        }
                        else
                        {
                            cachedThingTypes = sectionCache[cultureName][sections];
                        }
                    }

                    sendRequest = true;
                }

                if (!sendRequest)
                {
                    return cachedThingTypes;
                }

                WriteSectionSpecs(writer, sections);

                if ((imageTypes != null) && (imageTypes.Count > 0))
                {
                    foreach (string imageType in imageTypes)
                    {
                        writer.WriteStartElement("image-type");

                        writer.WriteString(imageType);

                        writer.WriteEndElement();
                    }
                }

                writer.Flush();

                HealthServiceResponseData responseData = await connection.ExecuteAsync(
                    HealthVaultMethods.GetThingType,
                    1,
                    requestParameters.ToString()).ConfigureAwait(false);

                Dictionary<Guid, ThingTypeDefinition> result =
                    CreateThingTypesFromResponse(
                        cultureName,
                        responseData,
                        sections,
                        cachedThingTypes);

                lock (sectionCache)
                {
                    foreach (Guid id in result.Keys)
                    {
                        sectionCache[cultureName][sections][id] = result[id];
                    }
                }

                return result;
            }
        }

        private async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionByDateAsync(
            IList<Guid> typeIds,
            ThingTypeSections sections,
            IList<string> imageTypes,
            DateTime lastClientRefreshDate,
            IHealthVaultConnection connection)
        {
            StringBuilder requestParameters = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                if ((typeIds != null) && (typeIds.Count > 0))
                {
                    foreach (Guid id in typeIds)
                    {
                        writer.WriteStartElement("id");

                        writer.WriteString(id.ToString("D"));

                        writer.WriteEndElement();
                    }
                }

                WriteSectionSpecs(writer, sections);

                if ((imageTypes != null) && (imageTypes.Count > 0))
                {
                    foreach (string imageType in imageTypes)
                    {
                        writer.WriteStartElement("image-type");

                        writer.WriteString(imageType);

                        writer.WriteEndElement();
                    }
                }

                writer.WriteElementString(
                    "last-client-refresh",
                    SDKHelper.XmlFromLocalDateTime(lastClientRefreshDate));

                writer.Flush();

                HealthServiceResponseData responseData = await connection.ExecuteAsync(
                    HealthVaultMethods.GetThingType,
                    1,
                    requestParameters.ToString()).ConfigureAwait(false);

                Dictionary<Guid, ThingTypeDefinition> result =
                    CreateThingTypesFromResponse(
                        CultureInfo.CurrentCulture.Name,
                        responseData,
                        sections,
                        null);

                lock (sectionCache)
                {
                    sectionCache[CultureInfo.CurrentCulture.Name][sections] = result;
                }

                return result;
            }
        }

        private static void WriteSectionSpecs(
            XmlWriter writer,
            ThingTypeSections sectionSpecs)
        {
            if ((sectionSpecs & ThingTypeSections.Core) != ThingTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(ThingTypeSections.Core.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & ThingTypeSections.Xsd) != ThingTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(ThingTypeSections.Xsd.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & ThingTypeSections.Columns) != ThingTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(ThingTypeSections.Columns.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & ThingTypeSections.Transforms) != ThingTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(ThingTypeSections.Transforms.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & ThingTypeSections.TransformSource) != ThingTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(ThingTypeSections.TransformSource.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & ThingTypeSections.Versions) != ThingTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(ThingTypeSections.Versions.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & ThingTypeSections.EffectiveDateXPath) != ThingTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(ThingTypeSections.EffectiveDateXPath.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }
        }

        private Dictionary<Guid, ThingTypeDefinition> CreateThingTypesFromResponse(
            string cultureName,
            HealthServiceResponseData response,
            ThingTypeSections sections,
            Dictionary<Guid, ThingTypeDefinition> cachedThingTypes)
        {
            Dictionary<Guid, ThingTypeDefinition> thingTypes;

            if (cachedThingTypes != null && cachedThingTypes.Count > 0)
            {
                thingTypes = new Dictionary<Guid, ThingTypeDefinition>(cachedThingTypes);
            }
            else
            {
                thingTypes = new Dictionary<Guid, ThingTypeDefinition>();
            }

            XPathNodeIterator thingTypesIterator =
                response.InfoNavigator.Select("thing-type");

            lock (sectionCache)
            {
                if (!sectionCache.ContainsKey(cultureName))
                {
                    sectionCache.Add(cultureName, new Dictionary<ThingTypeSections, Dictionary<Guid, ThingTypeDefinition>>());
                }

                if (!sectionCache[cultureName].ContainsKey(sections))
                {
                    sectionCache[cultureName].Add(sections, new Dictionary<Guid, ThingTypeDefinition>());
                }

                foreach (XPathNavigator navigator in thingTypesIterator)
                {
                    ThingTypeDefinition thingType =
                        ThingTypeDefinition.CreateFromXml(navigator);

                    sectionCache[cultureName][sections][thingType.TypeId] = thingType;
                    thingTypes[thingType.TypeId] = thingType;
                }
            }

            return thingTypes;
        }

        #endregion GetHealthRecordItemType

        #region SelectInstance

        /// <summary>
        /// Gets the instance where a HealthVault account should be created
        /// for the specified account location.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to perform the operation.
        /// </param>
        ///
        /// <param name="preferredLocation">
        /// A user's preferred geographical location, used to select the best instance
        /// in which to create a new HealthVault account. If there is a location associated
        /// with the credential that will be used to log into the account, that location
        /// should be used.
        /// </param>
        ///
        /// <remarks>
        /// If no suitable instance can be found, a null value is returned. This can happen,
        /// for example, if the account location is not supported by HealthVault.
        ///
        /// Currently the returned instance IDs all parse to integers, but that is not
        /// guaranteed and should not be relied upon.
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="HealthServiceInstance"/> object represents the selected instance,
        /// or null if no suitable instance exists.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="preferredLocation"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        public virtual async Task<HealthServiceInstance> SelectInstanceAsync(
            IHealthVaultConnection connection,
            Location preferredLocation)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.TypeManagerConnectionNull);
            Validator.ThrowIfArgumentNull(preferredLocation, nameof(preferredLocation), Resources.SelectInstanceLocationRequired);

            StringBuilder requestParameters = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                preferredLocation.WriteXml(writer, "preferred-location");
                writer.Flush();
            }

            HealthServiceResponseData responseData = await connection.ExecuteAsync(
                HealthVaultMethods.SelectInstance,
                1,
                requestParameters.ToString()).ConfigureAwait(false);

            XPathExpression infoPath = SDKHelper.GetInfoXPathExpressionForMethod(
                responseData.InfoNavigator,
                "SelectInstance");
            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(infoPath);

            XPathNavigator instanceNav = infoNav.SelectSingleNode("selected-instance");

            if (instanceNav != null)
            {
                return HealthServiceInstance.CreateInstance(instanceNav);
            }

            return null;
        }

        #endregion
    }
}
