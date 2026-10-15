namespace Serenity.ComponentModel;

/// <summary>
/// Specifies the category name for a property or class. Unlike CategoryAttribute, it can specify a local text key.
/// </summary>
/// <example>
/// [CategoryName("My Display Text", TextKey = "My.Text.Key")] // with key and invariant text
/// [CategoryName(TextKey = "My.Text.Key")] // with only key and no text
/// [CategoryName("My Display Text")] // with only invariant text, no key
/// [CategoryName("Db.Text.Key")] // still with only invariant text, no key unlike CategoryAttribute.
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field, AllowMultiple = false)]
public class CategoryNameAttribute : CategoryAttribute, IHasTextKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryNameAttribute"/> class with no category name.
    /// Only useful when you want to specify the category name later through the TextKey property.
    /// </summary>
    public CategoryNameAttribute()
        : base(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryNameAttribute"/> class with the specified category name.
    /// </summary>
    /// <param name="category">The category.</param>
    public CategoryNameAttribute(string category) : base(category ?? throw new ArgumentNullException(nameof(category)))
    {
    }

    /// <summary>
    /// Gets or sets the local text key for the category.
    /// </summary>
    public string? TextKey { get; set => field = value ?? throw new ArgumentNullException(nameof(value)); }
}