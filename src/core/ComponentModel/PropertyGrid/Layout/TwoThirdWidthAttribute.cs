namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-md-8" CSS class, which makes it allocate two thirds of the form row
/// on device widths >= 992px (e.g. medium desktop).
/// </summary>
public class TwoThirdWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TwoThirdWidthAttribute"/> class.
    /// </summary>
    public TwoThirdWidthAttribute()
        : base("col-md-8")
    {
    }
}