using Serenity.Services;
using System;

namespace Serenity.Data
{
    public static class Config
    {
        public static string GetSettingKey(Type settingsType, out string scope)
        {
            if (settingsType == null)
                throw new ArgumentNullException("settingsType");

            var attr = settingsType.GetAttribute<SettingAttribute>(true);
            if (attr == null)
                throw new ArgumentOutOfRangeException("settingsType");

            scope = attr.Scope ?? "Default";

            return attr.Key ?? settingsType.Name;
        }

        public static object Get(Type settingType)
        {
            string scope;
            string key = GetSettingKey(settingType, out scope);

            return TwoLevelCache.GetLocalStoreOnly("Configuration:" + scope + ":" + key, TimeSpan.FromDays(1),
                "Configuration", () =>
            {
                var store = IoC.CanResolve<IConfigurationStore>(scope) ?
                    IoC.Resolve<IConfigurationStore>(scope) :
                    IoC.Resolve<IConfigurationStore>();

                return JSON.ParseTolerant(store.Load(scope, key).TrimToNull() ?? "{}", settingType);
            });
        }

        public static TSettings Get<TSettings>()
            where TSettings: class, new()
        {
            return (TSettings)Get(typeof(TSettings));
        }
    }
}