namespace Serenity.ComponentModel;

/// <summary>
/// Resets the form label width to null. It may be used to cancel a prior LabelWidth attribute with UntilNext = true.
/// </summary>
public class ResetLabelWidthAttribute : LabelWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetLabelWidthAttribute"/> class.
    /// </summary>
    public ResetLabelWidthAttribute()
        : base(null)
    {
    }
}