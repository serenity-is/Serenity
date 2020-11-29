using System;

namespace Serenity.Services
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class GenericHandlerTypeAttribute : Attribute
    {
        public GenericHandlerTypeAttribute(Type type)
        {
            Value = type;
        }

        public Type Value { get; }
    }
}