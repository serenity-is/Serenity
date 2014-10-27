using System;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Abstraction for configuration system
    /// </summary>
    public interface IConfigurationRepository
    {
        /// <summary>
        /// Loads configuration for the specified setting type.
        /// Returns an object instance, even if setting is not found.
        /// </summary>
        /// <param name="settingType">Setting type</param>
        object Load(Type settingType);
        /// <summary>
        /// Saves configuration for the specified setting type.
        /// </summary>
        /// <param name="settingType">Setting type</param>
        /// <param name="value">Setting value</param>
        void Save(Type settingType, object value);
    }
}