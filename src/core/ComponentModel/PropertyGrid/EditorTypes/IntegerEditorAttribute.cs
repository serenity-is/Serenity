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
    /// This is a global setting that controls the default of the AllowNegatives property in this attribute.
    /// Returns the local value if any is set through <see cref="SetLocalAllowNegativesByDefault"/>,
    /// otherwise the default value. The local value should be used for unit tests.
    /// </summary>
    /// <value>
    ///   <c>true</c> if editors should allow negatives by default; otherwise, <c>false</c>.
    /// </value>
    public static bool AllowNegativesByDefault
    {
        get => localAllowNegativesByDefault.Value ?? defaultAllowNegativesByDefault;
        set => defaultAllowNegativesByDefault = value;
    }

    /// <summary>
    /// Sets the local value for <see cref="AllowNegativesByDefault"/> for the current thread
    /// and async context. Useful for background tasks, async methods, and testing to set the
    /// value locally without affecting other threads or tests.
    /// </summary>
    /// <param name="value">The local value. Can be null to use the default value.</param>
    /// <returns>The old local value, if any.</returns>
    public static bool? SetLocalAllowNegativesByDefault(bool? value)
    {
        var old = localAllowNegativesByDefault.Value;
        localAllowNegativesByDefault.Value = value;
        return old;
    }

    private static bool defaultAllowNegativesByDefault;
    private static readonly AsyncLocal<bool?> localAllowNegativesByDefault = new();
}