#if !COREFX
using System.Configuration;
#else
using Microsoft.Extensions.Configuration;
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
#if COREFX
                var setting = Dependency.Resolve<IConfigurationRoot>().GetSection("AppSettings")[key];
#else
                var setting = ConfigurationManager.AppSettings[key];
#endif

                return JSON.ParseTolerant(setting.TrimToNull() ?? "{}", settingType);
            });
        }
    }
}