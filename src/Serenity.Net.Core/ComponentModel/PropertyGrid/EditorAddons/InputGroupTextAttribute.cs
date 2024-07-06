namespace Serenity.ComponentModel;

/// <summary>
/// Adds an input group text addon to the target property.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class InputGroupTextAttribute : EditorAddonAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="InputGroupTextAttribute"/>.
    /// </summary>
    /// <param name="text">Text</param>
    public InputGroupTextAttribute(string text)
        : base(Key)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    /// <summary>
    /// Editor addon key
    /// </summary>
    public const string Key = "Serenity.InputGroupTextAddon";

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