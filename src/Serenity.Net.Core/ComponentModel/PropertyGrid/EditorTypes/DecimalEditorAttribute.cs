namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Decimal" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class DecimalEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Decimal";

    /// <summary>
    /// Initializes a new instance of the <see cref="DecimalEditorAttribute"/> class.
    /// </summary>
    public DecimalEditorAttribute()
        : base(Key)
    {
        if (AllowNegativesByDefault)
            AllowNegatives = true;
    }

    /// <summary>
    /// Gets or sets the number of decimals allowed.
    /// </summary>
    /// <value>
    /// The decimals allowed.
    /// </value>
    public int Decimals
    {
        get { return GetOption<int>("decimals"); }
        set { SetOption("decimals", value); }
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    /// <value>
    /// The maximum value.
    /// </value>
    public object? MaxValue
    {
        get { return GetOption<string>("maxValue"); }
        set { SetOption("maxValue", value == null ? null : Convert.ToString(value, CultureInfo.InvariantCulture)); }
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    /// <value>
    /// The minimum value.
    /// </value>
    public object? MinValue
    {
        get { return GetOption<string>("minValue"); }
        set { SetOption("minValue", value == null ? null : Convert.ToString(value, CultureInfo.InvariantCulture)); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to pad decimals with zero.
    /// </summary>
    /// <value>
    ///   <c>true</c> if pad decimals with zero; otherwise, <c>false</c>.
    /// </value>
    public bool PadDecimals
    {
        get { return GetOption<bool>("padDecimals"); }
        set { SetOption("padDecimals", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to allow negatives.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should allow negatives; otherwise, <c>false</c>.
    /// </value>
    public bool AllowNegatives
    {
        get { return GetOption<bool>("allowNegatives"); }
        set { SetOption("allowNegatives", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to allow negatives by default.
    /// This is a global setting that controls if decimal editors should allow
    /// negative values unless specified otherwise.
    /// </summary>
    /// <value>
    ///   <c>true</c> if negatives should be allowed by default; otherwise, <c>false</c>.
    /// </value>
    public static bool AllowNegativesByDefault { get; set; }
}