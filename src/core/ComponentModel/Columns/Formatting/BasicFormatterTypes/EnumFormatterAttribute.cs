namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "Enum".
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class EnumFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "Enum";

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumFormatterAttribute"/> class.
    /// </summary>
    public EnumFormatterAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the enum key which is full namespace and 
    /// class name of the enum or it should match the value
    /// set with [EnumKey] attribute on the enum type.
    /// </summary>
    /// <value>
    /// The enum key.
    /// </value>
    public string? EnumKey
    {
        get { return GetOption<string>("enumKey"); }
        set { SetOption("enumKey", value); }
    }
}
