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

        public SettingAttribute(string scope, string key)
        {
            this.Scope = scope;
            this.Key = key;
        }

        public string Scope { get; private set; }
        public string Key { get; private set; }
    }
}