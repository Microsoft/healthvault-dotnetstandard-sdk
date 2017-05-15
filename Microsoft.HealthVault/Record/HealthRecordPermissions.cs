// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.ObjectModel;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Record
{
    /// <summary>
    /// Provides the permission information that the current
    /// authenticated person has for the record when using the current application.
    /// </summary>
    ///
    internal class HealthRecordPermissions
    {
        /// <summary>
        /// Constructor for HealthRecordPermissions.
        /// </summary>
        public HealthRecordPermissions()
        {
            ItemTypePermissions = new Collection<ThingTypePermission>();
        }

        /// <summary>
        /// A collection of <see cref="ThingTypePermission" />(s) describing
        /// the permissions for current person record in the context of the application.
        /// </summary>
        public Collection<ThingTypePermission> ItemTypePermissions { get; private set; }

        /// <summary>
        /// Gets or sets whether the current record has opted in for
        /// Meaningful Use reporting.
        /// </summary>
        ///
        /// <remarks>
        /// If set to true, the current record has explicitly opted into Meaningful Use reporting.
        /// If set to false, the current record has explicitly opted out of Meaningful Use reporting.
        /// If no value, the current record has not explicitly opted in or out of Meaningful Use reporting.
        /// </remarks>
        public bool? MeaningfulUseOptIn { get; set; }
    }
}
