#if COREFX
using Microsoft.Extensions.Configuration;
#else
using System.Configuration;
#endif

namespace Serenity.Configuration
{
    using Serenity;
    using Serenity.Abstractions;
    using Serenity.ComponentModel;
    using System;
    using System.Reflection;

    public class AppSettingsJsonConfigRepository : IConfigurationRepository
    {
#if COREFX
        private IConfiguration configuration;

        public AppSettingsJsonConfigRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
#endif
        public void Save(Type settingType, object value)
        {
            throw new NotImplementedException();
        }

        public object Load(Type settingType)
        {
#if COREFX
            var keyAttr = settingType.GetCustomAttribute<SettingKeyAttribute>();
            var key = keyAttr == null ? settingType.Name : keyAttr.Value;
            return configuration.GetSection("AppSettings:" + key).Get(settingType);
#else
            return LocalCache.Get("ApplicationSetting:" + settingType.FullName, TimeSpan.Zero, delegate()
            {
                var keyAttr = settingType.GetCustomAttribute<SettingKeyAttribute>();
                var key = keyAttr == null ? settingType.Name : keyAttr.Value;

                
                var setting = ConfigurationManager.AppSettings[key];
                return JSON.ParseTolerant(setting.TrimToNull() ?? "{}", settingType);
            });
#endif
        }
    }
}