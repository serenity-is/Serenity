using System;

namespace Serenity.Extensibility
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class NestedPermissionKeysAttribute : Attribute
    {
        public NestedPermissionKeysAttribute()
        {
        }

        public string LanguageID { get; set; }
    }
}