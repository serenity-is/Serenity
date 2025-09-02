namespace Serenity.ComponentModel;

/// <summary>
/// Marks form field with "col-md-6 col-lg-3" css class, which makes it allocate half of form row
/// on device widths >= 992px (some desktop), and quarter on device widths >= 1200px
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