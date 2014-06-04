using System;
using Serenity.Services;
using Newtonsoft.Json;
using System.Data;

namespace Serenity.Data
{
    public static class Config
    {
        public static string GetSettingKey(Type settingsType)
        {
            string type = settingsType.Name;
            string local = "LocalSettings";
            string firm = "Settings";
            if (type.EndsWith(local, StringComparison.OrdinalIgnoreCase))
                type = type.Substring(0, type.Length - local.Length);
            else if (type.EndsWith(firm, StringComparison.OrdinalIgnoreCase))
                type = type.Substring(0, type.Length - firm.Length);
            return @"Settings\" + type;
        }

        public static object Get(Type settingType, bool cached = true)
        {
            string key = GetSettingKey(settingType);

            if (typeof(ISiteSetting).IsAssignableFrom(settingType))
            {
                if (cached)
                    return SiteStateCache.Get(key);
                else
                    return Json.ParseTolerant(IoC.Resolve<ISiteStateService>().Load(key).TrimToNull() ?? "{}", settingType);
            }
            else
            {
                if (cached)
                    return LocalUserStateCache.Get(key);
                else
                    return Json.ParseTolerant(IoC.Resolve<ILocalUserStateService>().Load(key).TrimToNull() ?? "{}", settingType);
            }
        }

        public static TSettings Get<TSettings>(bool cached = true) where TSettings : class, ISettings, new()
        {
            string key = GetSettingKey(typeof(TSettings));

            if (typeof(ISiteSetting).IsAssignableFrom(typeof(TSettings)))
            {
                if (cached)
                    return SiteStateCache.Deserialize<TSettings>(key);
                else
                    return Json.ParseTolerant<TSettings>(IoC.Resolve<ISiteStateService>().Load(key).TrimToNull() ?? "{}");
            }
            else
            {
                if (cached)
                    return LocalUserStateCache.Deserialize<TSettings>(key);
                else
                    return Json.ParseTolerant<TSettings>(IoC.Resolve<ILocalUserStateService>().Load(key).TrimToNull() ?? "{}");
            }
        }

        public static void Set(object state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            var settingType = state.GetType();

            string key = GetSettingKey(state.GetType());
            string stateString = Json.ToStringIndented(state);

            if (typeof(ISiteSetting).IsAssignableFrom(settingType))
                IoC.Resolve<ISiteStateService>().Save(key, stateString);
            else if (typeof(ILocalSetting).IsAssignableFrom(settingType))
                IoC.Resolve<ILocalUserStateService>().Save(key, stateString);
            else
                throw new ArgumentOutOfRangeException("stateType");
        }
    }
}