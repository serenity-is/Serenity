namespace Serenity.ComponentModel;

/// <summary>
/// Sets the form field width class to null, e.g. full width.
/// </summary>
public class FullWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FullWidthAttribute"/> class.
    /// </summary>
    public FullWidthAttribute()
        : base(null)
    {
    }
}