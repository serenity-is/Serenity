namespace Serenity.ComponentModel;

/// <summary>
/// Marks the form field with the "col-md-3" CSS class, which makes it allocate
/// a quarter on device widths >= 992px (some desktops).
/// </summary>
public class MediumQuarterWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediumQuarterWidthAttribute"/> class.
    /// </summary>
    public MediumQuarterWidthAttribute()
        : base("col-md-3")
    {
    }
}