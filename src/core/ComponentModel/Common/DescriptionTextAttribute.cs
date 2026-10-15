namespace Serenity.ComponentModel;

/// <summary>
/// Specifies the Description text for a property or class. Unlike DescriptionAttribute, it can specify a local text key.
/// </summary>
/// <example>
/// [DescriptionText("My Description Text", TextKey = "My.Text.Key")] // with key and invariant text
/// [DescriptionText(TextKey = "My.Text.Key")] // with only key and no text
/// [DescriptionText("My Description Text")] // with only invariant text, no key
/// [DescriptionText("Db.Text.Key")] // still with only invariant text, no key unlike DescriptionAttribute.
/// </example>
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class DescriptionTextAttribute : DescriptionAttribute, IHasTextKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionTextAttribute"/> class with no Description text.
    /// </summary>
    public DescriptionTextAttribute()
        : base(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionTextAttribute"/> class with the specified Description text.
    /// </summary>
    /// <param name="text">The Description text.</param>
    public DescriptionTextAttribute(string text) : base(text ?? throw new ArgumentNullException(nameof(text)))
    {
    }

    /// <summary>
    /// Gets or sets the local text key for the Description text.
    /// </summary>
    public string? TextKey { get; set => field = value ?? throw new ArgumentNullException(nameof(value)); }
}