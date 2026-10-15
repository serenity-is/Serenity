namespace Serenity.ComponentModel;

/// <summary>
/// Sets a hint for a form field.
/// The hint is shown when the field label is hovered.
/// This has no effect on columns.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HintAttribute"/> class.
/// </remarks>
/// <param name="hint">The hint.</param>
public class HintAttribute(string? hint) : Attribute, IHasTextKey
{
    /// <summary>
    /// Creates a new instance of the <see cref="HintAttribute"/> class.
    /// Use this overload for creating a hint with a local text key, and no default hint.
    /// </summary>
    public HintAttribute() : this(null)
    {
    }

    /// <summary>
    /// Gets the hint.
    /// </summary>
    /// <value>
    /// The hint.
    /// </value>
    public string? Hint { get; private set; } = hint;

    /// <summary>
    /// Gets or sets the local text key for the category.
    /// </summary>
    public string? TextKey { get; set => field = value ?? throw new ArgumentNullException(nameof(value)); }
}
