namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that this class contains permission keys with optional
/// subclasses that also contains permission keys.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class NestedPermissionKeysAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NestedPermissionKeysAttribute"/> class.
    /// </summary>
    public NestedPermissionKeysAttribute()
    {
    }

    /// <summary>
    /// Gets / sets optional language identifier that specifies
    /// language for texts specified in [DisplayName] attribute.
    /// </summary>
    /// <value>
    /// The language identifier.
    /// </value>
    public string? LanguageID { get; set; }
}