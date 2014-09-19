using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class LookupScriptAttribute : Attribute
    {
        public LookupScriptAttribute(string key)
        {
            this.Key = key;
        }

        public string Key { get; private set; }

        public string Permission { get; set; }
    }
}