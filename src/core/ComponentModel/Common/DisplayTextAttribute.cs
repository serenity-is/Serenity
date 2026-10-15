namespace Serenity.ComponentModel;

/// <summary>
/// Specifies the display text for a property or class. Unlike DisplayNameAttribute, it can specify a local text key.
/// </summary>
/// <example>
/// [DisplayText("My Display Text", TextKey = "My.Text.Key")] // with key and invariant text
/// [DisplayText(TextKey = "My.Text.Key")] // with only key and no text
/// [DisplayText("My Display Text")] // with only invariant text, no key
/// [DisplayText("Db.Text.Key")] // still with only invariant text, no key unlike DisplayNameAttribute.
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class DisplayTextAttribute : DisplayNameAttribute, IHasTextKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayTextAttribute"/> class with no display text.
    /// </summary>
    public DisplayTextAttribute()
        : base(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayTextAttribute"/> class with the specified display text.
    /// </summary>
    /// <param name="text">The display text.</param>
    public DisplayTextAttribute(string text) : base(text ?? throw new ArgumentNullException(nameof(text)))
    {
    }

    /// <summary>
    /// Gets or sets the local text key for the display text.
    /// </summary>
    public string? TextKey { get; set => field = value ?? throw new ArgumentNullException(nameof(value)); }
}