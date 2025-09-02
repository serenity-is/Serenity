
namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that this field should have decimal type of filtering
/// </summary>
public class DecimalFilteringAttribute : CustomFilteringAttribute
{
    /// <summary>
    /// Filtering type key
    /// </summary>
    public const string Key = "Decimal";

    /// <summary>
    /// Initializes a new instance of the <see cref="DecimalFilteringAttribute"/> class.
    /// </summary>
    public DecimalFilteringAttribute()
        : base(Key)
    {
    }
}