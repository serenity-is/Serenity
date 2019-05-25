using System;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Abstraction for ConfigurationManager to remove dependency on System.Configuration
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the app setting with given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="settingType">Type of the setting.</param>
        /// <returns>Loaded setting.</returns>
        object AppSetting(string key, Type settingType);

        /// <summary>
        /// Gets the connection string with given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Connection string and its provider type as tuple.</returns>
        Tuple<string, string> ConnectionString(string key);
    }
}