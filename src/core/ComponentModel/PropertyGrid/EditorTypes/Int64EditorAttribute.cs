namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Int64" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class Int64EditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Int64";

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64EditorAttribute"/> class.
    /// </summary>
    public Int64EditorAttribute()
        : base(Key)
    {
        if (IntegerEditorAttribute.AllowNegativesByDefault)
            AllowNegatives = true;
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    /// <value>
    /// The maximum value.
    /// </value>
    public long MaxValue
    {
        get { return GetOption<long>("maxValue"); }
        set { SetOption("maxValue", value); }
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    /// <value>
    /// The minimum value.
    /// </value>
    public long MinValue
    {
        get { return GetOption<long>("minValue"); }
        set { SetOption("minValue", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the editor should allow negatives.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [allow negatives]; otherwise, <c>false</c>.
    /// </value>
    public bool AllowNegatives
    {
        get { return GetOption<bool>("allowNegatives"); }
        set { SetOption("allowNegatives", value); }
    }
}
