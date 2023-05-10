namespace Serenity.ComponentModel;

/// <summary>
/// Marks form field with "col-lg-9" css class, which makes it allocate 
/// three quarter on device widths >= 1200px
/// </summary>
public class ThreeQuarterWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThreeQuarterWidthAttribute"/> class.
    /// </summary>
    public ThreeQuarterWidthAttribute()
        : base("col-lg-9")
    {
    }
}