using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Marks the property so that it should be included in lookup by default.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class LookupIncludeAttribute : Attribute
    {
    }
}