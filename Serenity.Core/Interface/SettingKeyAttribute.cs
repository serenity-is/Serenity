using System;

namespace Serenity.ComponentModel
{
    public class SettingKeyAttribute : Attribute
    {
        public SettingKeyAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}