namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the property this attribute is placed on should have an edit link.
/// </summary>
public sealed class EditLinkAttribute : Attribute
{
    /// <summary>
    /// Creates a new EditLinkAttribute.
    /// </summary>
    public EditLinkAttribute()
    {
        Value = true;
    }

    /// <summary>
    /// Creates a new EditLinkAttribute with enable/disable option.
    /// </summary>
    /// <param name="value">True if edit link should be enabled.</param>
    public EditLinkAttribute(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets or sets the edit link enable state.
    /// </summary>
    public bool Value { get; private set; }

    /// <summary>
    /// Optional item type that this edit link should open. 
    /// It should match the dialog namespace / class name.
    /// </summary>
    public string? ItemType { get; set; }

    /// <summary>
    /// Gets or sets the property from which this edit link should get its ID value to edit.
    /// </summary>
    public string? IdField { get; set; }

    /// <summary>
    /// Gets or sets the optional CSS class to add to edit links.
    /// </summary>
    public string? CssClass { get; set; }

    private bool? quiet;

    /// <summary>
    /// Gets or sets whether the edit link is a quiet one, which looks like the text of its cell until the row
    /// is hovered or the link has the focus, and opens the record all the same. True makes it quiet, e.g. for
    /// a column whose link repeats another one of the row, and false a full link, whatever the grid decides
    /// for the others. When it is not set, the grid decides: with the "primary" edit links of the grid, only
    /// the first visible link to the record of the row is a full link. See <see cref="GetQuiet"/> to tell
    /// whether it is set, as an attribute property can not be nullable.
    /// </summary>
    public bool Quiet
    {
        get => quiet ?? false;
        set => quiet = value;
    }

    /// <summary>
    /// Returns the value of <see cref="Quiet"/>, or null when it is not set, so that the grid decides.
    /// </summary>
    public bool? GetQuiet() => quiet;
}