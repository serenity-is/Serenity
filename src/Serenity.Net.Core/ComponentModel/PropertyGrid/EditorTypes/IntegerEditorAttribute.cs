namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Integer" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class IntegerEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Integer";

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerEditorAttribute"/> class.
    /// </summary>
    public IntegerEditorAttribute()
        : base(Key)
    {
        if (AllowNegativesByDefault)
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


    /// <summary>
    /// Gets or sets a value indicating whether editors should allow negatives by default.
    /// This is a global setting that controls default of AllowNegatives property in this attribute.
    /// </summary>
    /// <value>
    ///   <c>true</c> if editors should allow negatives by default; otherwise, <c>false</c>.
    /// </value>
    public static bool AllowNegativesByDefault { get; set; }
}