#if COREFX
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace System.Configuration
{
    public static class ConfigurationManager
    {
        public static readonly AppSettingsAccessor AppSettings = new AppSettingsAccessor();
        public class AppSettingsAccessor
        {
            public string this[string key]
            {
                get
                {
                    return Serenity.Dependency.Resolve<IConfiguration>().GetSection("AppSettings")[key];
                }
            }
        }
    }

}
#endif