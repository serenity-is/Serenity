namespace Serenity.ComponentModel;

/// <summary>
/// Adds an input group text addon to the target property.
/// </summary>
public partial class InputGroupButtonAttribute : EditorAddonAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="InputGroupButtonAttribute"/>.
    /// </summary>
    /// <param name="text">Text</param>
    public InputGroupButtonAttribute(string text)
        : base(Key)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    /// <summary>
    /// Editor addon key
    /// </summary>
    public const string Key = "Serenity.InputGroupButtonAddon";

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
    /// Gets or sets the icon
    /// </summary>
    public string? Icon
    {
        get => GetOption<string?>("icon");
        set => SetOption("icon", value);
    }

    /// <summary>
    /// Gets or sets the action key which is set as data-action attribute.
    /// </summary>
    public string? ActionKey
    {
        get => GetOption<string?>("actionKey");
        set => SetOption("actionKey", value);
    }

    /// <summary>
    /// Gets or sets the button class like "primary", "secondary", "danger" etc.
    /// </summary>
    public string? ButtonClass
    {
        get => GetOption<string?>("buttonClass");
        set => SetOption("buttonClass", value);
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