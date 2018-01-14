using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class EditorOptionAttribute : Attribute
    {
        public EditorOptionAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }

        [IntrinsicProperty]
        public string Key { get; private set; }
        [IntrinsicProperty]
        public object Value { get; private set; }
    }
}
