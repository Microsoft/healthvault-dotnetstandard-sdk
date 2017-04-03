﻿using System;
using System.Collections.Specialized;

namespace Microsoft.HealthVault.Web.Extensions
{
    internal static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Retrieves the specified setting for GUIDs.
        /// </summary>
        /// <param name="key">A string specifying the name of the setting.</param>
        /// <param name="collection">The NameValueCollection</param>
        /// <returns>
        /// The GUID of the setting.
        /// </returns>
        public static Guid GetGuid(this NameValueCollection collection, string key)
        {
            Guid result = Guid.Empty;
            string resultString = collection[key];
            if (!string.IsNullOrEmpty(resultString))
            {
                // Let the FormatException propagate out.
                result = new Guid(resultString);
            }

            return result;
        }

        /// <summary>
        /// Gets the URL value matching the configuration key.
        /// </summary>
        /// <param name="collection">The NameValueCollection</param>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="appendSlash">If set to true, append a '/' character at the end of URL.</param>
        /// <returns>URL value from configuration if exists, null if not found.</returns>
        public static Uri GetUrl(this NameValueCollection collection, string key, bool appendSlash)
        {
            string resultString = collection[key];
            if (string.IsNullOrEmpty(resultString))
            {
                return null;
            }
            else
            {
                if (appendSlash)
                {
                    return
                        new Uri(resultString.EndsWith("/", StringComparison.Ordinal)
                            ? resultString
                            : (resultString + "/"));
                }
                else
                {
                    return new Uri(resultString);
                }
            }
        }

        /// <summary>
        /// Retrieves the specified item and attempts to convert it to a <see cref="{T}"/>.
        /// </summary>
        /// <typeparam name="T">The expected Type of the stored value.</typeparam>
        /// <param name="collection">The collection from which to retrieve the value.</param>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">The value to return if the key was not found.</param>
        /// <returns>The value from the config, if it exists, or a default value.</returns>
        public static T GetTypedValue<T>(this NameValueCollection collection, string key, T defaultValue = default(T)) where T : IConvertible
        {
            string resultString = collection[key];

            if (string.IsNullOrEmpty(resultString))
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(resultString, typeof(T));
        }

        /// <summary>
        /// Retrieves the specified item and attempts to convert it to a <see cref="{T}"/>.
        /// </summary>
        /// <typeparam name="T">The expected Type of the stored value.</typeparam>
        /// <param name="collection">The collection from which to retrieve the value.</param>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">The value to return if the key was not found or was invalid.</param>
        /// <param name="isValueValid">A function that returns <c>true</c> if a value is valid.</param>
        /// <returns>The value from the config, if it exists, or a default value.</returns>
        public static T GetValidTypedValue<T>(this NameValueCollection collection, string key, T defaultValue, Func<T, bool> isValueValid) where T : IConvertible
        {
            var item = collection.GetTypedValue(key, defaultValue);
            return isValueValid(item) ? item : defaultValue;
        }

        /// <summary>
        /// Retrieves the specified item as an <see cref="int"/> for minutes and converts to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="collection">The <see cref="NameValueCollection"/>.</param>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">The value to return if the key was not found.</param>
        /// <returns>The value from the config, null if it was -1, or a default value if not found.</returns>
        public static TimeSpan? GetTimeSpanFromMinutes(this NameValueCollection collection, string key, TimeSpan? defaultValue)
        {
            return GetTimeSpanFromInt(collection, key, defaultValue, num => TimeSpan.FromMinutes(num));
        }

        /// <summary>
        /// Retrieves the specified item as an <see cref="int"/> for seconds and converts to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="collection">The <see cref="NameValueCollection"/>.</param>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">The value to return if the key was not found.</param>
        /// <returns>The value from the config, null if it was -1, or a default value if not found.</returns>
        public static TimeSpan? GetTimeSpanFromSeconds(this NameValueCollection collection, string key, TimeSpan? defaultValue)
        {
            return GetTimeSpanFromInt(collection, key, defaultValue, num => TimeSpan.FromSeconds(num));
        }

        private static TimeSpan? GetTimeSpanFromInt(NameValueCollection collection, string key, TimeSpan? defaultValue, Func<int, TimeSpan> converter)
        {
            string resultString = collection[key];

            if (int.TryParse(resultString, out int result))
            {
                if (result == -1)
                {
                    return null;
                }

                return converter(result);
            }

            return defaultValue;
        }
    }
}
