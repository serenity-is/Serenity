
namespace Serenity.Configuration
{
    using Serenity;
    using Serenity.Abstractions;
    using System;

    public class ConfigurationWrapper : IConfigurationManager
    {
        public object AppSetting(string key, Type settingType)
        {
#if !COREFX
            var setting = System.Configuration.ConfigurationManager.AppSettings[key];
            if (settingType == typeof(string))
                return setting;

            if (setting.IsTrimmedEmpty())
                return null;

            return JSON.ParseTolerant(setting.TrimToNull() ?? "{}", settingType);
#else
            var configuration = Dependency.Resolve<Microsoft.Extensions.Configuration.IConfiguration>();
            if (configuration == null)
                return null;

            var section = configuration.GetSection("AppSettings:" + key);

            if (section == null)
                return null;

            return Microsoft.Extensions.Configuration.ConfigurationBinder.Get(section, settingType);
#endif
        }

        public Tuple<string, string> ConnectionString(string key)
        {
#if !COREFX
            var cs = System.Configuration.ConfigurationManager.ConnectionStrings[key];
            if (cs == null)
                return null;

            return new Tuple<string, string>(cs.ConnectionString, cs.ProviderName);
#else
            var configuration = Dependency.TryResolve<Microsoft.Extensions.Configuration.IConfiguration>();
            if (configuration == null)
                return null;

            var connectionString = Microsoft.Extensions.Configuration.ConfigurationBinder
                .GetValue<string>(configuration, "Data:" + key + ":ConnectionString");

            if (connectionString == null)
                return null;

            var providerName = Microsoft.Extensions.Configuration.ConfigurationBinder
                .GetValue<string>(configuration, "Data:" + key + ":ProviderName") ?? "System.Data.SqlClient";

            return new Tuple<string, string>(connectionString, providerName);
#endif
        }
    }
}
