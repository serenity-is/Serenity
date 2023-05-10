namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that property should use enum type of filtering
/// </summary>
/// <seealso cref="CustomFilteringAttribute" />
public class EnumFilteringAttribute : CustomFilteringAttribute
{
    /// <summary>
    /// Filtering type key
    /// </summary>
    public const string Key = "Enum";

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumFilteringAttribute"/> class.
    /// </summary>
    public EnumFilteringAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the enum key.
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