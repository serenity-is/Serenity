using System;
using Serenity.Extensibility;

namespace Serenity.Extensibility
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