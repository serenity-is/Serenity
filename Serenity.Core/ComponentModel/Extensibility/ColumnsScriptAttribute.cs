using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class ColumnsScriptAttribute : Attribute
    {
        public ColumnsScriptAttribute(string key)
        {
            if (key.IsEmptyOrNull())
                throw new ArgumentNullException("key");

            this.Key = key;
        }

        public String Key { get; private set; }
    }
}