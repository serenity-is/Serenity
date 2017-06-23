
namespace Serenity
{
    using Abstractions;
    using Serenity.ComponentModel;
    using System;

    /// <summary>
    /// Central location to access configuration settings.
    /// </summary>
    public static class Config
    {
        private static string GetSettingScope(Type settingType)
        {
            var scopeAttr = settingType.GetCustomAttributes(typeof(SettingScopeAttribute), true);
            var scope = scopeAttr.IsEmptyOrNull() ? "Application" : ((SettingScopeAttribute)scopeAttr[0]).Value;
            return scope;
        }

        /// <summary>
        /// Returns configuration settings for specified setting type. 
        /// If setting is not found in its storage, returns a new object instance.
        /// </summary>
        /// <param name="settingType">Setting type.</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// IConfigurationRepository for setting scope is not set.</exception>
        public static object Get(Type settingType)
        {
            var scope = GetSettingScope(settingType);
#if ASPNETCORE
            var repository = Dependency.Resolve<IConfigurationRepository>();
#else
            var repository = Dependency.Resolve<IConfigurationRepository>(scope);
#endif
            return repository.Load(settingType);
        }

        /// <summary>
        /// Returns configuration settings for specified setting type. 
        /// If setting is not found in its storage, returns a new object instance.
        /// If IConfigurationRepository for setting scope is not set returns null.
        /// </summary>
        /// <param name="settingType">Setting type.</param>
        public static object TryGet(Type settingType)
        {
            var scope = GetSettingScope(settingType);
#if ASPNETCORE
            var repository = Dependency.TryResolve<IConfigurationRepository>();
#else
            var repository = Dependency.TryResolve<IConfigurationRepository>(scope);
#endif
            return repository != null ? repository.Load(settingType) : null;
        }

        /// <summary>
        /// Returns configuration settings for specified setting type. 
        /// If setting is not found in its storage, returns a new object instance.
        /// </summary>
        /// <typeparam name="TSettings">Setting type</typeparam>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// IConfigurationRepository for setting scope is not set.</exception>
        public static TSettings Get<TSettings>()
            where TSettings: class, new()
        {
            return (TSettings)Get(typeof(TSettings));
        }

        /// <summary>
        /// Returns configuration settings for specified setting type. 
        /// If setting is not found in its storage, returns a new object instance.
        /// If IConfigurationRepository for setting scope is not set returns null.
        /// </summary>
        /// <typeparam name="TSettings">Setting type.</typeparam>
        public static TSettings TryGet<TSettings>()
            where TSettings : class, new()
        {
            return (TSettings)TryGet(typeof(TSettings));
        }
    }
}