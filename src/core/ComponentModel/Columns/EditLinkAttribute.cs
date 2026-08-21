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
}