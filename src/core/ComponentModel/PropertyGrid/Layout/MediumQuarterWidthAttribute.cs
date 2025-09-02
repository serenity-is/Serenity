namespace Serenity.ComponentModel;

/// <summary>
/// Marks form field with "col-md-3" css class, which makes it allocate 
/// quarter on device widths >= 992px (some desktops)
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