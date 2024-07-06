namespace Serenity.ComponentModel;

/// <summary>
/// Adds a form text addon to the target property.
/// </summary>
public partial class FormTextAttribute : EditorAddonAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="FormTextAttribute"/>.
    /// </summary>
    /// <param name="text">Text</param>
    public FormTextAttribute(string text)
        : base(Key)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    /// <summary>
    /// Editor addon key
    /// </summary>
    public const string Key = "Serenity.FormTextAddon";

    /// <summary>
    /// Gets the text.
    /// </summary>
    public string? Text
    {
        get => GetOption<string?>("text");
        set => SetOption("text", value);
    }

    /// <summary>
    /// Gets or sets the CSS class.
    /// </summary>
    public string? CssClass
    {
        get => GetOption<string?>("cssClass");
        set => SetOption("cssClass", value);
    }

    /// <summary>
    /// Gets or sets the placement before editor option.
    /// </summary>
    public bool Before
    {
        get => GetOption<bool?>("before") ?? false;
        set => SetOption("before", value);
    }
}