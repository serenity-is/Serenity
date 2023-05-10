namespace Serenity.ComponentModel;

/// <summary>
/// Sets a hint for a form field. 
/// Hint is shown when field label is hovered. 
/// This has no effect on columns.
/// </summary>
public class HintAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HintAttribute"/> class.
    /// </summary>
    /// <param name="hint">The hint.</param>
    public HintAttribute(string? hint)
    {
        Hint = hint;
    }

    /// <summary>
    /// Gets the hint.
    /// </summary>
    /// <value>
    /// The hint.
    /// </value>
    public string? Hint { get; private set; }
}
