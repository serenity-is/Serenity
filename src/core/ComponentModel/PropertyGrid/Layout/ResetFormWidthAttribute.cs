namespace Serenity.ComponentModel;

/// <summary>
/// Resets the form field width to null. It may be used to cancel the UntilNext flag of a prior width attribute.
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