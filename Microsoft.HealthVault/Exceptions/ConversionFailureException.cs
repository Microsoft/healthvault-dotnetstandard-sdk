// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when a failure occurs during a
    /// conversion operation.
    /// </summary>
    ///
    public class ConversionFailureException : HealthVaultException
    {
        /// <summary>
        /// Creates an instance of the <see cref="ConversionFailureException"/>
        /// class with the specified message.
        /// </summary>
        ///
        /// <param name="message">
        /// The exception message.
        /// </param>
        ///
        public ConversionFailureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ConversionFailureException"/>
        /// class with the specified message and inner exception.
        /// </summary>
        ///
        /// <param name="message">
        /// The exception message.
        /// </param>
        ///
        /// <param name="innerException">
        /// The exception that occurred during the conversion operation.
        /// </param>
        ///
        public ConversionFailureException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
