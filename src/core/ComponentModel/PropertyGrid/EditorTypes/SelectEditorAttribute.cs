namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Select" editor, whose items come from the
/// <see cref="SelectItemAttribute"/> attributes of the property, if any. A property with a
/// <see cref="SelectItemAttribute"/> does not need this attribute, as the items imply it.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public class SelectEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Select";

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectEditorAttribute"/> class.
    /// </summary>
    public SelectEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// The field of an item that holds its local text key, which the editor localizes, or "text" when the text of an
    /// item is its key. When it is not set, the text of an item is shown as it is. The items of a
    /// <see cref="SelectItemAttribute"/> set it themselves when they have a text key.
    /// </summary>
    public string? TextKeyField
    {
        get { return GetOption<string>("textKeyField"); }
        set { SetOption("textKeyField", value); }
    }

    /// <summary>
    /// The field of an item that holds its CSS class, e.g. "text-danger", which the editor adds to the item in its
    /// results. The items of a <see cref="SelectItemAttribute"/> set it themselves when they have one.
    /// </summary>
    public string? CssClassField
    {
        get { return GetOption<string>("cssClassField"); }
        set { SetOption("cssClassField", value); }
    }
}
