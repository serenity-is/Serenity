namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-lg-3 col-sm-6" CSS class, which makes it allocate half of the form row
/// on device widths >= 768px (e.g. iPad), and a quarter on device widths >= 1200px (desktop).
/// </summary>
public class QuarterWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuarterWidthAttribute"/> class.
    /// </summary>
    public QuarterWidthAttribute()
        : base("col-lg-3 col-sm-6")
    {
    }
}