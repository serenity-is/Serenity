namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "Checkbox"
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class CheckboxFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "Checkbox";

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckboxFormatterAttribute"/> class.
    /// </summary>
    public CheckboxFormatterAttribute()
        : base(Key)
    {
    }
}