namespace Serenity.Reporting;

/// <summary>
/// A decorator used to export enum name instead of the enum integer value.
/// </summary>
public class EnumDecorator : BaseCellDecorator
{
    private readonly Type enumType;
    private readonly ITextLocalizer localizer;

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <param name="enumType">Enum type</param>
    /// <param name="localizer">Text localizer</param>
    public EnumDecorator(Type enumType, ITextLocalizer localizer)
    {
        this.enumType = enumType;
        this.localizer = localizer;
    }

    /// <inheritdoc/>
    public override void Decorate()
    {
        if (Value != null)
        {
            try
            {
                Value = EnumMapper.FormatEnum(localizer, enumType, Value);
            }
            catch
            {
            }
        }
    }
}
