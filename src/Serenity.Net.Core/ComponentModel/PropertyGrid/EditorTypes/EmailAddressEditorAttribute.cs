namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "EmailAddress" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class EmailAddressEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "EmailAddress";

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailAddressEditorAttribute"/> class.
    /// </summary>
    public EmailAddressEditorAttribute()
        : base(Key)
    {
    }
}