using System;

namespace Serenity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SettingAttribute : Attribute
    {
        public SettingAttribute()
        {
        }

        public SettingAttribute(string key)
        {
            this.Key = key;
        }

        public SettingAttribute(string repository, string key)
        {
            this.Repository = repository;
            this.Key = key;
        }

        public string Repository { get; private set; }
        public string Key { get; private set; }
    }
}