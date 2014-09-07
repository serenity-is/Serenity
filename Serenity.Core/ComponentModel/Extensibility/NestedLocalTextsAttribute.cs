using System;
using Serenity.Extensibility;

namespace Serenity.Extensibility
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class NestedLocalTextsAttribute : Attribute
    {
        public NestedLocalTextsAttribute()
        {
        }
    }
}