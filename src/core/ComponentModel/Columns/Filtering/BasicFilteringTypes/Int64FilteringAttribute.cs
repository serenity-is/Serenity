namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the property should use Int64 (64-bit integer) type of filtering.
/// </summary>
/// <seealso cref="CustomFilteringAttribute" />
public class Int64FilteringAttribute : CustomFilteringAttribute
{
    /// <summary>
    /// Filtering type key
    /// </summary>
    public const string Key = "Int64";

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64FilteringAttribute"/> class.
    /// </summary>
    public Int64FilteringAttribute()
        : base(Key)
    {
    }
}
