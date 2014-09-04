using System;

namespace Serenity.Data
{
    public class ConnectionKeyAttribute : Attribute
    {
        public ConnectionKeyAttribute(string connectionKey)
        {
            this.Value = connectionKey;
        }

        public string Value { get; private set; }
    }
}