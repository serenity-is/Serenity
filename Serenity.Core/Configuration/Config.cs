
namespace Serenity
{
    using Abstractions;
    using Serenity.ComponentModel;
    using System;

    public static class Config
    {
        private static string GetSettingScope(Type settingType)
        {
            var scopeAttr = settingType.GetCustomAttributes(typeof(SettingScopeAttribute), true);
            var scope = scopeAttr.IsEmptyOrNull() ? "Application" : ((SettingScopeAttribute)scopeAttr[0]).Value;
            return scope;
        }

        public static object Get(Type settingType)
        {
            var scope = GetSettingScope(settingType);
            var repository = Dependency.Resolve<IConfigurationRepository>(scope);
            return repository.Load(settingType);
        }

        public static object TryGet(Type settingType)
        {
            var scope = GetSettingScope(settingType);
            var repository = Dependency.TryResolve<IConfigurationRepository>(scope);
            return repository != null ? repository.Load(settingType) : null;
        }

        public static TSettings Get<TSettings>()
            where TSettings: class, new()
        {
            return (TSettings)Get(typeof(TSettings));
        }

        public static TSettings TryGet<TSettings>()
            where TSettings : class, new()
        {
            return (TSettings)TryGet(typeof(TSettings));
        }
    }
}