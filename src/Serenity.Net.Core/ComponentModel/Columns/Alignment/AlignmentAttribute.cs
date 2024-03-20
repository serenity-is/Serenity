namespace Serenity.ComponentModel;

/// <summary>
/// Controls horizontal alignment of text (usually in grid columns).
/// </summary>
/// <remarks>
/// This is an abstract base class. You need to use AlignCenter or AlignRight attributes.
/// </remarks>
/// <param name="align">Alignment</param>
public abstract class AlignmentAttribute(string align) : Attribute
{

    /// <summary>
    /// Gets/sets value of the alignment attribute
    /// </summary>
    public string Value { get; private set; } = align;
}