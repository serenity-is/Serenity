using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public sealed class LookupIncludeAttribute : Attribute
    {
    }
}