namespace Serenity.ComponentModel;

/// <summary>
/// Marks form field with "col-md-4 col-lg-3" css class, which makes it allocate third of form row
/// on device widths >= 992px (some desktop), and quarter on device widths >= 1200px
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