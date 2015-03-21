using System;

namespace Serenity.Data
{
    public class DatabaseAliasAttribute : Attribute
    {
        public DatabaseAliasAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}