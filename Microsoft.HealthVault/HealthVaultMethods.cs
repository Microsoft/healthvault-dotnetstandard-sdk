// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault
{
    /// <summary>
    /// The public HealthVault methods that are available for applications to call.
    /// </summary>
    /// <remarks>The numeric value is not important. These values don't map directly to the
    /// platform method enum values. The enum names are directly used as method names when
    /// calling HealthVault, so do not rename them.</remarks>
    public enum HealthVaultMethods
    {
        /// <summary>
        /// Pre-allocates a DOPU package id.
        /// </summary>
        AllocatePackageId,

        /// <summary>
        /// Creates a new alternate id for the record and person.
        /// </summary>
        AssociateAlternateId,

        /// <summary>
        /// Begin to stream binary data for a ThingBase.
        /// </summary>
        BeginPutBlob,

        /// <summary>
        /// Begin to stream binary data for a DOPU package.
        /// </summary>
        BeginPutConnectPackageBlob,

        /// <summary>
        /// Creates an application session token for use with the HealthVault service.
        /// </summary>
        [AnonymousMethod]
        CreateAuthenticatedSessionToken,

        /// <summary>
        /// Create a new DOPU package.
        /// </summary>
        CreateConnectPackage,

        /// <summary>
        /// Create a new Connect Request.
        /// </summary>
        CreateConnectRequest,

        /// <summary>
        /// Delete a DOPU package which has not yet been picked up.
        /// </summary>
        DeletePendingConnectPackage,

        /// <summary>
        /// Delete a connect request which has not yet been accepted.
        /// </summary>
        DeletePendingConnectRequest,

        /// <summary>
        /// Remove an alternate id for the record and person.
        /// </summary>
        DisassociateAlternateId,

        /// <summary>
        /// Get an alternate ids for the record and person.
        /// </summary>
        GetAlternateIds,

        /// <summary>
        /// Gets information about the registered application including name, description,
        /// authorization rules, and callback url.
        /// </summary>
        ///
        GetApplicationInfo,

        /// <summary>
        /// Saves application specific information for the logged in user.
        /// </summary>
        ///
        GetApplicationSettings,

        /// <summary>
        /// Get a list of accepted connect requests.
        /// </summary>
        GetAuthorizedConnectRequests,

        /// <summary>
        /// Get all people authorized for an application.
        /// </summary>
        GetAuthorizedPeople,

        /// <summary>
        /// Gets all the records that the user has authorized the application use.
        /// </summary>
        ///
        GetAuthorizedRecords,

        /// <summary>
        /// Get a list of event subscriptions for the application.
        /// </summary>
        GetEventSubscriptions,

        /// <summary>
        /// Get Meaningful Use Timley Access Report.
        /// </summary>
        GetMeaningfulUseTimelyAccessReport,

        /// <summary>
        /// Get Meaningful Use VDT Report.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        GetMeaningfulUseVDTReport,

        /// <summary>
        /// Gets information about the logged in user.
        /// </summary>
        ///
        GetPersonInfo,

        /// <summary>
        /// Gets generic service information about the HealthVault service.
        /// </summary>
        [AnonymousMethod]
        GetServiceDefinition,

        /// <summary>
        /// Gets data from a HealthVault record.
        /// </summary>
        ///
        GetThings,

        /// <summary>
        /// Gets information, including schemas, about the data types that can be stored in a
        /// health record.
        /// </summary>
        ///
        GetThingType,

        /// <summary>
        /// Get a list of updated records for the application.
        /// </summary>
        GetUpdatedRecordsForApplication,

        /// <summary>
        /// Get valid group memberships for the record.
        /// </summary>
        GetValidGroupMembership,

        /// <summary>
        /// Gets information about clinical and other vocabularies that HealthVault supports.
        /// </summary>
        ///
        GetVocabulary,

        /// <summary>
        /// Create a new application instance information from a master app id.  First step in SODA authentication.
        /// </summary>
        [AnonymousMethod]
        NewApplicationCreationInfo,

        /// <summary>
        /// Generate a new signup code.
        /// </summary>
        NewSignupCode,

        /// <summary>
        /// Adds or updates data in a health record.
        /// </summary>
        ///
        PutThings,

        /// <summary>
        /// Gets the permissions that the application and user have to a health record.
        /// </summary>
        ///
        QueryPermissions,

        /// <summary>
        /// Remove the application's record authorization.
        /// </summary>
        RemoveApplicationRecordAuthorization,

        /// <summary>
        /// Removes data from a health record.
        /// </summary>
        ///
        RemoveThings,

        /// <summary>
        /// Search a specific vocabulary and retrieve the matching vocabulary items.
        /// </summary>
        SearchVocabulary,

        /// <summary>
        /// Get the instance where a HealthVault account should be created for the specified account location.
        /// </summary>
        SelectInstance,

        /// <summary>
        /// Sends an SMTP message on behalf of the logged in user.
        /// </summary>
        ///
        SendInsecureMessage,

        /// <summary>
        /// Sends an SMTP message on behalf of the application.
        /// </summary>
        ///
        SendInsecureMessageFromApplication,

        /// <summary>
        /// Sets application specific data for the user.
        /// </summary>
        ///
        SetApplicationSettings,

        /// <summary>
        /// Subscribe to an event.
        /// </summary>
        SubscribeToEvent,

        /// <summary>
        /// Remove a subscription.
        /// </summary>
        UnsubscribeToEvent,

        /// <summary>
        /// Update a subscription.
        /// </summary>
        UpdateEventSubscription,

        /// <summary>
        /// Update DOPU packages external id.
        /// </summary>
        UpdateExternalId
    }
}
