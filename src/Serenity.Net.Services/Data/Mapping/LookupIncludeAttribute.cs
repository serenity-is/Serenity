namespace Serenity.Data.Mapping;

/// <summary>
/// Marks the property so that it should be included in lookup by default.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class LookupIncludeAttribute : Attribute
{
}