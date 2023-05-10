namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Masked" editor
/// and also defines an automatic lookup script for row fields.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class MaskedEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Masked";

    /// <summary>
    /// Initializes a new instance of the <see cref="MaskedEditorAttribute"/> class.
    /// </summary>
    public MaskedEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the mask. a = letter, 9 = numeric, * = alphanumeric.
    /// </summary>
    /// <value>
    /// The mask.
    /// </value>
    public string? Mask
    {
        get { return GetOption<string>("mask"); }
        set { SetOption("mask", value); }
    }

    /// <summary>
    /// Gets or sets the placeholder.
    /// </summary>
    /// <value>
    /// The placeholder.
    /// </value>
    public string? Placeholder
    {
        get { return GetOption<string>("placeholder"); }
        set { SetOption("placeholder", value); }
    }
}