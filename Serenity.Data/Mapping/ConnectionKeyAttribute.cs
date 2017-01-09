using System;
using System.Reflection;

namespace Serenity.Data
{
    public class ConnectionKeyAttribute : Attribute
    {
        public ConnectionKeyAttribute(string connectionKey)
        {
            this.Value = connectionKey;
        }

        public ConnectionKeyAttribute(Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");

            var attr = sourceType.GetCustomAttribute<ConnectionKeyAttribute>(true);
            if (attr == null)
                throw new ArgumentOutOfRangeException("sourceType",
                    "ConnectionKeyAttribute is created with source type " + sourceType.Name + 
                    ", but that class has no ConnectionKey attribute");

            this.Value = attr.Value;
            this.SourceType = sourceType;
        }

        public string Value { get; private set; }
        public Type SourceType { get; private set; }
    }
}