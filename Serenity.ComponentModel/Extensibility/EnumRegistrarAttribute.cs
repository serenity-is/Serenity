using System;
using Serenity.Extensibility;

namespace Serenity.Extensibility
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class EnumRegistrarAttribute : BaseRegistrarAttribute
    {
        public EnumRegistrarAttribute(Type type)
            : base(type)
        {
        }
    }
}