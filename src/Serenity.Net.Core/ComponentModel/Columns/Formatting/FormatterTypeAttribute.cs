namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatter type.
/// </summary>
/// <seealso cref="Attribute" />
public class FormatterTypeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormatterTypeAttribute"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public FormatterTypeAttribute(string type)
    {
        FormatterType = type;
    }

    /// <summary>
    /// Sets the formatter parameters.
    /// </summary>
    /// <param name="formatterParams">The formatter parameters.</param>
    public virtual void SetParams(IDictionary<string, object?> formatterParams)
    {
    }

    /// <summary>
    /// Gets the type of the formatter.
    /// </summary>
    /// <value>
    /// The type of the formatter.
    /// </value>
    public string FormatterType { get; private set; }
}
