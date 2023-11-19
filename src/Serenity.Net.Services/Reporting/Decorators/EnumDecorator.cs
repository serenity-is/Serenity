namespace Serenity.Reporting;

/// <summary>
/// A decorator used to export enum name instead of the enum integer value.
/// </summary>
/// <remarks>
/// Creates an instance of the class.
/// </remarks>
/// <param name="enumType">Enum type</param>
/// <param name="localizer">Text localizer</param>
public class EnumDecorator(Type enumType, ITextLocalizer localizer) : BaseCellDecorator
{
    private readonly Type enumType = enumType;
    private readonly ITextLocalizer localizer = localizer;

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
