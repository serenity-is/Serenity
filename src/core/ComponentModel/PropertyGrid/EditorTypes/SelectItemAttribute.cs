namespace Serenity.ComponentModel;

/// <summary>
/// Declares an item of a "Select" editor, see <see cref="SelectEditorAttribute"/>, on the property that the editor
/// belongs to, e.g. <c>[SelectItem("low", TextKey = "Controls.Priority.Low")]</c>. The items of a property imply the
/// editor, so the property does not need a <see cref="SelectEditorAttribute"/> as well.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class SelectItemAttribute : Attribute, IHasTextKey
{
    /// <summary>
    /// Creates a new instance of the <see cref="SelectItemAttribute"/> class.
    /// </summary>
    /// <param name="value">The value of the item, which the editor sets on its field.</param>
    public SelectItemAttribute(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The value of the item, which the editor sets on its field, and which the client knows as the id of the item.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The text of the item, which is shown as it is, and which the local text of <see cref="TextKey"/> falls back to
    /// when there is no text for it. When neither is set, the value of the item is its text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// The local text key of the item, which the editor localizes. When it is not set, an item that has a text takes the
    /// key that its form implies for it, e.g. <c>Forms.Namespace.MyForm.Priority_SelectItem_low</c>. An empty string
    /// leaves the item as it is, which is what an item whose text is a name, e.g. a company, needs.
    /// </summary>
    public string? TextKey { get; set; }

    /// <summary>
    /// The icon of the item, e.g. "fa fa-truck", which the editor shows before its text.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// The CSS class of the item, e.g. "text-danger", which the editor adds to the item in its results.
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// The order of the item among the items of the property, as the order of the attributes of a property is not
    /// guaranteed. Default is zero, and the items with the same order keep the order of their attributes.
    /// </summary>
    public int Order { get; set; }
}
