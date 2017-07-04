using System;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Abstraction for ConfigurationManager to remove dependency on System.Configuration
    /// </summary>
    public interface IConfigurationManager
    {
        object AppSetting(string key, Type settingType);
        Tuple<string, string> ConnectionString(string key);
    }
}