namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-md-4 col-lg-3" CSS class, which makes it allocate a third of the form row
/// on device widths >= 992px (some desktops), and a quarter on device widths >= 1200px.
/// </summary>
public class MediumThirdLargeQuarterWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediumThirdLargeQuarterWidthAttribute"/> class.
    /// </summary>
    public MediumThirdLargeQuarterWidthAttribute()
        : base("col-md-4 col-lg-3")
    {
    }
}