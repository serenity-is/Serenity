namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "String" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class StringEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "String";

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEditorAttribute"/> class.
    /// </summary>
    public StringEditorAttribute()
        : base(Key)
    {
    }
}