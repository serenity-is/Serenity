namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-md-6" CSS class, which makes it allocate half of the form row
/// on device widths >= 992px (some desktops).
/// </summary>
public class MediumHalfWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediumHalfWidthAttribute"/> class.
    /// </summary>
    public MediumHalfWidthAttribute()
        : base("col-md-6")
    {
    }
}