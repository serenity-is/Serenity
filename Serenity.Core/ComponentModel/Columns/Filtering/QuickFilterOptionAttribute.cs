using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class QuickFilterOptionAttribute : Attribute
    {
        public QuickFilterOptionAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }
        public object Value { get; private set; }
    }
}
