﻿using System;

namespace Microsoft.HealthVault.Client
{
    public class HealthVaultConnectionFactory
    {
        /// <summary>
        /// Gets the current factory instance.
        /// </summary>
        public static IHealthVaultConnectionFactory Current
        {
            // code change
            get
            {
                throw new NotImplementedException(ClientResources.BaitWithoutSwitchError);
            }
        }
    }
}
