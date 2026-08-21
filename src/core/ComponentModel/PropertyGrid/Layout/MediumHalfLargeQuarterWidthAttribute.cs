namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-md-6 col-lg-3" CSS class, which makes it allocate half of the form row
/// on device widths >= 992px (some desktops), and a quarter on device widths >= 1200px.
/// </summary>
public class MediumHalfLargeQuarterWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediumHalfLargeQuarterWidthAttribute"/> class.
    /// </summary>
    public MediumHalfLargeQuarterWidthAttribute()
        : base("col-md-6 col-lg-3")
    {
    }
}