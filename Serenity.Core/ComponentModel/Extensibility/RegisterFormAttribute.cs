using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class FormScriptAttribute : Attribute
    {
        public FormScriptAttribute()
        {
        }

        public FormScriptAttribute(string key)
        {
            this.Key = key;
        }

        public String Key { get; private set; }
    }
}