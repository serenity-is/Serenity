namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-md-4" CSS class, which makes it allocate a third of the form row
/// on device widths >= 992px (e.g. medium desktop).
/// </summary>
public class OneThirdWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneThirdWidthAttribute"/> class.
    /// </summary>
    public OneThirdWidthAttribute()
        : base("col-md-4")
    {
    }
}