using System;
using System.Runtime.CompilerServices;

namespace Serenity.ComponentModel
{
    public class LocalTextPrefixAttribute : Attribute
    {
        public LocalTextPrefixAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}