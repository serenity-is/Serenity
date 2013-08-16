using System;

namespace Serenity
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class RegisterReportAttribute : Attribute
    {
        public RegisterReportAttribute(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}