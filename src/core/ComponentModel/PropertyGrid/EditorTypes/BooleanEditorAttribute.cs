namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Boolean" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class BooleanEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Boolean";

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanEditorAttribute"/> class.
    /// </summary>
    public BooleanEditorAttribute()
        : base(Key)
    {
    }
}