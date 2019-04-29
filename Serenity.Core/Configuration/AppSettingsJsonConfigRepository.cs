namespace Serenity.Configuration
{
    using Serenity;
    using Serenity.Abstractions;
    using Serenity.ComponentModel;
    using System;
    using System.Reflection;

    /// <summary>
    /// Implementation of IConfigRepository which reads its settings
    /// from appSettings section in web.config (ASP.NET MVC), or "AppSettings"
    /// subkey in appsettings.json (.NET Core).
    /// </summary>
    /// <seealso cref="Serenity.Abstractions.IConfigurationRepository" />
    public class AppSettingsJsonConfigRepository : IConfigurationRepository
    {
        /// <summary>
        /// Saves configuration for the specified setting type.
        /// Not implemented for this provider.
        /// </summary>
        /// <param name="settingType">Setting type</param>
        /// <param name="value">Setting value</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Save(Type settingType, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads configuration for the specified setting type.
        /// Returns an object instance, even if setting is not found.
        /// </summary>
        /// <param name="settingType">Setting type</param>
        /// <returns>Loaded setting</returns>
        public object Load(Type settingType)
        {
            var keyAttr = settingType.GetCustomAttribute<SettingKeyAttribute>();
            var key = keyAttr == null ? settingType.Name : keyAttr.Value;

            return LocalCache.Get("ApplicationSetting:" + settingType.FullName, TimeSpan.Zero, delegate ()
            {
                var configuration = Dependency.TryResolve<IConfigurationManager>();
                if (configuration == null)
                    return Activator.CreateInstance(settingType);

                return configuration.AppSetting(key, settingType) ?? 
                    Activator.CreateInstance(settingType);
            });
        }
    }
}