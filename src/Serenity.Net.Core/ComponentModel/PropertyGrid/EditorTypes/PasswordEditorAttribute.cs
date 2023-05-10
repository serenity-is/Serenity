namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Password" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class PasswordEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Password";

    /// <summary>
    /// Initializes a new instance of the <see cref="PasswordEditorAttribute"/> class.
    /// </summary>
    public PasswordEditorAttribute()
        : base(Key)
    {
    }
}