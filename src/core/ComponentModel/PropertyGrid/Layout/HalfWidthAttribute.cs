namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-sm-6" CSS class, which makes it allocate half of the form row
/// on device widths >= 768px (e.g. iPad).
/// </summary>
public class HalfWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HalfWidthAttribute"/> class.
    /// </summary>
    public HalfWidthAttribute()
        : base("col-sm-6")
    {
    }
}