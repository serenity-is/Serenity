using System;

namespace Serenity.ComponentModel
{
    public class SettingScopeAttribute : Attribute
    {
        public SettingScopeAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}