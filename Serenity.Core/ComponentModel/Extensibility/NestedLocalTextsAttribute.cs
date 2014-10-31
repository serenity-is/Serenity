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

        public string LanguageID { get; set; }
        public string Prefix { get; set; }
    }
}