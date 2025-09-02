namespace Serenity.ComponentModel;

/// <summary>
/// Marks form field with "col-sm-6" css class, which makes it allocate half of form row
/// on device widths >= 768px (e.g. iPad)
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