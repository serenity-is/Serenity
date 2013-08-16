using System;

namespace Serenity
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class ScriptRegistrarAttribute : BaseRegistrarAttribute
    {
        public ScriptRegistrarAttribute(Type type)
            : base(type)
        {
        }
    }
}