using System;

namespace Serenity
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public abstract class BaseRegistrarAttribute : Attribute
    {
        public BaseRegistrarAttribute(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}