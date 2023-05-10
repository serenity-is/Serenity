namespace Serenity.ComponentModel;

/// <summary>
/// Controls horizontal alignment of text (usually in grid columns).
/// </summary>
/// <remarks>
/// This is an abstract base class. You need to use AlignCenter or AlignRight attributes.
/// </remarks>
public abstract class AlignmentAttribute : Attribute
{
    /// <summary>
    /// Creates a new AlignmentAttribute
    /// </summary>
    /// <param name="align">Alignment</param>
    protected AlignmentAttribute(string align)
    {
        Value = align;
    }

    /// <summary>
    /// Gets/sets value of the alignment attribute
    /// </summary>
    public string Value { get; private set; }
}