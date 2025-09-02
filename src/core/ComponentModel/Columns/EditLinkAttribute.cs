namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the property this attribute placed on should have an edit link
/// </summary>
public sealed class EditLinkAttribute : Attribute
{
    /// <summary>
    /// Creates a new EditLink attribute
    /// </summary>
    public EditLinkAttribute()
    {
        Value = true;
    }

    /// <summary>
    /// Creates a new EditLink attribute with enable/disable option
    /// </summary>
    /// <param name="value">True if edit link should be enabled</param>
    public EditLinkAttribute(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets/sets edit link enable state
    /// </summary>
    public bool Value { get; private set; }

    /// <summary>
    /// Optional item type that this edit link should open. 
    /// It should match the dialog namespace / class name.
    /// </summary>
    public string? ItemType { get; set; }

    /// <summary>
    /// Gets/sets from which property this edit link should get its ID value to edit
    /// </summary>
    public string? IdField { get; set; }

    /// <summary>
    /// Gets/sets optional css class to add to edit links
    /// </summary>
    public string? CssClass { get; set; }
}