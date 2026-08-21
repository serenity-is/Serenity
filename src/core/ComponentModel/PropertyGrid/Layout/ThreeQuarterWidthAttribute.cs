namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-lg-9" CSS class, which makes it allocate
/// three quarters on device widths >= 1200px.
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