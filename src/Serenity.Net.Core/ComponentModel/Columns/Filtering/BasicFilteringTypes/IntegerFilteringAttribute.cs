
namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that property should use integer type of filtering
/// </summary>
/// <seealso cref="CustomFilteringAttribute" />
public class IntegerFilteringAttribute : CustomFilteringAttribute
{
    /// <summary>
    /// Filtering type key
    /// </summary>
    public const string Key = "Integer";

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerFilteringAttribute"/> class.
    /// </summary>
    public IntegerFilteringAttribute()
        : base(Key)
    {
    }
}