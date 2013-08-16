using System;

namespace Serenity
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