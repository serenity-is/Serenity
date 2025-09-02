namespace Serenity.ComponentModel;

/// <summary>
/// Resets form field width to null. It maybe used to cancel UntilNext flag of a prior width attribute.
/// </summary>
public class ResetFormWidthAttribute : FormWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetFormWidthAttribute"/> class.
    /// </summary>
    public ResetFormWidthAttribute()
        : base(null)
    {
    }
}