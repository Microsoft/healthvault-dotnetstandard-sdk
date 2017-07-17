using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Android.OS;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.Client
{
    internal static class ClientIoc
    {
        internal static void EnsureTypesRegistered()
        {
            // CoreClientIoc will call RegisterPlatformTypes.  Do all our registering there.
            CoreClientIoc.EnsureTypesRegistered(RegisterPlatformTypes);
        }

        private static void RegisterPlatformTypes(DependencyInjectionContainer container)
        {
            AndroidBrowserAuthBroker authBroker = container.Locate<AndroidBrowserAuthBroker>();
            container.Configure(c => c.ExportInstance(authBroker).As<IAndroidBrowserAuthBroker>());
            container.Configure(c => c.ExportInstance(authBroker).As<IBrowserAuthBroker>());

            container.RegisterSingleton<ISecretStore, AndroidSecretStore>();
            container.RegisterSingleton<IEncryptionKeyService, EncryptionKeyService>();
            container.RegisterSingleton<IMessageHandlerFactory, AndroidMessageHandlerFactory>();

            RegisterTelemetryInformation(container);
        }

        /// <summary>
        /// Register Telemetry information for SDK
        /// </summary>
        /// <param name="container">IOC container</param>
        private static void RegisterTelemetryInformation(DependencyInjectionContainer container)
        {
            string androidVersion = Regex.Replace(Build.VERSION.Release, "[^0-9.]", string.Empty);
            if (string.IsNullOrWhiteSpace(androidVersion))
            {
                // Preview SDKs do not have integer Release versions. Identify them uniquely using SDK version and preview SDK version.
                androidVersion = $"{(int)Build.VERSION.SdkInt}.{Build.VERSION.PreviewSdkInt}";
            }

            Version version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;

            var sdkTelemetryInformation = new SdkTelemetryInformation
            {
                Category = HealthVaultConstants.SdkTelemetryInformationCategories.AndroidClient,
                FileVersion = version.ToString(),
                OsInformation = $"Android {androidVersion}"
            };

            container.Configure(c => c.ExportInstance(sdkTelemetryInformation).As<SdkTelemetryInformation>());
        }
    }
}
