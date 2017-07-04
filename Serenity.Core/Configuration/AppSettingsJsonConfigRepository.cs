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