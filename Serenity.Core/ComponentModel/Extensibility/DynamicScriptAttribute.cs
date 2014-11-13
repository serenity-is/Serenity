using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public abstract class DynamicScriptAttribute : Attribute
    {
        public DynamicScriptAttribute(string key)
        {
            this.Key = key;
        }

        public string Key { get; private set; }
        public int CacheDuration { get; set; }
        public string CacheGroupKey { get; set; }
    }
}