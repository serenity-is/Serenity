
namespace Serenity.Configuration
{
    using Serenity;
    using Serenity.Abstractions;
    using Serenity.ComponentModel;
    using System;
    using System.Configuration;
    using System.Reflection;

    public class AppSettingsJsonConfigRepository : IConfigurationRepository
    {
        public void Save(Type settingType, object value)
        {
            throw new NotImplementedException();
        }

        public object Load(Type settingType)
        {
            return LocalCache.Get("ApplicationSetting:" + settingType.FullName, TimeSpan.Zero, delegate()
            {
                var keyAttr = settingType.GetCustomAttribute<SettingKeyAttribute>();
                var key = keyAttr == null ? settingType.Name : keyAttr.Value;
                return JSON.Parse(ConfigurationManager.AppSettings[key].TrimToNull() ?? "{}", settingType);
            });
        }
    }
}