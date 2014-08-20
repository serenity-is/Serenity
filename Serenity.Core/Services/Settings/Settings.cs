using Serenity.Services;
using System;

namespace Serenity
{
    public static class Config
    {
        public static string GetSettingKey(Type settingType, out string repository)
        {
            if (settingType == null)
                throw new ArgumentNullException("settingType");

            var attr = settingType.GetAttribute<SettingAttribute>(true);
            if (attr == null)
                throw new ArgumentOutOfRangeException("settingType");

            repository = attr.Repository ?? "Default";

            return attr.Key ?? settingType.Name;
        }

        public static object Get(Type settingType)
        {
            string repository;
            string key = GetSettingKey(settingType, out repository);

            return TwoLevelCache.GetLocalStoreOnly("Settings:" + repository + ":" + key, TimeSpan.FromDays(1),
                "Settings", () =>
            {
                return IoC.Resolve<IConfigurationRepository>(repository)
                    .Load(settingType);
            });
        }

        public static TSettings Get<TSettings>()
            where TSettings: class, new()
        {
            return (TSettings)Get(typeof(TSettings));
        }
    }
}