namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that this class contains local texts with optionally
/// subclasses that also contains local texts with keys joined by "."
/// between each class name. The topmost class has no local text
/// prefix by default and its name doesn't take role in local text 
/// key generation.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class NestedLocalTextsAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NestedLocalTextsAttribute"/> class.
    /// </summary>
    public NestedLocalTextsAttribute()
    {
    }

    /// <summary>
    /// Gets or sets the optional language identifier.
    /// </summary>
    /// <value>
    /// The language identifier.
    /// </value>
    public string? LanguageID { get; set; }

    /// <summary>
    /// Gets or sets the optional local text prefix.
    /// </summary>
    /// <value>
    /// The local text prefix.
    /// </value>
    public string? Prefix { get; set; }
}