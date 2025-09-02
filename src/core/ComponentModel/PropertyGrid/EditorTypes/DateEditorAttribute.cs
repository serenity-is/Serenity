namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Date" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public class DateEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Date";

    /// <summary>
    /// Initializes a new instance of the <see cref="DateEditorAttribute"/> class.
    /// </summary>
    public DateEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    /// <value>
    /// The minimum value.
    /// </value>
    public DateTime MinValue
    {
        get { return GetOption<DateTime>("minValue"); }
        set { SetOption("minValue", value); }
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    /// <value>
    /// The maximum value.
    /// </value>
    public DateTime MaxValue
    {
        get { return GetOption<DateTime>("maxValue"); }
        set { SetOption("maxValue", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether SQL server min max dates should be used.
    /// </summary>
    /// <value>
    ///   <c>true</c> if SQL server min max dates should be used; otherwise, <c>false</c>.
    /// </value>
    public bool SqlMinMax
    {
        get { return GetOption<bool?>("sqlMinMax") ?? true; }
        set { SetOption("sqlMinMax", value); }
    }
}