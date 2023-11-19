namespace Serenity.ComponentModel;

/// <summary>
/// Sets a hint for a form field. 
/// Hint is shown when field label is hovered. 
/// This has no effect on columns.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HintAttribute"/> class.
/// </remarks>
/// <param name="hint">The hint.</param>
public class HintAttribute(string? hint) : Attribute
{

    /// <summary>
    /// Gets the hint.
    /// </summary>
    /// <value>
    /// The hint.
    /// </value>
    public string? Hint { get; private set; } = hint;
}
