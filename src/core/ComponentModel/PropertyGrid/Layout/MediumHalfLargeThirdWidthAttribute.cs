namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-md-6 col-lg-4" CSS class, which makes it allocate half of the form row
/// on device widths >= 992px (some desktops), and a third on device widths >= 1200px.
/// </summary>
public class MediumHalfLargeThirdWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediumHalfLargeThirdWidthAttribute"/> class.
    /// </summary>
    public MediumHalfLargeThirdWidthAttribute()
        : base("col-md-6 col-lg-4")
    {
    }
}