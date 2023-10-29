namespace Serenity.ComponentModel;

/// <summary>
/// Sets label width to "0", e.g. hides the label
/// </summary>
/// <seealso cref="LabelWidthAttribute" />
public class HideLabelAttribute : LabelWidthAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HideLabelAttribute"/> class.
    /// </summary>
    public HideLabelAttribute() : base("0") 
    {
        JustThis = true;
    }
}